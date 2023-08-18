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
using System.Diagnostics.CodeAnalysis;

namespace Jobbvin.Client.Shared
{
    partial class Loader
    {
        protected bool IsVisible { get; set; }


        [Inject]
        [AllowNull]
        LoaderService loaderService { get; set; }

        protected override void OnInitialized()
        {
            loaderService.OnShow += ShowLoader;
            loaderService.OnHide += HideLoader;
        }

        public void ShowLoader()
        {
            IsVisible = true;
            StateHasChanged();
        }

        public void HideLoader()
        {
            IsVisible = false;
            StateHasChanged();
        }
    }
}