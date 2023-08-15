using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class ApiResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public string ValidationName { get; set; }
    }
}
