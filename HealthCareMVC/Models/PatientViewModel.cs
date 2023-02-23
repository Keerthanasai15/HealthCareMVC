using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HealthCareMVC.Models
{
    public class PatientViewModel:PatientLoginModel
    {

        [Required]
        public string UserName { get; set; }

        public string? PatientName { get; set; }

        public string? PatientAddress { get; set; }
        [Range(1000000000, 9999999999,
            ErrorMessage = "Mobile no should be 10 digits")]
        public string PatientPhNo { get; set; }
        [Required]
        [EmailAddress]
        public string PatientEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DefaultValue("Pati@123")]
        public string PatientPassword { get; set; }
    }

    public class PatientLoginModel
    {
        [Key]
        public int PatientId { get; set; }
        [Required]
        public string UserName { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [DefaultValue("Pati@123")]
        public string PatientPassword { get; set; }

    }
}
