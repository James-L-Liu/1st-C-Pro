using Microsoft.AspNetCore.Authentication;
using A2.Data;
using Microsoft.Extensions.Options;
using System.Text.Encodings;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;


namespace A2.Handler
{
    public class A2AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private readonly IA2Repo _repository;
        public A2AuthHandler(
            IA2Repo repository,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _repository = repository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Response.Headers.Add("WWW-Authenticate", "Basic");
                return AuthenticateResult.Fail("Authorization header not found, be careful ^v^");
            }
            else
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credetials = Encoding.UTF8.GetString(credentialBytes).Split(":");
                var username = credetials[0];
                var password = credetials[1];
                if (_repository.ValidLogin(username, password))
                {
                    var claims = new[] { new Claim("normalUser", username) };
                    ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                else

                {
                    if (_repository.IsOrganizor(username, password))
                    {
                        var claims = new[] { new Claim("Organizor", username) };
                        ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return AuthenticateResult.Success(ticket);
                    }
                    else
                    {
                        Response.Headers.Add("WWW-Authenticate", "Basic");
                        return AuthenticateResult.Fail("Username and password does not match.");
                    }
                }
                    //return AuthenticateResult.Fail("Username and password do not match.");
            }
        }
    }
}