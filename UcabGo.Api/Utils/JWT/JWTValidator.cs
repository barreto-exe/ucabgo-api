using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using UcabGo.Application.Utils;

namespace UcabGo.Api.Utils.JWT
{
    public class JWTValidator
    {
        public IEnumerable<Claim> Claims { get; set; }
        public bool IsValid { get; set; } = false;

        public JWTValidator(HttpRequest request)
        {
            // Check if we have a header
            if (!request.Headers.ContainsKey("Authorization"))
            {
                return;
            }

            // Check if the value is empty
            string authorizationHeader = request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return;
            }

            //Check token and signature
            try
            {
                if (authorizationHeader.StartsWith("Bearer"))
                {
                    authorizationHeader = authorizationHeader.Substring(7);
                }

                authorizationHeader.ValidateToken(out List<Claim> claims);
                Claims = claims;
            }
            catch
            {
                return;
            }

            //Everything OK
            IsValid = true;
        }
    }
}