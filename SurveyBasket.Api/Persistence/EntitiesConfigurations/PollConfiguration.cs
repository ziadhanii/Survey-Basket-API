using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        // Configure `Title` property
        builder.HasIndex(x => x.Title).IsUnique();
        
        builder.Property(p => p.Title)
            .HasMaxLength(100); // Limit the length to 200 characters

        // Configure `Summary` property
        builder.Property(p => p.Summary)
            .HasMaxLength(1500); // Optional, with a max length of 500 characters

        // // Configure `IsPublished` property
        // builder.Property(p => p.IsPublished)
        //     .IsRequired(); // Ensures the value is always set (non-nullable)
        //
        // // Configure `StartsAt` and `EndsAt` properties
        // builder.Property(p => p.SrartsAt)
        //     .IsRequired(); // Required field
        //
        // builder.Property(p => p.EndsAt)
        //     .IsRequired(); // Required field
        //
        // // Add optional database table mapping details (e.g., table name)
        // builder.ToTable("Polls"); // Maps the entity to a table named 'Polls'
    }
}