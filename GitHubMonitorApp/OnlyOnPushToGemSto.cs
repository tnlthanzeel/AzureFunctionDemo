using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Bread2Bun.Service.Country.Interface;

namespace GitHubMonitorApp
{
    public class OnlyOnPushToGemSto
    {
        private readonly ICountryService countryService;

        public OnlyOnPushToGemSto(ICountryService countryService)
        {
            this.countryService = countryService;
        }
        [FunctionName("OnlyOnPushToGemSto")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var result = await countryService.GetCountries();

                //to return a response body with https status code 200
                return new OkObjectResult(new { result="Success from gemsto"});


                // to return http status code 200 without response body
                //return new OkResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
