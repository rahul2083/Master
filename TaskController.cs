using System.Security.Claims;
using master.core.Entities;
using master.core.Enums;
using master.core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskStatus = master.core.Enums.TaskStatus;

namespace Master.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskManager _tasks;
        private readonly IUserRepository _users;

        public TaskController(ITaskManager tasks, IUserRepository users)
        {
            _tasks = tasks;
            _users = users;
        }

        // Get Current User
        private (int userId, UserRole role) CurrentUser()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
            var roleStr = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            return (int.Parse(idStr),
                Enum.TryParse<UserRole>(roleStr, out var r) ? r : UserRole.User);
        }

        // Load users for dropdown
        private async Task PopulateUsers()
        {
            var users = await _users.GetByRoleAsync(UserRole.User);
            ViewBag.Assignees = new SelectList(users, "Id", "FullName");
        }

        // ---------------- INDEX ----------------
        public async Task<IActionResult> Index()
        {
            var (uid, role) = CurrentUser();
            var tasks = await _tasks.GetListForUserAsync(uid, role);
            return View(tasks);
        }

        // ---------------- CREATE (GET) - Drawer ----------------
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create()
        {
            await PopulateUsers();

            return PartialView("Create", new TaskItem
            {
                ProjectName = "",
                Priority = TaskPriority.Medium,
                Status = TaskStatus.Todo
            });
        }

        // ---------------- CREATE (POST) - AJAX/Drawer ----------------
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(TaskItem model)
        {
            var (uid, role) = CurrentUser();

            if (!ModelState.IsValid)
            {
                await PopulateUsers();
                return PartialView("Create", model);
            }

            if (model.Subtasks != null)
            {
                foreach (var st in model.Subtasks)
                    st.TaskItem = model;
            }

            var (ok, error) = await _tasks.CreateAsync(model, uid, role);

            if (!ok)
            {
                ModelState.AddModelError("", error);
                await PopulateUsers();
                return PartialView("Create", model);
            }

            return Json(new { success = true });
        }

        // ---------------- EDIT (GET) - Drawer Support Ready ----------------
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var (uid, role) = CurrentUser();
            var list = await _tasks.GetListForUserAsync(uid, role);
            var item = list.FirstOrDefault(t => t.Id == id);

            if (item == null) return NotFound();

            await PopulateUsers();
            return PartialView("Edit", item);
        }

        // ---------------- EDIT (POST) - AJAX Ready ----------------
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Edit(TaskItem model)
        {
            var (uid, role) = CurrentUser();

            if (!ModelState.IsValid)
            {
                await PopulateUsers();
                return PartialView("Edit", model);
            }

            var (ok, error) = await _tasks.UpdateAsync(model, uid, role);

            if (!ok)
            {
                ModelState.AddModelError("", error);
                await PopulateUsers();
                return PartialView("Edit", model);
            }

            return Json(new { success = true });
        }

        // ---------------- DELETE ----------------
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var (uid, role) = CurrentUser();
            var list = await _tasks.GetListForUserAsync(uid, role);
            var item = list.FirstOrDefault(t => t.Id == id);

            if (item == null) return NotFound();
            return PartialView("Delete", item);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (uid, role) = CurrentUser();
            var (ok, error) = await _tasks.DeleteAsync(id, uid, role);

            if (!ok)
                return Json(new { success = false, error });

            return Json(new { success = true });
        }

        // ---------------- DETAILS ----------------
        [Authorize]
        public async Task<IActionResult> Detail(int id)
        {
            var (uid, role) = CurrentUser();
            var (ok, item, error) = await _tasks.GetByIdAsync(id, uid, role);

            if (!ok)
            {
                if (error == "Task not found") return NotFound();
                if (error == "Forbidden") return Forbid();
                return BadRequest(error);
            }

            return PartialView("Detail", item!);
        }
    }
}
