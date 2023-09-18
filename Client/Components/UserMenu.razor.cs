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

namespace Jobbvin.Client.Components
{
    public partial class UserMenu
    {
        Sidebar sidebar;
        IEnumerable<NavItem> navItems;

        private async Task<SidebarDataProviderResult> SidebarDataProvider(SidebarDataProviderRequest request)
        {
            if (navItems is null)
                navItems = GetNavItems();

            return await Task.FromResult(request.ApplyTo(navItems));
        }

        private IEnumerable<NavItem> GetNavItems()
        {
            navItems = new List<NavItem>
        {
            new NavItem { Id = "0", IconName = IconName.LayoutSidebarInset, Text = "Pofile" },
            new NavItem { Id = "1", Href = "/View-Profile", IconName = IconName.Circle, Text = "View Profile",ParentId="0"},
            new NavItem { Id = "2", Href = "/Manage-Profile", IconName = IconName.Circle, Text = "Manage Profile",ParentId="0"},
            new NavItem { Id = "3", Href = "/View-Customer", IconName = IconName.Circle, Text = "View Customer",ParentId="0"},
            new NavItem { Id = "4", Href = "/Add-Customer", IconName = IconName.Circle, Text = "Add Customer",ParentId="0"},

            new NavItem { Id = "10", IconName = IconName.LayoutSidebarInset, Text = "Manage Ads Scehme" },
            new NavItem { Id = "11", Href = "/Purchase-Scheme", IconName = IconName.Circle, Text = "Purchase Scheme",ParentId="10"},
            new NavItem { Id = "12", Href = "/Purchase-History", IconName = IconName.Circle, Text = "Purchase History",ParentId="10"},
            new NavItem { Id = "13", Href = "/Ads-Count", IconName = IconName.Circle, Text = "Ads Account",ParentId="10"},

            new NavItem { Id = "20", IconName = IconName.LayoutSidebarInset, Text = "Ads History" },
            new NavItem { Id = "21", Href = "/View-Ads-History", IconName = IconName.Circle, Text = "View All",ParentId="20"},

            new NavItem { Id = "30", IconName = IconName.LayoutSidebarInset, Text = "Like History" },
            new NavItem { Id = "31", Href = "/Like-History", IconName = IconName.Circle, Text = "List of Like's",ParentId="30"},

              new NavItem { Id = "40", IconName = IconName.LayoutSidebarInset, Text = "Specials Location & Category" },
            new NavItem { Id = "41", Href = "/My-Location-Ads", IconName = IconName.Circle, Text = "My Location Ads",ParentId="40"},
            new NavItem { Id = "42", Href = "/My-Special-Ads", IconName = IconName.Circle, Text = "My Special Ads",ParentId="40"},
        };

            return navItems;
        }
    }
}