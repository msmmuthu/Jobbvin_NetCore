using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Jobbvin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobbvinController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JobbvinController> _logger;

        public AppDb Db { get; }

        public JobbvinController(AppDb db, IConfiguration configuration, ILogger<JobbvinController> logger)
        {
            Db = db;
            _configuration = configuration;
            _logger = logger;
        }

        // GET api/jobbvin/mathi/123
        [HttpGet("{userName}/{pass}")]
        public async Task<IActionResult> GetOne(string userName, string pass)
        {
            try
            {
                await Db.Connection.OpenAsync();
                var query = new JobbvinQuery(Db, _configuration);

                var resp = await query.FindOneAsync(userName, pass);
                var result = new ApiResponse();
                if (resp != null)
                {
                    return Ok(resp);
                }
                else
                {
                    result.Message = "Invalid user";
                    result.Status = false;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                
                _logger.LogError("Login user  : " + ex.Message);
                return new StatusCodeResult(500);

            }
        }

        [HttpGet("GetCalendarDisplayData/{userId}")]
        public async Task<IActionResult> GetCalendarDisplayData(int userId)
        {
            try
            {
                await Db.Connection.OpenAsync();
                var query = new JobbvinQuery(Db, _configuration);

                var resp = await query.FindDisplayCalendaAsync(userId);
                var result = new ApiResponse();
                if (resp != null)
                {
                    return Ok(resp);
                }
                else
                {
                    result.Message = "Invalid user";
                    result.Status = false;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Login user  : " + ex.Message);
                return new StatusCodeResult(500);

            }
        }

        // GET api/jobbvin/2/08/2022
        [HttpGet("GetMonthlyCalendarDataByAdId/{adId}/{month}/{year}")]
        public async Task<IActionResult> GetMonthlyCalendarDataByAdId(int adId,int month,int year)
        {
            try
            {
                var userInput = new UserInput();
                userInput.adid = adId;
                userInput.month = month;
                userInput.year = year;
                await Db.Connection.OpenAsync();
                var query = new JobbvinQuery(Db, _configuration);
                var resp = await query.FindMultipleAsync(userInput);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        //// GET api/jobbvin/2/08/2022
        //[HttpGet("GetMonthlyDataByUserId/{userId}")]
        //public async Task<IActionResult> GetMonthlyDataByUserId(int userId)
        //{
        //    var userInput = new UserInput();
        //    userInput.userid = userId;
     
        //    await Db.Connection.OpenAsync();
        //    var query = new JobbvinQuery(Db, _configuration);
        //    var resp = await query.FindMultipleByUserIdAsync(userInput);
        //    return Ok(resp);
        //}

        // POST api/jobbvin
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] users body)
        {
            try
            {
                await Db.Connection.OpenAsync();
                var query = new JobbvinQuery(Db, _configuration);
                var resp = await query.CheckExistsRecord(body.user_id, body.user_date);
                if (!resp)
                {
                    if(Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                    body.Db = Db;
                    var rec = await body.InsertAsync();
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
                    body.Db = Db;
                    var rec = await body.UpdateAsync();
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

        // POST api/jobbvin/UpdateByUser
        [HttpPost("UpdateByUser")]
        public async Task<IActionResult> UpdateByUser([FromBody] List<users> bodyContent)
        {
            try
            {
                await Db.Connection.OpenAsync();
                var query = new JobbvinQuery(Db, _configuration);
                var result = new ApiResponse();

                foreach (var body in bodyContent)
                {
                    var resp = await query.CheckExistsRecord(body.user_id, body.user_date);
                    if (!resp)
                    {
                        if (Db.Connection.State != System.Data.ConnectionState.Open)
                            await Db.Connection.OpenAsync();
                        body.Db = Db;
                        var rec = await body.InsertAsync();
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
                    }
                    else
                    {
                        if (Db.Connection.State != System.Data.ConnectionState.Open)
                            await Db.Connection.OpenAsync();
                        body.Db = Db;
                        var rec = await body.UpdateAsync();
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
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        [HttpGet("mobileApi")]
        public async Task<IActionResult> mobileApi()
        {
            await Db.Connection.OpenAsync();
            var query = new JobbvinQuery(Db, _configuration);
            var resp = await query.GetAllBusinessServiceData();

            var result = new List<pic_website>();
            foreach (var web in resp)
            {
                var website = new pic_website();
                website.id = web.id.ToString();
                website.website_name = web.website_name;
                website.website_url = web.website_url;
                website.logo = web.logo;
                website.status = web.status;

                website.url = "https://jobbvin.com/admincp/media/weblogo/" + web.logo;
                //using (var webClient = new WebClient())
                //{
                //    byte[] imageBytes = webClient.DownloadData(imgUrl);
                //    website.profileImg = imageBytes;
                //}

                result.Add(website);
            }

            //return Ok(new { data = result });
            //var data = new data();
            //data.pic_website = result;
            return Ok(result);
        }

    }
}
