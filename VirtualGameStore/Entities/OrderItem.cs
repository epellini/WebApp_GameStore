namespace VirtualGameStore.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int GameId { get; set; }
        public Order? Order { get; set; }
        public Game? Game { get; set; }
    }
}
