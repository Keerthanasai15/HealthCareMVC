namespace HealthCareMVC.Models
{
    public class PatientResponses
    {
        public string result { get; set; }
        public PatientViewModel value { get; set; }
    }

    public class DoctorResponses
    {
        public string result { get; set; }
        public DoctorViewModel value { get; set; }
    }

    public class AdminResponses
    {
        public string result { get; set; }
        public AdminViewModel value { get; set; }
    }

}
