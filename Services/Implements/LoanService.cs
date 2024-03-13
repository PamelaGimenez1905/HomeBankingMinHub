using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class LoanService : ILoanService
    {
        public readonly IClientLoanRepository _clientLoanRepository;
        public readonly ITransactionRepository _transactionRepository;
        public readonly IAccountRepository _accountRepository;
        public readonly ILoanRepository _loanRepository;
        public LoanService(IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository,
            IAccountRepository accountRepository, ILoanRepository loanRepository)
        {
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
        }



        public void CreateLoan(LoanApplicationDTO loanApplicationDTO, Account account, Client client, Loan loan)
        {
            ClientLoan loanApp = new ClientLoan()
            {
                LoanId = loanApplicationDTO.LoanId,
                Amount = loanApplicationDTO.Amount * 1.2,
                Payments = loanApplicationDTO.Payments,
                ClientId = client.Id,
            };
            _clientLoanRepository.Save(loanApp);

            Transaction transaction = new Transaction()
            {
                Amount = loanApplicationDTO.Amount,
                Description = loan.Name + " loan approve",
                Date = DateTime.Now,
                Type = TransactionType.CREDIT,
                AccountId = account.Id,
            };

            _transactionRepository.Save(transaction);

            account.Balance += loanApplicationDTO.Amount;

            _accountRepository.Save(account);
        }
    

        public IEnumerable<Loan> GetAllLoans()
        {
        return _loanRepository.GetAllLoans();
        }

        public Loan FindById(long id)
        {
           return _loanRepository.FindById(id);
        }
    }
}
