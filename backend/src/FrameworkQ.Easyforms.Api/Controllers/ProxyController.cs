namespace FrameworkQ.Easyforms.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

/// <summary>
/// API controller for proxying external data fetch requests
/// Prevents CORS issues and enables server-side auth injection
/// </summary>
[ApiController]
[Route("v1/proxy")]
public class ProxyController : ControllerBase
{
    private readonly ILogger<ProxyController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private static readonly Dictionary<string, CachedResponse> _cache = new();

    public ProxyController(ILogger<ProxyController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Proxy external API calls
    /// GET /v1/proxy/fetch
    /// </summary>
    [HttpGet("fetch")]
    public async Task<IActionResult> FetchExternalData([FromQuery] string endpoint, [FromQuery] string method = "GET")
    {
        _logger.LogInformation("Proxying fetch request: {Method} {Endpoint}", method, endpoint);

        try
        {
            // Validate endpoint is in allowlist
            if (!IsEndpointAllowed(endpoint))
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "ENDPOINT_NOT_ALLOWED",
                        message = "Endpoint is not in the allowed list"
                    }
                });
            }

            // Check cache
            var cacheKey = $"{method}:{endpoint}";
            if (_cache.TryGetValue(cacheKey, out var cached))
            {
                if (cached.ExpiresAt > DateTime.UtcNow)
                {
                    _logger.LogInformation("Returning cached response for {Endpoint}", endpoint);
                    return Ok(cached.Data);
                }
                else
                {
                    _cache.Remove(cacheKey);
                }
            }

            // Make external request
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            // TODO: Inject authentication headers from configuration
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            if (method.ToUpper() == "GET")
            {
                response = await client.GetAsync(endpoint);
            }
            else if (method.ToUpper() == "POST")
            {
                response = await client.PostAsync(endpoint, null);
            }
            else
            {
                return BadRequest(new { error = new { code = "INVALID_METHOD", message = "Only GET and POST supported" } });
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("External API returned {StatusCode} for {Endpoint}", response.StatusCode, endpoint);
                return StatusCode((int)response.StatusCode, new
                {
                    error = new
                    {
                        code = "EXTERNAL_API_ERROR",
                        message = $"External API returned {response.StatusCode}"
                    }
                });
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<object>(content);

            // Cache response (10 minute default TTL)
            _cache[cacheKey] = new CachedResponse
            {
                Data = data,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            return Ok(data);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch from external endpoint");
            return StatusCode(502, new
            {
                error = new
                {
                    code = "EXTERNAL_API_ERROR",
                    message = ex.Message,
                    correlationId = HttpContext.TraceIdentifier
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proxy fetch failed");
            return BadRequest(new
            {
                error = new
                {
                    code = "PROXY_ERROR",
                    message = ex.Message,
                    correlationId = HttpContext.TraceIdentifier
                }
            });
        }
    }

    /// <summary>
    /// Check if endpoint is in allowlist
    /// </summary>
    private bool IsEndpointAllowed(string endpoint)
    {
        var allowedEndpoints = _configuration.GetSection("ExternalApi:AllowedEndpoints").Get<string[]>()
            ?? Array.Empty<string>();

        if (allowedEndpoints.Length == 0)
        {
            // If no allowlist configured, allow all (not recommended for production)
            _logger.LogWarning("No endpoint allowlist configured - allowing all endpoints");
            return true;
        }

        foreach (var pattern in allowedEndpoints)
        {
            if (pattern.EndsWith("/*"))
            {
                var baseUrl = pattern.Substring(0, pattern.Length - 2);
                if (endpoint.StartsWith(baseUrl))
                {
                    return true;
                }
            }
            else if (endpoint == pattern)
            {
                return true;
            }
        }

        return false;
    }

    private class CachedResponse
    {
        public object? Data { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
