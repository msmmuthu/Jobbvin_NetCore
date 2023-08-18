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
using System.ComponentModel.DataAnnotations;
using Jobbvin.Client.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace Jobbvin.Client.Pages
{
    public partial class Login
    {

        private Model model = new Model();
        private bool loading;
        private string error;
        protected override void OnInitialized()
        {
            // redirect to home if already logged in
            if (AuthenticationService.User != null)
            {
                navigationManager.NavigateTo("");
            }
        }

        private async void HandleValidSubmit()
        {
            loading = true;
            try
            {
                await AuthenticationService.Login(model.Username, model.Password);
                var returnUrlUri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(returnUrlUri.Query).TryGetValue("returnUrl", out var param1))
                    // var returnUrl = navigationManager.QueryString("returnUrl") ?? "/";
                    navigationManager.NavigateTo(param1.First());
                else
                    navigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                error = ex.Message;
                loading = false;
                StateHasChanged();
            }
        }

        private class Model
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }
        }
    }
}