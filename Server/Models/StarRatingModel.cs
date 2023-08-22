using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Models
{
    public class StarRatingModel
    {
        public int itemId { get; set; }
        public int userId { get; set; }
        public int ratingNumber { get; set; }

    }
}
