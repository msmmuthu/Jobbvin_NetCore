using Jobbvin.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Jobbvin.Server.Utility
{
    public class FtpFileUpload<T> where T : class
    {
        private ILogger<T> _logger;

        private IConfiguration _configuration;
        public FtpFileUpload(ILogger<T> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task UploadFile(byte[] myData, string sourceFileName, bool pofilePic)
        {
            String ftpurl = _configuration.GetSection("ftpBaseUrl").Value;
            String ftpusername = _configuration.GetSection("DocFtpUser").Value; // e.g. username
            String ftppassword = _configuration.GetSection("DocFtpPwd").Value;

            if (pofilePic)
            {
                ftpusername = _configuration.GetSection("ProfileFtpUser").Value; 
                ftppassword = _configuration.GetSection("ProfileFtpPwd").Value;
            }
            try
            {
                //string filename = Path.GetFileName(source);
                string ftpfullpath = ftpurl + sourceFileName;
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
                ftp.Credentials = new NetworkCredential(ftpusername, ftppassword);

                ftp.KeepAlive = true;
                //ftp.UseBinary = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;

                ftp.UsePassive = true;
                ftp.UseBinary = true;
                ftp.ServicePoint.ConnectionLimit = myData.Length;
                ftp.EnableSsl = false;
                ftp.ContentLength = myData.Length;

                //HttpPostedFile myFile = Fupimage.PostedFile;
                //int nFileLen = myFile.ContentLength;
                //byte[] myData = new byte[nFileLen];


                Stream ftpstream = ftp.GetRequestStream();
                ftpstream.Write(myData, 0, myData.Length);
                ftpstream.Close();
                ftpstream.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
