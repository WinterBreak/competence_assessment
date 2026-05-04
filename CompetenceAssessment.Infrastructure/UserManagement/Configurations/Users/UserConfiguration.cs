using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
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
        
        // builder.Property(x => x.AccessFailedCount)
        //     .HasColumnName("access_failed_count");
        //
        // builder.Property(x => x.ConcurrencyStamp)
        //     .HasColumnName("concurrency_stamp");
        
        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash");
        
        // builder.Property(x => x.SecurityStamp)
        //     .HasColumnName("security_stamp");
        //
        // builder.Property(x => x.NormalizedEmail)
        //     .HasColumnName("normalized_email");
        //
        // builder.Property(x => x.LockoutEnabled)
        //     .HasColumnName("lockout_enabled");
        //
        // builder.Property(x => x.LockoutEnd)
        //     .HasColumnName("lockout_end");
        //
        // builder.Property(x => x.TwoFactorEnabled)
        //     .HasColumnName("two_factor_enabled");
        //
        // builder.Property(x => x.PhoneNumber)
        //     .HasColumnName("phone_number");
        //
        // builder.Property(x => x.PhoneNumberConfirmed)
        //     .HasColumnName("phone_number_confirmed");
        //
        // builder.Property(x => x.EmailConfirmed)
        //             .HasColumnName("email_confirmed");
        //
        // builder.Property(x => x.NormalizedUserName)
        //     .HasColumnName("normalized_user_name");
        //
        // builder.Property(x => x.UserName)
        //     .HasColumnName("user_name");
        
        
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