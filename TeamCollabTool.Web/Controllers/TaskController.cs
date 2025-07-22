using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamCollabTool.Data;
using TeamCollabTool.Data.Models;
using TeamCollabTool.Web.Models;

namespace TeamCollabTool.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Task
        public async Task<IActionResult> Index(int? projectId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var tasks = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Include(t => t.CreatedBy)
                .Include(t => t.Activities)
                .AsQueryable();

            if (projectId.HasValue)
            {
                tasks = tasks.Where(t => t.ProjectId == projectId);
            }
            else
            {
                // Show tasks from projects where the user is a member
                var userProjectIds = await _context.ProjectMembers
                    .Where(pm => pm.UserId == currentUser.Id)
                    .Select(pm => pm.ProjectId)
                    .ToListAsync();

                tasks = tasks.Where(t => userProjectIds.Contains(t.ProjectId));
            }

            return View(await tasks.ToListAsync());
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Include(t => t.CreatedBy)
                .Include(t => t.Activities)
                    .ThenInclude(a => a.User)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Task/Create
        public async Task<IActionResult> Create(int? projectId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userProjectIds = await _context.ProjectMembers
                .Where(pm => pm.UserId == currentUser.Id)
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            ViewBag.Projects = new SelectList(
                await _context.Projects
                    .Where(p => userProjectIds.Contains(p.Id))
                    .ToListAsync(),
                "Id",
                "Name",
                projectId
            );

            ViewBag.Users = new SelectList(
                await _userManager.Users.ToListAsync(),
                "Id",
                "FullName"
            );

            var viewModel = new TaskCreateViewModel
            {
                ProjectId = projectId ?? 0,
                Status = "To Do",
                Priority = "Medium"
            };

            return View(viewModel);
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,ProjectId,AssignedUserId,Status,Priority,DueDate")] TaskCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var task = new Data.Models.Task
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    ProjectId = viewModel.ProjectId,
                    AssignedUserId = viewModel.AssignedUserId,
                    Status = viewModel.Status,
                    Priority = viewModel.Priority,
                    DueDate = viewModel.DueDate,
                    CreatedById = currentUser.Id,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };

                _context.Add(task);
                await _context.SaveChangesAsync(); // Save first so task.Id is generated

                // Log activity
                var activity = new Activity
                {
                    TaskId = task.Id,
                    UserId = currentUser.Id,
                    Description = "Task created",
                    Timestamp = DateTime.UtcNow
                };
                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = task.Id });
            }

            // If we got this far, something failed, redisplay form
            var currentUserId = (await _userManager.GetUserAsync(User)).Id;
            var userProjectIds = await _context.ProjectMembers
                .Where(pm => pm.UserId == currentUserId)
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            ViewBag.Projects = new SelectList(
                await _context.Projects
                    .Where(p => userProjectIds.Contains(p.Id))
                    .ToListAsync(),
                "Id",
                "Name",
                viewModel.ProjectId
            );

            ViewBag.Users = new SelectList(
                await _userManager.Users.ToListAsync(),
                "Id",
                "FullName",
                viewModel.AssignedUserId
            );

            return View(viewModel);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Forbid();
            }
            var isManager = await _userManager.IsInRoleAsync(currentUser, "Manager");
            if (!isManager && task.AssignedUserId != currentUser.Id)
            {
                return Forbid();
            }

            var userProjectIds = await _context.ProjectMembers
                .Where(pm => pm.UserId == currentUser.Id)
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            ViewBag.Projects = new SelectList(
                await _context.Projects
                    .Where(p => userProjectIds.Contains(p.Id))
                    .ToListAsync(),
                "Id",
                "Name",
                task.ProjectId
            );

            ViewBag.Users = new SelectList(
                await _userManager.Users.ToListAsync(),
                "Id",
                "FullName",
                task.AssignedUserId
            );

            var viewModel = new TaskEditViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description ?? string.Empty,
                ProjectId = task.ProjectId,
                AssignedUserId = task.AssignedUserId ?? string.Empty,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate
            };

            return View(viewModel);
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ProjectId,AssignedUserId,Status,Priority,DueDate")] TaskEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var task = await _context.Tasks.FindAsync(id);
                    if (task == null)
                    {
                        return NotFound();
                    }

                    var currentUser = await _userManager.GetUserAsync(User);
                    var oldStatus = task.Status;
                    var oldAssignedUser = task.AssignedUserId;

                    task.Title = viewModel.Title;
                    task.Description = viewModel.Description;
                    task.ProjectId = viewModel.ProjectId;
                    task.AssignedUserId = viewModel.AssignedUserId;
                    task.Status = viewModel.Status;
                    task.Priority = viewModel.Priority;
                    task.DueDate = viewModel.DueDate;
                    task.LastModifiedDate = DateTime.UtcNow;

                    // Log activity for status change
                    if (oldStatus != task.Status)
                    {
                        var activity = new Activity
                        {
                            TaskId = task.Id,
                            UserId = currentUser.Id,
                            Description = $"Status changed from {oldStatus} to {task.Status}",
                            Timestamp = DateTime.UtcNow
                        };
                        _context.Activities.Add(activity);
                    }

                    // Log activity for assignment change
                    if (oldAssignedUser != task.AssignedUserId)
                    {
                        var activity = new Activity
                        {
                            TaskId = task.Id,
                            UserId = currentUser.Id,
                            Description = $"Task reassigned to {task.AssignedUser?.FullName ?? "Unassigned"}",
                            Timestamp = DateTime.UtcNow
                        };
                        _context.Activities.Add(activity);
                    }

                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = viewModel.Id });
            }

            // If we got this far, something failed, redisplay form
            var currentUserId = (await _userManager.GetUserAsync(User)).Id;
            var userProjectIds = await _context.ProjectMembers
                .Where(pm => pm.UserId == currentUserId)
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            ViewBag.Projects = new SelectList(
                await _context.Projects
                    .Where(p => userProjectIds.Contains(p.Id))
                    .ToListAsync(),
                "Id",
                "Name",
                viewModel.ProjectId
            );

            ViewBag.Users = new SelectList(
                await _userManager.Users.ToListAsync(),
                "Id",
                "FullName",
                viewModel.AssignedUserId
            );

            return View(viewModel);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Task/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int taskId, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Forbid();
            var task = await _context.Tasks.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) return NotFound();
            var isMember = await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == task.ProjectId && pm.UserId == currentUser.Id);
            var isAssigned = task.AssignedUserId == currentUser.Id;
            if (!isMember && !isAssigned) return Forbid();
            var comment = new Comment
            {
                TaskId = taskId,
                UserId = currentUser.Id,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = taskId });
        }

        // POST: Task/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Forbid();
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            if (task.AssignedUserId != currentUser.Id) return Forbid();
            var oldStatus = task.Status;
            task.Status = status;
            task.LastModifiedDate = DateTime.UtcNow;
            // Log activity
            var activity = new Activity
            {
                TaskId = task.Id,
                UserId = currentUser.Id,
                Description = $"Status changed from {oldStatus} to {status}",
                Timestamp = DateTime.UtcNow
            };
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}