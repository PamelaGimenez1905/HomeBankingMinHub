using HomeBankingMindHub.Models;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;


namespace HomeBankingMindHub.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private IClientService _clientService;
        private IAccountService _accountService;
        private ITransactionService _transactionService;

        public TransactionController(IClientService clientService, IAccountService accountService, ITransactionService transactionService)

        {

            _clientService = clientService;
            _accountService = accountService;
            _transactionService = transactionService;
        }



        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            using (var scope = new TransactionScope())
          try
           {
            // Obtener el email del cliente autenticado
              string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
              if (email == string.Empty)
              {
                  return StatusCode(403);
              }

             // Buscar el cliente en la base de datos por su correo electrónico
              Client client = _clientService.FindByEmail(email);

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
                Account fromAccount = _accountService.FindByNumber(transferDTO.FromAccountNumber);

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
              Account toAccount = _accountService.FindByNumber(transferDTO.ToAccountNumber);

           // Verificar que exista la cuenta de destino
              if (toAccount == null)
              {
               return StatusCode(403, "La cuenta de destino no existe.");
              }               
                    _transactionService.CreateTransaction(fromAccount, toAccount, transferDTO);
                    scope.Complete();
                    return Created(); 

            }
            catch
            {
                return StatusCode(500, "No se pudo efectuar la transferencia");
            }

        }
    }
}
    

