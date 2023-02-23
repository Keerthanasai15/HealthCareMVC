using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace HealthCareMVC.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Plese provide the Patient Name")]
        [Display(Name = "Patient Name")]

        public string PatientName { get; set; }

        [Required(ErrorMessage = "Plese provide the Gender (Male-Female-Others)")]

        public string Gender { get; set; }

        [Required(ErrorMessage = "Plese provide the Age")]

        public int Age { get; set; }

        [Required(ErrorMessage = "Plese provide the Appointment Date")]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Plese provide the Specialization Of Doctor")]
        [Display(Name = "Specialization Of Doctor")]
        public string SpecializationOfDoctor { get; set; }

        [Required(ErrorMessage = "Plese provide the Time Sessions")]
        [Display(Name = "Time Sessions")]
        public DateTime TimeSessions { get; set; }


        [Required(ErrorMessage = "Plese provide the Doctor Name")]
        [Display(Name = "Doctor Name")]

        public string DoctorName { get; set; }

    }
}
