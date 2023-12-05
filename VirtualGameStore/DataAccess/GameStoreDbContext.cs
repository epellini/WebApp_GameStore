using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtualGameStore.Entities;

namespace VirtualGameStore.DataAccess
{
    public class GameStoreDbContext : IdentityDbContext<User>
    {
        // Add a private field to store the web host environment:
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor with DbContext options (so EFCore can connect to DB): 
        public GameStoreDbContext(DbContextOptions options, IWebHostEnvironment webHostEnvironment) : base(options)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // Insert an admin user into the database (method called in program.cs):
        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Temporarily replace the password validator with my custom validator:
            userManager.PasswordValidators.Clear();
            userManager.PasswordValidators.Add(new CustomPasswordValidator());

            string username = "admin";
            string password = "admin";
            string roleName = "Admin";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                User user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }

            // Restore the default password validator:
            userManager.PasswordValidators.Add(new PasswordValidator<User>());
        }

        // Generate an array of bytes from an image file located in wwwroot/images:
        private byte[] GetBytes(string fileName)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

            // Check that the file exists:
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Image not found");
            }

            // Read all the bytes and return as byte array:
            return File.ReadAllBytes(path);
        }

        // DbSets of every entity (used to query and save instances of entities):
        public DbSet<Game>? Games { get; set; }
        public DbSet<Genre>? Genres { get; set; }
        public DbSet<Language>? Languages { get; set; }
        public DbSet<Platform>? Platforms { get; set; }
        public DbSet<FavouriteGenre>? FavouriteGenres { get; set; }
        public DbSet<FavouritePlatform>? FavouritePlatforms { get; set; }
        public DbSet<PreferredLanguage>? PreferredLanguages { get; set; }
        public DbSet<GameGenre>? GameGenres { get; set; }
        public DbSet<GameLanguage>? GameLanguages { get; set; }
        public DbSet<GamePlatform>? GamePlatforms { get; set; }
        public DbSet<Profile>? Profiles { get; set; }
        public DbSet<ShippingAddress>? ShippingAddresses { get; set; }
        public DbSet<Picture>? Pictures { get; set; }
        public DbSet<Photo>? Photos { get; set; }
        public DbSet<WishedGame> WishedGames { get; set; }
        public DbSet<FriendConnect> FriendConnects { get; set; }
        public DbSet<Event>? Events { get; set; }
        public DbSet<EventRegistration>? EventRegistrations { get; set; }
        public DbSet<Rating>? Ratings { get; set; }
        public DbSet<Review>? Reviews { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<OrderItem>? OrderItems { get; set; }
        public DbSet<Cart>? Carts { get; set; }
        public DbSet<CartItem>? CartItems { get; set; }
        public DbSet<Province>? Provinces { get; set; }
        public DbSet<Country>? Country { get; set; }


        // Override base class method OnModelCreating to establish DB relationships
        // or seed the DB or generate primary keys as a composites of 2 foreign keys:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Establish PK for Language
            modelBuilder.Entity<Language>()
                .HasKey(l => l.LanguageId);

            // Establish FavouriteGenre relationships:
            // Each FavouriteGenre has 1 user - Each user could have many FavouriteGenres - FavouriteGenre has FK to user:
            modelBuilder.Entity<FavouriteGenre>()
                .HasOne(fg => fg.User)
                .WithMany(u => u.Genres)
                .HasForeignKey(fg => fg.UserId);
            // Each FavouriteGenre has 1 genre - Each genre could have many FavouriteGenres - FavouriteGenre has FK to genre:
            modelBuilder.Entity<FavouriteGenre>()
                .HasOne(fg => fg.Genre)
                .WithMany(g => g.Users)
                .HasForeignKey(fg => fg.GenreId);

            // Establish WishedGame relationships:
            // Each WishedGame has 1 user - Each user could have many WishedGames - WishedGame has FK to user:
            modelBuilder.Entity<WishedGame>()
                .HasOne(wg => wg.User)
                .WithMany(u => u.WishedGames)
                .HasForeignKey(wg => wg.UserId);
            // Each WishedGame has 1 game - Each game could have many WishedGames - WishedGame has FK to game:
            modelBuilder.Entity<WishedGame>()
                .HasOne(wg => wg.Game)
                .WithMany(g => g.WishedGames)
                .HasForeignKey(wg => wg.GameId);

            // Establish FriendConnect relationships:
            // Each FriendConnect has 1 user - Each user could have many Friends - FriendConnect has FK to user:
            modelBuilder.Entity<FriendConnect>()
                .HasOne(fc => fc.User)
                .WithMany(ui => ui.Friends)
                .HasForeignKey(fc => fc.UserId);
            // Each FriendConnect has 1 friend - Each friend could have many connects - FriendConnect has FK to friend:
            modelBuilder.Entity<FriendConnect>()
                .HasOne(fc => fc.Friend)
                .WithMany(uii => uii.Connects)
                .HasForeignKey(fc => fc.FriendId)
                .OnDelete(DeleteBehavior.NoAction);

            // Establish FavouritePlatform relationships:
            // Each FavouritePlatform has 1 user - Each user could have many FavouritePlatforms - FavouritePlatform has FK to user:
            modelBuilder.Entity<FavouritePlatform>()
                .HasOne(fp => fp.User)
                .WithMany(u => u.Platforms)
                .HasForeignKey(fp => fp.UserId);
            // Each FavouritePlatform has 1 platform - Each platform could have many FavouritePlatforms - FavouritePlatform has FK to platform:
            modelBuilder.Entity<FavouritePlatform>()
                .HasOne(fp => fp.Platform)
                .WithMany(p => p.Users)
                .HasForeignKey(fp => fp.PlatformId);

            // Establish PreferredLanguage relationships:
            // Each PreferredLanguage has 1 user - Each user could have many PreferredLanguages - PreferredLanguage has FK to user:
            modelBuilder.Entity<PreferredLanguage>()
                .HasOne(pl => pl.User)
                .WithMany(u => u.Languages)
                .HasForeignKey(pl => pl.UserId);
            // Each PreferredLanguage has 1 language - Each language could have many PreferredLanguages - PreferredLanguage has FK to language:
            modelBuilder.Entity<PreferredLanguage>()
                .HasOne(pl => pl.Language)
                .WithMany(l => l.Users)
                .HasForeignKey(pl => pl.LanguageId);

            // Establish GameGenre relationships:
            // Each GameGenre has 1 game - Each game could have many GameGenres - GameGenre has FK to game:
            modelBuilder.Entity<GameGenre>()
                .HasOne(gg => gg.Game)
                .WithMany(g => g.Genres)
                .HasForeignKey(gg => gg.GameId);
            // Each GameGenre has 1 genre - Each genre could have many GameGenres - GameGenre has FK to genre:
            modelBuilder.Entity<GameGenre>()
                .HasOne(gg => gg.Genre)
                .WithMany(g => g.Games)
                .HasForeignKey(gg => gg.GenreId);

            // Establish GamePlatform relationships:
            // Set composite PK for GamePlatform:
            modelBuilder.Entity<GamePlatform>()
                .HasKey(gp => new { gp.GameId, gp.PlatformId });
            // Each GamePlatform has 1 game - Each game could have many GamePlatforms - GamePlatform has FK to game:
            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.Platforms)
                .HasForeignKey(gp => gp.GameId);
            // Each GamePlatform has 1 platform - Each platform could have many GamePlatforms - GamePlatform has FK to platform:
            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Platform)
                .WithMany(p => p.Games)
                .HasForeignKey(gp => gp.PlatformId);

            // Establish GameLanguage relationships:
            // Each GameLanguage has 1 game - Each game could have many GameLanguages - GameLanguage has FK to game:
            modelBuilder.Entity<GameLanguage>()
                .HasOne(gl => gl.Game)
                .WithMany(g => g.Languages)
                .HasForeignKey(gl => gl.GameId);
            // Each GameLanguage has 1 language - Each language could have many GameLanguages - GameLanguage has FK to language:
            modelBuilder.Entity<GameLanguage>()
                .HasOne(gl => gl.Language)
                .WithMany(l => l.Games)
                .HasForeignKey(gl => gl.LanguageId);

            // Establish Profile relationships:
            // Each Profile has 1 user - Each user could have only one Profile - Profile has FK to user:
            modelBuilder.Entity<Profile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<Profile>(p => p.UserId);

            // Establish ShippingAddress relationships:
            // Each ShippingAddress has 1 user - Each user could have many ShippingAddresses - ShippingAddress has FK to user:
            modelBuilder.Entity<ShippingAddress>()
                .HasOne(sa => sa.User)
                .WithMany(u => u.ShippingAddresses)
                .HasForeignKey(sa => sa.UserId);

            // Establish Picture relationships:
            // Each Picture has 1 game - Each game could have many Pictures - Picture has FK to game:
            modelBuilder.Entity<Picture>()
                .HasOne(p => p.Game)
                .WithMany(g => g.Pictures)
                .HasForeignKey(p => p.GameId);

            // Establish Photo relationships:
            // Each Photo has 1 Profile - Each Profile could have many Photos - Photo has FK to Profile:
            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Profile)
                .WithMany(pr => pr.Photos)
                .HasForeignKey(p => p.ProfileId);

            // Establish Cart relationships:
            // Each Cart has 1 user - Each user could have only one Cart - Cart has FK to user:
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId);

            // Establish CartItem relationships:
            // Each CartItem has 1 cart - Each cart could have many CartItems - CartItem has FK to cart:
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            // Establish Order relationships:
            // Each Order has 1 user - Each user could have many Orders - Order has FK to user:
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);
            // Each Order has 1 shipping address - Each shipping address could have many Orders - Order has FK to shipping address:
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany(sa => sa.Orders)
                .HasForeignKey(o => o.ShippingAddressId);

            // Establish OrderItem relationships:
            // Each OrderItem has 1 order - Each order could have many OrderItems - OrderItem has FK to order:
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            // Establish Review relationships:
            // Each Review has 1 game - Each game could have many Reviews - Review has FK to game:
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Game)
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GameId);
            // Each Review has 1 user - Each user could have many Reviews - Review has FK to user:
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            // Establish Rating relationships:
            // Each Rating has 1 game - Each game could have many Ratings - Rating has FK to game:
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Game)
                .WithMany(g => g.Ratings)
                .HasForeignKey(r => r.GameId);
            // Each Rating has 1 user - Each user could have many Ratings - Rating has FK to user:
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId);

            // Establish EventRegistration relationships:
            // Each EventRegistration has 1 event - Each event could have many EventRegistrations - EventRegistration has FK to event:
            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.Event)
                .WithMany(e => e.EventRegistrations)
                .HasForeignKey(er => er.EventId);
            // Each EventRegistration has 1 user - Each user could have many EventRegistrations - EventRegistration has FK to user:
            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.User)
                .WithMany(u => u.EventRegistrations)
                .HasForeignKey(er => er.UserId);

            // Seed the Platforms table:
            modelBuilder.Entity<Platform>().HasData(
                new Platform()
                {
                    PlatformId = 1,
                    PlatformName = "Windows"
                },
                new Platform()
                {
                    PlatformId = 2,
                    PlatformName = "Mac"
                },
                new Platform()
                {
                    PlatformId = 3,
                    PlatformName = "Linux"
                },
                new Platform()
                {
                    PlatformId = 4,
                    PlatformName = "MS-DOS"
                });

            // Seed the Genres table:
            modelBuilder.Entity<Genre>().HasData(
                new Genre()
                {
                    GenreId = 1,
                    GenreName = "Action"
                },
                new Genre()
                {
                    GenreId = 2,
                    GenreName = "Adventure"
                },

                new Genre()
                {
                    GenreId = 3,
                    GenreName = "Arcade"
                },
                new Genre()
                {
                    GenreId = 4,
                    GenreName = "Co-op"
                },
                new Genre()
                {
                    GenreId = 5,
                    GenreName = "FPS"
                },
                new Genre()
                {
                    GenreId = 6,
                    GenreName = "Horror"
                },
                new Genre()
                {
                    GenreId = 7,
                    GenreName = "MMO"
                },
                new Genre()
                {
                    GenreId = 8,
                    GenreName = "Puzzle"
                },
                new Genre()
                {
                    GenreId = 9,
                    GenreName = "Racing"
                },
                new Genre()
                {
                    GenreId = 10,
                    GenreName = "RPG"
                },
                new Genre()
                {
                    GenreId = 11,
                    GenreName = "Simulation"
                },
                new Genre()
                {
                    GenreId = 12,
                    GenreName = "Sports"
                },
                new Genre()
                {
                    GenreId = 13,
                    GenreName = "Strategy"
                },
                new Genre()
                {
                    GenreId = 14,
                    GenreName = "Survival"
                },
                new Genre()
                {
                    GenreId = 15,
                    GenreName = "VR"
                });

            //Seed the Languages table:
            modelBuilder.Entity<Language>().HasData(
                new Language()
                {
                    LanguageId = "en",
                    LanguageName = "English",
                },
                new Language()
                {
                    LanguageId = "fr",
                    LanguageName = "French",
                },
                new Language()
                {
                    LanguageId = "es",
                    LanguageName = "Spanish",
                });

            //Seed the Pictures table:
            modelBuilder.Entity<Picture>().HasData(
                new Picture
                {
                    PictureId = 1,
                    GameId = 1,
                    Title = "Doom Cover Art",
                    AltText = "Cover image for the 1993 game, Doom",
                    IsCoverArt = true,
                    Image = GetBytes("doom-cover.jpg")
                },
                new Picture
                {
                    PictureId = 2,
                    GameId = 2,
                    Title = "RuneScape Cover Art",
                    AltText = "Cover image for the 2001 game, RuenScape",
                    IsCoverArt = true,
                    Image = GetBytes("runescape-cover.jpg")
                },
                new Picture
                {
                    PictureId = 3,
                    GameId = 3,
                    Title = "Minecraft Cover Art",
                    AltText = "Cover image for the 2011 game, Minecraft",
                    IsCoverArt = true,
                    Image = GetBytes("minecraft-cover.png")
                },
                new Picture
                {
                    PictureId = 4,
                    GameId = 4,
                    Title = "RollerCoaster Tycoon Cover Art",
                    AltText = "Cover image for the 1999 game, RollerCoaster Tycoon",
                    IsCoverArt = true,
                    Image = GetBytes("rollercoaster-tycoon-cover.png")
                });

            // Seed the Games table:
            modelBuilder.Entity<Game>().HasData(
                new Game()
                {
                    GameId = 1,
                    Name = "Doom",
                    Description = "You’re a marine—one of Earth’s best—recently assigned to the Union Aerospace Corporation (UAC) research facility on Mars. When an experiment malfunctions and creates a portal to Hell, the base is overrun by blood-thirsty demons. You must shoot your way out to survive.",
                    Developer = "id Software",
                    ReleaseDate = new DateTime(1993, 12, 10),
                    RetailPrice = 4.99
                },
                new Game()
                {
                    GameId = 2,
                    Name = "RuneScape",
                    Description = "Explore an ever changing and evolving living world where new challenges, skills, and quests await. Featuring unprecedented player freedom, you choose how to play, adventure, and grow.",
                    Developer = "Jagex",
                    ReleaseDate = new DateTime(2001, 01, 04),
                    RetailPrice = 0.0
                },
                new Game()
                {
                    GameId = 3,
                    Name = "Minecraft",
                    Description = "Explore randomly generated worlds and build amazing things from the simplest of homes to the grandest of castles. Play in creative mode with unlimited resources or mine deep into the world in survival mode, crafting weapons and armor to fend off the dangerous mobs.",
                    Developer = "Mojang Studios",
                    ReleaseDate = new DateTime(2011, 11, 18),
                    RetailPrice = 29.99
                },
                new Game()
                {
                    GameId = 4,
                    Name = "RollerCoaster Tycoon",
                    Description = "Create the ultimate theme park using a variety of coaster types with giant loops and barrel rolls and in-park attractions like suspended monorails and water-soaked plume rides.",
                    Developer = "Chris Sawyer",
                    ReleaseDate = new DateTime(1999, 03, 25),
                    RetailPrice = 5.99
                });

            // Seed the GameGenres table:
            modelBuilder.Entity<GameGenre>().HasData(
                new GameGenre
                {
                    GameGenreId = 1,
                    GameId = 1,
                    GenreId = 1
                },
                new GameGenre
                {
                    GameGenreId = 2,
                    GameId = 1,
                    GenreId = 5
                },
                new GameGenre
                {
                    GameGenreId = 3,
                    GameId = 2,
                    GenreId = 7
                },
                new GameGenre
                {
                    GameGenreId = 4,
                    GameId = 2,
                    GenreId = 10
                },
                new GameGenre
                {
                    GameGenreId = 5,
                    GameId = 3,
                    GenreId = 2
                },
                new GameGenre
                {
                    GameGenreId = 6,
                    GameId = 4,
                    GenreId = 11
                });

            // Seed the GamePlatforms table
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform
                {
                    GameId = 1,
                    PlatformId = 4,
                },
                new GamePlatform
                {
                    GameId = 2,
                    PlatformId = 1,
                },
                new GamePlatform
                {
                    GameId = 3,
                    PlatformId = 1,
                },
                new GamePlatform
                {
                    GameId = 3,
                    PlatformId = 2,
                },
                new GamePlatform
                {
                    GameId = 3,
                    PlatformId = 3,
                },
                new GamePlatform
                {
                    GameId = 4,
                    PlatformId = 1,
                });

            // Seed the GameLanguages table
            modelBuilder.Entity<GameLanguage>().HasData(
                new GameLanguage
                {
                    GameLanguageId = 1,
                    GameId = 1,
                    LanguageId = "en"
                },
                new GameLanguage
                {
                    GameLanguageId = 2,
                    GameId = 1,
                    LanguageId = "es"
                },
                new GameLanguage
                {
                    GameLanguageId = 3,
                    GameId = 2,
                    LanguageId = "en"
                },
                new GameLanguage
                {
                    GameLanguageId = 4,
                    GameId = 2,
                    LanguageId = "fr"
                },
                new GameLanguage
                {
                    GameLanguageId = 5,
                    GameId = 3,
                    LanguageId = "en"
                },
                new GameLanguage
                {
                    GameLanguageId = 6,
                    GameId = 3,
                    LanguageId = "es"
                },
                new GameLanguage
                {
                    GameLanguageId = 7,
                    GameId = 3,
                    LanguageId = "fr"
                },
                new GameLanguage
                {
                    GameLanguageId = 8,
                    GameId = 4,
                    LanguageId = "en"
                });

            // Seed the Countries table:
            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    CountryId = "CA",
                    Name = "Canada"
                });

            // Seed the Provinces table:
            modelBuilder.Entity<Province>().HasData(
                new Province
                {
                    ProvinceId = "AB",
                    Name = "Alberta"
                },
                new Province
                {
                    ProvinceId = "BC",
                    Name = "British Columbia"
                },
                new Province
                {
                    ProvinceId = "MB",
                    Name = "Manitoba"
                },
                new Province
                {
                    ProvinceId = "NB",
                    Name = "New Brunswick"
                },
                new Province
                {
                    ProvinceId = "NL",
                    Name = "Newfoundland and Labrador"
                },
                new Province
                {
                    ProvinceId = "NT",
                    Name = "Northwest Territories"
                },
                new Province
                {
                    ProvinceId = "NS",
                    Name = "Nova Scotia"
                },
                new Province
                {
                    ProvinceId = "NU",
                    Name = "Nunavut"
                },
                new Province
                {
                    ProvinceId = "ON",
                    Name = "Ontario"
                },
                new Province
                {
                    ProvinceId = "PE",
                    Name = "Prince Edward Island"
                },
                new Province
                {
                    ProvinceId = "QC",
                    Name = "Quebec"
                },
                new Province
                {
                    ProvinceId = "SK",
                    Name = "Saskatchewan"
                },
                new Province
                {
                    ProvinceId = "YT",
                    Name = "Yukon"
                });
        }
    }
}
