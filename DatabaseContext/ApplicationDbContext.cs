
using BusServcies.Models;
using BusServices.Models;
using Microsoft.EntityFrameworkCore;

namespace BusServcies.DatabaseContext
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Event> Events{ get; set; }
        public DbSet<BusType> BusTypes { get; set; }
        public DbSet<BusImage> BusImages { get; set; }
        public DbSet<BusCategory> BusCategory { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= Event =================
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.FromPlace)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.ToPlace)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.TravelDate)
                      .IsRequired();

                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)")
                      .HasDefaultValue(0.0m);

                entity.Property(e => e.Status)
                      .HasMaxLength(50)
                      .HasDefaultValue("Upcoming");

                entity.Property(e => e.Organizer)
                      .HasMaxLength(100);

                entity.Property(e => e.ContactInfo)
                      .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);

                // 🔗 Relationship with BusType
                entity.HasOne(e => e.BusType)
                      .WithMany(bt => bt.Buses)
                      .HasForeignKey(e => e.BusTypeId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 🔗 Relationship with BusImages
                //entity.HasMany(e => e.Images)
                //      .WithOne(i => i.)
                //      .HasForeignKey(i => i.EventId)
                //      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================= BusType =================
            modelBuilder.Entity<BusType>(entity =>
            {
                entity.ToTable("BusTypes");

                entity.HasKey(bt => bt.Id);

                entity.Property(bt => bt.Name)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(bt => bt.Description)
                      .HasMaxLength(250);

                entity.Property(bt => bt.Features)
                      .HasMaxLength(250);

                entity.Property(bt => bt.Owner)
                      .HasMaxLength(100);

                entity.Property(bt => bt.OwnerContact)
                      .HasColumnType("bigint");

                entity.Property(bt => bt.IsActive)
                      .HasDefaultValue(true);

            });

            // ================= BusImage =================
            modelBuilder.Entity<BusImage>(entity =>
            {
                entity.HasKey(bi => bi.PhotoId);

                entity.Property(bi => bi.PhotoId)
                      .ValueGeneratedOnAdd(); // 👈 ensure auto increment
            });


            //entity.Property(bi => bi.)
            //      .HasMaxLength(100);

            // seeding

            //// --- Seed BusTypes ---
            //modelBuilder.Entity<BusType>().HasData(
            //    new BusType
            //    {
            //        Id = 1,
            //        Name = "Seater",
            //        Description = "40 Seater AC Bus",
            //        DefaultSeats = 40,
            //        Features = "AC, WiFi, Charging Port",
            //        Owner = "ABC Travels",
            //        IsActive = true,
            //        OwnerContact = 9876543210
            //    },
            //    new BusType
            //    {
            //        Id = 2,
            //        Name = "Sleeper",
            //        Description = "30 Sleeper AC Bus",
            //        DefaultSeats = 30,
            //        Features = "AC, Blanket, Charging Port",
            //        Owner = "XYZ Travels",
            //        IsActive = true,
            //        OwnerContact = 9123456780
            //    }
            //);

            //// --- Seed Events (Trips) ---
            //modelBuilder.Entity<Event>().HasData(
            //    new Event
            //    {
            //        Id = 1,
            //        Title = "Hyderabad to Bangalore",
            //        Description = "Comfortable overnight journey",
            //        FromPlace = "Hyderabad",
            //        ToPlace = "Bangalore",
            //        TravelDate = DateTime.UtcNow.AddDays(3),
            //        TotalSeats = 40,
            //        AvailableSeats = 35,
            //        Price = 1200.50m,
            //        Status = "Upcoming",
            //        Organizer = "ABC Travels",
            //        ContactInfo = "abc@gmail.com",
            //        IsActive = true,
            //        BusTypeId = 1
            //    },
            //    new Event
            //    {
            //        Id = 2,
            //        Title = "Chennai to Pune",
            //        Description = "Luxury sleeper bus service",
            //        FromPlace = "Chennai",
            //        ToPlace = "Pune",
            //        TravelDate = DateTime.UtcNow.AddDays(5),
            //        TotalSeats = 30,
            //        AvailableSeats = 28,
            //        Price = 1800.75m,
            //        Status = "Upcoming",
            //        Organizer = "XYZ Travels",
            //        ContactInfo = "xyz@gmail.com",
            //        IsActive = true,
            //        BusTypeId = 2
            //    }
            //);

            //// --- Seed Bus Images ---
            //modelBuilder.Entity<BusImage>().HasData(
            //    new BusImage
            //    {
            //        Id = 1,
            //        ImageUrl = "https://ts1.explicit.bing.net/th?id=OIP.7IFURQmvycxLNYYig8TiSgHaE8&pid=15.1",
            //        Caption = "Front View - Seater Bus",
            //        EventId = 1
            //    },
            //    new BusImage
            //    {
            //        Id = 2,
            //        ImageUrl = "https://thfvnext.bing.com/th/id/OIP.TasbJC0pWiRaD-8dTvgi8wHaGW?w=226&h=193&c=7&r=0&o=7&cb=thfvnext&dpr=1.3&pid=1.7&rm=3",
            //        Caption = "Inside View - Seater Bus",
            //        EventId = 1
            //    },
            //    new BusImage
            //    {
            //        Id = 3,
            //        ImageUrl = "https://thfvnext.bing.com/th/id/OIP.Z4oSl51ufLg-gKLKOAwgvwHaEK?w=333&h=187&c=7&r=0&o=7&cb=thfvnext&dpr=1.3&pid=1.7&rm=3",
            //        Caption = "Side View - Sleeper Bus",
            //        EventId = 2
            //    }
            //);



            // --- Seed Bus Categories ---
            modelBuilder.Entity<BusCategory>().HasData(
                    new BusCategory
                    {
                        Id = 1,
                        Name = "Seater",
                        Description = "Standard seater buses",
                        IsActive = true
                    },
                    new BusCategory
                    {
                        Id = 2,
                        Name = "Sleeper",
                        Description = "Comfortable sleeper buses",
                        IsActive = true
                    },
                    new BusCategory
                    {
                        Id = 3,
                        Name = "Express",
                        Description = "High speed express buses",
                        IsActive = true
                    }
                );

                // --- Seed Bus Types ---
                modelBuilder.Entity<BusType>().HasData(
                    new BusType
                    {
                        Id = 1,
                        Name = "AC 2+2 Seater",
                        Description = "40 Seater AC Bus with 2+2 layout",
                        DefaultSeats = 40,
                        Features = "AC, WiFi, Charging Port",
                        Owner = "ABC Travels",
                        IsActive = true,
                        OwnerContact = 9876543210,
                        BusCategoryId = 1
                    },
                    new BusType
                    {
                        Id = 2,
                        Name = "AC Sleeper",
                        Description = "30 Sleeper AC Bus with comfortable beds",
                        DefaultSeats = 30,
                        Features = "AC, Blanket, Charging Port",
                        Owner = "XYZ Travels",
                        IsActive = true,
                        OwnerContact = 9123456780,
                        BusCategoryId = 2
                    },
                    new BusType
                    {
                        Id = 3,
                        Name = "Volvo Express",
                        Description = "Luxury Volvo Express Bus",
                        DefaultSeats = 45,
                        Features = "AC, WiFi, TV, Charging Port",
                        Owner = "PQR Travels",
                        IsActive = true,
                        OwnerContact = 9988776655,
                        BusCategoryId = 3
                    }
                );

                // --- Seed Events (Trips) ---
                modelBuilder.Entity<Event>().HasData(
                    new Event
                    {
                        Id = 1,
                        Title = "Hyderabad to Bangalore",
                        Description = "Comfortable overnight journey",
                        FromPlace = "Hyderabad",
                        ToPlace = "Bangalore",
                        TravelDate = DateTime.UtcNow.AddDays(3),
                        Price = 1200.50m,
                        Status = "Upcoming",
                        Organizer = "ABC Travels",
                        ContactInfo = "abc@gmail.com",
                        IsActive = true,
                        BusTypeId = 1,
                        BusCategoryId = 1   // 👈 FIXED

                    },
                    new Event
                    {
                        Id = 2,
                        Title = "Chennai to Pune",
                        Description = "Luxury sleeper bus service",
                        FromPlace = "Chennai",
                        ToPlace = "Pune",
                        TravelDate = DateTime.UtcNow.AddDays(5),
                        Price = 1800.75m,
                        Status = "Upcoming",
                        Organizer = "XYZ Travels",
                        ContactInfo = "xyz@gmail.com",
                        IsActive = true,
                        BusTypeId = 2,
                        BusCategoryId = 2   // 👈 FIXED

                    },
                    new Event
                    {
                        Id = 3,
                        Title = "Delhi to Jaipur",
                        Description = "Fast express Volvo service",
                        FromPlace = "Delhi",
                        ToPlace = "Jaipur",
                        TravelDate = DateTime.UtcNow.AddDays(2),
                        Price = 900.00m,
                        Status = "Upcoming",
                        Organizer = "PQR Travels",
                        ContactInfo = "pqr@gmail.com",
                        IsActive = true,
                        BusTypeId = 3,
                        BusCategoryId = 3  // 👈 FIXED

                    }
                );

                // --- Seed Bus Images ---
                modelBuilder.Entity<BusImage>().HasData(
                    new BusImage
                    {
                        PhotoId = 1,
                        Url = "https://ts1.explicit.bing.net/th?id=OIP.7IFURQmvycxLNYYig8TiSgHaE8&pid=15.1",
                        EventId = 1,
                        IsPrimary = true,
                        PublicId= "OIP_ma4xfs"
                    },
                    new BusImage
                    {
                        PhotoId = 2,
                        Url = "https://thfvnext.bing.com/th/id/OIP.TasbJC0pWiRaD-8dTvgi8wHaGW?w=226&h=193&c=7&r=0&o=7&cb=thfvnext&dpr=1.3&pid=1.7&rm=3",
                        EventId = 1,
                        PublicId= "OIP_3_jqikgp"
                    },
                    new BusImage
                    {
                        PhotoId = 3,
                        Url = "https://thfvnext.bing.com/th/id/OIP.Z4oSl51ufLg-gKLKOAwgvwHaEK?w=333&h=187&c=7&r=0&o=7&cb=thfvnext&dpr=1.3&pid=1.7&rm=3",
                        EventId = 3,
                        PublicId= "OIP_2_wlazak",
                        IsPrimary = true
                    },
                    new BusImage
                    {
                        PhotoId = 4,
                        Url = "https://picsum.photos/seed/bus4/600/400",
                        EventId = 4,
                        PublicId= "bus_ifrwmm",
                        IsPrimary = true
                    }
                );

            }
        }
    }

