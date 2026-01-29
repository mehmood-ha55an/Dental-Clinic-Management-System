using Dental_Clinic.Data;
using Dental_Clinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Create()
        {
            ViewBag.Branches = new List<string>
            {
                "Islamabad",
                "Peshawar",
                "Karachi"
            };
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Status = "Pending";   // 🔒 force default
            _context.Appointments.Add(model);
            _context.SaveChanges();

            TempData["success"] = "Appointment added successfully!";
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var appt = _context.Appointments.Find(id);
            if (appt == null)
                return Json(new { success = false });

            appt.Status = status;
            _context.SaveChanges();

            return Json(new { success = true });
        }


        // Optional: List
        public IActionResult List()
        {
            var data = _context.Appointments
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(data);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Branches = new List<string>
            {
                "Islamabad",
                "Peshawar",
                "Karachi"
            };
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Appointment model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Appointments.Update(model);
            _context.SaveChanges();

            TempData["success"] = "Appointment updated successfully!";
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return Json(new { success = false, message = "Appointment not found." });
            }

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return Json(new { success = true, message = "Appointment deleted successfully!" });
        }


    }
}
