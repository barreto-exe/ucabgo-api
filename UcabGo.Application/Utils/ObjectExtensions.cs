using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UcabGo.Core.Data;
using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Application.Utils
{
    public static class ObjectExtensions
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            DateFormatString = "yyyy-MM-dd hh:mm:ss",
            Culture = System.Globalization.CultureInfo.GetCultureInfo("en-US")
        };

        public static T ToObject<T>(this IEnumerable<KeyValuePair<string, StringValues>> source)
            where T : class, new()
        {
            var items = new Dictionary<string, string>();

            foreach (var item in source)
            {
                items.Add(item.Key, item.Value);
            }

            var json = JsonConvert.SerializeObject(items, JsonSerializerSettings);

            var someObject = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);

            return someObject;
        }


        public static T ToObject<T>(this Stream source)
            where T : class, new()
        {
            var json = new StreamReader(source).ReadToEnd();

            var someObject = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);

            return someObject;
        }

        public static async Task<T> ToObjectAsync<T>(this Stream source)
            where T : class, new()
        {
            var json = await new StreamReader(source).ReadToEndAsync();

            var someObject = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);

            return someObject;
        }

        public static bool IsValid(this BaseRequest model, out List<ValidationResult> validationResults)
        {
            return Validator.TryValidateObject(model, new ValidationContext(model), validationResults = new(), true);
        }


        public static string GetDescription(this Enum e)
        {
            var attribute =
                e.GetType()
                    .GetTypeInfo()
                    .GetMember(e.ToString())
                    .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .SingleOrDefault()
                    as DescriptionAttribute;

            return attribute?.Description ?? e.ToString();
        }

        #region Bearer Token Methods
        public static string ObtainToken(this UserDto user)
        {
            // Get the secret from the environment variables
            string issuer = Environment.GetEnvironmentVariable("JWT_ISS");
            string audience = Environment.GetEnvironmentVariable("JWT_AUD");
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

            // Create the header
            var header = new Dictionary<string, object>
            {
                { "alg", "HS256" },
                { "typ", "JWT" }
            };

            // Create the secret
            var keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
            var key = new HMACSHA256(keyBytes);

            // Define expiration time
            var now = DateTime.UtcNow;
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var iat = (int)(now - unixEpoch).TotalSeconds;
            var exp = (int)(now.AddHours(1) - unixEpoch).TotalSeconds;

            // Create the payload
            var claims = new List<Claim>()
            {
                new Claim("userid", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("jti", Guid.NewGuid().ToString()),
            };
            var payload = new Dictionary<string, object>
            {
                { "iss", issuer },
                { "aud", audience },
                { "iat", iat },
                { "exp", exp },
            };

            foreach (var claim in claims)
            {
                payload.Add(claim.Type, claim.Value);
            }

            // Encode the token
            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

            var encodedHeader = Convert.ToBase64String(headerBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            var encodedPayload = Convert.ToBase64String(payloadBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            var signatureBytes = key.ComputeHash(Encoding.UTF8.GetBytes($"{encodedHeader}.{encodedPayload}"));
            var encodedSignature = Convert.ToBase64String(signatureBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            var tokenString = $"{encodedHeader}.{encodedPayload}.{encodedSignature}";

            return tokenString;
        }
        public static void ValidateToken(this string authorizationHeader, out List<Claim> claims)
        {
            // Get the JWT secret from the environment variables
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            string issuer = Environment.GetEnvironmentVariable("JWT_ISS");
            string audience = Environment.GetEnvironmentVariable("JWT_AUD");

            // Encode key
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(jwtSecret);
            var key = new HMACSHA256(keyBytes);

            // Validate header format
            var tokenParts = authorizationHeader.Split(' ');
            if (tokenParts.Length != 2 || !tokenParts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Invalid authorization header format");
            }

            // Validate token format
            var tokenString = tokenParts[1];
            var tokenPartsArray = tokenString.Split('.');
            if (tokenPartsArray.Length != 3)
            {
                throw new SecurityTokenException("Invalid token format");
            }

            for(int i = 0; i < 3; i++)
            {
                int mod4 = tokenPartsArray[i].Length % 4;
                if (mod4 > 0)
                {
                    tokenPartsArray[i] += new string('=', 4 - mod4);
                }
            }

            // Validate header and payload
            var headerBytes = Convert.FromBase64String(tokenPartsArray[0].Replace('-', '+').Replace('_', '/'));
            var payloadBytes = Convert.FromBase64String(tokenPartsArray[1].Replace('-', '+').Replace('_', '/'));
            var headerJson = Encoding.UTF8.GetString(headerBytes);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);
            var header = JsonConvert.DeserializeObject<Dictionary<string, object>>(headerJson);
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);
            if (!header.TryGetValue("alg", out object algObj) || !algObj.Equals("HS256"))
            {
                throw new SecurityTokenException("Invalid algorithm");
            }

            // Validate signature
            var signatureBytes = Convert.FromBase64String(tokenPartsArray[2].Replace('-', '+').Replace('_', '/'));
            var signatureComputedBytes = key.ComputeHash(Encoding.UTF8.GetBytes($"{tokenPartsArray[0].Replace("=","")}.{tokenPartsArray[1].Replace("=","")}"));
            if (!signatureBytes.SequenceEqual(signatureComputedBytes))
            {
                throw new SecurityTokenException("Invalid signature");
            }

            // Set claims
            claims = new List<Claim>();
            foreach (var kvp in payload)
            {
                claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
            }
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            // Final validation
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out SecurityToken validatedToken);
        }
        #endregion
    }
}
