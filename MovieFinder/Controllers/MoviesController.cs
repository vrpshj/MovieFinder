using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieFinder.Models;
using MovieFinder.Services;
using MovieFinder.Utils;

namespace MovieFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IProviderHTTPClient _clients;
        private MoviesService moviesService;
        /// <summary>
        /// Initialise Movies Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="clients"></param>
        public MoviesController(ILogger<MoviesController> logger, IProviderHTTPClient clients)
        {
            _clients = clients;
            _logger = logger;
            moviesService = new MoviesService(logger, clients);
        }
        /// <summary>
        /// Method to get all avilable movies from all providers
        /// </summary>
        /// <param name="SearchText"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAvailableMoviesByProviders(string SearchText = "")
        {
            var response = new ApiResponse();
            try
            {
                response = await moviesService.GetAvailableMoviesGroupedByProvider(SearchText);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"failed to get available movies list");
                return StatusCode((int)HttpStatusCode.InternalServerError, response.AddServerError());
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAvailableMovies(string SearchText = "")
        {
            var response = new ApiResponse();
            try
            {
                response = await moviesService.GetAvailableMovies(SearchText);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"failed to get available movies list");
                return StatusCode((int)HttpStatusCode.InternalServerError, response.AddServerError());
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
        /// <summary>
        /// Method to get Movie details by ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMovieDetailsByID(string ID)
        {
            var response = new ApiResponse();
            try
            {
                response = await moviesService.GetMoviesByID(ID);
                if(response.data == null)
                    return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"failed to get movies details");
                return StatusCode((int)HttpStatusCode.InternalServerError, response.AddServerError());
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
        /// <summary>
        /// Method to get movie  price from providers
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMoviePriceByID(string ID)
        {
            var response = new ApiResponse();
            try
            {
                response = await moviesService.GetMoviePriceByID(ID);
                if (response.data == null)
                    return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"failed to get movie Price");
                return StatusCode((int)HttpStatusCode.InternalServerError, response.AddServerError());
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
        /// <summary>
        /// Method to get movies cheapest price from all providers by movie name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMovieCheapestPriceByName(string Name)
        {
            var response = new ApiResponse();
            try
            {
                response = await moviesService.GetMovieCheapestPriceByName(Name);
                if (response.data == null)
                    return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"failed to get movie Price");
                return StatusCode((int)HttpStatusCode.InternalServerError, response.AddServerError());
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
    }