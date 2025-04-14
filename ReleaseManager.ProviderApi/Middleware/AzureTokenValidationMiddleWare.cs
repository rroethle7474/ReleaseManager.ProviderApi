﻿using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ReleaseManager.ProviderApi.Middleware
{
    public class AzureTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AzureTokenValidationMiddleware> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AzureTokenValidationMiddleware(
            RequestDelegate next,
            ILogger<AzureTokenValidationMiddleware> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("IdentityApi");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only check for provider-token header in appropriate endpoints
            if (context.Request.Path.StartsWithSegments("/api/v1/pipelines") ||
                context.Request.Path.StartsWithSegments("/api/v1/releases") ||
                context.Request.Path.StartsWithSegments("/api/v1/projects"))
            {
                if (!context.Request.Headers.TryGetValue("Provider-Token", out var token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "missing_token",
                        error_description = "Provider token is required"
                    });
                    return;
                }

                try
                {
                    // Check if token is valid by examining its claims
                    var tokenHandler = new JwtSecurityTokenHandler();
                    if (tokenHandler.CanReadToken(token))
                    {
                        var jwt = tokenHandler.ReadJwtToken(token);
                        var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp");

                        if (expClaim != null)
                        {
                            var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                            var expiresIn = exp - DateTimeOffset.UtcNow;

                            // If token expires in less than 5 minutes, try to refresh it
                            if (expiresIn.TotalMinutes < 5)
                            {
                                _logger.LogInformation("Token expires soon, attempting refresh");
                                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                                if (!string.IsNullOrEmpty(userId))
                                {
                                    var refreshResponse = await _httpClient.PostAsync(
                                        $"{_configuration["IdentityApi:BaseUrl"]}/api/CloudProvider/refresh-token",
                                        new StringContent(
                                            JsonSerializer.Serialize(new { UserId = userId }),
                                            Encoding.UTF8,
                                            "application/json"));

                                    if (refreshResponse.IsSuccessStatusCode)
                                    {
                                        var tokenResult = await refreshResponse.Content
                                            .ReadFromJsonAsync<TokenRefreshResponse>();

                                        if (tokenResult != null && !string.IsNullOrEmpty(tokenResult.Token))
                                        {
                                            // Replace token in request header for downstream handlers
                                            context.Request.Headers.Remove("Provider-Token");
                                            context.Request.Headers.Add("Provider-Token", tokenResult.Token);

                                            // Also add special header to convey to client that token was refreshed
                                            context.Response.OnStarting(() => {
                                                context.Response.Headers.Add("X-Token-Refreshed", "true");
                                                context.Response.Headers.Add("X-New-Token", tokenResult.Token);
                                                return Task.CompletedTask;
                                            });
                                        }
                                    }
                                    else if (refreshResponse.StatusCode == HttpStatusCode.Unauthorized)
                                    {
                                        // Token couldn't be refreshed, user needs to re-authenticate
                                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                        await context.Response.WriteAsJsonAsync(new
                                        {
                                            error = "invalid_token",
                                            error_description = "Token expired and could not be refreshed",
                                            requires_reauth = true
                                        });
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating token");
                    // Continue to the next middleware even on token validation error
                    // The actual API call will fail with a proper error message
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

    public class TokenRefreshResponse
    {
        public string Token { get; set; }
    }
}
