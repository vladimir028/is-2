namespace MovieApp.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        //public EShopApplicationUser User { get; set; }
        public Guid UserId { get; set; }
        public ICollection<TicketsOrder> ?TicketsOrder { get; set; }
    }
}
