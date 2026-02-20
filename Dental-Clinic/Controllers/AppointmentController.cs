using Dental_Clinic.Data;
using Dental_Clinic.Filters;
using Dental_Clinic.Models;
using Dental_Clinic.Services;
using Dental_Clinic.ViewModels;
using Hangfire;
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
                return View(model);

            model.Status = "Pending";

            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();

            // Combine date + time
            DateTime appointmentDateTime =
                model.AppointmentDate.Date
                .Add(model.AppointmentTime);

            // Subtract 2 hours
            DateTime callTime = appointmentDateTime.AddHours(-2);

            // If callTime already passed → trigger immediately
            if (callTime <= DateTime.Now)
            {
                await TriggerAppointmentCall(model.Id);
            }
            else
            {
                BackgroundJob.Schedule(
                    () => TriggerAppointmentCall(model.Id),
                    callTime
                );
            }

            TempData["success"] = "Appointment saved. Call scheduled successfully!";
            return RedirectToAction("List");
        }

        public async Task TriggerAppointmentCall(int appointmentId)
        {
            var appointment = _context.Appointments.Find(appointmentId);
            if (appointment == null) return;

            string appointmentDate =
                appointment.AppointmentDate.ToString("dd MMM yyyy");

            await _roboCall.SendAppointmentCall(
                phoneNumber: appointment.PhoneNumber,
                voiceId: 252,
                clinicName: "Nouman Dental Clinic",
                appointmentDate: appointmentDate
            );

            appointment.CallStatus = "Triggered";
            appointment.CallTriggeredAt = DateTime.Now;

            await _context.SaveChangesAsync();
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

        public IActionResult List(string search, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Appointments.AsQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(a =>
                    a.PatientName.ToLower().Contains(search) ||
                    a.PhoneNumber.ToLower().Contains(search) ||
                    a.Branch.ToLower().Contains(search)
                );
            }

            int totalRecords = query.Count();

            var appointments = query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<Appointment>
            {
                Items = appointments,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            ViewBag.Search = search;

            return View(result);
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
