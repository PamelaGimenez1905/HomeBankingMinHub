using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        Account FindByIdAndClientEmail(long id, string email);
        bool ExistsByAccount(string accountNumber);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account FindByNumber(string number);

    }
}
