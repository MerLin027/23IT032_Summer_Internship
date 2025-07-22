using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamCollabTool.Data;
using TeamCollabTool.Data.Models;
using TeamCollabTool.Web.Models;

namespace TeamCollabTool.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProjectController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async System.Threading.Tasks.Task LogActivity(string action, string userId, int? projectId = null)
        {
            var log = new ActivityLog
            {
                Action = action,
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                ProjectId = projectId
            };
            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        // GET: Project
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }
            var projects = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.TeamMembers)
                    .ThenInclude(pm => pm.User)
                .Where(p => p.OwnerId == user.Id || p.TeamMembers.Any(pm => pm.UserId == user.Id))
                .ToListAsync();

            return View(projects);
        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.TeamMembers)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // GET: Project/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async System.Threading.Tasks.Task<IActionResult> Create([Bind("Name,Description")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found or not logged in.");
                        return View(project);
                    }
                    project.OwnerId = user.Id;
                    project.CreatedDate = DateTime.UtcNow;
                    project.LastModifiedDate = DateTime.UtcNow;

                    _context.Add(project);
                    await _context.SaveChangesAsync();

                    // Add the owner as a team member
                    var projectMember = new ProjectMember
                    {
                        ProjectId = project.Id,
                        UserId = user.Id,
                        Role = "Owner",
                        JoinedDate = DateTime.UtcNow
                    };
                    _context.ProjectMembers.Add(projectMember);
                    await _context.SaveChangesAsync();

                    await LogActivity("Created project", user.Id, project.Id);
                    TempData["SuccessMessage"] = "Project created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Model validation failed. Please check your input.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }
            return View(project);
        }

        // GET: Project/Edit/5
        [Authorize(Roles = "Manager")]
        public async System.Threading.Tasks.Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || project.OwnerId != user.Id)
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async System.Threading.Tasks.Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProject = await _context.Projects.FindAsync(id);
                    if (existingProject == null)
                    {
                        return NotFound();
                    }

                    var user = await _userManager.GetUserAsync(User);
                    if (user == null || existingProject.OwnerId != user.Id)
                    {
                        return Forbid();
                    }

                    existingProject.Name = project.Name;
                    existingProject.Description = project.Description;
                    existingProject.LastModifiedDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    await LogActivity("Edited project", user.Id, existingProject.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "Manager")]
        public async System.Threading.Tasks.Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || project.OwnerId != user.Id)
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || project.OwnerId != user.Id)
            {
                return Forbid();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            await LogActivity("Deleted project", user.Id, project.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Project/AddMember/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddMember(int? id)
        {
            if (id == null) return NotFound();
            var project = await _context.Projects.Include(p => p.TeamMembers).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return NotFound();
            
            // Get only users with "Member" role
            var memberUsers = new List<User>();
            var allUsers = await _userManager.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Member"))
                {
                    memberUsers.Add(user);
                }
            }
            
            ViewBag.Users = memberUsers;
            ViewBag.ProjectId = id;
            return View();
        }

        // POST: Project/AddMember/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddMember(int id, string userId, string role)
        {
            var project = await _context.Projects.Include(p => p.TeamMembers).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return NotFound();
            if (project.TeamMembers?.Any(pm => pm.UserId == userId) == true)
            {
                TempData["ErrorMessage"] = "User is already a member of this project.";
                return RedirectToAction("Details", new { id });
            }
            var member = new ProjectMember
            {
                ProjectId = id,
                UserId = userId,
                Role = role,
                JoinedDate = DateTime.UtcNow
            };
            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Member added successfully.";
            return RedirectToAction("Details", new { id });
        }

        // TODO: Assign Users to Project action (Manager only, log activity)

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}