using BlazorBootstrap;
using global::Microsoft.AspNetCore.Components;
using Jobbvin.Client.Services;
using Jobbvin.Shared.Models;
using System.Diagnostics.CodeAnalysis;

namespace Jobbvin.Client.Pages
{
    public partial class Index
    {
        private string error;

        [Inject] protected PreloadService PreloadService { get; set; }

        [Inject]
        [AllowNull]
        IUserServiceClient UserServiceClient { get; set; }

        public List<pic_categories_ViewModel> pic_categories_ViewModels { get; set; }
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
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
        }
    }
}