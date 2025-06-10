namespace ImageCaptioningProject.Models
{
    public class ImageCaptionViewModel
    {
        public IFormFile? ImageFile { get; set; } // The uploaded file (optional for redisplay)
        public string? ImageBase64 { get; set; } // To display the uploaded image on the page
        public CaptionResponse? Captions { get; set; } // The results from the captioning service
        public string? ErrorMessage { get; set; } // For displaying errors to the user
    }
}
