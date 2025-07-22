using System;
using System.ComponentModel.DataAnnotations;
using TeamCollabTool.Data.Models;
using ProjectMember = TeamCollabTool.Web.Models.ProjectViewModel;

namespace TeamCollabTool.Web.Models
{
    public class ProjectMemberViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Joined Date")]
        public DateTime JoinedDate { get; set; }
    }

    public class ProjectMemberCreateViewModel
    {
        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }

    public class ProjectMemberEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
} 