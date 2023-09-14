using Jobbvin.Shared.Models;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Jobbvin.Server.Utility
{
    public  class SMS
    {
        public static RestResponse SendSMS(string mobileNumbe, string otp)
        {
            try
            {
                //var client = new RestClient("https://www.fast2sms.com/dev/bulkV2");
                //var request = new RestRequest("", Method.Post);
                //request.AddHeader("content-type", "application/x-www-form-urlencoded");
                //request.AddHeader("authorization", "lHAUV50X68RMCJbNZG7Yman1BDkruxqjWdKTg2eyosz3OcihFSAp3KHl6Wgk7yY1SCRbiLo8NxPvXfj0");
                //request.AddParameter("variables_values", otp + " is to register with jobbvin.Valid only 10 mins.");
                //request.AddParameter("route", "otp");
                //request.AddParameter("numbers", mobileNumbe);

                var client = new RestClient("https://www.fast2sms.com/dev/bulkV2");
                var request = new RestRequest("", Method.Post);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("authorization", "lHAUV50X68RMCJbNZG7Yman1BDkruxqjWdKTg2eyosz3OcihFSAp3KHl6Wgk7yY1SCRbiLo8NxPvXfj0");
                request.AddParameter("variables_values", otp);
                request.AddParameter("route", "otp");
                request.AddParameter("numbers", mobileNumbe);
                RestResponse response = client.Execute(request);
                return response;
            }
            catch (Exception ex)
            {
                RestResponse response = new RestResponse();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ErrorMessage = ex.Message;
                return response;
            }
        }

        public async static Task<RestResponse> SendLikeSMS(string mobileNumbe, pic_likes like)
        {
            try
            {
                var client = new RestClient("https://www.fast2sms.com/dev/bulkV2");
                var request = new RestRequest("", Method.Post);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("authorization", "lHAUV50X68RMCJbNZG7Yman1BDkruxqjWdKTg2eyosz3OcihFSAp3KHl6Wgk7yY1SCRbiLo8NxPvXfj0");
                request.AddParameter("variables_values", like.contact_no + " is liked user post #" + like.likes_product_id);
                request.AddParameter("route", "otp");
                request.AddParameter("numbers", like.likes_cus_mobile);
                RestResponse response = client.Execute(request, Method.Post);
                return response;
            }
            catch (Exception ex)
            {
                RestResponse response = new RestResponse();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ErrorMessage = ex.Message;
                return response;
            }
        }
    }
}
