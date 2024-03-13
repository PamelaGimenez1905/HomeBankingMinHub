using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using System.Security.Cryptography.Xml;

namespace HomeBankingMindHub.Services.Implements
{
    public class TransactionService : ITransactionService
    {
       private  IAccountRepository _accountRepository;
       private ITransactionRepository _transactionRepository;

        public TransactionService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        { 
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
                            
        
        public void CreateTransaction(Account fromAccount, Account toAccount, TransferDTO transferDTO)
        {
            Transaction newTransactionFrom = new Transaction
            {
                AccountId = fromAccount.Id,
                Type = TransactionType.DEBIT,
                Amount = transferDTO.Amount,
                Description = $"{transferDTO.Description} (DEBIT from {transferDTO.FromAccountNumber} to {transferDTO.ToAccountNumber})",
                Date = DateTime.Now

            };

            Transaction newTransactionTo = new Transaction
            {
                AccountId = toAccount.Id,
                Type = TransactionType.CREDIT,
                Amount = transferDTO.Amount,
                Description = $"{transferDTO.Description} (CREDIT from {transferDTO.FromAccountNumber} to {transferDTO.ToAccountNumber})",
                Date = DateTime.Now
            };
            _transactionRepository.Save(newTransactionFrom);
            _transactionRepository.Save(newTransactionTo);

            //Actualizamos los montos
            fromAccount.Balance -= transferDTO.Amount;
            toAccount.Balance += transferDTO.Amount;
            //guardamos
            _accountRepository.Save(fromAccount);
            _accountRepository.Save(toAccount);
            
        }
    }
}
