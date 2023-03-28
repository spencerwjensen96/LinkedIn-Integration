using System.Net.Http.Headers;
using System.Text.Json;
using Emmersion.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using HttpClient = Emmersion.Http.HttpClient;
using HttpHeaders = Emmersion.Http.HttpHeaders;
using HttpMethod = Emmersion.Http.HttpMethod;
using HttpRequest = Emmersion.Http.HttpRequest;

namespace linkedIn.Controllers;

[ApiController]
[Route("linkedin")]
public class LinkedInController : ControllerBase
{
    [HttpGet("certificate")]
    public ActionResult Get()
    {
        Console.WriteLine("Getting Certificate");
        var certificate = GetCertificate();
        return File(certificate.FileStream, certificate.ContentType, certificate.FileName);
    }

    [HttpPost]
    public async Task<ActionResult> Post()
    {
        Console.WriteLine("Posting To Linked In");
        var settings = new Settings
        {
            ClientId = "869u9fs4dk3owk",
            ClientSecret = "Zynz84hIxz4IsoOn",
            ReturnUri = "https://localhost:44435/post-on-linked-in",
            Scopes = "r_liteprofile%20r_emailaddress%20w_member_social",
            AntiForgeryState = Guid.NewGuid().ToString()
        };
        var accessTokenUrl = await GetAuthCode(settings);
        var response = new PostOnLinkedInResponse
        {
            Url = accessTokenUrl,
        };
        return Ok(response);
    }
    
    [HttpPost("post")]
    public async Task<ActionResult> MakePostOnLinkedIn([FromQuery] string code, [FromBody] PostOnLinkedInRequest text)
    {
        var settings = new Settings
        {
            ClientId = "869u9fs4dk3owk",
            ClientSecret = "Zynz84hIxz4IsoOn",
            ReturnUri = "https://localhost:44435/post-on-linked-in",
        };
        var accessToken = await GetAccessToken(settings, code);
        var personId = await GetPersonId(accessToken);
        //var postId = await PostSimpleText(personId, accessToken, text.Text);
        var result = await PostCertificate(personId, accessToken, text.Text);
        var post = new Post
        {
            PostId = result.PostId,
            AccessToken = result.AccessToken
        };
        return Ok(post);
    }
    [HttpPost("post-text")]
    public async Task<ActionResult> PostTextOnLinkedIn([FromQuery] string code, [FromBody] PostOnLinkedInRequest text)
    {
        var settings = new Settings
        {
            ClientId = "869u9fs4dk3owk",
            ClientSecret = "Zynz84hIxz4IsoOn",
            ReturnUri = "https://localhost:44435/post-on-linked-in",
        };
        var accessToken = await GetAccessToken(settings, code);
        var personId = await GetPersonId(accessToken);
        var postId = await PostSimpleText(personId, accessToken, text.Text);
        //var postId = await PostCertificate(personId, accessToken, text.Text);
        var post = new Post
        {
            PostId = postId
        };
        return Ok();
    }

