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
using Jobbvin.Shared.Models;
using Jobbvin.Client.Services;
using System.Diagnostics.CodeAnalysis;

namespace Jobbvin.Client.Pages.UserMenus
{
    public partial class ManageAdScheme
    {
        private SchemeListViewModel schemes;
        private string error;
        private string success;
        private SchemeListModel schemeListModel = new SchemeListModel();

        [Inject]
        [AllowNull]
        IUserServiceClient UserServiceClient { get; set; }

        [Inject]
        [AllowNull]
        IAuthenticationService AuthenticationService { get; set; }
        private async Task<GridDataProviderResult<SchemeListModel>> PicSchemeDataProvider(GridDataProviderRequest<SchemeListModel> request)
        {
            if (schemes is null) // pull employees only one time for client-side filtering, sorting, and paging
                schemes = await GetSchemes(); // call a service or an API to pull the employees
            StateHasChanged();
            return await Task.FromResult(request.ApplyTo(schemes.schemeListModels));
        }

        private async Task<SchemeListViewModel> GetSchemes()
        {
            SchemeListViewModel res = new SchemeListViewModel();
            try
            {
                res = await UserServiceClient.SchemeList(AuthenticationService.User.user_id);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return res;
        }

        private Task OnSelectedItemsChanged(HashSet<SchemeListModel> scheme)
        {
            schemes.schemeListModels.ForEach(r => r.Select = false);
            scheme.FirstOrDefault().Select = true;
            return Task.CompletedTask;
        }

        void TypeChange(ChangeEventArgs e)
        {
            schemeListModel.Purpose = e.Value.ToString();
        }
        private async Task SubmitSchemePurchase()
        {
            var selectedSheme = schemes.schemeListModels.Where(s => s.Select).FirstOrDefault();
            if(selectedSheme != null)
            {
                schemeListModel.SchemeId = selectedSheme.SchemeId;
              
                try
                {
                    var res = await UserServiceClient.PostSchemePurchase(schemeListModel, AuthenticationService.User.user_id);
                    if(res.Status)
                    {
                        success = res.Message;
                    }
                    else
                    {
                        error = res.Message;
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            else
            {
                error = "Select any one of the scheme to purchase";
            }
        }
    }
}