using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotaryPlatform.Application.Common.Interfaces;

namespace NotaryPlatform.Infrastructure.Services.Messaging;

/// <summary>
/// SMS sender using the Twilio Messaging REST API via HttpClient.
/// Implements ISmsSender (Application) — callers never reference this class directly.
///
/// LEARNING — HttpClient over Twilio SDK:
///   No Twilio NuGet package is included. HttpClient is used directly to show:
///   • How HTTP Basic Auth works (Base64 encode SID:Token, set Authorization header).
///   • That any SMS provider (Vonage, MessageBird, local gateway) can be swapped in
///     by changing only the settings and this class — the ISmsSender contract stays.
///
/// LEARNING — Twilio REST API essentials:
///   URL  : POST /2010-04-01/Accounts/{SID}/Messages.json
///   Auth : HTTP Basic — Authorization: Basic Base64(AccountSid + ":" + AuthToken)
///   Body : application/x-www-form-urlencoded  (NOT JSON)
///   Phone: E.164 format — "+84901234567", not "0901234567"
/// </summary>
public sealed class SmsSender : ISmsSender
{
    private readonly HttpClient _httpClient;
    private readonly string _from;
    private readonly ILogger<SmsSender> _logger;

    public SmsSender(
        IHttpClientFactory httpClientFactory,
        IOptions<SmsSettings> options,
        ILogger<SmsSender> logger)
    {
        var settings = options.Value;
        _from = settings.From;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(nameof(SmsSender));

        var credentials = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{settings.AccountSid}:{settings.AuthToken}"));

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        _httpClient.BaseAddress =
            new Uri($"https://api.twilio.com/2010-04-01/Accounts/{settings.AccountSid}/");
    }

    public async Task SendAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        var body = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("To",   phoneNumber),
            new KeyValuePair<string, string>("From", _from),
            new KeyValuePair<string, string>("Body", message),
        ]);

        var response = await _httpClient.PostAsync("Messages.json", body, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "SMS send failed. Status: {Status}, Phone: {Phone}, Body: {Error}",
                response.StatusCode, phoneNumber, error);
            response.EnsureSuccessStatusCode();
        }

        _logger.LogInformation("SMS sent to {Phone}", phoneNumber);
    }
}
