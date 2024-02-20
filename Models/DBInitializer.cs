﻿using System;
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

            if (!context.Accounts.Any())
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
                        context.Accounts.Add(account);
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
                        context.Accounts.Add(account);
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
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();
                }

               


            }


            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(account => account.Number == "VIN001");

                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT },
                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT },
                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda Sony", Type = TransactionType.DEBIT },
                    };

                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }

                    context.SaveChanges();
                }
            }



        }

    }
}
