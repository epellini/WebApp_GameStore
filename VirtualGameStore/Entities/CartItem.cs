namespace VirtualGameStore.Entities
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int GameId { get; set; }
        public Cart? Cart { get; set; }
        public Game? Game { get; set; }
    }
}
