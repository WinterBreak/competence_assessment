using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("template_detail", Schema = "assessment")]
public class TemplateDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey("id_template")]
    public int TemplateId { get; set; }
    
    [Required]
    [ForeignKey("id_task")]
    public int TaskId { get; set; }
    
    [Required]
    [Column("weight")]
    public decimal Weight { get; set; }
    
    public virtual Template Template { get; set; }
    
    public virtual Task Task { get; set; }

    public TemplateDetail(Template template, Task task, decimal weight)
    {
        Template = template;
        Task = task;
        Weight = weight;
    }
    
    public TemplateDetail(int templateId, int taskId, decimal weight)
    {
        TemplateId = templateId;
        TaskId = taskId;
        Weight = weight;
    }
}