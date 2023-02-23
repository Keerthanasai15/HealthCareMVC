using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareMVC.Models
{
    public class ApplicationViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.DateTime)]

        public DateTime ApplicationDate { get; set; }

        [Required]


        public string ApplicationId { get; set; }
        [Required]
        [DefaultValue(false)]

        public bool IsAccepted { get; set; }

        public List<AppointmentStatusViewModel> AppointmentStatus { get; set; }
        public int AppointmentStatusId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }

    }
}
