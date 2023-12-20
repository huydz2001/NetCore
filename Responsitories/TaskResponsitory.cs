using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MobileBackend.IResponsitory;
using MobileBackend.Models;
using MobileBackend.Models.Task;
using MobileBackend.Models.User;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MobileBackend.Responsitories;

public class TaskResponsitory : ITaskResponsitory
{
    private readonly IMongoCollection<Tasks> _taskCollection;
    private readonly IMongoCollection<User> _userCollection;

    public TaskResponsitory(IOptions<MongoDbSetting> mogoDbSetting)
    {
        MongoClient mongoClient = new MongoClient(mogoDbSetting.Value.ConnectionURI);
        IMongoDatabase database = mongoClient.GetDatabase(mogoDbSetting.Value.DatabaseName);
        _taskCollection = database.GetCollection<Tasks>("Task");
        _userCollection = database.GetCollection<User>("User");
    }

    public async Task<bool> AddTask(Tasks task)
    {
        bool success = true;

        if (HasTimeRangeConflict(task.StartTime, task.EndTime, task.StartDate, task.Owner, string.Empty))
        {
            success = false;
            throw new Exception(task.Owner + " got a task from " + task.StartTime + " to " + task.EndTime);
        }

        if (success)
        {
            try
            {
                await _taskCollection.InsertOneAsync(task);
                AddTaskToUser(task.TaskId, task.Owner);
            }
            catch (Exception ex)
            {
                success = false;
                var filter = Builders<Tasks>.Filter.Eq(x => x.TaskId, task.TaskId);
                _taskCollection.DeleteOne(filter);
                throw new Exception(ex.Message);
            }
        }
        return success;

    }

    public async Task<bool> UpdateTask(string taskId, Tasks task)
    {
        var filter = Builders<Tasks>.Filter.Eq(x => x.TaskId, taskId);
        bool success = true;

        if (HasTimeRangeConflict(task.StartTime, task.EndTime, task.StartDate, task.Owner, taskId))
        {
            success = false;
            throw new Exception(task.Owner + " got a task from " + task.StartTime + " to " + task.EndTime);
        }
        if (success)
        {
            var update = Builders<Tasks>.Update
                            .Set(x => x.TaskName, task.TaskName)
                            .Set(x => x.StartDate, task.StartDate)
                            .Set(x => x.StartTime, task.StartTime)
                            .Set(x => x.EndTime, task.EndTime)
                            .Set(x => x.Priority, task.Priority)
                            .Set(x => x.UpdateBy, task.UpdateBy)
                            .Set(x => x.UpdateDate, DateTime.Now);

            var result = await _taskCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        return false;
    }

    private async void AddTaskToUser(string? taskId, string owner)
    {
        var update = Builders<User>.Update.Push(x => x.Tasks, taskId);
        var filter = Builders<User>.Filter.Eq(x => x.UserId, owner);
        var result = await _userCollection.UpdateOneAsync(filter, update);

        if (result.ModifiedCount == 0)
        {
            throw new Exception("Has error while updating");
        }

    }

    private static int ConvertToMinutes(string time)
    {
        var parts = time.Split(':');
        int hours = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        return hours * 60 + minutes;
    }

    public int GetTotalTasks()
    {
        return _taskCollection.Find(_ => true).ToList().Count;
    }

    private bool HasTimeRangeConflict(string startTime, string endTime, [FromQuery] DateOnly startDate, string userId, string taskId)
    {
        bool hasConflict = false;
        var tasks = new List<Tasks>() ;
        var filter = FilterDefinition<Tasks>.Empty;
        int newStart = ConvertToMinutes(startTime);
        int newEnd = ConvertToMinutes(endTime);

        if (taskId == string.Empty)
        {
            filter = Builders<Tasks>.Filter.And(
                Builders<Tasks>.Filter.Eq(x => x.StartDate, startDate),
                Builders<Tasks>.Filter.Eq(x => x.Owner, userId)
            );
        }
        else
        {
            filter = Builders<Tasks>.Filter.And(
                Builders<Tasks>.Filter.Eq(x => x.StartDate, startDate),
                Builders<Tasks>.Filter.Eq(x => x.Owner, userId),
                Builders<Tasks>.Filter.Ne(x => x.TaskId, taskId)
            );
        }

        tasks = _taskCollection.Find(filter).ToList();
        foreach (var range in tasks)
        {
            int existingStart = ConvertToMinutes(range.StartTime);
            int existingEnd = ConvertToMinutes(range.EndTime);

            if (newStart < existingEnd && existingStart < newEnd)
            {
                hasConflict = true; // Xảy ra xung đột
            }
        }
        return hasConflict;

        // if(tasks.Count > 0) return true;
        // else return false; // Không xảy ra xung đột
    }

    public async Task<bool> CompleteTask(string taskId)
    {
        var filter = Builders<Tasks>.Filter.Eq(x => x.TaskId, taskId);
        var update = Builders<Tasks>.Update.Set(x => x.Status, 2);
        var result = await _taskCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> StartTask(string taskId)
    {
        var filter = Builders<Tasks>.Filter.Eq(x => x.TaskId, taskId);
        var update = Builders<Tasks>.Update.Set(x => x.Status, 1);
        var result = await _taskCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }


}