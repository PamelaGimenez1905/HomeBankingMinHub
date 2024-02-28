using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCardsByClient(long clientId);
        bool ExistsByCardNumber(string cardNumber);
        void Save(Card card);
    }
}
