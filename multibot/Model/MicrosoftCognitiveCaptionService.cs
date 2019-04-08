using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace multibot.Model
{
    public class MicrosoftCognitiveCaptionService : ICaptionService
    {
        private static readonly string ApiKey = WebConfigurationManager.AppSettings["MicrosoftVisionApiKey"];

        private static readonly string ApiEndpoint = WebConfigurationManager.AppSettings["MicrosoftVisionApiEndpoint"];

        private static readonly VisualFeature[] VisualFeatures = { VisualFeature.Description };

        public async Task<string> GetCaptionAsync(Stream stream)
        {
            var client = new VisionServiceClient(ApiKey, ApiEndpoint);
            var result = await client.AnalyzeImageAsync(stream, VisualFeatures);
            return ProcessAnalysisResult(result);
        }

        public async Task<string> GetCaptionAsync(string url)
        {
            var client = new VisionServiceClient(ApiKey, ApiEndpoint);
            var result = await client.AnalyzeImageAsync(url, VisualFeatures);
            return ProcessAnalysisResult(result);
        }

        private static string ProcessAnalysisResult(AnalysisResult result)
        {
            string message = result?.Description?.Captions.FirstOrDefault()?.Text;

            return string.IsNullOrEmpty(message) ?
                        "Couldn't find a caption for this one" :
                        "I think it's " + message;
        }
    }
}