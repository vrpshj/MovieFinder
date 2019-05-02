using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MovieFinder.Controllers;
using MovieFinder.Models;
using MovieFinder.Utils;
using Newtonsoft.Json;

namespace MovieFinder.Services
{
    /// <summary>
    /// Movie Service Class
    /// </summary>
    public class MoviesService
    {
        private readonly ILogger _logger;
        private readonly List<HttpClientExtended> _clients;
        private readonly string _url;
        Dictionary<string, MoviesList> movieListdictionary;
        const int Maximum_Tries = 3;
        /// <summary>
        /// Constructor to initialise the logger using DI
        /// </summary>
        /// <param name="logger"></param>
        internal MoviesService(ILogger<MoviesController> logger,IProviderHTTPClient clients)
        {
            _logger = logger;
            _clients = clients.GetProviderHTTPClients();
            _url = "movies";
            movieListdictionary = new Dictionary<string, MoviesList>();
        }
        /// <summary>
        /// Get all available movies grouped by providers 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetAvailableMoviesGroupedByProvider(string searchText = "")
        {
            var response = new ApiResponse();

            var apiurl = $"{_url}";
            movieListdictionary.Clear();

            foreach (var client in _clients)
            {
                int attempts = 0;
                do
                {
                    if (attempts > 0)
                        Thread.Sleep(300);
                    var res = await client.GetAsync(apiurl);
                    if (res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<MoviesList>(content);
                        movieListdictionary.Add(client.ProviderName, obj);
                        break;
                    }
                    attempts++;
                } while (attempts < Maximum_Tries);


            }
            if (string.IsNullOrEmpty(searchText))
            {
                response.success = true;
                response.data = movieListdictionary;
            }
            else
            {
                var filteredmovieListdictionary = new Dictionary<string, MoviesList>();
                foreach (KeyValuePair<string, MoviesList> movieList in movieListdictionary)
                {
                    var list = (from mov in movieList.Value.Movies where mov.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1 select mov).ToList();
                    if(list.Count > 0)
                    {
                        MoviesList tempList = new MoviesList();
                        tempList.Movies = list;
                        filteredmovieListdictionary.Add(movieList.Key, tempList);
                    }
                }
                response.success = true;
                response.data = filteredmovieListdictionary;
            }
            return response;
        }
        /// <summary>
        /// Method to get movie cheapest price by name
        /// </summary>
        /// <param name="movieName"></param>
        /// <returns></returns>
        internal async Task<ApiResponse> GetMovieCheapestPriceByName(string movieName)
        {
            var response = new ApiResponse();
            movieListdictionary.Clear();
            var apiurl = $"{_url}";
            MoviePrice bestPrice = new MoviePrice();
            foreach (var client in _clients)
            {
                int attempts = 0;
                var movieFound = false;
                do
                {
                    if (attempts > 0)
                        Thread.Sleep(300);
                    var res = await client.GetAsync(apiurl);
                    if (res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<MoviesList>(content);
                        var mov = (from m in obj.Movies where m.Title == movieName select m).FirstOrDefault();

                        if(mov != null && !string.IsNullOrEmpty(mov.ID) )
                        {
                            var getmovieapiurl = $"movie/{mov.ID}";
                            int detailsRequestattempts = 0;
                            movieFound = true;
                            do
                            {
                                if (detailsRequestattempts > 0)
                                    Thread.Sleep(300);
                                var detailsResponse = await client.GetAsync(getmovieapiurl);
                                if (detailsResponse.IsSuccessStatusCode)
                                {
                                    var movieDetailsContent = await detailsResponse.Content.ReadAsStringAsync();
                                    var movieDetailssobj = JsonConvert.DeserializeObject<MovieDetails>(movieDetailsContent);


                                    if (bestPrice.CheapestPrice == null || bestPrice.CheapestPrice > movieDetailssobj.Price)
                                    {
                                        bestPrice.ID = movieDetailssobj.ID;
                                        bestPrice.Title = movieDetailssobj.Title;
                                        bestPrice.Provider = client.ProviderName;
                                        bestPrice.CheapestPrice = movieDetailssobj.Price;
                                    }
                                    break;
                                }
                                detailsRequestattempts++;
                            } while (detailsRequestattempts < Maximum_Tries);

                            if (movieFound)
                                break;

                        }
                    }
                    attempts++;
                } while (attempts < Maximum_Tries);
            }
            if (bestPrice.CheapestPrice == null)
            {
                response.success = false;
                response.AddError("Movie ID not found");
                return response;
            }
            response.success = true;
            response.data = bestPrice;
            return response;
        }
        /// <summary>
        /// method to get movie price by ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        internal async Task<ApiResponse> GetMoviePriceByID(string ID)
        {
            var response = new ApiResponse();

            var apiurl = $"movie/{ID}";
            MoviePrice bestPrice = new MoviePrice();
            var movieFound = false;

            foreach (var client in _clients)
            {
                int attempts = 0;
                do
                {
                    if (attempts > 0)
                        Thread.Sleep(300);
                    var res = await client.GetAsync(apiurl);
                    if (res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<MovieDetails>(content);
                        if(bestPrice.CheapestPrice == null || bestPrice.CheapestPrice > obj.Price)
                        {
                            bestPrice.ID = obj.ID;
                            bestPrice.Title = obj.Title;
                            bestPrice.Provider = client.ProviderName;
                            bestPrice.CheapestPrice = obj.Price;
                            movieFound = true;
                        }
                        break;
                    }
                    attempts++;
                } while (attempts < Maximum_Tries);

                if (movieFound)
                    break;

            }
            if (bestPrice.CheapestPrice == null)
            {
                response.success = false;
                response.AddError("Movie ID not found");
                return response;
            }
            response.success = true;
            response.data = bestPrice;
            return response;
        }
        /// <summary>
        /// Get movie details by movie ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        internal async Task<ApiResponse> GetMoviesByID(string ID)
        {
            var response = new ApiResponse();

            var apiurl = $"movie/{ID}";
            movieListdictionary.Clear();

            foreach (var client in _clients)
            {
                int attempts = 0;
                do
                {
                    if (attempts > 0)
                        Thread.Sleep(300);
                    var res = await client.GetAsync(apiurl);
                    if (res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<MovieDetails>(content);
                        response.success = true;
                        response.data = obj;
                        return response;
                    }
                    attempts++;
                } while (attempts < Maximum_Tries);


            }
            response.success = false;
            response.AddError("Movie ID not found");
            return response;




            //if (movieListdictionary != null && movieListdictionary.Count > 0)
            //{
            //    var provider = movieListdictionary.Where(ml => ml.Value.Movies.Any(m => String.Equals(m.ID, ID, StringComparison.OrdinalIgnoreCase))).FirstOrDefault().Key;
            //    if (!string.IsNullOrEmpty(provider))
            //    {
            //        var apiurl = $"movie/{ID}";
            //        var client = _clients.Where(c => c.ProviderName == provider).FirstOrDefault();
            //        int attempts = 0;
            //        do
            //        {
            //            if (attempts > 0)
            //                Thread.Sleep(300);
            //            var res = await client.GetAsync(apiurl);
            //            if (res.IsSuccessStatusCode)
            //            {
            //                var content = await res.Content.ReadAsStringAsync();
            //                var obj = JsonConvert.DeserializeObject<MovieDetails>(content);
            //                response.success = true;
            //                response.data = obj;
            //                return response;
            //            }
            //            attempts++;
            //        } while (attempts < Maximum_Tries);

            //    }

            //}


        }
        /// <summary>
        /// Get all available movies from the providers
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetAvailableMovies(string searchText = "")
        {
            var response = new ApiResponse();
            List<Movie> availableMovies = new List<Movie>();

            var apiurl = $"{_url}";

            foreach (var client in _clients)
            {
                int attempts = 0;
                do
                {
                    if (attempts > 0)
                        Thread.Sleep(300);
                    var res = await client.GetAsync(apiurl);
                    if (res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<MoviesList>(content);
                        foreach(Movie item in obj.Movies)
                        {
                            if (item.providers == null)
                                item.providers = new List<string>();
                            item.providers.Add(client.ProviderName);
                            if (availableMovies.Count == 0)
                            {
                                availableMovies.Add(item);
                            }
                            else
                            {
                                var existingMovie = (from m in availableMovies where m.Title == item.Title select m).FirstOrDefault();
                                if (existingMovie != null)
                                {
                                    existingMovie.providers.Add(client.ProviderName);
                                }
                                else
                                {
                                    availableMovies.Add(item);
                                }
                            }
                        }
                        break;
                    }
                    attempts++;
                } while (attempts < Maximum_Tries);


            }
            if (string.IsNullOrEmpty(searchText))
            {
                response.success = true;
                response.data = availableMovies;
            }
            else
            {

                var filterAvailableMovies = (from m in availableMovies where m.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1 select m).ToList();
                response.success = true;
                response.data = filterAvailableMovies;
            }
            return response;
        }
    }



}
