using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>,ILoanRepository
    {
         public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }
    public Loan FindById(long id)
    {
        return RepositoryContext.Loans.Find(id);
    }

    public IEnumerable <Loan> GetAllLoans()
    {
        return RepositoryContext.Loans.ToList();
    }
}
}
