using master.core.Entities;
using master.core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Master.Controllers
{
    [Authorize]
    public class SubtaskController : Controller
    {
        private readonly ISubtaskRepository _subs;
        private readonly ITaskRepository _tasks;

        public SubtaskController(ISubtaskRepository subs, ITaskRepository tasks)
        {
            _subs = subs;
            _tasks = tasks;
        }

        // ---------------- PERMISSION CHECK ----------------
        private bool UserCanModify(TaskItem parent)
        {
            // Manager always allowed
            if (User.IsInRole("Manager"))
                return true;

            // Logged-in user ID
            int loggedUser = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Only assigned user allowed
            return parent.AssigneeId == loggedUser;
        }

        // ---------------- CREATE (GET) ----------------
        [HttpGet]
        public async Task<IActionResult> Create(int taskId)
        {
            var parent = await _tasks.GetByIdAsync(taskId);
            if (parent == null) return NotFound();

            if (!UserCanModify(parent))
                return RedirectToAction("AccessDenied", "Account");

            return View(new Subtask
            {
                TaskItemId = taskId,
                IsDone = false
            });
        }

        // ---------------- CREATE (POST) ----------------
        [HttpPost]
        public async Task<IActionResult> Create(Subtask model)
        {
            var parent = await _tasks.GetByIdAsync(model.TaskItemId);
            if (parent == null) return NotFound();

            if (!UserCanModify(parent))
                return RedirectToAction("AccessDenied", "Account");

            await _subs.AddAsync(model);

            return RedirectToAction("Detail", "Task", new { id = model.TaskItemId });
        }

        // ---------------- EDIT (GET) ----------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sub = await _subs.GetByIdAsync(id);
            if (sub == null) return NotFound();

            var parent = await _tasks.GetByIdAsync(sub.TaskItemId);
            if (parent == null) return NotFound();

            if (!UserCanModify(parent))
                return RedirectToAction("AccessDenied", "Account");

            return View(sub);
        }

        // ---------------- EDIT (POST) ----------------
        [HttpPost]
        public async Task<IActionResult> Edit(Subtask model)
        {
            var parent = await _tasks.GetByIdAsync(model.TaskItemId);
            if (parent == null) return NotFound();

            if (!UserCanModify(parent))
                return RedirectToAction("AccessDenied", "Account");

            await _subs.UpdateAsync(model);

            return RedirectToAction("Detail", "Task", new { id = model.TaskItemId });
        }

        // ---------------- DELETE ----------------
        public async Task<IActionResult> Delete(int id)
        {
            var sub = await _subs.GetByIdAsync(id);
            if (sub == null) return NotFound();

            var parent = await _tasks.GetByIdAsync(sub.TaskItemId);
            if (parent == null) return NotFound();

            if (!UserCanModify(parent))
                return RedirectToAction("AccessDenied", "Account");

            await _subs.DeleteAsync(sub);

            return RedirectToAction("Detail", "Task", new { id = sub.TaskItemId });
        }

        // ---------------- CLOSE (MARK DONE) ----------------
        [HttpPost]
        public async Task<IActionResult> Close(int id)
        {
            var sub = await _subs.GetByIdAsync(id);
            if (sub == null) return NotFound();

            var parent = await _tasks.GetByIdAsync(sub.TaskItemId);
            if (parent == null) return NotFound();

            if (!UserCanModify(parent))
                return RedirectToAction("AccessDenied", "Account");

            sub.IsDone = true;
            await _subs.UpdateAsync(sub);

            return RedirectToAction("Detail", "Task", new { id = sub.TaskItemId });
        }
    }
}