    private async Task<CertReturnObject> PostCertificate(string? personId, string? accessToken, string text)
    {
        var url = "https://api.linkedin.com/v2/assets?action=registerUpload";
        var client = new HttpClient();
        var request = new HttpRequest
        {
            Url = url,
            Method = HttpMethod.POST,
            Body = @"{""registerUploadRequest"": {""recipes"": [""urn:li:digitalmediaRecipe:feedshare-image""],
            ""owner"": ""urn:li:person:" + personId + @""",""serviceRelationships"": [{""relationshipType"": ""OWNER"",
            ""identifier"": ""urn:li:userGeneratedContent""}],""supportedUploadMechanism"":[""SYNCHRONOUS_UPLOAD""
            ]}}",
            Headers = new HttpHeaders().Add("Authorization", "Bearer " + accessToken)
        };
        var result = await client.ExecuteAsync(request);
        
        var uploadUrl = JObject.Parse(result.Body)["value"]?
            ["uploadMechanism"]?
            ["com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest"]?
            ["uploadUrl"]?.ToString();
        var assetId = JObject.Parse(result.Body)["value"]?["asset"]?.ToString();
        
        var filePath = "example.jpg";
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        using var uploadClient = new System.Net.Http.HttpClient();
        var uploadRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Put, uploadUrl);
        uploadRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
        uploadRequest.Content = new ByteArrayContent(fileBytes);
        uploadRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
        var uploadResponse = await uploadClient.SendAsync(uploadRequest);

        var postRequest = new HttpRequest
        {
            Method = HttpMethod.POST,
            Url = "https://api.linkedin.com/v2/ugcPosts",
            Body = @"{""author"": ""urn:li:person:" + personId + @""",""lifecycleState"": ""PUBLISHED"",""specificContent"": {
            ""com.linkedin.ugc.ShareContent"": {""shareCommentary"": {""text"": """ + text + @"""},""shareMediaCategory"": ""IMAGE"",
            ""media"": [{""status"": ""READY"",""description"": {""text"": ""Test Image Upload""},
            ""media"": """ + assetId + @""",""title"": {""text"": ""This is an API test!""}}]}},""visibility"": {
            ""com.linkedin.ugc.MemberNetworkVisibility"": ""CONNECTIONS""}}",
            Headers = new HttpHeaders().Add("Authorization", "Bearer " + accessToken)
        };
        postRequest.Headers.Add("X-Restli-Protocol-Version", "2.0.0");
        var finalResult = await client.ExecuteAsync(postRequest);
        var finalId = JObject.Parse(finalResult.Body)["id"]?.ToString();
        return new CertReturnObject{ AccessToken = accessToken, PostId = finalId };
    }

    public static byte[] streamToByteArray(Stream input)
    {
        MemoryStream ms = new MemoryStream();
        input.CopyTo(ms);
        return ms.ToArray();
    }
    
    private async Task<string?> PostSimpleText(string? personId, string? accessToken, string? text)
    {
        var url = "https://api.linkedin.com/v2/ugcPosts";
        var client = new HttpClient();
        var request = new HttpRequest
        {
            Url = url,
            Method = HttpMethod.POST,
            Body = @"{""author"": ""urn:li:person:" + personId + @""",""lifecycleState"": ""PUBLISHED"",
""specificContent"": {""com.linkedin.ugc.ShareContent"": {""shareCommentary"": {""text"": """ + text + @"""},
""shareMediaCategory"": ""NONE""}},""visibility"": {""com.linkedin.ugc.MemberNetworkVisibility"": ""CONNECTIONS""}}",
            Headers = new HttpHeaders().Add("Authorization", "Bearer " + accessToken)
        };
        var result = await client.ExecuteAsync(request);
        
        var postId = JObject.Parse(result.Body)["id"];

        return postId?.ToString();
    }

    private async Task<string?> GetPersonId(string? accessToken)
    {
        var url = "https://api.linkedin.com/v2/me";
        var client = new HttpClient();
        var request = new HttpRequest
        {
            Url = url,
            Method = HttpMethod.GET,
            Headers = new HttpHeaders().Add("Authorization", "Bearer " + accessToken)
        };
        var result = await client.ExecuteAsync(request);
        var personId = JObject.Parse(result.Body)["id"];
        return personId?.ToString();
    }

    private async Task<string?> GetAccessToken(Settings settings, string code)
    {
        var baseUrl = "https://www.linkedin.com/oauth/v2/accessToken";
        var url = baseUrl + 
                  $"?grant_type=authorization_code" +
                  $"&client_id={settings.ClientId}" +
                  $"&client_secret={settings.ClientSecret}" +
                  $"&code={code}" +
                  $"&redirect_uri={settings.ReturnUri}";
        var client = new HttpClient();
        var request = new HttpRequest
        {
            Url = url,
            Method = HttpMethod.POST,
            Body = "{}",
            Headers = new HttpHeaders().Add("Content-Type", "application/x-www-form-urlencoded")
        };
        var result = await client.ExecuteAsync(request);

        var accessToken = JObject.Parse(result.Body)["access_token"];

        return accessToken?.ToString();
    }

    private async Task<string> GetAuthCode(Settings settings)
    {
        var baseUrl = "https://www.linkedin.com/oauth/v2/authorization";
        var url = baseUrl + $"?response_type=code" +
                  $"&client_id={settings.ClientId}" +
                  $"&redirect_uri={settings.ReturnUri}" +
                  $"&state={settings.AntiForgeryState}" +
                  $"&scope={settings.Scopes}";
        return url;
    }

    private FileResponse GetCertificate()
    {
        var result = new FileResponse
        {
            ContentType = "application/pdf",
            FileName = "example.pdf",
            FileStream = new FileStream("example.pdf", new FileStreamOptions
            {
                Access = FileAccess.Read
            })
        };
        return result;
    }
}

internal class CertReturnObject
{
    public string PostId { get; set; }
    public string AccessToken { get; set; }
}

public class Post
{
    public string? PostId { get; set; }
    public string AccessToken { get; set; }
}

public class Settings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string ReturnUri { get; set; }
    public string Scopes { get; set; }
    public string AntiForgeryState { get; set; }
}

public class PostOnLinkedInRequest
{
    public string Text { get; set; }
}

public class PostOnLinkedInResponse
{
    public string Url { get; set; }
}

public class FileResponse
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public Stream FileStream { get; set; }
}