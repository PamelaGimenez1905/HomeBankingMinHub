

namespace HomeBankingMindHub.Utils
{


    public static class RandomUtils
    {
        private static  Random random = new Random();
       

        public static string  GenerateRandomNumber()
        {
            return random.Next(10000000,99999999).ToString();
        }


       public static string GenerateRandomCardNumber()
        {
            
            string cardNumber = random.Next(1000, 9999).ToString() + "-" + random.Next(1000, 9999).ToString() + "-" + random.Next(1000, 9999).ToString() + "-" +
            random.Next(1000, 9999).ToString();
            return cardNumber;
        }


        public static int GenerateRandomCVV()
        {
            int cvvNumber = random.Next(1, 999);
            return cvvNumber;
        }
    }
}

    




