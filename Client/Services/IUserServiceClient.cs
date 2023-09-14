using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json;

namespace Jobbvin.Client.Services
{
    public interface IUserServiceClient
    {
        Task<List<pic_categories_ViewModel>> GetMenus();
        Task<ApiResponse> InsertTempUser(temp_user temp_User);
        Task<ApiResponse> ValidateOtp(temp_user temp_User);
    }

    public class UserServiceClient : IUserServiceClient
    {
        [Inject]

        private HttpClient HttpClient { get; set; }

        public UserServiceClient(HttpClient httpClient)
        {
            HttpClient = httpClient;

        }

        public async Task<List<pic_categories_ViewModel>> GetMenus()
        {
            var viewModel = new List<pic_categories_ViewModel>();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await HttpClient.GetFromJsonAsync<List<pic_categories>>($"api/Users/GetMenuItems");
                var child = resp.Where(c => c.categories_parent == 0 && c.categories_sub > 0).ToList();
                var parents = resp.Where(c => c.categories_parent == 1).ToList();

                foreach (var (p, vm) in from p in parents
                                        let vm = new pic_categories_ViewModel()
                                        select (p, vm))
                {
                    vm.Maincategory = p;
                    vm.SubCategories = (from c in child
                                        where c.categories_sub == p.categories_id
                                        select c).OrderBy(v => v.category_order).ToList();
                    viewModel.Add(vm);
                }

                return viewModel.OrderBy(v=>v.Maincategory.category_order).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on Login : " + ex.Message);
                return new List<pic_categories_ViewModel>();
            }
        }

        public async Task<ApiResponse> InsertTempUser(temp_user temp_User)
        {
            var viewModel = new ApiResponse();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await HttpClient.PostAsJsonAsync<temp_user>($"api/Users/TempUser/", temp_User);
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        viewModel = await resp.Content.ReadFromJsonAsync<ApiResponse>();
                        var options = new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error on content support InsertTempUser : " + ex.Message);

                    }
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on InsertTempUser : " + ex.Message);
                return new ApiResponse { Status = false, Message =  ex.Message};
            }
        }

        public async Task<ApiResponse> ValidateOtp(temp_user temp_User)
        {
            var viewModel = new ApiResponse();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await HttpClient.PostAsJsonAsync<temp_user>($"api/Users/ValidateOtp/", temp_User);
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        viewModel = await resp.Content.ReadFromJsonAsync<ApiResponse>();
                        var options = new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error on content support ValidateOtp : " + ex.Message);

                    }
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on validate otp : " + ex.Message);
                return new ApiResponse();
            }
        }
    }

}