namespace MovieApp.Models
{
    public class TicketsOrder
    {
        public Guid Id { get; set; }
        public Ticket ?Ticket { get; set; }
        public Guid TicketId { get; set; }
        public Order ?Order { get; set; }
        public Guid OrderId { get; set; }
    }
}
