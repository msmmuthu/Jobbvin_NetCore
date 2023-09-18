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
    public partial class AdsCount
    {
        private IEnumerable<Pic_Ads_Count> customers;
        private string error;

        [Inject]
        [AllowNull]
        IUserServiceClient UserServiceClient { get; set; }

        [Inject]
        [AllowNull]
        IAuthenticationService AuthenticationService { get; set; }
        private async Task<GridDataProviderResult<Pic_Ads_Count>> AdsCountDataProvider(GridDataProviderRequest<Pic_Ads_Count> request)
        {
            if (customers is null) // pull employees only one time for client-side filtering, sorting, and paging
                customers = await GetPurchaseHistory(); // call a service or an API to pull the employees
            return await Task.FromResult(request.ApplyTo(customers));
        }

        private async Task<IEnumerable<Pic_Ads_Count>> GetPurchaseHistory()
        {
            IEnumerable<Pic_Ads_Count> res = new List<Pic_Ads_Count>();
            try
            {
                res = await UserServiceClient.SchemeAdsCount(AuthenticationService.User.user_id);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return res;
        }
    }
}