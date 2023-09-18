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

namespace Jobbvin.Client.Shared
{
    public partial class NavMenu
    {
        private pic_user _user { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }


        public async Task SetUserAfteLogin(IAuthenticationService AuthenticationUser, NavigationManager navigationManager, string page)
        {
            _user = AuthenticationUser.User;
            await Task.CompletedTask;
            navigationManager.NavigateTo(page);
        }
    }
}