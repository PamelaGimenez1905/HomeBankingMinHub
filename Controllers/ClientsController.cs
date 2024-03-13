using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implements;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;



namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        public ClientsController(IClientService clientService, IAccountService accountService, ICardService cardService)

        {

            _clientService = clientService;
            _accountService = accountService;
            _cardService = cardService;
        }
           
        
        private IClientService _clientService;
        private IAccountService _accountService;
        private ICardService _cardService;

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()

        {
            try

            {
                var clients = _clientService.GetAllClients();

                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO(client);

                    clientsDTO.Add(newClientDTO);
                }
                return Ok(clientsDTO);
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
                var client = _clientService.GetClientById(id);

                if (client == null)
                {
                    return Forbid();
                }
               
                return Ok(client);
            }

            catch (Exception ex)

            {
                return StatusCode(500, ex.Message);

            }

        }


       
        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {   //Obtenemos la info del cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                //Busca al cliente por email en el repositorio
                Client client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }
                

                return Ok(new ClientDTO(client));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientDTO client)
        {
            try
            {
                //validamos datos antes

                if (String.IsNullOrEmpty(client.Email))
                {
                    return StatusCode(403, "El email ingresado es inválido o nulo");
                }        

                if (String.IsNullOrEmpty(client.Password))
                {
                    return StatusCode(403,"La contraseña ingresada es incorrecta o nula");
                }
                if(String.IsNullOrEmpty(client.FirstName))
                {
                    return StatusCode(403, "El nombre ingresado es incorrecto o nulo ");
                }
                if(String.IsNullOrEmpty(client.LastName))
                {
                    return StatusCode(403, "El apellido ingresado es incorrecto o nulo");
                }
               
               
                _clientService.CreateClient(client);
                return Created();        
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateAccount()
        {
            try
            {  
                             
                //Obtiene la info del cliente auténticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                var client  = _clientService.FindByEmail(email) ;
                if (client == null)
                {
                    return Forbid();
                }
               if (client.Accounts.Count == 3) 
                {
                    return StatusCode(403, "Ya no podés tener más cuentas, llegaste al límite");
                }
               _accountService.CreateAccount(client);
                return Created();
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetClientAccounts()
        {
            try
            {
                //Obtiene la info del cliente auténticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                var client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                List<AccountDTO> accountsDTO = new List<AccountDTO>();   

                foreach (var account in client.Accounts)
                {
                    AccountDTO newAccountDTO= new AccountDTO(account);
                    accountsDTO.Add(newAccountDTO);
                }
                    return Ok(accountsDTO);
            }
                    catch (Exception ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }


        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult RequestCard([FromBody] CardDTO card)
        {
            try
            {
                // Obtener el email del cliente autenticado
                string email = User.FindFirstValue("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                var client = _clientService.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }
                _cardService.CreateCard(client, card);
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetClientCards()
        {
            try
            {
                // Obtener el email del cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                // Buscar el cliente en la base de datos por su correo electrónico
                Client client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                List<CardDTO> cardsDTO = new List<CardDTO>();
                foreach (var cards in client.Cards)
                {
                    CardDTO newCardsDTO = new CardDTO(cards);
                    cardsDTO.Add(newCardsDTO);
                }

                return Ok(cardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }

}

