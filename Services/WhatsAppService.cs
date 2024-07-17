using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WhatsAppAPI.Services
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppService> _logger;

        public WhatsAppService(HttpClient httpClient, IConfiguration configuration, ILogger<WhatsAppService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public async Task<string> UploadImageAsync(string filePath)
        {
            var url = $"{_configuration["WhatsApp:BaseUrl"]}/v19.0/334658879722743/media";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["WhatsApp:AccessToken"]);

            var form = new MultipartFormDataContent();
            var fileStream = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));

            //Add Image file
            fileStream.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            form.Add(fileStream, "file", "image.jpg");
            form.Add(new StringContent("whatsapp"), "messaging_product");

            var response = await client.PostAsync(url, form);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = JObject.Parse(responseString);
                return jsonResponse["id"].ToString();
            }
            else
            {
                throw new Exception($"Failed to upload image: {responseString}");
            }
        }


        public async Task<string> UploadPDFAsync(string filePath)
        {
            var url = $"{_configuration["WhatsApp:BaseUrl"]}/v19.0/334658879722743/media";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["WhatsApp:AccessToken"]);

            var form = new MultipartFormDataContent();
            var fileStream = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
            fileStream.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            form.Add(fileStream, "file", "document.pdf");
            form.Add(new StringContent("whatsapp"), "messaging_product");

            var response = await client.PostAsync(url, form);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = JObject.Parse(responseString);
                return jsonResponse["id"].ToString();
            }
            else
            {
                throw new Exception($"Failed to upload PDF: {responseString}");
            }
        }

        public async Task<bool> StaticTemplateMessageService(string to, string templatename)
        {
            var url = $"{_configuration["WhatsApp:BaseUrl"]}/v19.0/334658879722743/messages";
            var payload = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "template",
                template = new
                {
                    name = templatename.Trim(), // Replace with your template name
                    language = new
                    {
                        code = "en_US" // Ensure this matches the template language
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"),

            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["WhatsApp:AccessToken"]);
            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();           
            _logger.LogInformation($"Response Status Code: {response.StatusCode}");
            _logger.LogInformation($"Response Content: {responseString}");

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendFeeMessage(string to, string templatename,string name,string from)
        {          
            var url = $"{_configuration["WhatsApp:BaseUrl"]}/v19.0/334658879722743/messages";
            var payload = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "template",
                template = new
                {
                    name = templatename.ToString(),
                    language = new
                    {
                        code = "en_US"
                    },
                    components = new object[]
                   {
                      
                        new {
                            type = "body",
                            parameters = new object[]
                            {
                                new {
                                    type = "text",
                                    text = name.Trim()
                                },
                                new
                                {
                                    type = "text",
                                    text = from.Trim()// Parameter 2
                                }
                            }
                        }
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"),

            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["WhatsApp:AccessToken"]);

            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            // var response = await _httpClient.SendAsync(request);

            //var responseString = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Response Status Code: {response.StatusCode}");
            _logger.LogInformation($"Response Content: {responseString}");

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }


    }



}

