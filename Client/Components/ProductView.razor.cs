using global::Microsoft.AspNetCore.Components;
using Jobbvin.Shared.Models;

namespace Jobbvin.Client.Components
{
    public partial class ProductView
    {
        [CascadingParameter(Name = "Item")]
        public ProductListViewModel Item { get; set; }
    }
}