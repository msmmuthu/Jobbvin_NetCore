using Jobbvin.Server.Controllers;
using Jobbvin.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Jobbvin.Server.Utility
{
    public class Email
    {
        private ILogger<ProductsController> _logger;

        private IConfiguration _configuration;
        public Email(ILogger<ProductsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(pic_likes like)
        {
            try
            {
                //MailMessage mail = new MailMessage();
                //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                //mail.From = new MailAddress(_configuration.GetSection("EmailId").Value);
                //mail.To.Add(like.likes_cus_email);
                //mail.Subject = "Jobbvin - Someone likes your post #" + like.likes_product_id;

                //mail.IsBodyHtml = true;
                //string htmlBody;

                //htmlBody = "Hi " + like.likes_cus_name + ", " + like.likes_cus_email + " liked your post.";

                //mail.Body = htmlBody;

                //smtpClient.Port = 587;
                //smtpClient.Credentials = new System.Net.NetworkCredential(_configuration.GetSection("EmailId").Value, _configuration.GetSection("EmailPwd").Value);
                //smtpClient.EnableSsl = true;
                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtpClient.UseDefaultCredentials = true;

                //smtpClient.Send(mail);

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_configuration.GetSection("EmailId").Value, _configuration.GetSection("EmailPwd").Value),
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };

                smtpClient.Send(_configuration.GetSection("EmailId").Value, like.likes_cus_email, "Jobbvin - Someone likes your post #" + like.likes_product_id, "Hi " + like.likes_cus_name + ", " + like.likes_cus_email + " liked your post.");

                _logger.LogError("Success - mail on like" +  like.likes_product_id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending email on like"   + ex.ToString());
                return false;
            }
        }
    }
}