using HealthCareMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCareMVC.Controllers
{
    public class ApplicationsController : Controller
    {
        public readonly IConfiguration _configuration;


        public ApplicationsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            List<ApplicationViewModel> orders = new List<ApplicationViewModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("ApplicationDetails/GetAllApplicationDetails");
                if (result.IsSuccessStatusCode)
                {
                    orders = await result.Content.ReadAsAsync<List<ApplicationViewModel>>();
                }

            }
            return View(orders);
            
        }

        public async Task<ActionResult> Create()
        {
            ApplicationViewModel appointment = new ApplicationViewModel
            {
                AppointmentStatus = await this.GetStatus(),
               
            };
            return View(appointment);
        }

        
        [NonAction]

        public async Task<List<AppointmentStatusViewModel>> GetStatus()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);

                var result = await client.GetAsync("AppointmentStatus/GetAllStatus");
                if (result.IsSuccessStatusCode)
                {
                    var statusList = await result.Content.ReadAsAsync<List<AppointmentStatusViewModel>>();
                    return statusList;

                }

            }
            return null;
        }

        [HttpPost]

        public async Task<IActionResult> Create(ApplicationViewModel application)
        {
            if (ModelState.IsValid)
            {
                application.AppointmentStatusId = 1;
                application.PatientId = 1;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync($"ApplicationDetails/Create", application);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return RedirectToAction("Index");

                    }



                }

            }
            return View(application);
        }

        public async Task<IActionResult> Details(int id)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("ApplicationDetails/GetAllApplicationDetails");
                if (result.IsSuccessStatusCode)
                {
                    var applicationList = await result.Content.ReadAsAsync<List<ApplicationViewModel>>();
                    ApplicationViewModel application = applicationList.Where(c => c.Id == id).FirstOrDefault();
                    if (application != null)
                    {
                        return View(application);
                    }

                }
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ApplicationViewModel application = new ApplicationViewModel();
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync("ApplicationDetails/GetAllApplicationDetails");
                    if (result.IsSuccessStatusCode)
                    {
                        var orderList = await result.Content.ReadAsAsync<List<ApplicationViewModel>>();
                        application = orderList.Where(c => c.Id == id).FirstOrDefault();
                        if (application != null)
                        {


                            ViewBag.CargoStatus = await this.GetStatus();
                            


                            return View(application);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Application does not exist");
                        }
                    }
                }
                return View(application);
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ApplicationViewModel application)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"ApplicationDetails/UpdateApplicationDetail/{application.Id}", application);
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
            return View(application);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                ApplicationViewModel application = new();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync("ApplicationDetails/GetAllApplicationDetails");
                    if (result.IsSuccessStatusCode)
                    {
                        var orderList = await result.Content.ReadAsAsync<List<ApplicationViewModel>>();
                        application = orderList.Where(c => c.Id == id).FirstOrDefault();
                        if (application != null)
                        {
                            return View(application);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Application doesn't exist");
                        }

                    }
                }
            }
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(ApplicationViewModel application)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"/api/ApplicationDetails/DeleteApplicationDetail/{application.Id}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Server Error.Please try later");
                }
            }
            return View(application);
        }







    }
}
