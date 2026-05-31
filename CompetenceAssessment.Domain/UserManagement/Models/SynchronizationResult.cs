namespace CompetenceAssessment.Domain.UserManagement;

public class SynchronizationResult
{
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Deleted { get; set; }
    public int PositionCreated { get; set; }
    public int DepartmentCreated { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool IsSuccess => Errors.Count == 0;
}