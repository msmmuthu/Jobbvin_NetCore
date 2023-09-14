using BlazorBootstrap;
using global::Microsoft.AspNetCore.Components;
using Jobbvin.Client.Services;
using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Jobbvin.Client.Pages
{
    public partial class CreatePost
    {
        private List<BreadcrumbItem> NavItems1 { get; set; }
        [Inject]
        [AllowNull]
        IProductServiceClient ProductServiceClient { get; set; }
        [Inject] protected PreloadService PreloadService { get; set; }
        private string error;

        PostAdViewModel model = new PostAdViewModel();
        private bool disabledCalender= true;
        private bool disabledHome = true;

        private Modal creationModalSuccess = default!;
        private string changeLocDisplay ="none";
        private string displayHomePageToggle = "none";
        private string catId = "";
        public GetPostAdFieldsViewModel _postAdFieldsViewModel { get; set; } = new GetPostAdFieldsViewModel();

        protected override async Task OnInitializedAsync()
        {
            var returnUrlUri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (QueryHelpers.ParseQuery(returnUrlUri.Query).TryGetValue("cat_id", out var param1))
            {
                catId = param1.First();
                await GetPostFields(Convert.ToInt32(catId), AuthenticationService.User.user_id);
            }

            NavItems1 = new List<BreadcrumbItem>
            {
                new BreadcrumbItem{ Text = "Home", Href ="/" },
                new BreadcrumbItem{ Text = "Create Post", IsCurrentPage = true }
            };

            var selectedUser = AuthenticationService.User;
            model.pic_Addpost.pic_user_email = selectedUser.user_email;
            model.pic_Addpost.pic_user_mobile = selectedUser.user_mobile;
            model.pic_Addpost.pic_user_id = selectedUser.user_id;
            model.pic_Addpost.pic_post_city = selectedUser.user_city;
            model.pic_Addpost.pic_refer_id = selectedUser.user_id.ToString();
            model.pic_Addpost.addpost_scheme_user_id = selectedUser.user_id;
            model.pic_Addpost.pic_user_fullname = selectedUser.user_username;
            model.pic_Addpost.pic_category = Convert.ToInt32(catId);
        }

        private async Task GetPostFields(int categoryId, int userId)
        {
            PreloadService.Show();
            try
            {
                _postAdFieldsViewModel = await ProductServiceClient.GetPostFields(categoryId, userId);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
            PreloadService.Hide();
        }

        private async void HandleValidSubmit()
        {
            PreloadService.Show();
            try
            {
                var result = await ProductServiceClient.PostAd(model);
                if(result.Status)
                {
                    await creationModalSuccess.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
            PreloadService.Hide();
        }

        void ChangeUser(ChangeEventArgs e)
        {
            if(AuthenticationService.User.user_id == Convert.ToInt32(e.Value))
            {
                var selectedUser = AuthenticationService.User;
                model.pic_Addpost.pic_user_email = selectedUser.user_email;
                model.pic_Addpost.pic_user_mobile = selectedUser.user_mobile;
                model.pic_Addpost.pic_user_id = selectedUser.user_id;
                model.pic_Addpost.pic_post_city = selectedUser.user_city;
                model.pic_Addpost.pic_refer_id = selectedUser.user_id.ToString();
                model.pic_Addpost.pic_user_fullname = selectedUser.user_username;
                model.pic_Addpost.addpost_scheme_user_id = selectedUser.user_id;
            }
            else
            {
                var selectedUser = _postAdFieldsViewModel.ContactDetails.Where(u => u.user_id == Convert.ToInt32(e.Value)).FirstOrDefault();
                model.pic_Addpost.pic_user_email = selectedUser.user_email;
                model.pic_Addpost.pic_user_mobile = selectedUser.user_mobile;
                model.pic_Addpost.pic_user_id = selectedUser.user_id;
                model.pic_Addpost.pic_post_city = selectedUser.user_city;
                model.pic_Addpost.pic_refer_id = AuthenticationService.User.user_id.ToString();
                model.pic_Addpost.pic_user_fullname = selectedUser.user_username;
                model.pic_Addpost.addpost_scheme_user_id = selectedUser.user_id;
            }
        }

        void ShowLocationSelector()
        {
            changeLocDisplay = "block";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //if (firstRender)
            //{
            //    await JSRuntime.InvokeVoidAsync("initializeAutocompletePostAd", null);
            //}
        }

        private void ToggleEnableCalender(bool value)
        {
            model.pic_Addpost.EnableCalender = value;

            if (value)
            {
                displayHomePageToggle = "block";
            }
            else
            {
                displayHomePageToggle = "none";
                model.pic_Addpost.DisplayOnHomePage = false;
            }
        }

        private async Task OnFileSelectedAsync(InputFileChangeEventArgs e)
        {

            long maxFileSize = 1024L * 1024L * 1024L * 2L;
            using (MemoryStream ms = new MemoryStream())
            {
                await e.File.OpenReadStream(maxFileSize).CopyToAsync(ms);
                var bytes = ms.ToArray();
                model.Profile_Doc_FileName = e.File.Name;
                model.Pofile_Doc_BaseString = Convert.ToBase64String(bytes);
            }
        }

        private async Task OnFileSelectedImageAsync(InputFileChangeEventArgs e)
        {
           
            long maxFileSize = 1024L * 1024L * 1024L * 2L;
            using (MemoryStream ms = new MemoryStream())
            {
                await e.File.OpenReadStream(maxFileSize).CopyToAsync(ms);
                var bytes = ms.ToArray();
                model.Profile_Pic_FileName = e.File.Name;
                model.Pofile_Pic_BaseString = Convert.ToBase64String(bytes);
            }
        }

        private async Task OnHideModalClick()
        {
            await creationModalSuccess.HideAsync();
            navigationManager.NavigateTo("/");
        }
    }
}