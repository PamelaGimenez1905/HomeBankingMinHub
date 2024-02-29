using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HomeBankingMindHub.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)

        {

            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }



        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
          try
           {
            // Obtener el email del cliente autenticado
              string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
              if (email == string.Empty)
              {
                  return StatusCode(403);
              }

             // Buscar el cliente en la base de datos por su correo electrónico
              Client client = _clientRepository.FindByEmail(email);

              if (client == null)
               {
                   return StatusCode(403, "El cliente no existe");
               }
             // Verificar que los parámetros no estén vacíos
              if (string.IsNullOrEmpty(transferDTO.FromAccountNumber) || string.IsNullOrEmpty(transferDTO.ToAccountNumber)
                    || string.IsNullOrEmpty(transferDTO.Description) || transferDTO.Amount <= 0)
                {
                    return StatusCode(403);
                }

             // Verificar que los números de cuenta no sean iguales
              if (transferDTO.FromAccountNumber.Equals(transferDTO.ToAccountNumber))
                {
                  return StatusCode(403, "Las cuentas de origen y destino no pueden ser iguales.");
                }

             // Obtener la cuenta de origen
                var fromAccount = _accountRepository.FindByNumber(transferDTO.FromAccountNumber);

             // Verificar que exista la cuenta de origen
              if (fromAccount == null)
               {
                 return StatusCode(403, "La cuenta de origen no existe.");
               }

            // Verificar que la cuenta de origen tenga el monto disponible
              if (fromAccount.Balance < transferDTO.Amount)
               {
               return StatusCode(403, "La cuenta de origen no tiene fondos suficientes para realizar la transferencia.");
               }

            // Obtener la cuenta de destino
              var toAccount = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);

           // Verificar que exista la cuenta de destino
              if (toAccount == null)
              {
               return StatusCode(403, "La cuenta de destino no existe.");
              }

                // Crear las transacciones 
                Transaction newTransactionFrom = new Transaction
                {
                    AccountId = fromAccount.Id,
                    Type = TransactionType.DEBIT,
                    Amount = transferDTO.Amount,
                    Description = $"{transferDTO.Description} (DEBIT from {transferDTO.FromAccountNumber} to {transferDTO.ToAccountNumber})",
                    Date = DateTime.Now

                };

                Transaction newTransactionTo = new Transaction
                {
                    AccountId = toAccount.Id,
                    Type = TransactionType.CREDIT,
                    Amount = transferDTO.Amount,
                    Description = $"{transferDTO.Description} (CREDIT from {transferDTO.FromAccountNumber} to {transferDTO.ToAccountNumber})",
                    Date = DateTime.Now
                };
                _transactionRepository.Save(newTransactionFrom);
                _transactionRepository.Save(newTransactionTo);

                //Actualizamos los montos
                fromAccount.Balance -= transferDTO.Amount;
                toAccount.Balance += transferDTO.Amount;

                //guardamos
                _accountRepository.Save(fromAccount);
                _accountRepository.Save(toAccount);

                return StatusCode(201, "Transaccion creada con Exito");

            }
            catch
            {
                return StatusCode(500, "No se pudo efectuar la transferencia");
            }

        }
    }
}
    

