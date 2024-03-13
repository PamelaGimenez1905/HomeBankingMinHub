using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        public void CreateTransaction(Account account1, Account account2, TransferDTO transferDTO);
    }
}
