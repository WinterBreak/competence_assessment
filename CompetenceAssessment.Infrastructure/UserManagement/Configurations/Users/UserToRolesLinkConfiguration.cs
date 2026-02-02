using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserToRolesLinkConfiguration: IEntityTypeConfiguration<UserToRolesLink>
{
    public void Configure(EntityTypeBuilder<UserToRolesLink> builder)
    {
        builder.ToTable("users_to_roles");
        builder.HasKey(x => new { x.UserId, x.RoleId });
        
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id");
        
        builder.Property(x => x.RoleId)
            .HasColumnName("role_id");


        builder.HasOne(x => x.User)
            .WithMany(x => x.RoleLinks)
            .HasForeignKey(x => x.UserId);
        
        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserLinks)
            .HasForeignKey(x => x.RoleId);
    }
}