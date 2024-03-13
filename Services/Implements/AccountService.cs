using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Utils;

namespace HomeBankingMindHub.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IAccountService _accountService;
        private IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;

        }

        public void CreateAccount(Client client)
        {
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
                ClientId = client.Id,

            };
            _accountRepository.Save(newAccount);
        }

        public Account FindClientById(long id, string email)
        {
            return _accountRepository.FindByIdAndClientEmail(id, email);
        }

        public Account FindByNumber(string number)
        {
            return _accountRepository.FindByNumber(number);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _accountRepository.GetAllAccounts();
        }

       
    }


    
    }




