namespace Dental_Clinic.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }

        public int TodayAppointments { get; set; }
        public int PendingInvoices { get; set; }

        public List<RecentInvoiceVM> RecentInvoices { get; set; } = new();
        public List<TodayAppointmentVM> TodayAppointmentsList { get; set; } = new();
    }

    public class RecentInvoiceVM
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public string PatientName { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public DateTime Date { get; set; }
    }
    public class TodayAppointmentVM
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string Phone { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; }
        public string Branch { get; set; }
    }

}
