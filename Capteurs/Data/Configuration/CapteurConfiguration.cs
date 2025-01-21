using Capteurs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capteurs.Data.Configuration
{
    public class CapteurConfiguration : IEntityTypeConfiguration<Capteur>
    {
        public void Configure(EntityTypeBuilder<Capteur> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nom)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.IsActive).HasDefaultValue(true);

            builder.Property(c => c.Description)
                .IsRequired(false)
                .HasMaxLength(500);
        }
    }
}
