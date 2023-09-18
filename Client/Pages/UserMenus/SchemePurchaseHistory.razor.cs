using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Jobbvin.Client.Shared;
using BlazorBootstrap;
using Jobbvin.Shared.Models;
using Jobbvin.Client.Services;
using System.Diagnostics.CodeAnalysis;

namespace Jobbvin.Client.Pages.UserMenus
{
    public partial class SchemePurchaseHistory
    {
        private IEnumerable<pic_scheme_user> customers;
        private string error;

        [Inject]
        [AllowNull]
        IUserServiceClient UserServiceClient { get; set; }

        [Inject]
        [AllowNull]
        IAuthenticationService AuthenticationService { get; set; }
        private async Task<GridDataProviderResult<pic_scheme_user>> SchemeListDataProvider(GridDataProviderRequest<pic_scheme_user> request)
        {
            if (customers is null) // pull employees only one time for client-side filtering, sorting, and paging
                customers = await GetPurchaseHistory(); // call a service or an API to pull the employees
            return await Task.FromResult(request.ApplyTo(customers));
        }

        private async Task<IEnumerable<pic_scheme_user>> GetPurchaseHistory()
        {
            IEnumerable<pic_scheme_user> res = new List<pic_scheme_user>();
            try
            {
                res = await UserServiceClient.SchemePurchaseHistory(AuthenticationService.User.user_id);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return res;
        }
    }
}