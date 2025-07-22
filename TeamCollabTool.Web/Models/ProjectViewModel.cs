using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamCollabTool.Data.Models;
using Task = TeamCollabTool.Data.Models.Task;

namespace TeamCollabTool.Web.Models
{
    public class ProjectViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters")]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Owner")]
        public string OwnerId { get; set; }
        public User Owner { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime LastModifiedDate { get; set; }

        [Display(Name = "Team Members")]
        public ICollection<ProjectMember> TeamMembers { get; set; }

        [Display(Name = "Tasks")]
        public ICollection<Task> Tasks { get; set; }
    }

    public class ProjectCreateViewModel
    {
        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters")]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class ProjectEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters")]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
} 