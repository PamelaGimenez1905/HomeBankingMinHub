using System;
using System.Linq;

namespace HomeBankingMindHub.Models
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

            if (!context.Account.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                if (accountVictor != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 0 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Account.Add(account);
                    }
                    context.SaveChanges();
                }


                var accountJuan = context.Clients.FirstOrDefault(c => c.Email == "juan@gmail.com");
                if (accountJuan != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountJuan.Id, CreationDate = DateTime.Today, Number = "VIN002", Balance =10000 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Account.Add(account);
                    }
                    context.SaveChanges();
                }

                var accountMaria = context.Clients.FirstOrDefault(c => c.Email == "maria@gmail.com");
                if (accountMaria != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountMaria.Id, CreationDate = DateTime.Now, Number = "VIN003", Balance = 20000 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Account.Add(account);
                    }
                    context.SaveChanges();
                }

               


            }

                


        }
    }
}
