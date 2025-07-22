using System;
using System.Collections.Generic;

namespace TeamCollabTool.Data.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } // e.g., To Do, In Progress, Done
        public string Priority { get; set; } // e.g., Low, Medium, High
        public DateTime? DueDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string? AssignedUserId { get; set; }
        public User AssignedUser { get; set; }
        public string CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}