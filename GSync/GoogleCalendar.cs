using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetOpenAuth.OAuth2;

using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Services;
using Google.Apis.Calendar.v3;
using Google.Apis.Util;

using System.Diagnostics;

namespace GSync
{
    class GoogleCalendar
    {
        public GoogleCalendar()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, ClientCredentials.ClientID, ClientCredentials.ClientSecret);

            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

            // Create the service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                Authenticator = auth,
                ApplicationName = "GSync",
            });

            var list = service.CalendarList.List().Execute().Items;
        }

        private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            // Get the auth URL:
            IAuthorizationState state = new AuthorizationState(new[] { CalendarService.Scopes.Calendar.GetStringValue() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            // Request authorization from the user (by opening a browser window):
            Process.Start(authUri.ToString());
            Console.Write("  Authorization Code: ");
            string authCode = Console.ReadLine();
            Console.WriteLine();

            // Retrieve the access token by using the authorization code:
            return arg.ProcessUserAuthorization(authCode, state);
        }
    }
}
