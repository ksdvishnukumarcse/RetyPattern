using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RetyPattern.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RetyPattern.Api.Controllers
{
    /// <summary>
    /// SampleController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SampleController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="logger">The logger.</param>
        public SampleController(IHttpClientFactory httpClientFactory, ILogger<SampleController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet("todo", Name = "Get To Do's")]
        public async Task<IActionResult> Get()
        {
            var result = await _httpClientFactory
                .CreateClient(AppConstants.JsonPlaceFolderClientName)
                .GetAsync($"/todos");
            if (result.IsSuccessStatusCode)
            {
                var apiResult = await result.Content.ReadAsStringAsync();
                var jsonObj = JsonConvert.DeserializeObject<List<TODO>>(apiResult);
                return Ok(jsonObj);
            }
            return NoContent();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet("todo/{id:int}", Name = "Get To Do")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _httpClientFactory
                .CreateClient(AppConstants.JsonPlaceFolderClientName)
                .GetAsync($"/todos/1");
            if (result.IsSuccessStatusCode)
            {
                var apiResult = await result.Content.ReadAsStringAsync();
                var jsonObj = JsonConvert.DeserializeObject<TODO>(apiResult);
                return Ok(jsonObj);
            }
            return NoContent();
        }
    }
}
