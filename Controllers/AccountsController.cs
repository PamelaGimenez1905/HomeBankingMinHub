using HomeBankingMindHub.Models;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;





namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }


       
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var accounts = _accountService.GetAllAccounts();

                List<AccountDTO> accountsDTO = new List<AccountDTO>();

                foreach (var account in accounts)
                {
                   AccountDTO newAccountDTO = new AccountDTO(account);
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
                var account = _accountService.FindClientById(id,email);
                if (account == null)
                {
                    return Unauthorized();
                }
                AccountDTO accountDTO = new AccountDTO(account);
                return Ok(accountDTO);

            

               
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }


}

