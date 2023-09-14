using BlazorBootstrap;
using global::Microsoft.AspNetCore.Components;
using Jobbvin.Client.Services;
using Jobbvin.Shared.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;

namespace Jobbvin.Client.Pages
{
    public partial class ProductDetails
    {
        public int AdId { get; set; }
        private bool loading;

        [Inject] protected PreloadService PreloadService { get; set; }

        [Inject]
        [AllowNull]
        IProductServiceClient ProductServiceClient { get; set; }

        string likedClass = "text-success fa fa-check-circle";
        string likeText = "Liked";
        private string error;
        private List<BreadcrumbItem> NavItems1 { get; set; }

        public pic_likes picLikes { get; set; } = new pic_likes();
        public ProductDetailsViewModel _productDetailsViewModel { get; set; }
        protected override async Task OnInitializedAsync()
        {
           

            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("adId", out var param1))
            {
                AdId = Convert.ToInt32(param1.First());
            }
            await GetProductDetails();
            NavItems1 = new List<BreadcrumbItem>
            {
                new BreadcrumbItem{ Text = "Home", Href ="/" },
                new BreadcrumbItem{ Text = "Listing", Href ="/JobListing?categoryId="+_productDetailsViewModel.ProductListViewModel.pic_categoryId },
                new BreadcrumbItem{ Text = "Details", IsCurrentPage = true }
            };

        }

        private async Task GetProductDetails()
        {
            PreloadService.Show();
            try
            {
                _productDetailsViewModel = await ProductServiceClient.GetProductDetails(AdId, AuthenticationService.User.user_id);
                if (_productDetailsViewModel.DisplayContact)
                {
                    likeText = "Liked";
                    likedClass = "text-success fa fa-check-circle";
                }
                else
                {
                    likedClass = ""; likeText = "Like";
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
            PreloadService.Hide();
        }

        private Modal modalLiked = default!;
        private Modal modalLike = default!;

        private async Task OnLikeClick()
        {
            if (_productDetailsViewModel.DisplayContact)
            {
                await modalLiked.ShowAsync();
            }
            else
            {
                picLikes.likes_product_id = Convert.ToString(AdId);
                picLikes.likes_cus_id = Convert.ToString(AuthenticationService.User.user_id);
                //picLikes.likes_cus_ip = await GetIp();
                picLikes.likes_cus_name = AuthenticationService.User.user_username;
                picLikes.likes_cus_mobile = AuthenticationService.User.user_mobile;
                picLikes.likes_cus_email = AuthenticationService.User.user_email;
                picLikes.likes_ads_user_id = Convert.ToString( _productDetailsViewModel.ProductListViewModel.pic_user_id);
                await modalLike.ShowAsync();
            }
        }

        private async Task OnHideModalClick()
        {
            await modalLiked.HideAsync();
        }

        private async Task OnHideLikeModalClick()
        {
            await modalLike.HideAsync();
        }

        private async Task OnLikeSubmit(EditContext context)
        {
            try
            {
                loading = true;
                var res = await ProductServiceClient.LikeAd(picLikes);
                if (res.Status)
                {
                    await GetProductDetails();
                    await OnHideLikeModalClick();
                }
                else
                    error = res.Message;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
            finally
            {
                loading = false;
            }
        }

        private void OnLikeInvalidSubmit(EditContext context)
        {
            try
            {
                if (!context.Validate())
                {
                    var errors = context.GetValidationMessages();
                    error = string.Join(",<b/>", errors);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
        }

        private async Task RatingUpdate(int val, int adId)
        {
            try
            {
                StarRatingModel starRating = new StarRatingModel();
                starRating.ratingNumber = val;  
                starRating.itemId = adId;
                starRating.userId = AuthenticationService.User.user_id;
                var res = await ProductServiceClient.StarRating(starRating);
                if (res.Status)
                {
                    _productDetailsViewModel.StarRateValue = val;
                }
                else
                    error = res.Message;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                StateHasChanged();
            }
        }
    }
}