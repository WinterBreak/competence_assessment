using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class PositionConfiguration: IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("position");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();
        
        builder.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired();

        
        builder.HasMany(x => x.Employees)
            .WithOne(x => x.Position)
            .HasForeignKey(x => x.PositionId);
    }
}