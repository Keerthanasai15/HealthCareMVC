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
    public class AdminController : Controller
    {
        public readonly IConfiguration _configuration;
        public AdminController(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        public async Task<IActionResult>Dashboard()
        {
            List<ApplicationViewModel> orders = new List<ApplicationViewModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var resultOrders = await client.GetAsync("/GetAllCargoOrderDetails");
                if (resultOrders.IsSuccessStatusCode)
                {
                    orders = await resultOrders.Content.ReadAsAsync<List<ApplicationViewModel>>();
                    var count = orders.Where(c => c.AppointmentStatusId == 1).Count();
                    ViewBag.OrderCount = orders.Count;
                    ViewBag.YetToDeliverOrder = count;
                }

                var resultEmp = await client.GetAsync("Doctors/GetAllDoctors");
                if (resultEmp.IsSuccessStatusCode)
                {
                    var employees = await resultEmp.Content.ReadAsAsync<List<DoctorViewModel>>();
                    var count = employees.Where(e => e.IsApproved == 0).Count();
                    ViewBag.TotalEmployee = employees.Count;
                    ViewBag.PendingApproval = count;
                }

            }

            return View(orders);
        }

        public async Task<IActionResult> Index()
        {

            List<AdminViewModel> admins = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Admins/GetAllAdmin");
                if (result.IsSuccessStatusCode)
                {
                    admins = await result.Content.ReadAsAsync<List<AdminViewModel>>();
                }

            }
            return View(admins);
            
        }
        public async Task<IActionResult> Details(int id)
        {
            AdminViewModel admin = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Admins/GetAllAdmin");
                if (result.IsSuccessStatusCode)
                {
                    var Adminlist = await result.Content.ReadAsAsync<List<AdminViewModel>>();
                    admin = Adminlist.Where(c => c.Id == id).FirstOrDefault();
                    if (admin != null)
                    {
                        return View(admin);
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
        public async Task<IActionResult> Create(AdminViewModel admin)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync($"Admins/CreateAdmin", admin);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return RedirectToAction("Index");

                    }

                }

            }
            return View(admin);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            AdminResponses admin = new AdminResponses();
            if (ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"Admins/GetAdminById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        admin = await result.Content.ReadAsAsync<AdminResponses>();
                        return View(admin.value);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Admin does not exist");
                    }
                }
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(AdminViewModel admin)
        {

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Admins/UpdateAdmin/{admin.Id}", admin);
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
            return View(admin);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                AdminViewModel admin = new();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync("Admins/GetAllAdmin");
                    if (result.IsSuccessStatusCode)
                    {
                        var adminList = await result.Content.ReadAsAsync<List<AdminViewModel>>();
                        admin = adminList.Where(c => c.Id == id).FirstOrDefault();
                        if (admin != null)
                        {
                            return View(admin);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Admin doesn't exist");
                        }
                    }
                }

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AdminViewModel admin)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"Admins/DeleteAdmin/{admin.Id}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");

                }
            }
            return View(admin);


        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginModel login)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Admins/Login", login);
                    if (result.IsSuccessStatusCode)
                    {
                        string token = await result.Content.ReadAsAsync<string>();
                        HttpContext.Session.SetString("token", token);
                        return RedirectToAction("Dashboard", "Admin");
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

        //[HttpGet]
        //public async Task<IActionResult> PendingOrders()
        //{
        //    List<ApplicationViewModel> orders = new List<ApplicationViewModel>();

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
        //        var resultOrders = await client.GetAsync("CargoOrderDetails/GetAllCargoOrderDetails");
        //        if (resultOrders.IsSuccessStatusCode)
        //        {
        //            orders = await resultOrders.Content.ReadAsAsync<List<ApplicationViewModel>>();
        //            var count = orders.Where(c => c.CargoTypeId == 1).Count();
        //            ViewBag.OrderCount = orders.Count;
        //            ViewBag.YetToDeliverOrder = count;
        //        }



        //    }

        //    return View(orders);
        //}





        [HttpGet]
        [Route("Admin/SearchPatiByName/{Name?}")]
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
                    List<PatientViewModel> patient = patients.Where(c => c.PatientName.Contains(Name)).ToList();

                    return View(patient);

                }
            }
            return View();
        }

        [HttpGet]
        [Route("Admin/SearchDocByName/{Name?}")]
        public async Task<IActionResult> SearchDocByName(string Name)
        {


            List<DoctorViewModel> doc = new List<DoctorViewModel>();
            using (var client = new HttpClient())

            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Doctors/GetAllDoctors");
                if (result.IsSuccessStatusCode)
                {
                   doc = await result.Content.ReadAsAsync<List<DoctorViewModel>>();
                    if (string.IsNullOrEmpty(Name))
                    {
                        return View(doc);
                    }
                    List<DoctorViewModel> docs = doc.Where(c => c.UserName.Contains(Name)).ToList();

                    return View(docs);

                }
            }
            return View();
        }
        public async Task<ActionResult> DocDetails(int id)
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





        [HttpGet]
        public async Task<IActionResult> DocEdit(int id)
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
                        ModelState.AddModelError("", "doctor does not exist");
                    }
                }
            }
            return View(doctor.value);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DocEdit(DoctorViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Doctors/UpdateDoctor/{doctor.DoctorId}", doctor);
                    if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return RedirectToAction("Dashboard");

                    }
                    else
                    {
                        ModelState.AddModelError("", "Server Error, Please try later");
                    }
                }
            }
            return View(doctor);
        }









    }
}
