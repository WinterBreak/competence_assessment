using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceModelDetailConfiguration: IEntityTypeConfiguration<CompetenceModelDetail>
{
    public void Configure(EntityTypeBuilder<CompetenceModelDetail> builder)
    {
        builder.ToTable("competence_model_detail");
        builder.HasKey(x => new { x.CompetenceModelId, x.CompetenceId });
        
        
        builder.Property(x => x.CompetenceModelId)
            .HasColumnName("competence_model_id")
            .IsRequired();

        builder.Property(x => x.CompetenceId)
            .HasColumnName("competence_id")
            .IsRequired();
        
        builder.Property(x => x.Weight)
            .HasColumnName("weight")
            .IsRequired();


        builder.HasOne(x => x.Competence)
            .WithMany(x => x.CompetenceModelDetails)
            .HasForeignKey(x => x.CompetenceId);
        
        builder.HasOne(x => x.Model)
            .WithMany(x => x.Weights)
            .HasForeignKey(x => x.CompetenceModelId);
    }
}