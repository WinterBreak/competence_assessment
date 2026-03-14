namespace CompetenceAssessment.Domain.UserManagement;

public class UserQuery
{
    public List<int> Ids { get; } = [];
    
    public List<int> RolesIds { get; } = [];
    
    public int PositionId { get;  }
    
    public int DepartmentId { get;  }
    
    public int? BossId { get; }
    
    public UserQuery() {}

    public UserQuery(List<int> ids = default, List<int> rolesId = default, int positionId = default
                   , int departmentId = default, int? bossId = default)
    {
        Ids = ids;
        RolesIds = rolesId;
        PositionId = positionId;
        DepartmentId = departmentId;
        BossId = bossId;
    }
    
    public UserQuery(int id = default, int roleId = default, int positionId = default
        , int departmentId = default, int? bossId = default)
    {
        Ids.Add(id);
        RolesIds.Add(roleId);
        PositionId = positionId;
        DepartmentId = departmentId;
        BossId = bossId;
    }
}