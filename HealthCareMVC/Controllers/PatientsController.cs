using HealthCareMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HealthCareMVC.Controllers
{
    public class PatientsController : Controller
    {
        public readonly IConfiguration _configuration;
        public PatientsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<IActionResult> Index()
        {
            List<PatientViewModel> patients = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Patients/GetAllPatients");
                if (result.IsSuccessStatusCode)
                {
                    patients = await result.Content.ReadAsAsync<List<PatientViewModel>>();
                }

            }
            return View(patients);
        }

        public async Task<IActionResult> Details(int id)
        {
            PatientViewModel patient = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Patients/GetAllPatients");
                if (result.IsSuccessStatusCode)
                {
                    var patientlist = await result.Content.ReadAsAsync<List<PatientViewModel>>();
                    patient = patientlist.Where(c => c.PatientId == id).FirstOrDefault();
                    if (patient != null)
                    {
                        return View(patient);
                    }
                }

            }

            return null;

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {


            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientViewModel patient)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync($"Patients/Create", patient);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return RedirectToAction("Login", "Patients");

                    }



                }

            }
            return View(patient);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            PatientResponses customer = new PatientResponses();
            if (ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"Patients/GetPatientById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        customer = await result.Content.ReadAsAsync<PatientResponses>();
                        return View(customer.value);
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
        public async Task<IActionResult> Edit(PatientViewModel patient)
        {

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Patients/UpdatePatient/{patient.PatientId}", patient);
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
            return View(patient);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                PatientViewModel patient = new();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync("Patients/GetAllPatients");
                    if (result.IsSuccessStatusCode)
                    {
                        var customerList = await result.Content.ReadAsAsync<List<PatientViewModel>>();
                        patient = customerList.Where(c => c.PatientId == id).FirstOrDefault();
                        if (patient != null)
                        {
                            return View(patient);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Patient doesn't exist");
                        }
                    }
                }

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(PatientViewModel patient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"Patients/DeletePatient/{patient.PatientId}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");

                }
            }
            return View(patient);


        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(PatientLoginModel login)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Customers/Login", login);
                    if (result.IsSuccessStatusCode)
                    {
                        string token = await result.Content.ReadAsAsync<string>();
                        HttpContext.Session.SetString("token", token);


                        return RedirectToAction("Dashboard", "Patients", new { id = login.PatientId });
                    }
                    ModelState.AddModelError("", "Invalid Username or Password");
                }

            }
            return View(login);
        }
        [HttpPost]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("token");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Customer/SearchByName/{Name?}")]
        public async Task<IActionResult> SearchByName(string Name)
        {


            List<PatientViewModel> customers = new List<PatientViewModel>();
            using (var client = new HttpClient())

            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Customers/GetAllCustomers");
                if (result.IsSuccessStatusCode)
                {
                    customers = await result.Content.ReadAsAsync<List<PatientViewModel>>();
                    if (string.IsNullOrEmpty(Name))
                    {
                        return View(customers);
                    }
                    List<PatientViewModel> customer = customers.Where(c => c.PatientName.Contains(Name)).ToList();

                    return View(customer);

                }
            }
            return View();
        }
    }
}


    
