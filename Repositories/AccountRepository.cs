using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories
{
    public class AccountRepository : RepositoryBase<Account>,IAccountRepository
    {
        public AccountRepository (HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public Account FindById(long id)
        {
            return FindByCondition(account => account.Id == id)
            .Include(account=>account.Transactions)
            .FirstOrDefault();
        }

        public Account FindByIdAndClientEmail(long id, string email)
        {
            return FindByCondition(Account => Account.Id == id  && Account.Client.Email.Equals(email) )
            .Include(account => account.Transactions)
            .FirstOrDefault();
        }


        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(account => account.Transactions)
                .ToList();
        }

        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }

        //Agrego el método implementado en la interfaz
        public IEnumerable<Account> GetAccountsByClient(long clientId)

        {

            return FindByCondition(account => account.ClientId == clientId)

            .Include(account => account.Transactions)

            .ToList();

        }
        //verifica si existe alguna cuenta en la base de datos cuyo número coincida con el que recibe
        public bool ExistsByAccount(string accountNumber)
        {
            return RepositoryContext.Accounts.Any(account => account.Number == accountNumber);
        }
        

    }
}
