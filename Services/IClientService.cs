using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
       
       public IEnumerable<Client> GetAllClients();
       void CreateClient(ClientDTO clientDTO);
       public Client FindByEmail(string email);
       ClientDTO GetClientById(long id);
    }
}
