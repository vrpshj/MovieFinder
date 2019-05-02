using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieFinder.Models
{
    /// <summary>
    /// Model class to store list of movies from providers
    /// </summary>
    public class MoviesList
    {
        public List<Movie> Movies { get; set; }
    }
}
