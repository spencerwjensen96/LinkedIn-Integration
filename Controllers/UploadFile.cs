using System.Net.Http.Headers;

namespace linkedIn.Controllers;

public class UploadFile
{
    public static async Task<HttpResponseMessage> UploadImageToLinkedIn(string postingURL, string accessToken, byte[] imgBytes)
    {
        using (HttpClient client = new HttpClient())
        {
            var fileContent = new ByteArrayContent(imgBytes);
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), postingURL))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + accessToken);
                request.Headers.TryAddWithoutValidation("Content-Type", "application/pdf");
                request.Content = fileContent;
                return await client.SendAsync(request);
            }
        }
    }
}