using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers; 
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; 
using System.Text.Json;
using ImageCaptioningProject.Models; 

namespace ImageCaptioningProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageCaptionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _captioningServiceUrl;
        private readonly ILogger<ImageCaptionController> _logger; 

        public ImageCaptionController(HttpClient httpClient, IConfiguration configuration, ILogger<ImageCaptionController> logger)
        {
            _httpClient = httpClient; 
            _configuration = configuration;
            _captioningServiceUrl = _configuration["CAPTIONING_SERVICE_URL"] ??
                                    _configuration.GetValue<string>("ServiceUrls:CaptioningService") ??
                                    "http://localhost:8000"; 
            _logger = logger; 

        }
        [HttpGet]
        [Route("")] 
        public IActionResult Index()
        {
            return View(new ImageCaptionViewModel()); 
        }


        [HttpPost]
        [Route("describe")] 
        public async Task<IActionResult> DescribeImage(IFormFile file)
        {
            var viewModel = new ImageCaptionViewModel();

            if (file == null || file.Length == 0)
            {
                viewModel.ErrorMessage = "No image uploaded. Please select an image file.";
                return View("Index", viewModel);
            }

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                viewModel.ImageBase64 = Convert.ToBase64String(ms.ToArray());
            }

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var imageContent = new StreamContent(stream);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(imageContent, "image", file.FileName);

            try
            {
                var response = await _httpClient.PostAsync($"{_captioningServiceUrl}/caption", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Raw JSON Response from FastAPI: {jsonResponse}"); 

                var captionResult = JsonSerializer.Deserialize<CaptionResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
                });
                viewModel.Captions = captionResult;
                return View("Index", viewModel);
            }
            catch (HttpRequestException e)
            {
                viewModel.ErrorMessage = $"Error communicating with captioning service: {e.Message}";
                if (e.StatusCode.HasValue)
                {
                    viewModel.ErrorMessage += $" (Status: {(int)e.StatusCode.Value})";
                }
                _logger.LogError(e, $"HttpRequestException: {e.Message}"); 
                return View("Index", viewModel);
            }
            catch (JsonException e)
            {
                viewModel.ErrorMessage = $"Error processing captioning service response: {e.Message}. Response might be malformed.";
                _logger.LogError(e, $"JsonException: {e.Message}"); 
                return View("Index", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorMessage = $"An unexpected error occurred: {e.Message}";
                _logger.LogError(e, $"General Exception: {e.Message}"); 
                return View("Index", viewModel);
            }
        }
    }
}
