using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Linq;

namespace Jobbvin.Client.Services
{
    public interface IUserServiceClient
    {
        Task<List<pic_categories_ViewModel>> GetMenus();
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
    }

}