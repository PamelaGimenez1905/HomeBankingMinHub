using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;
using System.Net;



namespace HomeBankingMindHub.Services.Implements
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        public void CreateClient(ClientDTO clientDTO)
        {
            try
            {
                // Verifica que el email no esté en uso
                if (_clientRepository.ExistsByEmail(clientDTO.Email))
                {
                    throw new InvalidOperationException("El email ya está en uso.");
                }

                // Crea un nuevo cliente
                Client newClient = new Client
                {
                    Email = clientDTO.Email,
                    Password = clientDTO.Password,
                    FirstName = clientDTO.FirstName,
                    LastName = clientDTO.LastName,
                };

                // Verifica que el número de cuenta no exista en el repositorio
                string accountNumber;
                string randomNumber;
                do
                {
                    randomNumber = RandomUtils.GenerateRandomNumber();
                    accountNumber = $"VIN-{randomNumber}";
                } while (_accountRepository.ExistsByAccount(accountNumber));

                // Crea una nueva cuenta asociada al cliente
                Account newAccount = new Account
                {
                    Number = accountNumber,
                    Balance = 0,
                    CreationDate = DateTime.Now,
                    Client = newClient,
                };

                // Guarda el nuevo cliente y la cuenta asociada
                _clientRepository.Save(newClient);
                _accountRepository.Save(newAccount);
            }
            catch (Exception ex)
            {
                
                throw new Exception("Error al crear el cliente.", ex);
            }
        }

        public Client FindByEmail(string email)
        {
            return _clientRepository.FindByEmail(email);
        }

        public IEnumerable<Client> GetAllClients()
        {
            return _clientRepository.GetAllClients();
        }

        public ClientDTO GetClientById(long id)
        {
            Client client = _clientRepository.FindById(id);
            ClientDTO clientDTO = new ClientDTO(client);
            if (clientDTO == null)
            {
                return null;
            }
            return clientDTO;
        }





        





    }

}
      
