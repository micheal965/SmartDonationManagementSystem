using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.API.Middlewares;

public class LogoutMiddleware
{
    private readonly RequestDelegate _next;
    public LogoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var jwtToken = authHeader["Bearer ".Length..].Trim();

            if (!string.IsNullOrEmpty(jwtToken))
            {
                var authService = context.RequestServices.GetRequiredService<IAuthServices>();

                if (await authService.IsTokenBlacklistedAsync(jwtToken))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = Result<string>.Unauthorized("Token is invalid or expired.");

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(response)
                    );
                    return;
                }
            }
        }
        await _next(context);
    }
}
