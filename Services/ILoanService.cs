using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        public void CreateLoan(LoanApplicationDTO loanApplicationDTO, Account account, Client client, Loan loan);
        public Loan FindById(long id);
        public IEnumerable<Loan> GetAllLoans();
    }
}
