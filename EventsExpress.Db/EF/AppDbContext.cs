﻿using EventsExpress.Db.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventsExpress.Db.EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Rate> Rates { get; set; }

        public DbSet<Relationship> Relationships { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Comments> Comments { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // user config
            builder.Entity<User>()
                .Property(u => u.Birthday).HasColumnType("date");

            // user-event many-to-many configs
            // user as visitor
            builder.Entity<UserEvent>()
                .HasKey(t => new { t.UserId, t.EventId });
            builder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.EventsToVisit)
                .HasForeignKey(ue => ue.UserId);
            builder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.Visitors)
                .HasForeignKey(ue => ue.EventId);

            // user-event configs
            // user as owner
            builder.Entity<Event>()
                .HasOne(e => e.Owner)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.OwnerId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
                .Property(u => u.DateFrom).HasColumnType("date");
            builder.Entity<Event>()
                .Property(u => u.DateTo).HasColumnType("date");

            // rates config
            builder.Entity<Rate>()
                .HasOne(r => r.UserFrom)
                .WithMany(u => u.Rates)
                .HasForeignKey(r => r.UserFromId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Rate>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Rates)
                .HasForeignKey(r => r.EventId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Relationship>()
                .HasOne(r => r.UserFrom)
                .WithMany(u => u.Relationships)
                .HasForeignKey(r => r.UserFromId).OnDelete(DeleteBehavior.Restrict);

            // user-category many-to-many
            builder.Entity<UserCategory>()
                .HasKey(t => new { t.UserId, t.CategoryId });
            builder.Entity<UserCategory>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(uc => uc.UserId);
            builder.Entity<UserCategory>()
                .HasOne(uc => uc.Category)
                .WithMany(c => c.Users)
                .HasForeignKey(uc => uc.CategoryId);

            // event-category many-to-many
            builder.Entity<EventCategory>()
                .HasKey(t => new { t.EventId, t.CategoryId });
            builder.Entity<EventCategory>()
                .HasOne(ec => ec.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(ec => ec.EventId);
            builder.Entity<EventCategory>()
                .HasOne(ec => ec.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(uc => uc.CategoryId);

            // category config
            builder.Entity<Category>()
                .Property(c => c.Name).IsRequired();

            // country config
            builder.Entity<Country>()
                .Property(c => c.Name).IsRequired();
            builder.Entity<Country>()
                .HasIndex(c => c.Name).IsUnique();

            // city config
            builder.Entity<City>()
                .Property(c => c.Name).IsRequired();

            // comment config
            builder.Entity<Comments>()
                .HasOne(c => c.Parent).WithMany(prop => prop.Children).HasForeignKey(c => c.CommentsId);

            // event config
            builder.Entity<Event>()
                .Property(c => c.MaxParticipants).HasDefaultValue(Int32.MaxValue);
        }
    }
}
