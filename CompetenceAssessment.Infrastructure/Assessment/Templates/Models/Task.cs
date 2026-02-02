using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("task", Schema = "assessment")]
public class Task
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    public int TaskTypeId { get; set; }
    
    [Required]
    [Column("text")]
    public string Text { get; set; }
    
    [Column("answer")]
    public string Answer { get; set; }

    public Task(int typeId, string text, string answer)
    {
        TaskTypeId = typeId;
        Text = text;
        Answer = answer;
    }
}