using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtualGameStore.Entities;

namespace VirtualGameStore.DataAccess
{
    public class GameStoreDbContext : IdentityDbContext<User>
    {
        // Constructor with DbContext options (so EFCore can connect to DB): 
        public GameStoreDbContext(DbContextOptions options) : base(options)
        {
        }

        // Insert an admin user into the database (method called in program.cs):
        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Temporarily replace the password validator with my custom validator:
            userManager.PasswordValidators.Clear();
            userManager.PasswordValidators.Add( new CustomPasswordValidator());

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
                User user = new User { UserName = username};
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }

            // Restore the default password validator:
            userManager.PasswordValidators.Add(new PasswordValidator<User>());
        }

        // DbSets of every entity (used to query and save instances of entities):
        public DbSet<Game>? Games { get; set; }
        public DbSet<Platform>? Platforms { get; set; }

        // Override base class method OnModelCreating to establish DB relationships
        // or seed the DB or generate primary keys as a composites of 2 foreign keys:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Establish Game relationships:
            // Each game has 1 platform - Each platform could have many games - Game has FK to platform:
            modelBuilder.Entity<Game>().HasOne(g => g.Platform).WithMany(p => p.Games).HasForeignKey(g => g.PlatformId);

            // Seed the Platforms table:
            modelBuilder.Entity<Platform>().HasData( 
                new Platform() { PlatformId = "b353f49e-6a3c-4aaa-bc9e-23249a27f1c5", PlatformName = "MS-DOS" });

            // Seed the Gamse table:
            modelBuilder.Entity<Game>().HasData(
                new Game()
                {
                    GameId = "a1e657d8-0a6d-445a-b00c-81a553d1f4ef",
                    Name = "Doom",
                    Developer = "id Software",
                    ReleaseDate = new DateTime(1993, 12, 10),
                    RetailPrice = 4.99,
                    PlatformId = "b353f49e-6a3c-4aaa-bc9e-23249a27f1c5"
                });
        }
    }
}
