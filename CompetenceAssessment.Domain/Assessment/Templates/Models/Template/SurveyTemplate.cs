namespace CompetenceAssessment.Domain.Assessment;

public class SurveyTemplate : ITemplate
{
    public SurveyTemplate() { }

    public SurveyTemplate(int id, string name, string description
        , DateTime creationDate, CompetenceModel competenceModel
        , List<SurveyQuestion> tasks)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.CreationDate = creationDate;
        this.CompetenceModel = competenceModel;
        this.Tasks = tasks;
    }
}