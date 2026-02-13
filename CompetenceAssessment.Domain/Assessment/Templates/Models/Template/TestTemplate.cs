namespace CompetenceAssessment.Domain.Assessment;

public class TestTemplate : ITemplate
{
    public TestTemplate() { }

    public TestTemplate(int id, string name, string description
        , DateTime creationDate, CompetenceModel competenceModel
        , List<TestQuestion> tasks)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.CreationDate = creationDate;
        this.CompetenceModel = competenceModel;
        this.Tasks = tasks;
    }
}