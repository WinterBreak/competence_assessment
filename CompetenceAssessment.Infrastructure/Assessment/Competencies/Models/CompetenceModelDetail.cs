namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceModelDetail
{
    public int CompetenceModelId { get; set; }
    
    public int CompetenceId { get; set; }
    
    public decimal Weight { get; set; }
    
    public virtual CompetenceModel Model { get; set; }
    
    public virtual Competence Competence { get; set; }

    public CompetenceModelDetail(CompetenceModel model, int competenceId
        , decimal weight)
    {
        Model = model;
        CompetenceId = competenceId;
        Weight = weight;
    }

    public CompetenceModelDetail(int modelId, int competenceId, decimal weight)
    {
        CompetenceModelId = modelId;
        CompetenceId = competenceId;
        Weight = weight;
    }
}