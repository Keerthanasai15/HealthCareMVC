using HealthCareMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace HealthCareMVC.Controllers
{
    public class SpecializationController : Controller
    {
        private readonly IConfiguration _configuration;
        public SpecializationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SpecializationViewModel specialization)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Specialization/CreateSpecialization", specialization);
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return RedirectToAction("Create", "Doctors");
                    }
                }
            }
            return View(specialization);
        }
    }
}
    

