﻿using HomeBankingMindHub.Models;

using System.Collections.Generic;

using System.Text.Json.Serialization;



namespace HomeBankingMindHub.Models

{

    public class ClientDTO

    {

        [JsonIgnore]

        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Loans { get; set; }
        public ICollection<CardDTO> Cards { get; set; }


        public ClientDTO() { }
        public ClientDTO(Client client)
        {
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Password = client.Password;
            Accounts = client.Accounts.Select(account => new AccountDTO(account)).ToList();
            Loans = client.ClientLoans.Select(loan => new ClientLoanDTO(loan)).ToList();
            Cards = client.Cards.Select(card => new CardDTO(card)).ToList();
            
        }
    }
       

}