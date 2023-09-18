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
using Jobbvin.Client.Services;
using Jobbvin.Shared.Models;
using System.Diagnostics.CodeAnalysis;
using BlazorBootstrap;

namespace Jobbvin.Client.Pages.UserMenus
{
    public partial class ManageProfile
    {
        private string error;
        private pic_user model = new pic_user();

        [Inject]
        [AllowNull]
        IAuthenticationService AuthenticationService { get; set; }

        [Inject] protected PreloadService PreloadService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            PreloadService.Show();
            model = AuthenticationService.User;
            PreloadService.Hide();
        }

        private async void HandleValidSubmit()
        {
            //loading = true;
            //error = "";
            //try
            //{
            //    if (!string.IsNullOrEmpty(model.mobile_val))
            //    {
            //        if (response.Status)
            //        {
            //            timeRemains = response.Message;
            //            displaySubmitOTP = "none";
            //            displaySubmit = "none";
            //            displaySubmitResendOTP = "none";
            //            displayOTPText = "none";
            //            displayLogin = "block";
            //        }
            //        else
            //        {
            //            error = response.Message;
            //        }
            //    }
            //    else
            //    {
            //        var response = await UserServiceClient.InsertTempUser(model);
            //        if (response.Status)
            //        {
            //            displaySubmitOTP = "block";
            //            displaySubmit = "none";
            //            displaySubmitResendOTP = "none";
            //            displayOTPText = "block";
            //            StartTimer();
            //        }
            //        error = response.Message;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    error = ex.Message;
            //    loading = false;
            //}
            //loading = false;
            //StateHasChanged();
        }
    }
}