using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class CartViewModel
    {
        // Properties:

        public int? ShoppingCartId { get; set; }
        public Cart? ShoppingCart { get; set; }
        public string? UserId { get; set; }
        public ICollection<Game>? shoppingCartGames { get; set; }
    }
}
