using CompetenceAssessment.Infrastructure.Assessment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentInspectorConfiguration: IEntityTypeConfiguration<AssessmentInspector>
{
    public void Configure(EntityTypeBuilder<AssessmentInspector> builder)
    {
        builder.ToTable("assessment_inspectors");
        builder.HasKey(x => new { x.AssessmentId, x.UserId });
        
        
        builder.Property(x => x.AssessmentId)
            .HasColumnName("id_assessment")
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();


        builder.HasOne(x => x.Assessment)
            .WithMany(x => x.Inspectors)
            .HasForeignKey(x => x.AssessmentId);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Inspections)
            .HasForeignKey(x => x.UserId);
    }
}