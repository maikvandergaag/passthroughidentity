using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Linq;
using System.Security.Claims;

namespace Msft.Gab.Api {
    public static class AuthenticationHelper {
        public static AuthenticationResult GetAuthenticationResult(IHttpContextAccessor httpContextAccessor, AuthenticationSettings authSettings) {

            AuthenticationResult retVal = null;

            UserAssertion userInfo = GetUserAssertion(httpContextAccessor);
            ClientCredential clientCred = new ClientCredential(authSettings.ClientId, authSettings.ClientSecret);
            AuthenticationContext authContext = new AuthenticationContext(authSettings.AadInstance + authSettings.Domain);

            bool retry = false;
            int retryCount = 0;

            do {
                retry = false;
                try {
                    retVal = authContext.AcquireTokenAsync(authSettings.AzureSqlResource, clientCred, userInfo).Result;

                } catch (AdalException ex) {
                    if (ex.ErrorCode == "temporarily_unavailable") {
                        retry = true;
                        retryCount++;
                    }
                }
            } while ((retry == true) && (retryCount < 1));

            return retVal;
        }

        private static UserAssertion GetUserAssertion(IHttpContextAccessor httpContextAccessor) {
            UserAssertion retVal = null;

            string accessToken = httpContextAccessor.HttpContext.Request.Headers["Authorization"][0];
            string userAccessToken = accessToken.Substring(accessToken.LastIndexOf(' ')).Trim();

            Claim upn = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");

            if (upn != null) {
                retVal = new UserAssertion(userAccessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer", upn.Value);
            }

            return retVal;
        }
    }
}