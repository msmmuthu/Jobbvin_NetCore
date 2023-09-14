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
using Microsoft.AspNetCore.WebUtilities;

namespace Jobbvin.Client.Pages
{
    public partial class PostAd
    {
        private bool _showPostFields = false;
        string returnUrl;
       
        public EventCallback<int> OnShowPostFields { get; set; }
        protected override async Task OnInitializedAsync()
        {
            OnShowPostFields = new EventCallback<int>(this, (Action<int>)ShowPostFields);
            returnUrl = "/";
        }

        void ShowPostFields(int subCategoyId)
        {
            _showPostFields = true;
        }
    }
}