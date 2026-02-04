using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceModelConfiguration: IEntityTypeConfiguration<CompetenceModel>
{
    public void Configure(EntityTypeBuilder<CompetenceModel> builder)
    {
        builder.ToTable("competence_model");
        builder.HasKey(x => x.Id);
        
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500);
        
        builder.Property(x => x.CreationDate)
            .HasColumnName("creation_date")
            .IsRequired();


        builder.HasMany(x => x.Weights)
            .WithOne(x => x.Model)
            .HasForeignKey(x => x.CompetenceId);

        builder.HasMany(x => x.Templates)
            .WithOne(x => x.CompetenceModel)
            .HasForeignKey(x => x.CompetenceModelId);
    }
}