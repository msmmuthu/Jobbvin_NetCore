using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Jobbvin.Client;
using Jobbvin.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using BlazorBootstrap;
using Jobbvin.Client.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using Jobbvin.Shared.Models;
using Jobbvin.Client.Components;

namespace Jobbvin.Client.Pages
{
    public partial class JobListing
    {
        private string error;

        [Inject] protected PreloadService Loader { get; set; }


        [Inject]
        [AllowNull]
        IProductServiceClient ProductServiceClient { get; set; }

        private bool loading;
        private string loadingText = "Loading...";
        int pageNumber = 1;
        int catId = 0;
        public List<ProductListViewModel> ProductListViewModels { get; set; } = new List<ProductListViewModel>();
        protected override async Task OnInitializedAsync()
        {
            Loader.Show(SpinnerColor.Primary);
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("categoryId", out var param1))
            {
                PoductFilterModel productFilterModel = new PoductFilterModel();
                catId = Convert.ToInt32(param1.First());
                productFilterModel.cat_id = catId;
                productFilterModel.offset = pageNumber;
                await GetProductListBySubCategoryFilter(productFilterModel);
                pageNumber++;
            }
            if (ProductListViewModels.Count < 1)
                loadingText = "No records found";
            Loader.Hide();
        }

        private async Task GetProductListBySubCategoryFilter(PoductFilterModel productFilterModel)
        {
            try
            {
                var productListViewModels = await ProductServiceClient.GetProductListBySubCategoryFilter(productFilterModel);
                if (productListViewModels != null && productListViewModels.Count > 0)
                    ProductListViewModels.AddRange(productListViewModels);
                else
                {

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
        }

        private async Task BrowseMore()
        {
            loading = true;
            PoductFilterModel productFilterModel = new PoductFilterModel();
            productFilterModel.cat_id = catId;
            productFilterModel.offset = pageNumber;
            await GetProductListBySubCategoryFilter(productFilterModel);
            pageNumber++;
            loading = false;

        }
    }
}