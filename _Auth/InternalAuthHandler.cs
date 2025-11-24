using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class InternalAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string INTERNAL_KEY_HEADER = "X-Internal-Key";
    private const string INTERNAL_SECRET = "SERVICE-SECRET-KEY-123";

    public InternalAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            Console.WriteLine("🔐 InternalAuthHandler: Checking for internal key...");

            // Check if the request has the internal key header
            if (!Request.Headers.TryGetValue(INTERNAL_KEY_HEADER, out var internalKey))
            {
                Console.WriteLine("🔐 InternalAuthHandler: No X-Internal-Key header found");
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            Console.WriteLine($"🔐 InternalAuthHandler: Found X-Internal-Key: {internalKey}");

            // Validate the internal key
            if (string.IsNullOrEmpty(internalKey) || internalKey != INTERNAL_SECRET)
            {
                Console.WriteLine("🔐 InternalAuthHandler: Invalid internal key");
                return Task.FromResult(AuthenticateResult.Fail("Invalid internal service key"));
            }

            // Create claims identity for internal service
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "INTERNAL_SERVICE"),
                new Claim(ClaimTypes.NameIdentifier, "INTERNAL_SERVICE"),
                new Claim(ClaimTypes.Role, "InternalService"),
                new Claim("AuthenticationType", "Internal"),
                new Claim("IsInternalService", "true"),
                new Claim("ServiceName", "NotificationService")
            };

            var identity = new ClaimsIdentity(claims, "Internal");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Internal");

            Console.WriteLine("🔐 InternalAuthHandler: Authentication successful for internal service");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔐 InternalAuthHandler: Error - {ex.Message}");
            return Task.FromResult(AuthenticateResult.Fail($"Internal authentication failed: {ex.Message}"));
        }
    }
}