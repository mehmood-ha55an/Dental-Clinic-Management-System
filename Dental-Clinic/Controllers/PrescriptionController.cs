using Dental_Clinic.Data;
using Dental_Clinic.Filters;
using Dental_Clinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Clinic.Controllers
{
    public class PrescriptionController : Controller
    {
        [AuthorizeRole("Admin", "Receptionist")]

        private readonly ApplicationDbContext _context;

        public PrescriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Create(int appointmentId)
        {
            var appointment = _context.Appointments.Find(appointmentId);
            if (appointment == null)
                return NotFound();

            ViewBag.Appointment = appointment;
            return View(new Prescription { AppointmentId = appointmentId });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Prescription model)
        {
            try
            {
                //if (!ModelState.IsValid)
                //    return View(model);

                _context.Prescriptions.Add(model);
                _context.SaveChanges();

                // Redirect to Print after save
                return RedirectToAction("Print", new { appointmentId = model.AppointmentId });
            }
            catch (Exception ex) 
            {
               throw new Exception(ex.Message);
            }
        }

        // View Prescription
        public IActionResult Print(int appointmentId)
        {
            var prescription = _context.Prescriptions
                .Include(x => x.Items)
                .Include(x => x.Appointment)
                .FirstOrDefault(x => x.AppointmentId == appointmentId);

            if (prescription == null)
                return NotFound();

            return View(prescription);
        }

    }
}
