using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UserAuth.Utility
{
    public class HttpClientHelper
    {
        private HttpClient _client;
        private readonly string _apiKey;
        public HttpClientHelper(IConfiguration configuration)
        {
            _client = new HttpClient();
            _apiKey = ConfigHelper.GetAPIKey(configuration);
            _client.DefaultRequestHeaders.Add("Request-Token", _apiKey);
        }

        public async Task<T> GetAsync<T>(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    T result = JsonConvert.DeserializeObject<T>(responseBody);
                    return result;
                }
                else
                {
                    throw new HttpRequestException($"Failed to call the API. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }

        public async Task<T> PostAsync<T>(string apiUrl, Dictionary<string, string> formData)
        {
            try
            {
                var encodedFormData = new FormUrlEncodedContent(formData);

                // Send POST request
                HttpResponseMessage response = await _client.PostAsync(apiUrl, encodedFormData);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    T result = JsonConvert.DeserializeObject<T>(responseBody);
                    return result;
                }
                else
                {
                    throw new HttpRequestException($"Failed to call the API. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }

        ~HttpClientHelper()
        {
            _client.Dispose();
        }
    }
}
