using Jobbvin.Shared.Models;
using Jobbvin.Server.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jobbvin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        public AppDb Db { get; }

        private readonly IConfiguration _configuration;

        public UsersController(AppDb db, ILogger<UsersController> logger, IConfiguration configuration)
        {
            Db = db;
            _logger = logger;
            _configuration = configuration;
        }

        // GET api/GetMenuItems
        [HttpGet("GetMenuItems")]
        public async Task<IActionResult> GetMenuItems()
        {
            var resp = new List<pic_categories>();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new UserQuery<UsersController>(Db, _configuration,_logger);
                resp = await query.GetAllActiveMenuItems();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMenuItems : " + ex.Message);
            }
            return Ok(resp);
        }

        // POST api/users/tempuser
        [HttpPost("TempUser")]
        public async Task<IActionResult> TempUser([FromBody] temp_user temp_User)
        {
            var result = new ApiResponse();

            try
            {
                await Db.Connection.OpenAsync();
                var query = new UserQuery<UsersController>(Db, _configuration,_logger);
                var resp = await query.CheckEmailsExists(temp_User.user_email);
                if (!resp)
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();
                    resp = await query.CheckMobileExists(temp_User.user_mobile);
                    if (resp)
                    {
                        result = new ApiResponse();
                        result.Message = "Mobile Number already exists";
                        result.Status = false;
                        result.ValidationName = "user_mobile";
                        _logger.LogInformation("Insert TempUsers not completed ");
                        return Ok(result);
                    }
                    else
                    {
                        if (Db.Connection.State != System.Data.ConnectionState.Open)
                            await Db.Connection.OpenAsync();

                        Random random = new Random();
                        int otp = random.Next(1000, 9999);
                        temp_User.mobile_val = otp.ToString();
                        bool rec = false;
                        var userId = await query.CheckEmailMobilExists(temp_User.user_email, temp_User.user_mobile,false);
                        if (userId > 0)
                        {
                            temp_User.user_id = userId;
                            _logger.LogInformation("TempUser : Insise update");
                            rec = await query.UdateTempUserAsync(temp_User);
                             result.Message = "Updated";
                            _logger.LogInformation("TempUser : Insise update completed");

                        }
                        else
                        {
                            _logger.LogInformation("TempUser : Insise Insert");
                            rec = await query.InsertTempUserAsync(temp_User);
                            result.Message = "Inserted";
                            _logger.LogInformation("TempUser : Insise Insert completed");

                        }
                        result = new ApiResponse();
                        if (rec)
                        {

                            var response = SMS.SendSMS(temp_User.user_mobile, temp_User.mobile_val);
                            _logger.LogInformation("SMS Status :  + response.ResponseStatus");
                            result.Status = true;
                        }
                        else
                        {
                            result.Message = "Invalid request";
                            result.Status = false;
                        }
                        _logger.LogInformation("Insert TempUsers not completed ");

                        return Ok(result);
                    }
                }
                else
                {
                    result.Message = "Email Id already exists";
                    result.Status = false;
                    result.ValidationName = "user_email";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Insert TempUsers : " + ex.Message);
            }
            return Ok(result);
        }

        [HttpPost("ValidateOtp")]
        public async Task<IActionResult> ValidateOtp([FromBody] temp_user temp_User)
        {
            var result = new ApiResponse();

            try
            {
                await Db.Connection.OpenAsync();
                var query = new UserQuery<UsersController>(Db, _configuration,_logger);
                var resp = await query.CheckOtP(temp_User);
                if (resp.Message == "all_success")
                {
                    result.Message = "Registration completed successfully";
                    result.Status = true;
                    result.ValidationName = "otp_validation";
                    await query.InsertTempUserIntoUserAsync(temp_User);
                    await query.DeleteTempUserIntoUserAsync(temp_User);
                    return Ok(result);
                }
                else
                {
                    result.Message = resp.Message;
                    result.Status = false;
                    result.ValidationName = "otp_validation";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Insert TempUsers : " + ex.Message);
            }
            return Ok(result);
        }

        [HttpGet("ViewCustomers")]
        public async Task<IActionResult> ViewCustomers(int userId)
        {
            var resp = new List<pic_user>();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new UserQuery<UsersController>(Db, _configuration,_logger);
                resp = await query.GetUserById(userId, true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetMenuItems : " + ex.Message);
            }
            return Ok(resp);
        }

        [HttpGet("SchemePurchaseHistory")]
        public async Task<IActionResult> SchemePurchaseHistory(int userId)
        {
            var resp = new List<pic_scheme_user>();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new UserQuery<UsersController>(Db, _configuration, _logger);
                resp = await query.GetPicSchemeUser(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SchemePurchaseHistory : " + ex.Message);
            }
            return Ok(resp);
        }

        [HttpGet("SchemeAdsCount")]
        public async Task<IActionResult> SchemeAdsCount(int userId)
        {
            var resp = new List<Pic_Ads_Count>();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new UserQuery<UsersController>(Db, _configuration, _logger);
                resp = await query.GetSchemeAdsCount(userId);
                var totalSchemes = await query.GetTotalScehmeCount(userId);
                resp.ForEach(r => r.TotalSchemes = totalSchemes);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SchemeAdsCount : " + ex.Message);
            }
            return Ok(resp);
        }

        [HttpGet("SchemeList")]
        public async Task<IActionResult> SchemeList(int userId)
        {
            var schemeListViewModels = new SchemeListViewModel();
            try
            {
                await Db.Connection.OpenAsync();
                var query = new UserQuery<UsersController>(Db, _configuration, _logger);
                var schemeList = await query.GetSchemeList();
                var displayButton = await query.GetSchemeListSubmitButton(userId);
                schemeListViewModels.DisplaySubmitButton = displayButton;
                schemeListViewModels.schemeListModels = schemeList;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SchemeList : " + ex.Message);
            }
            return Ok(schemeListViewModels);
        }

        // POST api/users/tempuser
        [HttpPost("PostSchemePuchase")]
        public async Task<IActionResult> PostSchemePuchase([FromBody] SchemeListModel schemeListModel, int userId)
        {
            var result = new ApiResponse();

            try
            {
              
                var query = new UserQuery<UsersController>(Db, _configuration, _logger);
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                var picScheme = await query.GetPicScheme(schemeListModel.SchemeId);
                var pic_scheme_user_id = await query.GetTopPicSchemeUser();
                var cashid = "PACASH00" + pic_scheme_user_id;

                var picSchemeUser = new pic_scheme_user();

                picSchemeUser.pic_scheme_id = picScheme.scheme_id;
                picSchemeUser.pic_scheme_name = picScheme.scheme_name;
                picSchemeUser.pic_scheme_desc = picScheme.scheme_desc;
                picSchemeUser.pic_scheme_balance_qty = picScheme.scheme_ads_qty;
                picSchemeUser.total_ads = picScheme.scheme_ads_qty;
                picSchemeUser.cost_scheme = picScheme.scheme_price;
                picSchemeUser.payment_status = "Pending";
                picSchemeUser.payment_method = "Net Banking";
                picSchemeUser.pic_user_id = userId;
                picSchemeUser.scheme_purchased_date = DateTime.Now;
                picSchemeUser.scheme_purpose = "post";
                picSchemeUser.scheme_cash_id = cashid;
                picSchemeUser.payment_details = schemeListModel.PaymentDetails;
                picSchemeUser.photo_limit = picScheme.scheme_photo;
                picSchemeUser.ads_valid = picScheme.scheme_valid;
                var rec = await query.InsertPurchaseSchemeAsync(picSchemeUser);
                if (rec)
                {
                    result.Message = "Scheme purchase success";
                    result.Status = true;
                }
                else
                {
                    result.Message = "Scheme purchase failed";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Insert PostSchemePuchase : " + ex.Message);
                result.Message = "Scheme purchase failed: " + ex.Message;
                result.Status = false;
            }
            return Ok(result);
        }

        // POST api/users/PicUser
        [HttpPost("PicUser")]
        public async Task<IActionResult> PicUser([FromBody] pic_user pic_User)
        {
            _logger.LogInformation("Entered PicUser : " );

            var result = new ApiResponse();

            try
            {
                if (!string.IsNullOrEmpty(pic_User.user_pic_BaseString))
                {
                    _logger.LogInformation("Image base sting to byte aray start");
                    //pic_User.user_pic = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(pic_User.user_pic_string));
                    pic_User.user_pic = Convert.FromBase64String(pic_User.user_pic_BaseString);
                    _logger.LogInformation("Image base sting to byte aray completed");
                }

                await Db.Connection.OpenAsync();
       
             var query = new UserQuery<UsersController>(Db, _configuration,_logger);
                bool checkMailOnUpdate = true;
                bool checkMobileOnUpdate = true;
                _logger.LogInformation("Entered PicUser -1: ");

                if (pic_User.user_id > 0)
                {
                    var users = await query.GetUserById(pic_User.user_id, false);
                    _logger.LogInformation("Entered PicUser -2: ");

                    if (users != null && users.Count > 0)
                    {
                        _logger.LogInformation("Entered PicUser -3: ");

                        var user = users.FirstOrDefault();
                        pic_User.user_pic_url = user.user_pic_url;

                        if (user.user_email != pic_User.user_email)
                            checkMailOnUpdate = true;
                        else
                            checkMailOnUpdate = false;
                        if (user.user_mobile != pic_User.user_mobile)
                            checkMobileOnUpdate = true;
                        else
                            checkMobileOnUpdate = false;
                    }
                }
                bool mailValidation = false;

                if (checkMailOnUpdate)
                {
                    _logger.LogInformation("Entered PicUser -4: ");

                    mailValidation = await query.CheckEmailsExists(pic_User.user_email);
                    _logger.LogInformation("Entered PicUser -5: ");

                }

                bool mobValidation = false;
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                if (checkMobileOnUpdate)
                {
                    _logger.LogInformation("Entered PicUser -6: ");

                    mobValidation = await query.CheckMobileExists(pic_User.user_mobile);
                    _logger.LogInformation("Entered PicUser -7: ");

                }
                if (mailValidation)
                {
                    result.Message = "Email Id already exists";
                    result.Status = false;
                    result.ValidationName = "user_email";
                    return Ok(result);
                }
                if (mobValidation)
                {
                    result = new ApiResponse();
                    result.Message = "Mobile Number already exists";
                    result.Status = false;
                    result.ValidationName = "user_mobile";
                    _logger.LogInformation("Insert PicUsers not completed ");
                    return Ok(result);
                }
                else
                {
                    if (Db.Connection.State != System.Data.ConnectionState.Open)
                        await Db.Connection.OpenAsync();

                    bool rec = false;
                    //var userId = await query.CheckEmailMobilExists(pic_User.user_email, pic_User.user_mobile, true);
                    if (pic_User.user_id > 0)
                    {
                        _logger.LogInformation("Entered PicUser -8: ");

                        rec = await query.UdatePicUserAsync(pic_User);
                        result.Message = "Updated";
                    }
                    else
                    {
                        _logger.LogInformation("Entered PicUser -9: ");

                        pic_User.user_pic_url = Guid.NewGuid().ToString() + Path.GetExtension(pic_User.user_pic_url);
                        rec = await query.InsertPicUserAsync(pic_User);
                        result.Message = "Inserted";
                    }
                    result = new ApiResponse();
                    if (rec)
                    {
                        _logger.LogInformation("Entered PicUser -10: ");

                        _logger.LogInformation("pic user bytes " + pic_User.user_pic.ToString());
                        var ftp = new FtpFileUpload<UsersController>(_logger, _configuration);
                        await ftp.UploadFile(pic_User.user_pic, pic_User.user_pic_url, true);
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Invalid request";
                        result.Status = false;
                    }
                    _logger.LogInformation("Insert PicUsers completed ");

                    return Ok(result);
                }
            }

            catch (Exception ex)
            {
                result.Message = ex.Message;
                _logger.LogError("Insert PicUsers not completed : " + ex.Message);
                result.Status = false;

            }
            return Ok(result);
        }

        private async Task SaveByteArrayToFileWithStaticMethod(byte[] data, string filePath)
        {
            try
            {
                _logger.LogInformation("Insert user - Uploadphoto");
                System.IO.File.WriteAllBytes(_configuration.GetSection("Baseurl").Value + "/media/profile/" + filePath, data);
                _logger.LogInformation("Insert user - Uploadphoto Success");
            }
            catch (Exception ex)
            {
                _logger.LogError("Insert user - Uploadphoto : " + ex.Message);
            }
        }

        
    }
}
