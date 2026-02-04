using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserManagementContext: DbContext
{
    public UserManagementContext(DbContextOptions<UserManagementContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("security");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserManagementContext).Assembly);
    }
    
    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<Role> Roles { get; set; }
    
    public virtual DbSet<UserToRolesLink> UserRoleLinks { get; set; }
    
    public virtual DbSet<Department> Departments { get; set; }
    
    public virtual DbSet<Position> Positions { get; set; }
}