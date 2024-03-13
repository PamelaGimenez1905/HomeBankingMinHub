using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class CardService : ICardService
    {

        private ICardRepository _cardRepository;
       

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
           
        }




        public string CreateCard(Client client, CardDTO card)

        {
            

            // Verificar límite de tarjetas existentes
            var existingCards = _cardRepository.GetCardsByClient(client.Id);
            if (existingCards.Count() >= 6)
            {
                return (" El cliente ya tiene 6 tarjetas registradas.");
            }

            //Cuenta el número de tarjetas de débito y crédito entre las existentes
            int debitCards = existingCards.Count(c => c.Type == CardType.DEBIT);
            int creditCards = existingCards.Count(c => c.Type == CardType.CREDIT);

            //verifica si el tipo de tarjeta es de débito y si ya tiene tres tarjetas 
            if (card.Type.Equals(CardType.DEBIT) && debitCards >= 3)
            {
                return (" El Cliente ya posee 3 tarjetas de Débito Registradas");
            }
            //verifica si el tipo de tarjeta es de crédito y si ya tiene tres tarjetas 
            else if (card.Type.Equals(CardType.CREDIT) && creditCards >= 3)
            {
                return (" El Cliente ya posee 3 tarjetas de Crédito Registradas");
            }

            //Covierto string a Enum
            CardColor Color = Enum.Parse<CardColor>(card.Color);
            CardType Type = Enum.Parse<CardType>(card.Type);


            //CardColor color = (CardColor)Enum.Parse(typeof(CardColor), card.color;
            //CardType type = (CardType)Enum.Parse(typeof(CardType), card.type);

            string cardNumber = Utils.RandomUtils.GenerateRandomCardNumber();
            int cvv = Utils.RandomUtils.GenerateRandomCVV();

            Card newCard = new Card
            {
                CardHolder = client.FirstName + client.LastName,
                Type = Type,
                Color = Color,
                Number = cardNumber,
                Cvv = cvv,
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
                ClientId = client.Id,

            };
            _cardRepository.Save(newCard);
            return "Ok";


        }

        
    }
    }
