using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;


namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ILoanRepository _loanRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, IClientLoanRepository clientLoanRepository, ILoanRepository loanRepository)

        {

            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _clientLoanRepository = clientLoanRepository;
            _loanRepository = loanRepository;


        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();
                var LoansDTO = new List<LoanDTO>();
                foreach (Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                        
                    };
                    LoansDTO.Add(newLoanDTO);
                }
                return StatusCode(200, LoansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }





        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
           public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    //info usuario autenticado
                    string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                    if (email == string.Empty)
                    {
                        return Forbid();
                    }

                    Client client = _clientRepository.FindByEmail(email);

                    if (client == null)
                    {
                        return Forbid();
                    }

                    //préstamo existente
                    Loan loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                    if (loan == null)
                    {
                        return StatusCode(403,"El Préstamo no existe");
                    }

                    //Monto no sea = 0 y que no sobrepase el máximo autorizado
                    if (loanApplicationDTO.Amount <= 0 || loanApplicationDTO.Amount > loan.MaxAmount)
                    {
                        return Forbid();
                    }

                    //Payments no estén vacíos
                    if (loanApplicationDTO.Payments == null || loanApplicationDTO.Payments.Equals(0))
                    {
                        return StatusCode(403,"Los pagos no pueden estar vacíos");
                    }

                    //Numero de cuotas
                    var newPaymentValues = loan.Payments.Split(',').Select(s => s.Trim()).ToList();
                    var requestedPayments = loanApplicationDTO.Payments.ToString();
                    if(!newPaymentValues.Contains(requestedPayments))
                    
                    //if (!newPaymentValues.Contains(loanApplicationDTO.Payments.ToString()))
                    {
                        return StatusCode(403,"La cantidad de cuotas ingresadas no es valida para el tipo de prestamo solicitado");
                    }


                    //Existencia de la cuenta de destino
                    var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
                    if (account == null)
                    {
                        return StatusCode(403, "La cuenta de destino no ha sido encontrada");
                    }

                    //Que la cuenta de destino pertenezca al Cliente autenticado

                    if(account.ClientId != client.Id)
                    {
                        return StatusCode(403,"La cuenta de destino no pertenece al cliente");
                    }

                    //Al guardar ClientLoan multiplicar el monto por el 20%
                    double interestAmount = loanApplicationDTO.Amount * 0.20;
                    double totalAmount= interestAmount + loanApplicationDTO.Amount;         
                   

                    //Crear y Guardae el ClientLoan
                    var ClientLoan = new ClientLoan
                    {
                        ClientId = client.Id,
                        LoanId = loan.Id,
                        Amount = totalAmount,
                        Payments = loanApplicationDTO.Payments

                    };
                    _clientLoanRepository.Save(ClientLoan);

                    //Crédito para la cuenta de destino
                    var Transaction = new Models.Transaction
                    {
                        Type = TransactionType.CREDIT,
                        Amount = loanApplicationDTO.Amount,
                        Description = "Préstamo Aprobado " + loan.Name,
                        AccountId = account.Id,
                        Date = DateTime.Now,
                    };
                    _transactionRepository.Save(Transaction);


                    
                    //Actualizar el Balance de la cuenta sumando el monto del préstamo
                    account.Balance += loanApplicationDTO.Amount;

                    //Guardar la cuenta

                    _accountRepository.Save(account);

                    scope.Complete();
                    return StatusCode(201);

                }


                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }

            }


        }


    }


}
