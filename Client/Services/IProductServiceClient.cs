using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json;
using Blazored.LocalStorage;
using System.Net.Http;

namespace Jobbvin.Client.Services
{
    public interface IProductServiceClient
    {
        Task<List<ProductListViewModel>> GetProductListBySubCategoryFilter(PoductFilterModel productFilterModel);

        Task<ProductDetailsViewModel> GetProductDetails(int adId, int userId);
        Task<GetPostAdFieldsViewModel> GetPostFields(int categoryId, int userId);
        
        Task<ApiResponse> LikeAd(pic_likes pic_Likes);
        Task<ApiResponse> PostAd(PostAdViewModel postAdViewModel);
        Task<ApiResponse> StarRating(StarRatingModel rating);
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

        public async Task<GetPostAdFieldsViewModel> GetPostFields(int categoryId, int userId)
        {
            var viewModel = new GetPostAdFieldsViewModel();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                viewModel = await HttpClient.GetFromJsonAsync<GetPostAdFieldsViewModel>($"api/Products/GetPostFields?category_id=" + categoryId + "&userId=" + userId);

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on GetPostFields : " + ex.Message);
                return new GetPostAdFieldsViewModel();
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

        public async Task<ApiResponse> StarRating(StarRatingModel rating)
        {
            var viewModel = new ApiResponse();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await HttpClient.PostAsJsonAsync<StarRatingModel>($"api/Products/StarRating/", rating);
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
                        Console.WriteLine("Error on content support StarRating : " + ex.Message);

                    }
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on StarRating : " + ex.Message);
                return new ApiResponse();
            }
        }

        public async Task<ApiResponse> PostAd(PostAdViewModel postAdViewModel)
        {
            var viewModel = new ApiResponse();
            try
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await HttpClient.PostAsJsonAsync<PostAdViewModel>($"api/Products/PostAd/", postAdViewModel);
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
                        Console.WriteLine("Error on content support StarRating : " + ex.Message);

                    }
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on StarRating : " + ex.Message);
                return new ApiResponse();
            }
        }
    }
}