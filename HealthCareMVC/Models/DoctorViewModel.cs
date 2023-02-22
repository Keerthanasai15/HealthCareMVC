using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HealthCareMVC.Models
{
    public class DoctorViewModel:DoctorLoginModel
    {
        [Key]
        public int DoctorId { get; set; }
        [Required]
        public string UserName { get; set; }

        public string? DoctorName { get; set; }

        [Range(1000000000, 9999999999,
           ErrorMessage = "Mobile no should be 10 digits")]
        public string DoctorPhNo { get; set; }
        [Required]
        [EmailAddress]
        public string DoctorEmail { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [DefaultValue("Empl@123")]
        public string? Password { get; set; }
        // [Required]

    }

    public class DoctorLoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DefaultValue("Empl@123")]
        public string? Password { get; set; }
        [DefaultValue(-1)]
        public int IsApproved { get; set; }
    }
}
