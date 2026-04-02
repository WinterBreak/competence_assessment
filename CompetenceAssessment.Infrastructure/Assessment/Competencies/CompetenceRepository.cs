using DAL = CompetenceAssessment.Infrastructure.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace CompetenceAssessment.Infrastructure.Competencies;

public class CompetenceRepository: ICompetenceRepository
{
    private readonly AssessmentContext _context;

    public CompetenceRepository(AssessmentContext context)
    {
        _context = context;
    }
    
    public async Task<Competence?> GetCompetenceAsync(CompetenceQuery query
        , CancellationToken token = default)
    {
        var competencies = await GetCompetenciesAsync(query, token);
        return competencies.SingleOrDefault();
    }

    public async Task<List<Competence>> GetCompetenciesAsync(CompetenceQuery query
        , CancellationToken token = default)
    {
        var specification = new DAL.CompetenceSpecificationBuilder()
            .WithIds(query.Ids)
            .WithNames(query.Names)
            .Build();
        
        return await GetAllCompetences()
            .Where(specification)
            .Select(c => new Competence(c.Id, c.Name, c.Description))
            .ToListAsync(token);
    }

    public async Task AddCompetenceAsync(Competence competence
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(competence);
        var newCompetence = new DAL.Competence(competence.Name, competence.Description);
        await _context.Competences.AddAsync(newCompetence, token);
    }

    public async Task UpdateCompetenceAsync(Competence competence
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(competence);
        
        var updatingCategory = await GetCompetenceByIdAsync(competence.Id, token);
        
        updatingCategory.Name = competence.Name;
        updatingCategory.Description = competence.Description;
    }

    public async Task RemoveCompetenceAsync(int id
        , CancellationToken token = default)
    {
        var removingCategory = await GetCompetenceByIdAsync(id, token);
        _context.Competences.Remove(removingCategory);
    }

    public async Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);
    
    private IQueryable<DAL.Competence> GetAllCompetences() => _context.Competences;

    private async Task<DAL.Competence> GetCompetenceByIdAsync(int id, CancellationToken token = default)
    {
        var specification = new DAL.CompetenceSpecificationBuilder().WithId(id).Build();
        return await GetAllCompetences()
            .Where(specification)
            .SingleAsync(token);
    }
}