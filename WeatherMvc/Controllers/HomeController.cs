using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherMvc.Models;
using WeatherMvc.Services;

namespace WeatherMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ITokenService tokenService, ILogger<HomeController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Weather()
        {
            var data = new List<WeatherData>();

            using var client = new HttpClient();

            var tokenResponse = await _tokenService.GetToken("weatherapi.read");

            _logger.LogInformation(tokenResponse.AccessToken);

            client.SetBearerToken(tokenResponse.AccessToken);

            _logger.LogInformation(client.DefaultRequestHeaders.Connection.ToString());

            var result = await client.GetAsync("https://localhost:5445/WeatherForecast");

            if (result.IsSuccessStatusCode)
            {
                var model = await result.Content.ReadAsStringAsync();

                data = JsonConvert.DeserializeObject<List<WeatherData>>(model);

                return View(data);
            }
            else
            {
                throw new Exception("Unable to get content");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
