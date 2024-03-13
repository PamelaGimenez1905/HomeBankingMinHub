using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;


namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientService _clientService;
        private IAccountService _accountService;
        private ITransactionService _transactionService;
        private ILoanService _loanService;

        public LoansController(IClientService clientService, IAccountService accountService, ITransactionService transactionService, ILoanService loanService)

        {

            _clientService = clientService;
            _accountService = accountService;
            _transactionService = transactionService;
            _loanService = loanService;


        }

        [HttpGet]

        public IActionResult Get()
        {
            try
            {
                var loans = _loanService.GetAllLoans();
                var LoansDTO = new List<LoanDTO>();
                foreach (Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO(loan);
                   
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

                    Client client = _clientService.FindByEmail(email);

                    if (client == null)
                    {
                        return Forbid();
                    }

                    //préstamo existente
                    Loan loan = _loanService.FindById(loanApplicationDTO.LoanId);
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

                    //Obtiene las cuotas disponibles
                    var newPaymentValues = loan.Payments.Split(',').Select(s => s.Trim()).ToList();
                    //Obtiene las cuotas
                    var requestedPayments = loanApplicationDTO.Payments.ToString();
                    if(!newPaymentValues.Contains(requestedPayments))                    
                    {
                        return StatusCode(403,"La cantidad de cuotas ingresadas no es valida para el tipo de prestamo solicitado");
                    }


                    //Existencia de la cuenta de destino
                    var account = _accountService.FindByNumber(loanApplicationDTO.ToAccountNumber);
                    if (account == null)
                    {
                        return StatusCode(403, "La cuenta de destino no ha sido encontrada");
                    }

                    //Que la cuenta de destino pertenezca al Cliente autenticado

                    if(account.ClientId != client.Id)
                    {
                        return StatusCode(403,"La cuenta de destino no pertenece al cliente");
                    }

                    _loanService.CreateLoan(loanApplicationDTO, account, client, loan);

                                 
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
