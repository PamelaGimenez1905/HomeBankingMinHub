using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;





namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }


       
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();

                var accountsDTO = new List<AccountDTO>();

                foreach (var account in accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(transaction => new TransactionDTO
                        {
                            Id = transaction.Id,

                            Type = transaction.Type.ToString(),

                            Amount = transaction.Amount,

                            Description = transaction.Description,

                            Date = transaction.Date,

                        }).ToList()
                    };
                    accountsDTO.Add(newAccountDTO);
                }
                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

       
        [HttpGet("{id}")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                //Obtiene el mail del usuario auténticado
                string email = User.FindFirst("Client") !=null ? User.FindFirst("Client").Value : string.Empty;
                if(email == string.Empty)
                {
                    return Forbid();
                }
                //Busca una cuenta porel id y el email del cliente
                var account = _accountRepository.FindByIdAndClientEmail(id, email);
                if (account == null)
                {
                    return Unauthorized();
                }

                AccountDTO accountDTO = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    CreationDate = account.CreationDate,
                    Balance = account.Balance,
                    Transactions = account.Transactions.Select(transaction =>
                        new TransactionDTO
                        {
                            Id = transaction.Id,
                            Type = transaction.Type.ToString(),
                            Amount = transaction.Amount,
                            Description = transaction.Description,
                            Date = transaction.Date,
                        }).ToList()
                };

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }


}

