using HomeBankingMindHub.Models;


namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        public string CreateCard(Client client, CardDTO cardDTO);
    }
}
