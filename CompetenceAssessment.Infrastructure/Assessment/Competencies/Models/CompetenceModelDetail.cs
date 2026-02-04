namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceModelDetail
{
    public int CompetenceModelId { get; set; }
    
    public int CompetenceId { get; set; }
    
    public decimal Weight { get; set; }
    
    public virtual CompetenceModel Model { get; set; }
    
    public virtual Competence Competence { get; set; }

    public CompetenceModelDetail(CompetenceModel model, Competence competence
        , decimal weight)
    {
        Model = model;
        Competence = competence;
        Weight = weight;
    }

    public CompetenceModelDetail(int modelId, int competenceId, decimal weight)
    {
        CompetenceModelId = modelId;
        CompetenceId = competenceId;
        Weight = weight;
    }
}