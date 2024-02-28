using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Card> GetCardsByClient(long clientId)
        {
            return FindByCondition(card => card.ClientId == clientId).ToList();
        }
        public bool ExistsByCardNumber(string cardNumber) 
        { 
            return FindByCondition(card => card.Number == cardNumber).Any();
        }
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
