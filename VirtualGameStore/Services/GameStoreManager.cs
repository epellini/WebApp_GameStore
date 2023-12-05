using Microsoft.EntityFrameworkCore;
using VirtualGameStore.DataAccess;
using VirtualGameStore.Entities;

namespace VirtualGameStore.Services
{
    // GameStoreManager implements an interface
    // Don't forget to add method signatures to interface too!
    public class GameStoreManager : IGameStoreManager
    {
        // Constructor to initialize DbContext field:
        public GameStoreManager(GameStoreDbContext gameStoreDbContext)
        {
            _gameStoreDbContext = gameStoreDbContext;
        }

        // Implement all interface methods:

        // CRUD operations for game entity:
        // Create Game:
        public void CreateGame(Game game)
        {
            _gameStoreDbContext.Games.Add(game);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all Games and sort:
        /// <summary>
        /// Get all the games in the database including their platforms, genres, languages, and pictures.
        /// </summary>
        /// <returns>A list of Game objects</returns>
        public List<Game> GetAllGames(string sort)
        {
            List<Game> games = new List<Game>();
            if (string.IsNullOrEmpty(sort)) sort = "New";
            if (sort == "New")
            {
                games = _gameStoreDbContext.Games
                    .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                    .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                    .Include(g => g.Languages).ThenInclude(l => l.Language)
                    .Include(g => g.Pictures)
                    .OrderByDescending(g => g.ReleaseDate)
                    .ToList();
            }
            if (sort == "Popular")
            {
                games = _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .OrderByDescending(g => g.Reviews.Count())
                .ToList();
            }
            if (sort == "Top")
            {
                games = _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .OrderByDescending(g => g.Ratings.Average(r => r.RatingValue))
                .ToList();
            }
            if (sort == "Alphabetical")
            {
                games = _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .OrderBy(g => g.Name)
                .ToList();
            }
            return games;
        }
        // Read all Games by search query:
        public List<Game> GetGamesBySearch(string query)
        {
            if (query != null)
            {
                return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Pictures)
                .Where(g => g.Name.Contains(query))
                .OrderByDescending(g => g.Name.StartsWith(query))
                .ThenBy(g => g.Name)
                .ToList();
            }
            else
            {
                return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Pictures)
                .OrderByDescending(g => g.ReleaseDate)
                .ToList();
            }
        }
        // Read Game:
        /// <summary>
        /// Get a single game from the database including its platforms, genres, languages, and pictures.
        /// </summary>
        /// <param name="id">The desired game's ID</param>
        /// <returns>A Game object</returns>
        public Game? GetGameById(int id)
        {
            return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .Where(g => g.GameId == id)
                .FirstOrDefault();
        }
        // Update Game:
        public void UpdateGame(Game game)
        {
            _gameStoreDbContext.Games.Update(game);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete Game:
        public void DeleteGame(Game game)
        {
            _gameStoreDbContext.Remove(game);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for Profile entity:
        // Create Profile:
        public void CreateProfile(Profile profile)
        {
            _gameStoreDbContext.Profiles.Add(profile);
            _gameStoreDbContext.SaveChanges();
        }
        // Read Profile:
        public Profile GetProfileById(string id)
        {
            return _gameStoreDbContext.Profiles
                .Include(p => p.Photos)
                .Where(p => p.UserId == id).FirstOrDefault();
        }
        // Update Profile:
        public void UpdateProfile(Profile profile)
        {
            if (GetProfileById(profile.UserId) == null)
            {
                CreateProfile(profile);
            }
            else
            {
                _gameStoreDbContext.Profiles.Update(profile);
                _gameStoreDbContext.SaveChanges();
            }
        }


        // CRUD operations for preferred languages entity:
        // Create preferred language:
        public void CreatePreferredLanguage(PreferredLanguage preferredLanguage)
        {
            _gameStoreDbContext.PreferredLanguages.Add(preferredLanguage);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all preferred languages:
        public List<PreferredLanguage>? GetPreferredLanguagesById(string id)
        {
            return _gameStoreDbContext.PreferredLanguages.Include(pl => pl.Language).Where(pl => pl.UserId == id).ToList();
        }
        // Delete preferred language:
        public void DeletePreferredLanguage(PreferredLanguage preferredLanguage)
        {
            _gameStoreDbContext.PreferredLanguages.Remove(preferredLanguage);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for favourite genres entity:
        // Create favourite genre:
        public void CreateFavouriteGenre(FavouriteGenre favouriteGenre)
        {
            _gameStoreDbContext.FavouriteGenres.Add(favouriteGenre);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all favourite genres:
        public List<FavouriteGenre>? GetFavouriteGenresById(string id)
        {
            return _gameStoreDbContext.FavouriteGenres.Include(fg => fg.Genre).Where(fg => fg.UserId == id).ToList();
        }
        // Delete favourite genre:
        public void DeleteFavouriteGenre(FavouriteGenre favouriteGenre)
        {
            _gameStoreDbContext.FavouriteGenres.Remove(favouriteGenre);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for wished games entity:
        // Create wished game:
        public void CreateWishedGame(WishedGame wishedGame)
        {
            _gameStoreDbContext.WishedGames.Add(wishedGame);
            _gameStoreDbContext.SaveChanges();
        }
        // Read wished game:
        public WishedGame? GetWishedGame(int id)
        {
            return _gameStoreDbContext.WishedGames.Include(wg => wg.Game).Include(wg => wg.User).Where(wg => wg.WishedGameId == id).FirstOrDefault();
        }
        // Read all wished games:
        public List<WishedGame>? GetWishedGamesById(string id)
        {
            return _gameStoreDbContext.WishedGames.Include(wg => wg.Game).ThenInclude(g => g.Pictures).Where(wg => wg.UserId == id).ToList();
        }
        public List<WishedGame>? GetWishedGamesByGameId(int id)
        {
            return _gameStoreDbContext.WishedGames.Include(wg => wg.Game).Where(wg => wg.GameId == id).ToList();
        }
        // Delete wished game:
        public void DeleteWishedGame(WishedGame wishedGame)
        {
            _gameStoreDbContext.WishedGames.Remove(wishedGame);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for Order entity:
        // Create order:
        public void CreateOrder(Order order)
        {
            _gameStoreDbContext.Orders.Add(order);
            _gameStoreDbContext.SaveChanges();
        }
        // Read order:
        public Order? GetOrder(int id)
        {
            return _gameStoreDbContext.Orders.Include(o => o.ShippingAddress).Include(o => o.Items).Where(o => o.OrderId == id).FirstOrDefault();
        }
        // Read all orders:
        public List<Order>? GetOrdersById(string id)
        {
            return _gameStoreDbContext.Orders.Include(o => o.ShippingAddress).Include(o => o.Items).ToList();
        }
        // Update order:
        public void UpdateOrder(Order order)
        {
            _gameStoreDbContext.Orders.Update(order);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete order:
        public void DeleteOrder(Order order)
        {
            _gameStoreDbContext.Orders.Remove(order);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for OrderItem entity:
        // Create order item:
        public void CreateOrderItem(OrderItem orderItem)
        {
            _gameStoreDbContext.OrderItems.Add(orderItem);
            _gameStoreDbContext.SaveChanges();
        }
        // Read order item:
        public OrderItem? GetOrderItem(int id)
        {
            return _gameStoreDbContext.OrderItems.Include(oi => oi.Order).Include(oi => oi.Game).Where(oi => oi.OrderItemId == id).FirstOrDefault();
        }
        // Read all order items:
        public List<OrderItem>? GetOrderItemsById(string id)
        {
            return _gameStoreDbContext.OrderItems.Include(oi => oi.Order).Include(oi => oi.Game).ToList();
        }
        public List<OrderItem>? GetOrderItemsByGameId(int id)
        {
            return _gameStoreDbContext.OrderItems.Include(oi => oi.Order).Include(oi => oi.Game).Where(oi => oi.GameId == id).ToList();
        }
        // Update order item:
        public void UpdateOrderItem(OrderItem orderItem)
        {
            _gameStoreDbContext.OrderItems.Update(orderItem);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete order item:
        public void DeleteOrderItem(OrderItem orderItem)
        {
            _gameStoreDbContext.OrderItems.Remove(orderItem);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for friendConnect entity:
        // Create friendConnect:
        public void CreateFriendConnect(FriendConnect friendConnect)
        {
            _gameStoreDbContext.FriendConnects.Add(friendConnect);
            _gameStoreDbContext.SaveChanges();
        }
        // Read friendConnect:
        public FriendConnect? GetFriendConnect(int id)
        {
            return _gameStoreDbContext.FriendConnects.Include(fc => fc.User).Include(fc => fc.Friend).ThenInclude(f => f.Profile).ThenInclude(p => p.Photos).Where(fc => fc.FriendConnectId == id).FirstOrDefault();
        }
        // Read all friendConnects:
        public List<FriendConnect>? GetFriendConnectsById(string id)
        {
            return _gameStoreDbContext.FriendConnects.Include(fc => fc.User).Include(fc => fc.Friend).ThenInclude(f => f.Profile).ThenInclude(p => p.Photos).Where(fc => fc.UserId == id).ToList();
        }
        // Update friendConnect:
        public void UpdateFriendConnect(FriendConnect friendConnect)
        {
            _gameStoreDbContext.FriendConnects.Update(friendConnect);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete friendConnect:
        public void DeleteFriendConnect(FriendConnect friendConnect)
        {
            _gameStoreDbContext.FriendConnects.Remove(friendConnect);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for favourite platforms entity:
        // Create favourite platform:
        public void CreateFavouritePlatform(FavouritePlatform favouritePlatform)
        {
            _gameStoreDbContext.FavouritePlatforms.Add(favouritePlatform);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all favourite platforms:
        public List<FavouritePlatform>? GetFavouritePlatformsById(string id)
        {
            return _gameStoreDbContext.FavouritePlatforms.Include(fp => fp.Platform).Where(fp => fp.UserId == id).ToList();
        }
        // Delete favourite platform:
        public void DeleteFavouritePlatform(FavouritePlatform favouritePlatform)
        {
            _gameStoreDbContext.FavouritePlatforms.Remove(favouritePlatform);
            _gameStoreDbContext.SaveChanges();
        }

        // CRUD operations for Shipping address entity:
        // Create Shipping address:
        public void CreateShippingAddress(ShippingAddress shippingAddress)
        {
            _gameStoreDbContext.ShippingAddresses.Add(shippingAddress);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all Shipping addresses:
        public List<ShippingAddress>? GetShippingAddressesById(string id)
        {
            return _gameStoreDbContext.ShippingAddresses.Where(s => s.UserId == id).ToList();
        }
        // Read address:
        public ShippingAddress? GetAddressById(int id)
        {
            return _gameStoreDbContext.ShippingAddresses.Where(a => a.ShippingAddressId == id).FirstOrDefault();
        }
        // Update Shipping address:
        public void UpdateShippingAddress(ShippingAddress shippingAddress)
        {
            _gameStoreDbContext.ShippingAddresses.Update(shippingAddress);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete Shipping address:
        public void DeleteShippingAddress(ShippingAddress shippingAddress)
        {
            _gameStoreDbContext.ShippingAddresses.Remove(shippingAddress);
            _gameStoreDbContext.SaveChanges();
        }


        // Read all Platforms:
        public List<Platform>? GetAllPlatforms()
        {
            return _gameStoreDbContext.Platforms.ToList();
        }
        // Read all genres:
        public List<Genre>? GetAllGenres()
        {
            return _gameStoreDbContext.Genres.ToList();
        }
        // Read all languages:
        public List<Language>? GetAllLanguages()
        {
            return _gameStoreDbContext.Languages.ToList();
        }

        // Create GameGenre:
        public void CreateGameGenre(GameGenre gameGenre)
        {
            _gameStoreDbContext.GameGenres.Add(gameGenre);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete GameGenre:
        public void DeleteGameGenre(GameGenre gameGenre)
        {
            _gameStoreDbContext.GameGenres.Remove(gameGenre);
            _gameStoreDbContext.SaveChanges();
        }

        // Create GameLanguage:
        public void CreateGameLanguage(GameLanguage gameLanguage)
        {
            _gameStoreDbContext.GameLanguages.Add(gameLanguage);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete GameLanguage:
        public void DeleteGameLanguage(GameLanguage gameLanguage)
        {
            _gameStoreDbContext.GameLanguages.Remove(gameLanguage);
            _gameStoreDbContext.SaveChanges();
        }
        // Create GamePlatform:
        public void CreateGamePlatform(GamePlatform gamePlatform)
        {
            _gameStoreDbContext.GamePlatforms.Add(gamePlatform);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete GamePlatform:
        public void DeleteGamePlatform(GamePlatform gamePlatform)
        {
            _gameStoreDbContext.GamePlatforms.Remove(gamePlatform);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for Picture entity:
        // Create Picture:
        public void CreatePicture(IFormFile image, Picture picture)
        {
            picture.Image = ConvertImageToBytes(image);
            _gameStoreDbContext.Pictures.Add(picture);
            _gameStoreDbContext.SaveChanges();
        }
        // Read Picture:
        public Picture? GetPictureById(int id)
        {
            return _gameStoreDbContext.Pictures
                .Where(p => p.PictureId == id)
                .FirstOrDefault();
        }


        // CRUD operations for Photo entity:
        // Create Photo:
        public void CreatePhoto(IFormFile image, Photo photo)
        {
            photo.Image = ConvertImageToBytes(image);
            _gameStoreDbContext.Photos.Add(photo);
            _gameStoreDbContext.SaveChanges();
        }
        // Read Photo:
        public Photo? GetPhotoById(int photoId)
        {
            return _gameStoreDbContext.Photos
                .Where(p => p.PhotoId == photoId)
                .FirstOrDefault();
        }


        public byte[] ConvertImageToBytes(IFormFile image)
        {
            byte[] imageBytes;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            imageBytes = reader.ReadBytes((int)image.Length);
            return imageBytes;
        }

        public Cart? GetCartById(string id)
        {
            return _gameStoreDbContext.Carts.Where(sc => sc.UserId == id).Include(sc => sc.Items).FirstOrDefault();
        }

        public CartItem? GetCartItemById(int id)
        {
            return _gameStoreDbContext.CartItems.Where(ci => ci.CartItemId == id).FirstOrDefault();
        }

        public void AddItemToCart(CartItem cartItem)
        {
            _gameStoreDbContext.CartItems.Add(cartItem);
            _gameStoreDbContext.SaveChanges();
        }
        public void RemoveItemFromCart(CartItem cartItem)
        {
            _gameStoreDbContext.CartItems.Remove(cartItem);
            _gameStoreDbContext.SaveChanges();
        }

        public void CreateCart(Cart cart)
        {
            _gameStoreDbContext.Carts.Add(cart);
            _gameStoreDbContext.SaveChanges();
        }

        // CRUD operations for Event entity:
        // Create Event:
        public void CreateEvent(Event eventEntity)
        {
            _gameStoreDbContext.Events.Add(eventEntity);
            _gameStoreDbContext.SaveChanges();
        }
        // Read Event:
        public Event? GetEventById(int id)
        {
            return _gameStoreDbContext.Events.Include(e => e.EventRegistrations).ThenInclude(er => er.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Where(e => e.EventId == id).FirstOrDefault();
        }
        // Read all Events:
        public List<Event>? GetAllEvents()
        {
            return _gameStoreDbContext.Events.Include(e => e.EventRegistrations).ThenInclude(er => er.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).ToList();
        }
        // Update Event:
        public void UpdateEvent(Event eventEntity)
        {
            _gameStoreDbContext.Events.Update(eventEntity);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete Event:
        public void DeleteEvent(Event eventEntity)
        {
            _gameStoreDbContext.Events.Remove(eventEntity);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for EventRegistration entity:
        // Create EventRegistration:
        public void CreateEventRegistration(EventRegistration eventRegistration)
        {
            _gameStoreDbContext.EventRegistrations.Add(eventRegistration);
            _gameStoreDbContext.SaveChanges();
        }
        // Read EventRegistration:
        public EventRegistration? GetEventRegistrationById(string id)
        {
            return _gameStoreDbContext.EventRegistrations.Include(er => er.Event).Include(er => er.User).Where(er => er.UserId == id).FirstOrDefault();
        }
        // Read all EventRegistrations:
        public List<EventRegistration>? GetAllEventRegistrations()
        {
            return _gameStoreDbContext.EventRegistrations.Include(er => er.Event).Include(er => er.User).ToList();
        }
        public List<EventRegistration>? GetAllEventRegistrationsByUserId(string id)
        {
            return _gameStoreDbContext.EventRegistrations.Include(er => er.Event).Include(er => er.User).Where(er => er.UserId == id).ToList();
        }
        // Update EventRegistration:
        public void UpdateEventRegistration(EventRegistration eventRegistration)
        {
            _gameStoreDbContext.EventRegistrations.Update(eventRegistration);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete EventRegistration:
        public void DeleteEventRegistration(EventRegistration eventRegistration)
        {
            _gameStoreDbContext.EventRegistrations.Remove(eventRegistration);
            _gameStoreDbContext.SaveChanges();
        }

        // CRUD operations for review entity:
        // Create review:
        public void CreateReview(Review review)
        {
            _gameStoreDbContext.Reviews.Add(review);
            _gameStoreDbContext.SaveChanges();
        }
        // Read review:
        public Review? GetReviewById(int id)
        {
            return _gameStoreDbContext.Reviews.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).Where(r => r.ReviewId == id).FirstOrDefault();
        }
        // Read all reviews:
        public List<Review>? GetAllReviews()
        {
            return _gameStoreDbContext.Reviews.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).ToList();
        }
        // Read all reviews by userId:
        public List<Review>? GetAllReviewsByUserId(string id)
        {
            return _gameStoreDbContext.Reviews.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).Where(r => r.UserId == id).ToList();
        }
        // Read all reviews by gameId:
        public List<Review>? GetAllReviewsByGameId(int id)
        {
            return _gameStoreDbContext.Reviews.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).Where(r => r.GameId == id).ToList();
        }
        // Update review:
        public void UpdateReview(Review review)
        {
            _gameStoreDbContext.Reviews.Update(review);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete review:
        public void DeleteReview(Review review)
        {
            _gameStoreDbContext.Reviews.Remove(review);
            _gameStoreDbContext.SaveChanges();
        }


        // CRUD operations for rating entity:
        // Create rating:
        public void CreateRating(Rating rating)
        {
            _gameStoreDbContext.Ratings.Add(rating);
            _gameStoreDbContext.SaveChanges();
        }
        // Read rating:
        public Rating? GetRatingById(int id)
        {
            return _gameStoreDbContext.Ratings.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).Where(r => r.RatingId == id).FirstOrDefault();
        }
        // Read all ratings:
        public List<Rating>? GetAllRatings()
        {
            return _gameStoreDbContext.Ratings.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).ToList();
        }
        // Read all ratings by userId:
        public List<Rating>? GetAllRatingsByUserId(string id)
        {
            return _gameStoreDbContext.Ratings.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).Where(r => r.UserId == id).ToList();
        }
        // Read all ratings by gameId:
        public List<Rating>? GetAllRatingsByGameId(int id)
        {
            return _gameStoreDbContext.Ratings.Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(p => p.Photos).Include(r => r.Game).Where(r => r.GameId == id).ToList();
        }
        // Update rating:
        public void UpdateRating(Rating rating)
        {
            _gameStoreDbContext.Ratings.Update(rating);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete rating:
        public void DeleteRating(Rating rating)
        {
            _gameStoreDbContext.Ratings.Remove(rating);
            _gameStoreDbContext.SaveChanges();
        }


        



        // private DbContext field
        private GameStoreDbContext _gameStoreDbContext;
    }
}
