using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;
using A = System.Threading.Tasks;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateRepository: ITemplateRepository
{
    private readonly AssessmentContext _context;
    private readonly ITaskRepository _taskRepository;
    private readonly ICompetenceModelRepository _competenceModelRepository;

    public TemplateRepository(AssessmentContext context
                            , ITaskRepository taskRepository
                            , ICompetenceModelRepository competenceModelRepository)
    {
        _context = context;
        _taskRepository = taskRepository;
        _competenceModelRepository = competenceModelRepository;
    }

    public async Task<ITemplate?> GetTemplateAsync(TemplateQuery query, CancellationToken token = default)
    {
        var templates = await GetTemplatesAsync(query, token);
        return templates.SingleOrDefault();
    }
    
    public async Task<List<ITemplate>> GetTemplatesAsync(TemplateQuery query, CancellationToken token = default)
    {
        var specification = new TemplateSpecificationBuilder().WithQuery(query).Build();
        var templates = await GetAllTemplates()
            .Where(specification)
            .OrderByDescending(cm => cm.Name)
            .ToListAsync(token);
        
        var details = templates.SelectMany(t => t.TemplateDetails).ToList();
        var weights = await GetWeights(details, token);
        var modelIds = templates.Select(t => t.CompetenceModelId).Distinct().ToList();
        var modelQuery = new CompetenceModelQuery(ids: modelIds);
        var models =  await _competenceModelRepository.GetCompetenceModelsAsync(modelQuery, token);
        
        return templates
            .Select(t =>
            {
                var templateWeights = weights.Where(w => w.TemplateId == t.Id).ToList();
                var model = models.Single(m => m.Id == t.CompetenceModelId);
                return new ITemplate(t.Id, t.Name, (TemplateType)t.TemplateTypeId, (ScaleType)t.ScaleId, t.CreationDate
                                   , model, templateWeights);
            })
            .ToList();
    }

    public async A.Task AddTemplateAsync(ITemplate template, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        var newTemplate = new Template(template.CompetenceModelId, (int)template.Type, (int)template.Scale
                                     , template.Name, template.CreationDate);
        await _context.Templates.AddAsync(newTemplate, token);
        await AddWeights(newTemplate, template.Weights, token);
    }

    public async A.Task UpdateTemplateAsync(ITemplate template, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        
        var updatingTemplate = await _context.Templates.Include(t => t.TemplateDetails)
            .SingleAsync(t => t.Id == template.Id, token);
        
        updatingTemplate.Name = template.Name;
        updatingTemplate.TemplateTypeId = (int)template.Type;
        updatingTemplate.ScaleId = (int)template.Scale;
        updatingTemplate.CompetenceModelId = template.CompetenceModelId;
        await UpdateWeights(template.Weights, updatingTemplate.TemplateDetails.ToList(), template.Id, token);
    }

    public async A.Task RemoveTemplateAsync(int id, CancellationToken token = default)
    {
        var deletingModel = await _context.Templates.Include(t => t.TemplateDetails)
            .SingleAsync(t => t.Id == id, token);
        _context.Templates.Remove(deletingModel);
        _context.TemplateDetails.RemoveRange(deletingModel.TemplateDetails);
    }

    public async A.Task SaveAllChanges(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);
    
    private async A.Task AddWeights(Template template 
        , List<TemplateWeight> weights
        , CancellationToken token = default)
    {
        var newWeights = weights
            .Select(w => new TemplateDetail(template, w.Task.Id, w.CompetenceId, w.Weight))
            .ToList();
        await  _context.TemplateDetails.AddRangeAsync(newWeights, token);
    }
    
    private async A.Task UpdateWeights(List<TemplateWeight> updatedWeights
        , List<TemplateDetail> details, int templateId, CancellationToken token = default)
    {
        if (updatedWeights is null || !updatedWeights.Any())
        {
            return;
        }
        
        var existingDetailsDict = details.ToDictionary(d => d.TaskId);
        var updatedWeightsDict = updatedWeights.ToDictionary(w => w.Task.Id);
        
        foreach (var existingDetail in details)
        {
            if (updatedWeightsDict.TryGetValue(existingDetail.TaskId, out var updatedWeight))
            {
                existingDetail.Weight = updatedWeight.Weight;
                existingDetail.CompetenceId = updatedWeight.CompetenceId;
            }
        }
        
        var weightsToAdd = updatedWeights
            .Where(w => !existingDetailsDict.ContainsKey(w.Task.Id))
            .Select(w => new TemplateDetail(templateId, w.Task.Id, w.CompetenceId, w.Weight))
            .ToList();
        await _context.TemplateDetails.AddRangeAsync(weightsToAdd, token);
        
        var weightsToRemove = details
            .Where(d => !updatedWeightsDict.ContainsKey(d.TaskId))
            .ToList();
    
         _context.TemplateDetails.RemoveRange(weightsToRemove);
    }
    
    private void UpdateWeights(List<TemplateDetail> updatingWeights
        , List<TemplateWeight> weights)
    {
        foreach (var weight in weights)
        {
            var updatingWeight = updatingWeights
                .Single(w => w.TaskId == weight.Task.Id);
            
            updatingWeight.Weight = weight.Weight;
        }
    }

    private async Task<List<TemplateWeight>> GetWeights(IEnumerable<TemplateDetail> details
        , CancellationToken token = default)
    {
        var taskIds = details.Select(d => d.TaskId).Distinct().ToList();
        var taskQuery = new TaskQuery(ids: taskIds);
        var tasks = await _taskRepository.GetTasksAsync(taskQuery, token);

        var weights = new List<TemplateWeight>();
        foreach (var detail in details)
        {
            var task = tasks.SingleOrDefault(t => t.Id == detail.TaskId);
            weights.Add(new TemplateWeight(detail.TemplateId, task, detail.CompetenceId, detail.Weight));
        }
        
        return weights;
    }

    private IQueryable<Template> GetAllTemplates()
        => _context.Templates
            .Include(t => t.TemplateDetails);
}