using HealthCareMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCareMVC.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IConfiguration _configuration;

        public AppointmentsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            List<AppointmentViewModel> appointments = new();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync("Appointments/GetAllAppointment");
                if (result.IsSuccessStatusCode)
                {
                    appointments = await result.Content.ReadAsAsync<List<AppointmentViewModel>>();
                }
            }
            return View(appointments);
            
        }

        public async Task<IActionResult> Details(int id)
        {
            AppointmentViewModel appointment = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Appointments/GetAppointmentById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    appointment = await result.Content.ReadAsAsync<AppointmentViewModel>();
                }
            }
            return View(appointment);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentViewModel appointment)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Appointments/CreateAppointment", appointment);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                AppointmentViewModel appointment = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.GetAsync($"Appointments/GetAppointmentById/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        appointment = await result.Content.ReadAsAsync<AppointmentViewModel>();
                        return View(appointment);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Appointment doesn't exists");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentViewModel appointment)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PutAsJsonAsync($"Appointments/UpdateAppointment/{appointment.Id}", appointment);
                    return RedirectToAction("Index");

                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            AppointmentViewModel appointment = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Appointments/GetAppointmentById/{id}");
                if (result.IsSuccessStatusCode)
                {
                    appointment = await result.Content.ReadAsAsync<AppointmentViewModel>();
                }
            }
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AppointmentViewModel appointment)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.DeleteAsync($"Appointments/DeleteAppointment/{appointment.Id}");
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                return View();
            }
        }

    }
}
