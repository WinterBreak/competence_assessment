using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Infrastructure.UserManagement;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("assessment_result", Schema = "assessment")]
public class Assessment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey("id_assessment_type")]
    public int AssessmentTypeId { get; set; }
    
    [Required]
    [ForeignKey("id_user")]
    public int UserId { get; set; }
    
    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; }
    
    [Column("end_date")]
    public DateTime? EndDate { get; set; }
    
    [Column("comment")]
    public string Comment { get; set; }
    
    [Required]
    [Column("is_finished")]
    public bool IsFinished { get; set; }

    public virtual ICollection<AssessmentInspector> Inspectors { get; set; } = [];
    
    public virtual ICollection<AssessmentResult> Results { get; set; } = [];

    public Assessment(int typeId, int userId, DateTime startDate, DateTime endDate
        , bool isFinished = false)
    {
        AssessmentTypeId = typeId;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
        IsFinished = isFinished;
    }
}