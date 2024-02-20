namespace HomeBankingMindHub.Models
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public String Description { get; set; }
        public DateTime Date { get; set; }
       
    }
}
