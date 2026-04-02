using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TaskConfiguration: IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("task");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.TaskTypeId)
            .HasColumnName("id_task_type")
            .IsRequired();
        
        builder.Property(x => x.Text)
            .HasColumnName("text")
            .IsRequired();

        builder.Property(x => x.Answer)
            .HasColumnName("answer");


        builder.HasMany(x => x.TemplateDetails)
            .WithOne(x => x.Task)
            .HasForeignKey(x => x.TaskId);
        
        builder.HasMany(x => x.Results)
            .WithOne(x => x.Task)
            .HasForeignKey(x => x.TaskId);
    }
}