using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieFinder.Models
{
    /// <summary>
    /// model class to store movie price details
    /// </summary>
    public class MoviePrice
    {
        public string Title { get; set; }
        public string ID { get; set; }
        public string Provider { get; set; }
        public double? CheapestPrice { get; set; }
    }
}
