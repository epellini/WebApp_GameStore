using VirtualGameStore.DataAccess;
using VirtualGameStore.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VirtualGameStore.Services
{
    public interface IGameStoreManager
    {
        // CRUD operations for Game entity:
        // Create Game:
        public void CreateGame(Game game);
        // Read all Games and sort:
        public List<Game> GetAllGames(string sort);
        // Read all Games by search query:
        public List<Game> GetGamesBySearch(string query);
        // Read Game:
        public Game? GetGameById(int id);
        // Update Game:
        public void UpdateGame(Game game);
        // Delete Game:
        public void DeleteGame(Game game);


        // CRUD operations for Profile entity:
        // Create Profile:
        public void CreateProfile(Profile profile);
        // Read Profile:
        public Profile GetProfileById(string id);
        // Update Profile:
        public void UpdateProfile(Profile profile);


        // LCRUD operations for preferredLanguage entity:
        // Create preferred language:
        public void CreatePreferredLanguage(PreferredLanguage preferredLanguage);
        // Read all preferred languages:
        public List<PreferredLanguage>? GetPreferredLanguagesById(string id);
        // Delete preferred language:
        public void DeletePreferredLanguage(PreferredLanguage preferredLanguage);


        // CRUD operations for FavouriteGenre entity:
        // Create favourite genre:
        public void CreateFavouriteGenre(FavouriteGenre favouriteGenre);
        // Read all favourite genres:
        public List<FavouriteGenre>? GetFavouriteGenresById(string id);
        // Delete favourite genre:
        public void DeleteFavouriteGenre(FavouriteGenre favouriteGenre);


        // LCRUD operations for WishedGame entity:
        // Create wished game:
        public void CreateWishedGame(WishedGame wishedGame);
        // Read wished game:
        public WishedGame? GetWishedGame(int id);
        // Read all wished games:
        public List<WishedGame>? GetWishedGamesById(string id);
        public List<WishedGame>? GetWishedGamesByGameId(int id);
        // Delete wished game:
        public void DeleteWishedGame(WishedGame wishedGame);

        public Cart? GetCartById(string id);
        public CartItem? GetCartItemById(int id);
        public void AddItemToCart(CartItem cartItem);
        public void RemoveItemFromCart(CartItem cartItem);
        public void CreateCart(Cart cart);


        // CRUD operations for Order entity:
        // Create order:
        public void CreateOrder(Order order);
        // Read order:
        public Order? GetOrder(int id);
        // Read all orders:
        public List<Order>? GetOrdersById(string id);
        // Update order:
        public void UpdateOrder(Order order);
        // Delete order:
        public void DeleteOrder(Order order);


        // CRUD operations for OrderItem entity:
        // Create order item:
        public void CreateOrderItem(OrderItem orderItem);
        // Read order item:
        public OrderItem? GetOrderItem(int id);
        // Read all order items:
        public List<OrderItem>? GetOrderItemsById(string id);
        public List<OrderItem>? GetOrderItemsByGameId(int id);
        // Update order item:
        public void UpdateOrderItem(OrderItem orderItem);
        // Delete order item:
        public void DeleteOrderItem(OrderItem orderItem);


        // CRUD operations for FriendConnect entity:
        // Create friend connect:
        public void CreateFriendConnect(FriendConnect friendConnect);
        // Read friend connect:
        public FriendConnect? GetFriendConnect(int id);
        // Read all friend connects:
        public List<FriendConnect>? GetFriendConnectsById(string id);
        // Update friend connect:
        public void UpdateFriendConnect(FriendConnect friendConnect);
        // Delete friend connect:
        public void DeleteFriendConnect(FriendConnect friendConnect);


        // CRUD operations for FavouritePlatform entity:
        // Create favourite platform:
        public void CreateFavouritePlatform(FavouritePlatform favouritePlatform);
        // Read all favourite platforms:
        public List<FavouritePlatform>? GetFavouritePlatformsById(string id);
        // Delete favourite platform:
        public void DeleteFavouritePlatform(FavouritePlatform favouritePlatform);


        // CRUD operations for ShippingAddress entity:
        // Create shipping address:
        public void CreateShippingAddress(ShippingAddress shippingAddress);
        // Read all shipping addresses:
        public List<ShippingAddress>? GetShippingAddressesById(string id);
        // Read address:
        public ShippingAddress? GetAddressById(int id);
        // Update shipping address:
        public void UpdateShippingAddress(ShippingAddress shippingAddress);
        // Delete shipping address:
        public void DeleteShippingAddress(ShippingAddress shippingAddress);


        // Read all platforms:
        public List<Platform>? GetAllPlatforms();
        // Read all genres:
        public List<Genre>? GetAllGenres();
        // Read all languages:
        public List<Language>? GetAllLanguages();

        // Create GameGenre:
        public void CreateGameGenre(GameGenre gameGenre);
        // Delete GameGenre:
        public void DeleteGameGenre(GameGenre gameGenre);
        // Create GameLanguage:
        public void CreateGameLanguage(GameLanguage gameLanguage);
        // Delete GameLanguage:
        public void DeleteGameLanguage(GameLanguage gameLanguage);
        // Create GamePlatform:
        public void CreateGamePlatform(GamePlatform gamePlatform);
        // Delete GamePlatform:
        public void DeleteGamePlatform(GamePlatform gamePlatform);


        // CRUD operations for Picture entity:
        // Create Picture:
        public void CreatePicture(IFormFile image, Picture picture);
        // Read Picture:
        public Picture? GetPictureById(int id);


        // CRUD operations for Photo entity:
        // Create Photo:
        public void CreatePhoto(IFormFile image, Photo photo);
        // Read Photo:
        public Photo? GetPhotoById(int photoId);


        // CRUD operations for Event entity:
        // Create Event:
        public void CreateEvent(Event eventEntity);
        // Read Event:
        public Event? GetEventById(int id);
        // Read all Events:
        public List<Event>? GetAllEvents();
        // Update Event:
        public void UpdateEvent(Event eventEntity);
        // Delete Event:
        public void DeleteEvent(Event eventEntity);


        // CRUD operations for EventRegistration entity:
        // Create EventRegistration:
        public void CreateEventRegistration(EventRegistration eventRegistration);
        // Read EventRegistration:
        public EventRegistration? GetEventRegistrationById(string id);
        // Read all EventRegistrations:
        public List<EventRegistration>? GetAllEventRegistrations();
        public List<EventRegistration>? GetAllEventRegistrationsByUserId(string id);
        // Update EventRegistration:
        public void UpdateEventRegistration(EventRegistration eventRegistration);
        // Delete EventRegistration:
        public void DeleteEventRegistration(EventRegistration eventRegistration);


        // CRUD operations for review entity:
        // Create review:
        public void CreateReview(Review review);
        // Read review:
        public Review? GetReviewById(int id);
        public List<Review>? GetAllReviews();
        // Read all reviews by userId:
        public List<Review>? GetAllReviewsByUserId(string id);
        // Read all reviews by gameId:
        public List<Review>? GetAllReviewsByGameId(int id);
        // Update review:
        public void UpdateReview(Review review);
        // Delete review:
        public void DeleteReview(Review review);


        // CRUD operations for rating entity:
        // Create rating:
        public void CreateRating(Rating rating);
        // Read rating:
        public Rating? GetRatingById(int id);
        // Read all ratings:
        public List<Rating>? GetAllRatings();
        // Read all ratings by userId:
        public List<Rating>? GetAllRatingsByUserId(string id);
        // Read all ratings by gameId:
        public List<Rating>? GetAllRatingsByGameId(int id);
        // Update rating:
        public void UpdateRating(Rating rating);
        // Delete rating:
        public void DeleteRating(Rating rating);


        public byte[] ConvertImageToBytes(IFormFile image);

    }
}
