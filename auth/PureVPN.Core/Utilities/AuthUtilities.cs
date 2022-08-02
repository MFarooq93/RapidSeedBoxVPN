using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Utilities
{
    internal class AuthUtilities: IDisposable
    {
        private static AuthUtilities AuthUtilitiesInstance;
        internal string CodeVerifier { get; private set; }
        
        private AuthUtilities()
        {
        }

        internal static AuthUtilities GetAuthUtilitiesInstance()
        {
            if (null == AuthUtilitiesInstance)
                AuthUtilitiesInstance = new AuthUtilities();

            return AuthUtilitiesInstance;
        }

        internal string GetCodeVerifier()
        {
            //Generate a random string for our code verifier
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);

            CodeVerifier = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            return CodeVerifier;
        }

        internal string GetCodeChallenge(string codeVerifier)
        {
            //generate the code challenge based on the verifier
            string codeChallenge;
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                codeChallenge = Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');

                return codeChallenge;
            }
        }

        public void Dispose()
        {
            AuthUtilitiesInstance = null;
        }
    }
}
