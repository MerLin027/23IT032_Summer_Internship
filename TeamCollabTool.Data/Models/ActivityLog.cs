using System;

namespace TeamCollabTool.Data.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int? TaskId { get; set; }
        public Task Task { get; set; }
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
} 