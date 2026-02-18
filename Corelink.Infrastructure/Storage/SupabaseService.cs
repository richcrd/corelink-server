using System.Net.Http.Headers;
using Corelink.Application.Contracts.Storage;
using Microsoft.Extensions.Configuration;
using Supabase.Storage;

namespace Corelink.Infrastructure.Storage;

public class SupabaseService : IFileService
{
    private readonly HttpClient _httpClient;
    private readonly string _bucket;
    private readonly string _baseUrl;
    private readonly string _serviceKey;

    public SupabaseService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        _baseUrl = configuration["Supabase:Url"]!;
        _serviceKey = configuration["Supabase:ServiceKey"]!;
        _bucket = configuration["Supabase:Bucket"]!;
    }
    
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_baseUrl}/storage/v1/object/corelink-images/{safeFileName}");

        request.Headers.Add("Authorization", $"Bearer {_serviceKey}");
        request.Headers.Add("apikey", _serviceKey);

        request.Content = new StreamContent(fileStream);
        request.Content.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);

        var response = await _httpClient.SendAsync(request);

        var errorBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Supabase error: {errorBody}");

        return $"{_baseUrl}/storage/v1/object/public/corelink-images/{safeFileName}";
    }
}