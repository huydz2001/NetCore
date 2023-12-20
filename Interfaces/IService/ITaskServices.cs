using MobileBackend.Models.Task;

namespace MobileBackend.IService;
public interface ITaskService
{
    Task<bool> AddTask(Tasks task);
    Task<bool> UpdateTask(string taskId, Tasks task);
    Task<bool> CompleteTask(string taskId);
    Task<bool> StartTask(string taskId);
}