using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

using HomeBankingMindHub.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;



namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;
       

        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)

        {

            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }


        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()

        {

            try

            {

                var clients = _clientRepository.GetAllClients();



                var clientsDTO = new List<ClientDTO>();



                foreach (Client client in clients)

                {

                    var newClientDTO = new ClientDTO

                    {

                        Id = client.Id,

                        Email = client.Email,

                        FirstName = client.FirstName,

                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO

                        {

                            Id = ac.Id,

                            Balance = ac.Balance,

                            CreationDate = ac.CreationDate,

                            Number = ac.Number

                        }).ToList(),

                        Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),


                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color.ToString(),
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type.ToString(),
                        }).ToList()

                    };



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


                var client = _clientRepository.FindById(id);
                

                if (client == null)

                {

                    return Forbid();

                }



                var clientDTO = new ClientDTO

                {

                    Id = client.Id,

                    Email = client.Email,

                    FirstName = client.FirstName,

                    LastName = client.LastName,

                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {

                        Id = ac.Id,

                        Balance = ac.Balance,

                        CreationDate = ac.CreationDate,

                        Number = ac.Number

                    }).ToList(),

                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)

                    }).ToList(),

                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()

                };



                return Ok(clientDTO);

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
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }
                //Creamos el objeto ClientDTO
                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Client client)
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

                //Implementamos el método para saber si existe el usuario
                if(_clientRepository.ExistsByEmail(client.Email))
                {
                    return StatusCode(403, "El Email ingresado está en uso");
                }               

                 Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                   
                };


                //Verificar que el número de cuenta no exista en el repositorio
                string randomNumber;
                string accountNumber;
                do
                {
                    randomNumber = RandomUtils.GenerateRandomNumber();
                    accountNumber = $"VIN-{randomNumber}";
                } while (_accountRepository.ExistsByAccount(accountNumber));

                Account newAccount = new Account
                {
                    Number = accountNumber, 
                    Balance = 0,
                    CreationDate = DateTime.Now,
                    Client = newClient,
                };

                _clientRepository.Save(newClient);
                _accountRepository.Save(newAccount);
                return StatusCode(201, "El cliente se creó exitosamente");

               


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

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Verifico límite de cuentas
                var existingAccounts = _accountRepository.GetAccountsByClient(client.Id);
                if (existingAccounts.Count() >= 3)
                {
                    return StatusCode(403, "Prohibido: El cliente ya tiene 3 cuentas registradas.");
                }


                //Genera nuevo número de cuenta aleatorio
                string randomNumber = RandomUtils.GenerateRandomNumber();
                string accountNumber = $"VIN-{randomNumber}";


                //Verificar que el número de cuenta no exista en el repositorio
                do
                {
                    randomNumber = RandomUtils.GenerateRandomNumber();
                    accountNumber = $"VIN -{randomNumber}";
                } while (_accountRepository.ExistsByAccount(accountNumber));
               
                
                // Crea nueva cuenta
                var newAccount = new Account
                {
                    ClientId = client.Id,
                    Number = accountNumber,
                    Balance = 0,
                    CreationDate = DateTime.Now

                };

               

                _accountRepository.Save(newAccount);

               
                return StatusCode(201, "Cuenta creada exitosamente.");
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

                   string email = User.FindFirstValue("Client");
                       if (string.IsNullOrEmpty(email))
                        {
                            return Forbid();
                        }

                        // Buscar el cliente en la base de datos 
                        Client client = _clientRepository.FindByEmail(email);

                        if (client == null)
                        {
                            return Forbid();
                        }

                        // Obtener la lista de cuentas del cliente
                        var clientAccounts = _accountRepository.GetAccountsByClient(client.Id);


                        var accountDTO = clientAccounts.Select(account => new AccountDTO
                        {
                            Id = account.Id,
                            Number = account.Number,
                            Balance = account.Balance,
                            CreationDate = account.CreationDate

                        }).ToList();

                        return Ok(accountDTO);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }


        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult RequestCard([FromBody] CardTypeAndColorDTO cardTypeAndColor)
        {
            try
            {
                // Obtener el email del cliente autenticado
                string email = User.FindFirstValue("Client");
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                // Buscar el cliente en la base de datos por su correo electrónico
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Verificar límite de tarjetas existentes
                var existingCards = _cardRepository.GetCardsByClient(client.Id);
                if (existingCards.Count() >= 6)
                {
                    return StatusCode(403, " El cliente ya tiene 6 tarjetas registradas.");
                }
                
                //Cuenta el número de tarjetas de débito y crédito entre las existentes
                int debitCards = existingCards.Count(c => c.Type == CardType.DEBIT);
                int creditCards = existingCards.Count(c => c.Type == CardType.CREDIT);

                if (cardTypeAndColor.Type.Equals(CardType.DEBIT) && debitCards >= 3)
                {
                    return StatusCode(403, " El Cliente ya posee 3 tarjetas de Débito Registradas");
                }
                else if (cardTypeAndColor.Type.Equals(CardType.CREDIT) && creditCards >= 3)
                {
                    return StatusCode(403, " El Cliente ya posee 3 tarjetas de Crédito Registradas");
                }

                // Generar números aleatorios para la tarjeta y el CVV
                string cardNumber = RandomUtils.GenerateRandomCardNumber();
                int cvv = RandomUtils.GenerateRandomCVV();

                do
                {
                    cardNumber = RandomUtils.GenerateRandomCardNumber();

                } while (_cardRepository.ExistsByCardNumber(cardNumber));



                // Calcular fecha de vencimiento (5 años después de la creación)
                DateTime ExpirationDate = DateTime.Now.AddYears(5);

               
                //Covierto string a Enum
                CardColor Color = Enum.Parse<CardColor>(cardTypeAndColor.Color);
                CardType Type = Enum.Parse<CardType>(cardTypeAndColor.Type);


                string FullName = client.FirstName + " " + client.LastName;

                // Crear nueva tarjeta
                var newCard = new Card
                {
                    CardHolder = FullName,
                    Type = Type,
                    Color = Color,
                    Number = cardNumber,
                    Cvv = cvv,
                    FromDate = DateTime.Now,
                    ThruDate = ExpirationDate,
                    ClientId = client.Id
                };

                // Guardar la nueva tarjeta en el repositorio
                _cardRepository.Save(newCard);

                return StatusCode(201, "Tarjeta solicitada exitosamente.");
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
                string email = User.FindFirstValue("Client");
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                // Buscar el cliente en la base de datos por su correo electrónico
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Obtener las tarjetas del cliente
                var clientCards = _cardRepository.GetCardsByClient(client.Id);

               
                var cardDTOs = clientCards.Select(card => new CardDTO
                {
                    Id = card.Id,
                    CardHolder = card.CardHolder,
                    Type = card.Type.ToString(),
                    Color = card.Color.ToString(),
                    Number = card.Number,
                    Cvv = card.Cvv,
                    FromDate = card.FromDate,
                    ThruDate = card.ThruDate
                   
                }).ToList();

                return Ok(cardDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }

}

