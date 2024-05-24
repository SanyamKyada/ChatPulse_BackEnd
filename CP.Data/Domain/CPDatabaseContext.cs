using CP.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CP.Data.Domain
{
    public class CPDatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public CPDatabaseContext(DbContextOptions<CPDatabaseContext> options) : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<FriendRequestMessage> FriendRequestMessages { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Remove(typeof(CascadeDeleteConvention));
            configurationBuilder.Conventions.Remove(typeof(SqlServerOnDeleteConvention));
            base.ConfigureConventions(configurationBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ConversationsAsUser1)
                .WithOne(c => c.User1)
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ConversationsAsUser2)
                .WithOne(c => c.User2)
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.FriendRequestSender)
                .WithOne(f => f.SenderUser)
                .HasForeignKey(f => f.SenderUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.FriendRequestReceiver)
                .WithOne(f => f.ReceiverUser)
                .HasForeignKey(f => f.ReceiverUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendRequest>()
                .HasIndex(fr => new { fr.SenderUserId, fr.ReceiverUserId })
                .IsUnique()
                .HasDatabaseName("UQ_SenderReceiverUserIds");
        }
    }
}
