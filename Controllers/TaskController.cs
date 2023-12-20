using Microsoft.AspNetCore.Mvc;
using MobileBackend.Models.Task;
using MobileBackend.IService;
using Microsoft.AspNetCore.Authorization;

namespace MobileBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> AddTask(Tasks task)
    {
        try
        {
            bool success = await _taskService.AddTask(task);
            if (success)
            {
                return Ok("Add success");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message); // Internal server error
        }
        return BadRequest("Add failed");
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<ActionResult> UpdateTask(string id, Tasks task)
    {
        try
        {
            bool success = await _taskService.UpdateTask(id, task);
            if (success)
            {
                return Ok("Update Success");
            }
            return BadRequest("Has a error while updating");
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message); // Internal server error
        }
    }


    [HttpPatch("{id}/complete")]
    [Authorize]
    public async Task<ActionResult> CompleteTask(string id)
    {
        try
        {
            if (await _taskService.CompleteTask(id))
            {
                return Ok("Update Success");
            }
            return BadRequest("Has a error while updating");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id}/start")]
    [Authorize]
    public async Task<ActionResult> StartTask(string id)
    {
        try
        {
            if (await _taskService.StartTask(id))
            {
                return Ok("Update Success");
            }
            return BadRequest("Has a error while updating");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
