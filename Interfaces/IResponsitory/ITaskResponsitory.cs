using MobileBackend.Models.Task;

namespace MobileBackend.IResponsitory;
public interface ITaskResponsitory
{
    Task<bool> AddTask(Tasks task);
    Task<bool> UpdateTask(string taskId, Tasks task);
    Task<bool> CompleteTask(string taskId);
    Task<bool> StartTask(string taskId);
    int GetTotalTasks();
}