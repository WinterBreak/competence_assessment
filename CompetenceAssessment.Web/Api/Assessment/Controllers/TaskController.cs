using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Web.Assessment;
using Microsoft.AspNetCore.Mvc;

namespace TaskAssessment.Web.Assessment;

/// <summary>
/// Задания
/// </summary>
[Route("api/tasks")]
[ApiController]
public class TaskController: ControllerBase
{
    private readonly ITaskService _competenceService;

    public TaskController(ITaskService competenceService)
    {
        _competenceService = competenceService;
    }

    /// <summary>
    /// Получение заданий
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTasksAsync(CancellationToken token = default)
    {
        var query = new TaskQuery();
        var competencies = await _competenceService.GetTasksAsync(query, token);
        return Ok(competencies);
    }

    /// <summary>
    /// Создание заданий
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTaskAsync(TaskCreateRequest request
        , CancellationToken token = default)
    {
        var command = new CreateTaskCommand(request.Text, (TaskType)request.TypeId, request.Answer);
        var errors = await _competenceService.CreateTaskAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение заданий
    /// </summary>
    [HttpPatch]
    public async Task<IActionResult> UpdateTaskAsync(TaskUpdateRequest request
        , CancellationToken token = default)
    {
        var command = new UpdateTaskCommand(request.Id, request.Text,  request.Answer, (TaskType)request.Type);
        var errors = await _competenceService.UpdateTaskAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Удаление заданий
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteTaskAsync(TaskDeleteRequest request
        , CancellationToken token = default)
    {
        var command = new DeleteTaskCommand(request.Id);
        var errors = await _competenceService.DeleteTaskAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}