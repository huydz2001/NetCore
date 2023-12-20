using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MobileBackend.Models.Task;
public class Tasks
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string? Id { get; set; }

    [BsonElement("taskId")]
    public string? TaskId { get; set; }

    [BsonElement("taskType")]
    public int TaskType { get; set; }

    [BsonElement("taskName")]
    public string TaskName { get; set; }

    [BsonElement("startDate")]
    public DateOnly StartDate { get; set; }

    [BsonElement("startTime")]
    public string StartTime { get; set; }

    [BsonElement("endTime")]
    public string EndTime { get; set; }

    [BsonElement("priority")]
    public int Priority { get; set; }

    [BsonElement("status")]
    public int Status { get; set; } = 0;

    [BsonElement("owner")]
    public string Owner { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; } = null;

    [BsonElement("createDate")]
    public DateTime CreateDate { get; set; } = DateTime.Now;

    [BsonElement("createBy")]
    public string CreateBy { get; set; }

    [BsonElement("updateBy")]
    public string? UpdateBy { get; set; } = null;

    [BsonElement("updateDate")]
    public DateTime? UpdateDate { get; set; } = null;

    public Tasks(int taskType, string taskName, DateOnly startDate, string startTime, string endTime,
        int priority, int status, string owner, string description, DateTime createDate, string createBy,
        string? updateBy, DateTime? updateDate)
    {
        TaskType = taskType;
        TaskName = taskName;
        StartDate = startDate;
        StartTime = startTime;
        EndTime = endTime;
        Priority = priority;
        Status = status;
        Owner = owner;
        Description = description;
        CreateDate = createDate;
        CreateBy = createBy;
        UpdateBy = updateBy;
        UpdateDate = updateDate;
    }
}