using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;
using A = System.Threading.Tasks;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateRepository: ITemplateRepository
{
    private readonly AssessmentContext _context;
    private readonly ITaskRepository _taskRepository;

    public TemplateRepository(AssessmentContext context
                            , ITaskRepository taskRepository)
    {
        _context = context;
        _taskRepository = taskRepository;
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
        
        return templates
            .Select(t =>
            {
                var templateWeights = weights.Where(w => w.TemplateId == t.Id).ToList();
                return new ITemplate(t.Id, t.Name, (TemplateType)t.TemplateTypeId, (ScaleType)t.ScaleId, t.CreationDate
                                   , t.CompetenceModelId, templateWeights);
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
        await UpdateWeights(template.Weights, updatingTemplate.TemplateDetails.ToList(), token);
    }

    public async A.Task RemoveTemplateAsync(ITemplate template, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        
        var deletingModel = await _context.Templates.Include(t => t.TemplateDetails)
            .SingleAsync(t => t.Id == template.Id, token);
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
        , List<TemplateDetail> details, CancellationToken token = default)
    {
        if (updatedWeights is null || !updatedWeights.Any())
        {
            return;
        }

        var weightTaskIds = updatedWeights.Select(w => w.Task.Id).ToList();
        var weightsToUpdate = details.Where(uw => weightTaskIds.Contains(uw.TaskId)).ToList();
        UpdateWeights(weightsToUpdate, updatedWeights);
        
        var weightToAddOrRemoveIds = details.Except(weightsToUpdate).Select(w => w.TaskId).ToList();
        
        var weightToAddIds = weightTaskIds.Where(id => !weightToAddOrRemoveIds.Contains(id)).ToList();
        var weightsToAdd = details.Where(uw => weightToAddIds.Contains(uw.TaskId)).ToList();
        await _context.TemplateDetails.AddRangeAsync(weightsToAdd, token);
        
        var weightsToRemove = details
            .Except(weightsToAdd)
            .Except(weightsToUpdate)
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