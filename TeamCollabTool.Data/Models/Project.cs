using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamCollabTool.Data.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string? OwnerId { get; set; }
        public User? Owner { get; set; }
        public ICollection<Task>? Tasks { get; set; } = new List<Task>();
        public ICollection<ProjectMember>? TeamMembers { get; set; } = new List<ProjectMember>();
    }
}