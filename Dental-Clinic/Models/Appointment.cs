using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string PatientName { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }
        [Required]
        public string Branch { get; set; }   // Islamabad, Peshawar, Karachi

        public string Status { get; set; } = "Pending"; // default

        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Prescription? Prescription { get; set; }

        public int? RoboCallId { get; set; }
        public string? CallStatus { get; set; }
        public DateTime? CallTriggeredAt { get; set; }


    }
}
