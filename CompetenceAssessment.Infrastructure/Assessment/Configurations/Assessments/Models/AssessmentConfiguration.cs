using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentConfiguration: IEntityTypeConfiguration<Assessment.Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment.Assessment> builder)
    {
        builder.ToTable("assessment");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .HasColumnName("id_user")
            .IsRequired();
        
        builder.Property(x => x.ParentId)
            .HasColumnName("parent_id");
        
        builder.Property(x => x.TemplateId)
            .HasColumnName("id_template")
            .IsRequired();
        
        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date");
        
        builder.Property(x => x.AssessmentTypeId)
            .HasColumnName("id_assessment_type")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("comment");
        
        builder.Property(x => x.State)
            .HasColumnName("state")
            .IsRequired();

        builder.HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey(x => x.ParentId);
            

        builder.HasMany(x => x.Inspectors)
            .WithOne(x => x.Assessment)
            .HasForeignKey(x => x.AssessmentId);
        
        builder.HasMany(x => x.Results)
            .WithOne(x => x.Assessment)
            .HasForeignKey(x => x.AssessmentId);
        
        builder.HasOne(x => x.Template)
            .WithMany()
            .HasForeignKey(x => x.TemplateId);
    }
}