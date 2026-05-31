using CompetenceAssessment.Core.Models;
using BLL = CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;
using A = System.Threading.Tasks;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceModelRepository: BLL.ICompetenceModelRepository
{
    private readonly AssessmentContext _context;
    private readonly BLL.ICompetenceRepository _competenceRepository;

    public CompetenceModelRepository(AssessmentContext context,
        BLL.ICompetenceRepository competenceRepository)
    {
        _context = context;
        _competenceRepository = competenceRepository;
    }

    public async Task<BLL.CompetenceModel?> GetCompetenceModelAsync(BLL.CompetenceModelQuery query
        , CancellationToken token = default)
    {
        var models = await GetCompetenceModelsAsync(query, token);
        return models.SingleOrDefault();
    }

    public async Task<List<BLL.CompetenceModel>> GetCompetenceModelsAsync(BLL.CompetenceModelQuery query
        , CancellationToken token = default)
    {
        var specification = new CompetenceModelSpecificationBuilder().WithQuery(query).Build();
        var models = await GetAllModels()
            .Where(specification)
            .OrderBy(cm => cm.Name)
            .ToListAsync(token);
        var modelIds = models.Select(model => model.Id).ToList();
        var weights = await GetWeights(modelIds, token);
        
        return models
            .Select(cm =>
            {
                var modelWeights = weights.Where(w => w.ModelId == cm.Id).ToList();
                return new BLL.CompetenceModel(cm.Id, cm.Name, cm.Description, cm.CreationDate, modelWeights);
            })
            .ToList();
    }

    public async Task<PaginatedResponse<BLL.CompetenceModel>> GetPaginatedModelsAsync(BLL.CompetenceModelQuery query
        , CancellationToken token = default)
    {
        var specification = new CompetenceModelSpecificationBuilder().WithQuery(query).Build();
        var models = await GetAllModels()
            .Where(specification)
            .OrderBy(cm => cm.Name)
            .Skip(query.PageSize * (query.Page - 1))
            .Take(query.PageSize)
            .ToListAsync(token);
        
        var modelIds = models.Select(model => model.Id).ToList();
        var weights = await GetWeights(modelIds, token);
        
        var bllModels = models
            .Select(cm =>
            {
                var modelWeights = weights.Where(w => w.ModelId == cm.Id).ToList();
                return new BLL.CompetenceModel(cm.Id, cm.Name, cm.Description, cm.CreationDate, modelWeights);
            })
            .ToList();
        
        var totalCount = _context.CompetenceModels.Where(specification).Count();
        return new PaginatedResponse<BLL.CompetenceModel>()
        {
            Items = bllModels,
            CurrentPage = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async A.Task AddCompetenceModelAsync(BLL.CompetenceModel competenceModel
        , CancellationToken token = default)
    { 
        ArgumentNullException.ThrowIfNull(competenceModel);
        var newModel = new CompetenceModel(competenceModel.Name, competenceModel.Description
                                         , competenceModel.CreationDate);
        await  _context.CompetenceModels.AddAsync(newModel, token); 
        await AddWeights(newModel, competenceModel.Competencies, token);
    }

    public async A.Task UpdateCompetenceModelAsync(BLL.CompetenceModel competenceModel
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(competenceModel);
        
        var updatingModel = await GetModelByIdAsync(competenceModel.Id, token);
        
        updatingModel.Name = competenceModel.Name;
        updatingModel.Description = competenceModel.Description;
        
        await UpdateWeights(competenceModel.Id, competenceModel.Competencies, token);
    }

    public async A.Task RemoveCompetenceModelAsync(int id, CancellationToken token = default)
    {
        var deletingModel = await GetModelByIdAsync(id, token);
        _context.CompetenceModels.Remove(deletingModel);
        _context.CompetenceModelDetails.RemoveRange(deletingModel.Weights);
    }

    public async A.Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);

    private async Task<List<BLL.CompetenceWeight>> GetWeights(
        List<int> modelIds, CancellationToken token = default)
    {
        if (modelIds is null || !modelIds.Any())
        {
            return new List<BLL.CompetenceWeight>();
        }
        
        var dalWeights = await _context.CompetenceModelDetails
            .Where(d => modelIds.Contains(d.CompetenceModelId))
            .ToListAsync(token);
        var competenceIds = dalWeights.Select(d => d.CompetenceId).Distinct().ToList();
        var competenceQuery = new BLL.CompetenceQuery(ids: competenceIds);
        var competencies = await _competenceRepository
            .GetCompetenciesAsync(competenceQuery, token);
        
        return dalWeights
            .Select(w =>
            {
                var competence = competencies.SingleOrDefault(c => c.Id == w.CompetenceId);
                return new BLL.CompetenceWeight(competence, w.CompetenceModelId, w.Weight);
            })
            .ToList();
    }

    private async A.Task AddWeights(CompetenceModel model 
        , List<BLL.CompetenceWeight> weights
        , CancellationToken token = default)
    {
        var newWeights = weights
            .Select(w => new CompetenceModelDetail(model, w.Competence.Id, w.Weight))
            .ToList(); 
        await  _context.CompetenceModelDetails.AddRangeAsync(newWeights, token);
    }
    
    private async A.Task UpdateWeights(int modelId, List<BLL.CompetenceWeight> weights
        , CancellationToken token = default)
    {
        if (weights is null || !weights.Any())
        {
            return;
        }
        
        var existingWeights = await GetWeightsByModelIdAsync(modelId, token);
        var existingWeightDict = existingWeights.ToDictionary(w => w.CompetenceId);
        
        var inputWeightDict = weights.ToDictionary(w => w.Competence.Id);
        foreach (var existingWeight in existingWeights)
        {
            if (inputWeightDict.TryGetValue(existingWeight.CompetenceId, out var inputWeight))
            {
                existingWeight.Weight = inputWeight.Weight;
            }
        }

        var weightsToAdd = weights
            .Where(w => !existingWeightDict.ContainsKey(w.Competence.Id))
            .Select(w => new CompetenceModelDetail(modelId, w.Competence.Id, w.Weight))
            .ToList();
        await _context.CompetenceModelDetails.AddRangeAsync(weightsToAdd, token);
        
        var weightsToRemove = existingWeights
            .Where(w => !inputWeightDict.ContainsKey(w.CompetenceId))
            .ToList();
        _context.CompetenceModelDetails.RemoveRange(weightsToRemove);
    }
    
    private void UpdateWeights(List<CompetenceModelDetail> updatingWeights
        , List<BLL.CompetenceWeight> weights)
    {
        if (!updatingWeights.Any())
        {
            return;
        }
        
        foreach (var weight in weights)
        {
            var updatingWeight = updatingWeights
                .SingleOrDefault(w => w.CompetenceId == weight.Competence.Id);
            
            if (updatingWeight is null)
            {
                continue;
            }
            
            updatingWeight.Weight = weight.Weight;
        }
    }
    
    private async Task<List<CompetenceModelDetail>> GetWeightsByModelIdAsync(int modelId
        , CancellationToken token = default)
        => await _context.CompetenceModelDetails
            .Where(cm => cm.CompetenceModelId == modelId)
            .ToListAsync(token);

    private async Task<CompetenceModel> GetModelByIdAsync(int id, CancellationToken token = default)
    {
        return await _context.CompetenceModels.SingleAsync(cm => cm.Id == id, token);
    }
    
    private IQueryable<CompetenceModel> GetAllModels()
        => _context.CompetenceModels
            .Include(cm => cm.Weights)
                .ThenInclude(w => w.Competence);
}