using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fishermen.Utils
{
    public class Utils
    {
        public static string EncryptSecret { get; set; }
        static string appId = "bcae7ac8-cf00-4888-8004-b2b684e74207";
        static string secretKey = "phm97QY0mUq5JO=MvEXXHwFGcgqfw-:.";
        static string tenantId = "d7ad1504-fcfc-4797-9abe-0737e3708a7a";

        public static async Task<string> GetAccessToken(string azureTenantId, string azureAppId, string azureSecretKey)
        {

            var context = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext("https://login.windows.net/" + tenantId);
            ClientCredential clientCredential = new ClientCredential(appId, secretKey);
            var tokenResponse = await context.AcquireTokenAsync("https://vault.azure.net", clientCredential);
            var accessToken = tokenResponse.AccessToken;
            return accessToken;
        }
    }
}
