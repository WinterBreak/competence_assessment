namespace CompetenceAssessment.Domain.UserManagement;

public class UserQuery
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public List<int> Ids { get; } = [];
    
    public List<int> RolesIds { get; } = [];
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public int PositionId { get;  }
    
    public int DepartmentId { get;  }
    
    public int? BossId { get; }
    
    public UserQuery() {}

    public UserQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public UserQuery(List<int> ids = default, List<int> rolesId = default, int positionId = default
                   , int departmentId = default, int? bossId = default)
    {
        Ids = ids;
        RolesIds = rolesId;
        
        if (positionId != default)
        {
            PositionId = positionId;
        }
        
        if (departmentId != default)
        {
            DepartmentId = departmentId;
        }
        
        BossId = bossId;
    }
    
    public UserQuery(int id = default, int roleId = default, int positionId = default
        , int departmentId = default, int? bossId = default, string email = default, string passwordHash = default)
    {
        if (id != default)
        {
            Ids.Add(id);
        }

        if (roleId != default)
        {
            RolesIds.Add(roleId);
        }

        if (positionId != default)
        {
            PositionId = positionId;
        }

        if (departmentId != default)
        {
            DepartmentId = departmentId;
        }

        BossId = bossId;
        Email = email;
        PasswordHash = passwordHash;
    }
}