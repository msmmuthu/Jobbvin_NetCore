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
using BlazorBootstrap;
using Jobbvin.Client.Services;
using Jobbvin.Shared.Models;
using System.Diagnostics.CodeAnalysis;

namespace Jobbvin.Client.Pages.UserMenus
{
    public partial class ViewCustomer
    {
        private IEnumerable<pic_user> customers;
        private string error;

        [Inject]
        [AllowNull]
        IUserServiceClient UserServiceClient { get; set; }

        [Inject]
        [AllowNull]
        IAuthenticationService AuthenticationService { get; set; }
        private async Task<GridDataProviderResult<pic_user>> EmployeesDataProvider(GridDataProviderRequest<pic_user> request)
        {
            if (customers is null) // pull employees only one time for client-side filtering, sorting, and paging
                customers = await GetCustomers(); // call a service or an API to pull the employees
            return await Task.FromResult(request.ApplyTo(customers));
        }

        private async Task<IEnumerable<pic_user>> GetCustomers()
        {
            IEnumerable<pic_user> res = new List<pic_user>();
            try
            {
                res = await UserServiceClient.ViewCustomers(AuthenticationService.User.user_id);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return res;
        }
    }

}