using System;

namespace TeamCollabTool.Data.Models
{
    public class ProjectMember
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Role { get; set; }
        public DateTime JoinedDate { get; set; }
    }
} 