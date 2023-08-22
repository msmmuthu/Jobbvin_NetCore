using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json;

namespace Jobbvin.Client.Services
{
    public interface IProductServiceClient
    {
        Task<List<ProductListViewModel>> GetProductListBySubCategoryFilter(PoductFilterModel productFilterModel);

        Task<ProductDetailsViewModel> GetProductDetails(int adId, int userId);
        Task<ApiResponse> LikeAd(pic_likes pic_Likes);
    }

    public class ProductServiceClient : IProductServiceClient
    {
        [Inject]

        private HttpClient HttpClient { get; set; }

        public ProductServiceClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<List<ProductListViewModel>> GetProductListBySubCategoryFilter(PoductFilterModel productFilterModel)
        {
            var viewModel = new List<ProductListViewModel>();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await HttpClient.PostAsJsonAsync<PoductFilterModel>($"api/Products/GetProductListByFilter/", productFilterModel);
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        viewModel = await resp.Content.ReadFromJsonAsync<List<ProductListViewModel>>();
                        var options = new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error on content support GetProductListBySubCategoryFilter : " + ex.Message);

                    }
                }

                return viewModel.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on GetProductListBySubCategoryFilter : " + ex.Message);
                return new List<ProductListViewModel>();
            }
        }

        public async Task<ProductDetailsViewModel> GetProductDetails(int adId, int userId)
        {
            var viewModel = new ProductDetailsViewModel();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                viewModel = await HttpClient.GetFromJsonAsync<ProductDetailsViewModel>($"api/Products/GetProductDetails?adId=" + adId + "&customerId="+userId);
              
                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on GetProductListBySubCategoryFilter : " + ex.Message);
                return new ProductDetailsViewModel();
            }
        }

        public async Task<ApiResponse> LikeAd(pic_likes pic_Likes)
        {
            var viewModel = new ApiResponse();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await HttpClient.PostAsJsonAsync<pic_likes>($"api/Products/ProductLike/", pic_Likes);
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
                        Console.WriteLine("Error on content support LikeAd : " + ex.Message);

                    }
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on GetProductListBySubCategoryFilter : " + ex.Message);
                return new ApiResponse();
            }
        }
    }
}