using Blazored.LocalStorage;
using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.Design;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Jobbvin.Client.Services
{
    public interface IAuthenticationService
    {
        pic_user User { get; set; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private HttpClient _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        private pic_user user;
        public pic_user User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
            }
        }

        public AuthenticationService(
            HttpClient httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItemAsync<pic_user>("user");
        }

        public async Task Login(string username, string password)
        {

            _httpService.DefaultRequestHeaders.Accept.Add(
                   new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            User = await _httpService.GetFromJsonAsync<pic_user>($"api/Jobbvin/{username}/{password}");
            await _localStorageService.SetItemAsync("user", User);

        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItemAsync("user");
            _navigationManager.NavigateTo("login");
        }
    }
}
