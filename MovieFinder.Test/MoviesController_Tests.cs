using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieFinder.Controllers;
using MovieFinder.Models;
using MovieFinder.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieFinder.Test
{
    [TestClass]
    public class MoviesController_Tests
    {

        private readonly HttpClient _client;
        public MoviesController_Tests()
        {
            _client = TestUtils.CreateHttpClient();
        }
        /// <summary>
        /// Test to get all available movies from all providers
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAvailableMoviesTest()
        {
            string _url = "api/movies/GetAvailableMovies";
            var response = await _client.GetAsync($"{_url}");
            var result = await response.Content.ReadAsJsonAsync<ApiResponse<List<Movie>>>();
            List<Movie> data = JsonConvert.DeserializeObject<List<Movie>>(result.data.ToString());
            Assert.IsTrue(result.success, "Failed to get movies from providers");
            Assert.IsNotNull(result.data);
            Assert.IsTrue(result.errors.Count == 0, "Errors occured while generating movie list");
            Assert.IsTrue(data.Count >= 0, "Unable to get any movies from the providers");
        }
        /// <summary>
        /// Test to get all available movies from all providers
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAvailableMoviesByPrviderTest()
        {
            string _url = "api/movies/GetAvailableMoviesByProviders";
            var response = await _client.GetAsync($"{_url}");
            var result = await response.Content.ReadAsJsonAsync<ApiResponse<Dictionary<string, MoviesList>>>();
            Dictionary<string, MoviesList> data = JsonConvert.DeserializeObject<Dictionary<string, MoviesList>>(result.data.ToString());
            Assert.IsTrue(result.success, "Failed to get movies from providers");
            Assert.IsNotNull(result.data);
            Assert.IsTrue(result.errors.Count == 0, "Errors occured while generating movie list");
            Assert.IsTrue(data.Count >= 0, "Unable to get any movies from the providers");
        }
        /// <summary>
        /// Test to get available movies by name
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow("Star Wars: Episode II - Attack of the Clones",true)]
        [DataRow("zzzzzzzzzz",false)]
        public async Task GetAvailableMoviesByNameTest(string searchText,bool available)
        {
            string _url = $"api/movies/GetAvailableMovies?SearchText={searchText}";
            var response = await _client.GetAsync($"{_url}");
            var result = await response.Content.ReadAsJsonAsync<ApiResponse<Dictionary<string, MoviesList>>>();
            Dictionary<string, MoviesList> data = JsonConvert.DeserializeObject<Dictionary<string, MoviesList>>(result.data.ToString());
            Assert.IsTrue(result.success, "Failed to get movies from providers");
            Assert.IsNotNull(result.data);
            Assert.IsTrue(result.errors.Count == 0, "Errors occured while generating movie list");
            Assert.AreEqual(data.Count > 0, available);
        }
        /// <summary>
        /// Testing Get movies details by ID
        /// Testing various scenarios
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="errorCount"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow("fw0086190",0,true)]
        [DataRow("xyzwerfsdfs",1,false)]
        public async Task GetMovieDetailsByID(string ID,int errorCount,bool IsSuccess)
        {
            string _url = $"api/movies/GetMovieDetailsByID?ID={ID}";
            var response = await _client.GetAsync($"{_url}");
            var result = await response.Content.ReadAsJsonAsync<ApiResponse<MovieDetails>>();
            Assert.IsTrue(result.success == IsSuccess, "Failed to get movie details from providers");
            Assert.IsTrue(result.errors.Count == errorCount, "Errors occured while generating movie list");
        }
        /// <summary>
        /// Testing Get Movie Cheapest price for Movie by Name
        /// Testing various scenarios
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="bestPrice"></param>
        /// <param name="IsSuccess"></param>
        /// <param name="errorCount"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow("Star Wars: Episode I - The Phantom Menace", 900.5, true, 0)]
        [DataRow("Star Wars: Episode II - Attack of the Clones", 12.5, true, 0)]
        [DataRow("Star Wars: Episode III - Revenge of the Sith", 125.5, true, 0)]
        [DataRow("Star Wars: The Force Awakens", 129.5, true, 0)]
        [DataRow("Avengers",0.0, false,1)]
        public async Task GetMovieCheapestPriceByName(string Name,double bestPrice, bool IsSuccess,int errorCount)
        {
            string _url = $"api/movies/GetMovieCheapestPriceByName?Name={Name}";
            var response = await _client.GetAsync($"{_url}");
            var result = await response.Content.ReadAsJsonAsync<ApiResponse<MoviePrice>>();

            Assert.IsTrue(result.success == IsSuccess, "Failed to get movie details from providers");
            if(result.success)
            {
                var obj = JsonConvert.DeserializeObject<MoviePrice>(result.data.ToString());
                
                Assert.IsTrue(obj.CheapestPrice == bestPrice, "Failed to get movie details from providers");
            }
            Assert.IsTrue(result.errors.Count == errorCount, "Errors occured while generating movie list");
        }
    }
}
