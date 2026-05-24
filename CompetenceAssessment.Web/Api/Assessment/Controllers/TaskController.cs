using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Web.Assessment;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetTasksAsync([FromQuery] TaskGetRequest request
                                                 , CancellationToken token = default)
    {
        var query = new TaskQuery(page: request.Page, pageSize: request.PageSize, (TaskType) request.Type);
        var tasks = await _competenceService.GetPaginatedTasksAsync(query, token);
        return Ok(tasks);
    }

    /// <summary>
    /// Создание заданий
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CreateTaskAsync(TaskCreateRequest request
        , CancellationToken token = default)
    {
        var command = new CreateTaskCommand(request.Text, (TaskType)request.Type, request.Answers);
        var errors = await _competenceService.CreateTaskAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение заданий
    /// </summary>
    [HttpPatch]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> UpdateTaskAsync(TaskUpdateRequest request
        , CancellationToken token = default)
    {
        var command = new UpdateTaskCommand(request.Id, request.Text,  request.Answers, (TaskType)request.Type);
        var errors = await _competenceService.UpdateTaskAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Удаление заданий
    /// </summary>
    [Route("{id}")]
    [HttpDelete]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteTaskAsync(int id, CancellationToken token = default)
    {
        var command = new DeleteTaskCommand(id);
        var errors = await _competenceService.DeleteTaskAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}