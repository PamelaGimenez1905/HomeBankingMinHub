using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        public void CreateAccount(Client client);
        public IEnumerable<Account> GetAllAccounts();
        public Account FindByNumber(string number);
        public Account FindClientById(long id, string email);
        

    }
}
