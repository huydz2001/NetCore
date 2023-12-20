using MobileBackend.IResponsitory;
using MobileBackend.IService;
using MobileBackend.Models.Task;

namespace MobileBackend.Services;

public class TaskService : ITaskService
{
    private readonly ITaskResponsitory _taskRepo;

    public TaskService(ITaskResponsitory taskRepo)
    {
        _taskRepo = taskRepo;
    }

    public async Task<bool> AddTask(Tasks task)
    {
        int totalTask = _taskRepo.GetTotalTasks();

        if (totalTask < 9999)
        {
            task.TaskId = (totalTask + 1).ToString().PadLeft(5, '0');
        }
        else task.TaskId = (totalTask + 1).ToString();
        return await _taskRepo.AddTask(task);
    }

    public Task<bool> CompleteTask(string taskId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StartTask(string taskId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateTask(string taskId, Tasks task)
    {
        return await _taskRepo.UpdateTask(taskId, task);
    }

}