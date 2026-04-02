using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateDetailConfiguration: IEntityTypeConfiguration<TemplateDetail>
{
    public void Configure(EntityTypeBuilder<TemplateDetail> builder)
    {
        builder.ToTable("template_detail");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(x => x.CompetenceId)
            .HasColumnName("id_competence")
            .IsRequired();
        
        builder.Property(x => x.TemplateId)
            .HasColumnName("id_template")
            .IsRequired();
        
        builder.Property(x => x.TaskId)
            .HasColumnName("id_task")
            .IsRequired();
        
        builder.Property(x => x.Weight)
            .HasColumnName("weight")
            .IsRequired();


        builder.HasOne(x => x.Template)
            .WithMany(x => x.TemplateDetails)
            .HasForeignKey(x => x.TemplateId);
        
        builder.HasOne(x => x.Task)
            .WithMany(x => x.TemplateDetails)
            .HasForeignKey(x => x.TaskId);
        
        builder.HasOne(x => x.Competence)
            .WithMany()
            .HasForeignKey(x => x.CompetenceId);
            
    }
}