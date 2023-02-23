using System.ComponentModel.DataAnnotations;

namespace HealthCareMVC.Models
{
    public class AppointmentStatusViewModel
    {
        [Key]
        public int StatusId { get; set; }
        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }
    }
}
