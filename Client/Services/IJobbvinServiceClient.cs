using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Jobbvin.Client.Services
{
    public interface IJobbvinServiceClient
    {
        Task<ApiResponse> Login(string username, string password);
    }

    public class JobbvinServiceClient : IJobbvinServiceClient
    {
        [Inject]

        private HttpClient HttpClient { get; set; }

        public JobbvinServiceClient(HttpClient httpClient)
        {
            HttpClient = httpClient;

        }

        public async Task<ApiResponse> Login(string username, string password)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var respse = await HttpClient.GetFromJsonAsync<ApiResponse>($"api/Jobbvin/{username}/{password}");
                return response;
                    }
            catch (Exception ex)
            {
                Console.WriteLine("Error on Login : " + ex.Message);
            }
            return null;
        }
    }

}