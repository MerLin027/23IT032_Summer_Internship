using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamCollabTool.Data.Models;
using Task = TeamCollabTool.Data.Models.Task;

namespace TeamCollabTool.Web.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [StringLength(100, ErrorMessage = "Task title cannot be longer than 100 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedUserId { get; set; }
        public User AssignedUser { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public string Priority { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedById { get; set; }
        public User CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime LastModifiedDate { get; set; }

        [Display(Name = "Activities")]
        public ICollection<Activity> Activities { get; set; }
    }

    public class TaskCreateViewModel
    {
        [Required(ErrorMessage = "Task title is required")]
        [StringLength(100, ErrorMessage = "Task title cannot be longer than 100 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedUserId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public string Priority { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }

    public class TaskEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [StringLength(100, ErrorMessage = "Task title cannot be longer than 100 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedUserId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public string Priority { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }
} 