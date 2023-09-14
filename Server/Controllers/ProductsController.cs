using Jobbvin.Shared.Models;
using Jobbvin.Server.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        public AppDb Db { get; }

        private readonly IConfiguration _configuration;

        public ProductsController(AppDb db, ILogger<ProductsController> logger, IConfiguration configuration)
        {
            Db = db;
            _logger = logger;
            _configuration = configuration;
        }

        // POST api/GetProductListByFilter
        [HttpPost("GetProductListByFilter")]
        public async Task<IActionResult> GetProductListByFilter(PoductFilterModel filters)
        {
            var resp = new List<ProductListViewModel>();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration, _logger);
                resp = await query.GetProductListByFilter(filters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMenuItems : " + ex.Message);
            }
            return Ok(resp);
        }

        // POST api/GetProductListApplyByFilter
        [HttpPost("GetProductListApplyByFilter")]
        public async Task<IActionResult> GetProductListApplyByFilter(PostAdViewModel filters)
        {
            var resp = new List<ProductListViewModel>();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration, _logger);
                resp = await query.GetProductListApplyByFilter(filters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetProductListApplyByFilter : " + ex.Message);
            }
            return Ok(resp);
        }


        // POST api/StarRating
        [HttpPost("StarRating")]
        public async Task<IActionResult> StarRating(StarRatingModel starRatingModel)
        {
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration,_logger);
                var resp = await query.GetStarRatingByUser(starRatingModel.itemId, starRatingModel.userId);
                if (resp == 0)
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();
                    var rec = await query.InsertStarRatingAsync(starRatingModel);
                    var result = new ApiResponse();
                    if (rec)
                    {
                        result.Message = "Inserted";
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Invalid request";
                        result.Status = false;
                    }
                    return Ok(result);
                }
                else
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();
                    var rec = await query.UpdateStarRatingAsync(starRatingModel);
                    var result = new ApiResponse();
                    if (rec)
                    {
                        result.Message = "Updated";
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Invalid request";
                        result.Status = false;
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        // POST api/Calendar
        [HttpPost("Calendar")]
        public async Task<IActionResult> Calendar(pic_calendar calenderModel)
        {
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration, _logger);
                var resp = await query.CheckExistsCalenderRecord(calenderModel.addpost_uni_id, calenderModel.addpost_user_id, calenderModel.Cal_Date);
                if (!resp)
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();
                    var rec = await query.InsertCalendarDataAsync(calenderModel);
                    var result = new ApiResponse();
                    if (rec)
                    {
                        result.Message = "Inserted";
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Invalid request";
                        result.Status = false;
                    }
                    return Ok();
                }
                else
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();
                    var rec = await query.UpdateCalendarDataAsync(calenderModel);
                    var result = new ApiResponse();
                    if (rec)
                    {
                        result.Message = "Updated";
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Invalid request";
                        result.Status = false;
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        // POST api/ProductLike
        [HttpPost("ProductLike")]
        public async Task<IActionResult> ProductLike(pic_likes picLike)
        {
            try
            {
                var remoteIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                picLike.likes_cus_ip = remoteIp;

                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration,_logger);
                var resp = await query.CheckLikeExistsRecord(picLike.likes_product_id, picLike.likes_cus_id, picLike.likes_ads_user_id);
                if (!resp)
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();
                    var rec = await query.InsertLikeAsync(picLike);
                    var result = new ApiResponse();
                    if (rec)
                    {
                        result.Message = "Inserted";
                        result.Status = true;
                        _logger.LogError("Saved product like in ProductLike : " + picLike.likes_cus_id);
                        await SMS.SendLikeSMS("", picLike);
                        Email email = new Email(_logger, _configuration);
                        await email.SendEmail(picLike);
                    }
                    else
                    {
                        result.Message = "Invalid request";
                        result.Status = false;
                    }
                    return Ok(result);
                }
                else
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();
                    var rec = await query.UpdateLikeAsync(picLike);
                    var result = new ApiResponse();
                    if (rec)
                    {
                        result.Message = "Updated";
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Invalid request";
                        result.Status = false;
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        // GET api/GetLikeByPoductId
        [HttpGet("GetLikeByPoductId")]
        public async Task<IActionResult> GetLikeByPoductId(string likesProductId, string customerId, string likeAdsUserId)
        {
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration,_logger);
                var resp = await query.CheckLikeExistsRecord(likesProductId, customerId, likeAdsUserId);
                var result = new ApiResponse();
                result.Message = "Success";
                result.Status = resp;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        // GET api/GetProductDetails
        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProductDetails(string adId, string customerId)
        {
            var prodDetailsViewModel = new ProductDetailsViewModel();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration,_logger);
                prodDetailsViewModel.ProductListViewModel = await query.GetProductDetailsById(adId);
                prodDetailsViewModel.ProductDetailsFieds = await query.GetProductFieldDataById(adId);
                prodDetailsViewModel.ProductFiles = await query.GetProductFilesById(adId);
                prodDetailsViewModel.StarRateValue = await query.GetStarRatingByUser(Convert.ToInt32(adId), Convert.ToInt32(customerId));
                prodDetailsViewModel.DisplayContact = await query.CheckLikeExistsRecord(adId, customerId, prodDetailsViewModel.ProductListViewModel.pic_user_id.ToString());
                var userQuery = new UserQuery<ProductsController>(Db, _configuration,_logger);
                var user = await userQuery.GetUserById(prodDetailsViewModel.ProductListViewModel.pic_user_id, false);
                prodDetailsViewModel.ContactDetails = user.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetProductDetails : " + ex.Message);
            }
            return Ok(prodDetailsViewModel);
        }

        // GET api/GetProductDetails
        [HttpGet("GetPostFields")]
        public async Task<IActionResult> GetPostFields(int category_id, int userId)
        {
            var postAdViewModel = new GetPostAdFieldsViewModel();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration,_logger);
                //postAdViewModel.ProductListViewModel = await query.GetProductDetailsById(adId);
                postAdViewModel.ProductCategoryFields = await query.GetPostFieldCategoriesDataByCategoryId(category_id);
                postAdViewModel.BalanceScheme = await query.GetBalanceScheme(userId);
                postAdViewModel.CategoryName = await query.GetCategoryText(category_id);

                //prodDetailsViewModel.ProductFiles = await query.GetProductFilesById(adId);
                //prodDetailsViewModel.DisplayContact = await query.CheckLikeExistsRecord(adId, customerId, prodDetailsViewModel.ProductListViewModel.pic_user_id.ToString());
                var userQuery = new UserQuery<ProductsController>(Db, _configuration,_logger);
                var users = await userQuery.GetUserById(userId, true);
                users = users.Where(u => u.user_type.ToLower() == "customer" && u.user_status == 1).ToList();
                postAdViewModel.ContactDetails = users;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPostFileds : " + ex.Message);
            }
            return Ok(postAdViewModel);
        }

        // GET api/GetAdHistoryByUserId
        [HttpGet("GetAdHistoryByUserId")]
        public async Task<IActionResult> GetAdHistoryByUserId(string userId)
        {
            var productListViewModel = new List<ProductListViewModel>();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new PoductsQuery<ProductsController>(Db, _configuration, _logger);
                productListViewModel = await query.GetPostHistoryByUserId(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAdHistoryByUserId : " + ex.Message);
            }
            return Ok(productListViewModel);
        }

        [HttpPost("PostAd")]
        public async Task<IActionResult> PostAd(PostAdViewModel postAdView)
        {
            var result = new ApiResponse();
            byte[] Pofile_Doc_Bytes = null;
            byte[] Pofile_Pic_Bytes = null; ;

            if (!string.IsNullOrEmpty(postAdView.Pofile_Doc_BaseString))
            {
                _logger.LogInformation("Pofile_Doc_BaseString base string to byte aray start");
                Pofile_Doc_Bytes = Convert.FromBase64String(postAdView.Pofile_Doc_BaseString);
                _logger.LogInformation("Pofile_Doc_BaseString base string to byte aray completed");
            }
            if (!string.IsNullOrEmpty(postAdView.Pofile_Pic_BaseString))
            {
                _logger.LogInformation("Pofile_Pic_BaseString base string to byte aray start");
                Pofile_Pic_Bytes = Convert.FromBase64String(postAdView.Pofile_Pic_BaseString);
                _logger.LogInformation("Pofile_Pic_BaseString base string to byte aray completed");
            }

            bool res = false;
            var adPostId = 0;
            //Insert and get AdPost
            await Db.Connection.OpenAsync();
            var query = new PoductsQuery<ProductsController>(Db, _configuration, _logger);
            postAdView.pic_Addpost.pic_postdate = DateTime.Now;
            if (postAdView.pic_Addpost.pic_ads_id > 0)
            {
                _logger.LogInformation("UdateAdPostAsync starts from contoller");
                adPostId = await query.UpdateAdPostAsync(postAdView.pic_Addpost);
                _logger.LogInformation("UdateAdPostAsync ends from contoller");
            }
            else
            {
                _logger.LogInformation("InsertAdPostAsync starts from contoller");
                 adPostId = await query.InsertAdPostAsync(postAdView.pic_Addpost);
                _logger.LogInformation("InsertAdPostAsync ends from contoller");
            }

            if (adPostId > 0)
            {
                //Insert Ad Post Fields
                _logger.LogInformation("InsertOrUpdateAdPostFieldsAsync starts from contoller" + postAdView.pic_Addpost_Field.Count.ToString());
                postAdView.pic_Addpost_Field.ForEach(f => f.addpost_uni_id = adPostId);
                res = await query.InsertOrUpdateAdPostFieldsAsync(postAdView.pic_Addpost_Field);
                _logger.LogInformation("InsertOrUpdateAdPostFieldsAsync ends from contoller");

                //Insert Ad Post Location
                _logger.LogInformation("InsertAdPostLocationAsync starts from contoller");
                postAdView.pic_Addpost_Locations.ForEach(f => f.addpost_uni_id = adPostId);
                res = await query.InsertAdPostLocationAsync(postAdView.pic_Addpost_Locations, adPostId);
                _logger.LogInformation("InsertAdPostLocationAsync ends from contoller");

                if (res)
                {
                    var ftp = new FtpFileUpload<ProductsController>(_logger, _configuration);

                    //upload profile documents
                    if (!string.IsNullOrEmpty(postAdView.Pofile_Doc_BaseString))
                    {
                        postAdView.Profile_Doc_FileName = adPostId + Path.GetExtension(postAdView.Profile_Doc_FileName);
                        //Insert Ad Post Document file
                        _logger.LogInformation("InsertAdPostDocFilesAsync starts from contoller");
                        res = await query.InsertAdPostDocFilesAsync(adPostId, postAdView.Profile_Doc_FileName);
                        _logger.LogInformation("InsertAdPostDocFilesAsync ends from contoller");

                        _logger.LogInformation("Upload doc file starts from contoller");
                        await ftp.UploadFile(Pofile_Doc_Bytes, postAdView.Profile_Doc_FileName, false);
                        result.Status = true;
                        _logger.LogInformation("Upload doc file ends from contoller");

                    }
                    //upload profile image

                    if (!string.IsNullOrEmpty(postAdView.Pofile_Pic_BaseString))
                    {
                        postAdView.Profile_Pic_FileName = adPostId + Path.GetExtension(postAdView.Profile_Pic_FileName);
                        //Insert Ad Post Image file
                        _logger.LogInformation("InsertAdPostPofileImageAsync starts from contoller");
                        res = await query.InsertAdPostPofileImageAsync(adPostId, postAdView.Profile_Pic_FileName);
                        _logger.LogInformation("InsertAdPostPofileImageAsync ends from contoller");

                        _logger.LogInformation("Upload imgae file starts from contoller");
                        await ftp.UploadFile(Pofile_Pic_Bytes, postAdView.Profile_Pic_FileName, false);
                        result.Status = true;
                        _logger.LogInformation("Upload image file ends from contoller");
                    }
                }
                else
                {
                    result.Message = "Invalid request";
                    result.Status = false;
                }
            }
            else
            {
                result.Message = "Error in create ad post";
                result.Status = false;
            }

            return Ok(result);
        }
    }
}
