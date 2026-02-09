using CompetenceAssessment.Infrastructure.Assessment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentResultConfiguration: IEntityTypeConfiguration<AssessmentResult>
{
    public void Configure(EntityTypeBuilder<AssessmentResult> builder)
    {
        builder.ToTable("assessment_result_detail");
        builder.HasKey(x => new { x.AssessmentId, x.TaskId });
        
        
        builder.Property(x => x.AssessmentId)
            .HasColumnName("id_assessment_result")
            .IsRequired();
        
        builder.Property(x => x.TaskId)
            .HasColumnName("task_id")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("comment");
        
        builder.Property(x => x.Answer)
            .HasColumnName("answer");
        
        builder.Property(x => x.Score)
            .HasColumnName("score")
            .IsRequired();


        builder.HasOne(x => x.Assessment)
            .WithMany(x => x.Results)
            .HasForeignKey(x => x.AssessmentId);
        
        builder.HasOne(x => x.Task)
            .WithMany(x => x.Results)
            .HasForeignKey(x => x.TaskId);
    }
}