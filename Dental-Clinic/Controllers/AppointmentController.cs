using Dental_Clinic.Data;
using Dental_Clinic.Filters;
using Dental_Clinic.Models;
using Dental_Clinic.Services;
using Dental_Clinic.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic.Controllers
{
    [AuthorizeRole("Admin", "Receptionist")]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoboCallService _roboCall;

        public AppointmentController(ApplicationDbContext context, RoboCallService roboCall)
        {
            _context = context;
            _roboCall = roboCall;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = new List<string>
        {
            "Islamabad",
            "Peshawar",
            "Karachi"
        };
                return View(model);
            }

            model.Status = "Pending";

            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();

            // 🔥 Trigger RoboCall
            try
            {
                string appointmentDate =
                    model.AppointmentDate.ToString("dd MMM yyyy");

                var response = await _roboCall.SendAppointmentCall(
                    phoneNumber: model.PhoneNumber,
                    voiceId: 252, // 👈 replace with real voice ID
                    clinicName: "Nouman Dental Clinic",
                    appointmentDate: appointmentDate
                );

                model.CallStatus = "Triggered";
                model.CallTriggeredAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch
            {
                model.CallStatus = "Failed";
                await _context.SaveChangesAsync();
            }

            TempData["success"] = "Appointment added & Call triggered!";
            return RedirectToAction("List");
        }


        // POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Appointment model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    model.Status = "Pending";   // 🔒 force default
        //    _context.Appointments.Add(model);
        //    _context.SaveChanges();

        //    TempData["success"] = "Appointment added successfully!";
        //    return RedirectToAction("List");
        //}

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

        public IActionResult List(int page = 1)
        {
            int pageSize = 10;

            var query = _context.Appointments
                .OrderByDescending(x => x.CreatedAt);

            var totalRecords = query.Count();

            var appointments = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<Appointment>
            {
                Items = appointments,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return View(model);
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
