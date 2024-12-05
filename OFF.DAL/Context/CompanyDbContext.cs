using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OFF.DAL.Model;


namespace OFF.DAL.Context
{
    public class CompanyDbContext:IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(/*"server=.; database= LastOff ; trusted_connection=true"*/);
        }
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
        {

        }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<PLayer> Players { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agent>()
            .ToTable("Agents")
            .HasBaseType<AppUser>();

            modelBuilder.Entity<PLayer>()
            .ToTable("Players")
            .HasBaseType<AppUser>();



            modelBuilder.Entity<Post>()
            .HasOne(p => p.AppUser)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AppUserId)
            .OnDelete(DeleteBehavior.Cascade); 



            modelBuilder.Entity<Post>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Agent)
                .WithMany(a => a.Subscriptions)
                .HasForeignKey(s => s.AgentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Subscription)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade); 


            modelBuilder.Entity<Subscription>()
          .Property(s => s.Amount)
          .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Contact>()
           .HasOne(c => c.User)
           .WithMany(u => u.Contacts) 
           .HasForeignKey(c => c.UserId)
           .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Subscription>()
               .Property(e => e.SubscriptionType)
               .HasConversion(new EnumToStringConverter<SubscriptionType>());

            //modelBuilder.Entity<Payment>()
            //    .Property(e => e.PaymentStatus)
            //    .HasConversion(new EnumToStringConverter<PaymentStatus>());

            //modelBuilder.Entity<Subscription>()
            //    .Property(e => e.SubStatus)
            //    .HasConversion(new EnumToStringConverter<SubStatus>());

            modelBuilder.Entity<Subscription>()
                .Property(e => e.SubscriptionStatus)
                .HasConversion(new EnumToStringConverter<subscriptionStatus>());

        }


    }

}

