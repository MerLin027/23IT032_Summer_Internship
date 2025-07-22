using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamCollabTool.Data.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [NotMapped]
        public string? FullName => $"{FirstName} {LastName}".Trim();
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public ICollection<Project> OwnedProjects { get; set; }
        public ICollection<ProjectMember> ProjectMemberships { get; set; }
        public ICollection<Task> AssignedTasks { get; set; }
        public ICollection<Task> CreatedTasks { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}