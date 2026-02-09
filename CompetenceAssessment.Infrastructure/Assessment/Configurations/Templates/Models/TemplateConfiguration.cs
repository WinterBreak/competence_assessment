using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateConfiguration: IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.ToTable("template");
        builder.HasKey(x => x.Id);
        
        
        builder.Property(x => x.CompetenceModelId)
            .HasColumnName("id_competence_model")
            .IsRequired();
        
        builder.Property(x => x.TemplateTypeId)
            .HasColumnName("id_template_type")
            .IsRequired();
        
        builder.Property(x => x.ScaleId)
            .HasColumnName("id_scale")
            .IsRequired();
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(x => x.CreationDate)
            .HasColumnName("creation_date")
            .IsRequired();


        builder.HasOne(x => x.CompetenceModel)
            .WithMany(x => x.Templates)
            .HasForeignKey(x => x.CompetenceModelId);
    }
}