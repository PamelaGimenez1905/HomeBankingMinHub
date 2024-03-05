using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ILoanRepository
    {
        Loan FindById(long id);
        IEnumerable<Loan> GetAllLoans();
    }
}
