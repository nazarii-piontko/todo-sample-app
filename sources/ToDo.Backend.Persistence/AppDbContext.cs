using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Backend.Domain;

namespace ToDo.Backend.Persistence
{
    public sealed class AppDbContext : IdentityDbContext<User, 
        IdentityRole<long>, long, 
        IdentityUserClaim<long>,
        IdentityUserRole<long>, 
        IdentityUserLogin<long>, 
        IdentityRoleClaim<long>, 
        IdentityUserToken<long>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.HasIndex(u => u.NormalizedUserName).HasName("ix_users_user_name").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasName("ix_users_email");
                b.ToTable("users");
            });
            builder.Entity<IdentityRole<long>>(b =>
            {
                b.HasIndex(r => r.NormalizedName).HasName("ix_roles_name").IsUnique();
                b.ToTable("roles");
            });
            builder.Entity<IdentityUserRole<long>>().ToTable("user_roles");
            builder.Entity<IdentityUserClaim<long>>().ToTable("user_claims");
            builder.Entity<IdentityUserToken<long>>().ToTable("user_tokens");
            builder.Entity<IdentityUserLogin<long>>().ToTable("user_logins");
            builder.Entity<IdentityRole<long>>().ToTable("roles");
            builder.Entity<IdentityRoleClaim<long>>().ToTable("role_claims");
            
            builder.Entity<User>()
                .HasMany<ToDoList>()
                .WithOne()
                .HasForeignKey(l => l.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ToDoList>()
                .HasMany<ToDoItem>()
                .WithOne(i => i.List)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<ToDoList> ToDoLists { get; set; }
        
        public DbSet<ToDoItem> ToDoItems { get; set; }
    }
}