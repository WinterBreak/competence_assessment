using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class AnswerConfiguration: IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("answer");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.TaskId)
            .HasColumnName("task_id")
            .IsRequired();
        
        builder.Property(x => x.Text)
            .HasColumnName("answer")
            .IsRequired();
        
        builder.Property(x => x.IsCorrect)
            .HasColumnName("is_correct")
            .IsRequired();


        builder.HasOne(x => x.Task)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.TaskId);
    }
}