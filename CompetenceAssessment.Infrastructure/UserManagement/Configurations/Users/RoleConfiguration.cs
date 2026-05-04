using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class RoleConfiguration: IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("role");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasColumnName("description");
        
        
        builder.HasMany(x => x.UserLinks)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId);
    }
}