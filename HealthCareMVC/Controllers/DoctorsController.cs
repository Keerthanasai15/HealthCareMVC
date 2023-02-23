using HealthCareMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCareMVC.Controllers
{
    public class DoctorsController : Controller
    {
        public readonly IConfiguration _configuration;

        public DoctorsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]

        public async Task<IActionResult> Dashboard(int id)
        {
            List<ApplicationViewModel> applications = new List<ApplicationViewModel>();


            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var resultOrders = await client.GetAsync("ApplicationDetails/GetAllApplicationDetails");
                if (resultOrders.IsSuccessStatusCode)
                {
                    applications = await resultOrders.Content.ReadAsAsync<List<ApplicationViewModel>>();
                    var count = applications.Where(c => c.AppointmentStatusId == 1).Count();


                    return View(applications);
                }



            }

            return View();
        }
        public async Task<IActionResult> Index()
        {
            List<DoctorViewModel> doctors = new List<DoctorViewModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Employees/GetAllEmployees");
                if (result.IsSuccessStatusCode)
                {
                    doctors = await result.Content.ReadAsAsync<List<DoctorViewModel>>();
                }
            }
            return View(doctors);
           
        }

        public async Task<ActionResult> Details(int id)
        {
            DoctorViewModel doctor = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Doctors/GetAllDoctors");
                if (result.IsSuccessStatusCode)
                {
                    var Emplist = await result.Content.ReadAsAsync<List<DoctorViewModel>>();
                    doctor = Emplist.Where(c => c.DoctorId == id).FirstOrDefault();
                    if (doctor != null)
                    {
                        return View(doctor);
                    }
                }

            }
            return null;
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync($"Doctors/Create", doctor);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return RedirectToAction("Login", "Doctors");

                    }

                }

            }
            return View(doctor);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            DoctorResponses doctor = new DoctorResponses();
            if (ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"Doctors/GetDoctorById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        doctor = await result.Content.ReadAsAsync<DoctorResponses>();
                        return View(doctor.value);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Employee does not exist");
                    }
                }
            }
            return View(doctor.value);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DoctorViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Doctors/UpdateDoctor/{doctor.DoctorId}", doctor);
                    if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ModelState.AddModelError("", "Server Error, Please try later");
                    }
                }
            }
            return View(doctor);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                DoctorViewModel doctor = new();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync("Doctors/GetAllDoctors");
                    if (result.IsSuccessStatusCode)
                    {
                        var EmpList = await result.Content.ReadAsAsync<List<DoctorViewModel>>();
                        doctor = EmpList.Where(c => c.DoctorId == id).FirstOrDefault();
                        if (doctor != null)
                        {
                            return View(doctor);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Employee doesn't exist");
                        }

                    }

                }

            }
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DoctorViewModel doctor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"Doctors/DeleteDoctor/{doctor.DoctorId}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");

                }
            }
            return View(doctor);

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(DoctorLoginModel login)
        {


            if (ModelState.IsValid)
            {
                int id = 0;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Doctors/Login", login);
                    if (result.IsSuccessStatusCode)
                    {
                        string token = await result.Content.ReadAsAsync<string>();
                        if (token != null)
                        {

                            if (token[token.Length - 1] == '1')
                            {
                                token = token.Remove(token.Length - 1, 1);
                                HttpContext.Session.SetString("token", token);

                                return RedirectToAction("Dashboard");
                            }
                            else if (token[token.Length - 1] == '0')
                            {
                                ModelState.AddModelError("", "Admin yet to verify you account");
                                return View(login);
                            }


                        }


                        //return RedirectToAction("Details", "Employees");
                    }
                    else
                    {
                        return View(login);
                    }
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
            }
            return View(login);
        }

        [HttpGet]
        [Route("Patient/SearchPatiByName/{Name?}")]
        public async Task<IActionResult> SearchPatiByName(string Name)
        {


            List<PatientViewModel> patients = new List<PatientViewModel>();
            using (var client = new HttpClient())

            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Patients/GetAllPatients");
                if (result.IsSuccessStatusCode)
                {
                    patients = await result.Content.ReadAsAsync<List<PatientViewModel>>();
                    if (string.IsNullOrEmpty(Name))
                    {
                        return View(patients);
                    }
                    List<PatientViewModel> customer = patients.Where(c => c.PatientName.Contains(Name)).ToList();

                    return View(customer);

                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(int id)
        {
            PatientResponses patient = new PatientResponses();
            if (ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"Patients/GetPatientById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        patient = await result.Content.ReadAsAsync<PatientResponses>();
                        return View(patient.value);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Patient does not exist");
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditPatient(PatientViewModel patient)
        {

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Patients/UpdatePatient/{patient.PatientId}", patient);
                    if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return RedirectToAction("SearchPatiByName");

                    }
                    else
                    {
                        ModelState.AddModelError("", "Server Error, Please try later");
                    }
                }
            }
            return View(patient);
        }





    }
}
