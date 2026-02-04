using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.HasKey(x => x.Id);
        
        
        builder.Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .IsRequired();
        
        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .IsRequired();

        builder.Property(x => x.SecondName)
            .HasColumnName("second_name");
        
        builder.Property(x => x.DepartmentId)
            .HasColumnName("id_department")
            .IsRequired();
        
        builder.Property(x => x.PositionId)
            .HasColumnName("id_position")
            .IsRequired();

        builder.Property(x => x.BossId)
            .HasColumnName("boss_id");
        
        
        builder.HasOne(x => x.Department)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Position)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Boss)
            .WithMany(x => x.Subordinates)
            .HasForeignKey(x => x.BossId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.RoleLinks)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}