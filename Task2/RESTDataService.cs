using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Task2
{
    internal class RESTDataService
    {
        private readonly HttpClient _httpClient;

        public RESTDataService(string bearer)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7242/api/Movie");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress);
            response.EnsureSuccessStatusCode();
            string content = response.Content.ReadAsStringAsync().Result;
            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movies;
        }

        public void NewToken(string bearer)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }
    }
}
