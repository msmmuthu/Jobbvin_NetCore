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
using Jobbvin.Shared.Models;
using Jobbvin.Client.Services;
using BlazorBootstrap;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebUtilities;
using System.Runtime.CompilerServices;

namespace Jobbvin.Client.Components
{
    public partial class PostCanvas
    {
        private string error;

        [Inject] protected PreloadService PreloadService { get; set; }
        

        [CascadingParameter(Name = "returnUrl")]
        public string returnUrl { get; set; }

        [CascadingParameter(Name = "OnShowPostFields")]
        public EventCallback<int> OnShowPostFields { get; set; }

        [Inject]
        [AllowNull]
        IUserServiceClient UserServiceClient { get; set; }
        public List<pic_categories_ViewModel> pic_categories_ViewModels { get; set; }
        private Offcanvas offcanvasCategory;
        private Offcanvas offcanvasSubCategory;

        public List<pic_categories> SubCategories { get; set; }

        private bool showPostFields = false;
        protected override async Task OnInitializedAsync()
        {
            PreloadService.Show();
            await GetMenus();
            PreloadService.Hide();
        }
        private async Task GetMenus()
        {
            try
            {
                pic_categories_ViewModels = await UserServiceClient.GetMenus();
                await offcanvasCategory?.ShowAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
        }

        private async Task CloseOffCanvas()
        {
            await offcanvasCategory?.HideAsync();
            navigationManager.NavigateTo(returnUrl);
        }

        private async Task CloseSubCategoryOffCanvas()
        {
            await offcanvasSubCategory?.HideAsync();
            navigationManager.NavigateTo(returnUrl);
        }

        private async Task OnSelectSubCategory(int subCategoryId)
        {
            showPostFields = true;
            offcanvasSubCategory?.HideAsync();
            navigationManager.NavigateTo("CreatePost?cat_id="+ subCategoryId);
        }

        private async Task SelectSubCategory(List<pic_categories> subCategoies)
        {
            PreloadService.Show();
            SubCategories = subCategoies;
            await offcanvasCategory?.HideAsync();
            await offcanvasSubCategory?.ShowAsync();
            PreloadService.Hide();
        }
    }
}