using CompetenceAssessment.Infrastructure.Assessment;
using Microsoft.EntityFrameworkCore;
using Task = CompetenceAssessment.Infrastructure.Assessment.Task;

namespace CompetenceAssessment.Infrastructure;

public class AssessmentContext: DbContext
{
    public AssessmentContext(DbContextOptions<AssessmentContext> options): base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("assessment");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssessmentContext).Assembly);
    }
    
    public virtual DbSet<Competence> Competences { get; set; }
    
    public virtual DbSet<CompetenceModel> CompetenceModels { get; set; }
    
    public virtual DbSet<CompetenceModelDetail> CompetenceModelDetails { get; set; }
    
    public virtual DbSet<Task> Tasks { get; set; }
    
    public virtual DbSet<Answer> Answers { get; set; }
    
    public virtual DbSet<Template> Templates { get; set; }
    
    public virtual DbSet<TemplateDetail> TemplateDetails { get; set; }
    
    public virtual DbSet<Assessment.Assessment> Assessments { get; set; }
    
    public virtual DbSet<AssessmentResult> AssessmentResults { get; set; }
    
    public virtual DbSet<AssessmentInspector> AssessmentInspectors { get; set; }
}