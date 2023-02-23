using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HealthCareMVC.Models
{
    public class SpecializationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Plese provide the specialization")]
        [Display(Name = "Specialization")]
        public string SpecializationName { get; set; }
    }
}
