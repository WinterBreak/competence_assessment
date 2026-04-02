using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceConfiguration: IEntityTypeConfiguration<Competence>
{
    public void Configure(EntityTypeBuilder<Competence> builder)
    {
        builder.ToTable("competence");
        builder.HasKey(x => x.Id).HasName("id");
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500);
        
        
        builder.HasMany(x => x.CompetenceModelDetails)
            .WithOne(x => x.Competence)
            .HasForeignKey(x => x.CompetenceId);
    }
}