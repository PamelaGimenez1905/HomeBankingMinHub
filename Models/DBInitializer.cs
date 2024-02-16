using System;
using System.Linq;

namespace HomeBankingMinHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123456"},
                     new Client { Email = "juan@gmail.com", FirstName="Juan", LastName="Perez", Password="548932"},
                     new Client { Email = "Maria@gmail.com", FirstName="Maria", LastName="Rodriguez", Password="458712"},
                     new Client { Email = "juan@gmail.com", FirstName="Pedro", LastName="Gonzalez", Password="654321"}


                };

                context.Clients.AddRange(clients);

                //guardamos
                context.SaveChanges();
            }

        }
    }
}
