using Microsoft.AspNetCore.Components.Forms;
using System;

namespace Jobbvin.Client.Helpers
{
    public class FileUpload
    {
        public static async Task<string> GetFileBaseString(IBrowserFile _file1)
        {
            long maxFileSize = 1024L * 1024L * 1024L * 2L;
            using (MemoryStream ms = new MemoryStream())
            {
                var buffers = new byte[_file1.Size];
                //await _file1.OpenReadStream(maxFileSize).CopyToAsync(ms);
                await _file1.OpenReadStream(_file1.Size).ReadAsync(buffers);

                //var bytes = ms.ToArray();
                return Convert.ToBase64String(buffers);
            }
        }
    }
}