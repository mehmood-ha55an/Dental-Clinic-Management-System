using Dental_Clinic.Data;
using Dental_Clinic.Filters;
using Dental_Clinic.Models;
using Dental_Clinic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Dental_Clinic.Controllers
{
    [AuthorizeRole("Admin", "Receptionist")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

            var model = new DashboardViewModel
            {
                TodayRevenue = _context.Invoices
                    .Where(x => x.InvoiceDate.Date == today)
                    .Sum(x => (decimal?)x.PaidAmount) ?? 0,

                MonthRevenue = _context.Invoices
                    .Where(x => x.InvoiceDate.Month == today.Month && x.InvoiceDate.Year == today.Year)
                    .Sum(x => (decimal?)x.PaidAmount) ?? 0,

                YearRevenue = _context.Invoices
                    .Where(x => x.InvoiceDate.Year == today.Year)
                    .Sum(x => (decimal?)x.PaidAmount) ?? 0,

                TodayAppointments = _context.Appointments
                    .Count(x => x.AppointmentDate.Date == today),

                PendingInvoices = _context.Invoices
                    .Count(x => x.RemainingAmount > 0),

                RecentInvoices = _context.Invoices
                    .OrderByDescending(x => x.InvoiceDate)
                    .Take(5)
                    .Select(x => new RecentInvoiceVM
                    {
                        InvoiceId = x.Id,
                        InvoiceNo = x.InvoiceNumber,
                        PatientName = x.PatientName,
                        Total = x.TotalAmount,
                        Paid = x.PaidAmount,
                        Date = x.InvoiceDate
                    })
                    .ToList(),

                TodayAppointmentsList = _context.Appointments
            .Where(x => x.AppointmentDate.Date == today)
            .OrderBy(x => x.AppointmentDate)
            .Select(x => new TodayAppointmentVM
            {
                Id = x.Id,
                PatientName = x.PatientName,
                Phone = x.PhoneNumber,
                AppointmentTime = x.AppointmentTime,
                Status = x.Status,
                Branch = x.Branch
            }).ToList()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
