using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string DoctorName { get; set; } = null!;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Appointment Appointment { get; set; } = null!;
        public List<PrescriptionItem> Items { get; set; } = new();

    }

    public class PrescriptionItem
    {
        public int Id { get; set; }

        public int PrescriptionId { get; set; }

        [Required]
        public string MedicineName { get; set; } = null!;

        public string? Dosage { get; set; }     // e.g. 1-0-1
        public string? Duration { get; set; }   // e.g. 5 days
        public string? Instructions { get; set; }

        public Prescription Prescription { get; set; } = null!;
    }

}
