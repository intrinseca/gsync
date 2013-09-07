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
using Google.Apis.Calendar.v3.Data;
using System.Security.Cryptography;

namespace GSync
{
    [Serializable]
    public class GoogleCalendarException : Exception
    {
        public GoogleCalendarException() { }
        public GoogleCalendarException(string message) : base(message) { }
        public GoogleCalendarException(string message, Exception inner) : base(message, inner) { }
        protected GoogleCalendarException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    public class GoogleCalendar
    {
        private Func<Uri, string> authFunction;
        private CalendarService service;
        private NativeApplicationClient provider;
        private OAuth2Authenticator<NativeApplicationClient> auth;

        private static byte[] aditionalEntropy = { 1, 2, 3, 4, 5 };
        private bool authRefreshOnly = true;

        public bool Authenticated
        {
            get
            {
                auth.LoadAccessToken();
                return (auth.State != null);
            }
        }

        public GoogleCalendar()
        {
            initialiseService();
        }

        private void initialiseService()
        {
            provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, ClientCredentials.ClientID, ClientCredentials.ClientSecret);
            auth = new OAuth2Authenticator<NativeApplicationClient>(provider, getAuthorisation);

            // Create the service.
            service = new CalendarService(new BaseClientService.Initializer()
            {
                Authenticator = auth,
                ApplicationName = "GSync",
            });
        }

        public void Authorise(Func<Uri, string> _authFunction)
        {
            authFunction = _authFunction;
            authRefreshOnly = false;

            auth.LoadAccessToken();

            authRefreshOnly = true;
            authFunction = null;
        }

        public void Deauthorise()
        {
            GoogleCalendar.DestroyRefreshToken();
            initialiseService();
        }

        public IList<CalendarListEntry> GetCalendarList()
        {
            if (!Authenticated)
                throw new GoogleCalendarException("Not Authenticated");

            try
            {
                return service.CalendarList.List().Execute().Items;
            }
            catch (Exception ex)
            {
                throw new GoogleCalendarException("Failed to retrieve calendar list", ex);
            }
        }

        public void AddEvent(CalendarEntry newEntry, string calendarID)
        {
            if (!Authenticated)
                throw new GoogleCalendarException("Not Authenticated");

            Event e = new Event();
            e.Start = new EventDateTime() { DateTime = newEntry.Start.ToUniversalTime().ToString("O"), TimeZone="UTC" };
            e.End = new EventDateTime() { DateTime = newEntry.End.ToUniversalTime().ToString("O"), TimeZone = "UTC" };
            e.Summary = newEntry.Title;
            e.Description = newEntry.Description + String.Format("\r\nOutlook ID:{0}", newEntry.UniqueID);
            e.Location = newEntry.Location;

            service.Events.Insert(e, calendarID).Execute();
        }

        private IAuthorizationState getAuthorisation(NativeApplicationClient client)
        {
            IAuthorizationState state = new AuthorizationState(new[] { CalendarService.Scopes.Calendar.GetStringValue() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);

            string refreshToken = LoadRefreshToken();
            if (!String.IsNullOrWhiteSpace(refreshToken))
            {
                state.RefreshToken = refreshToken;

                if (client.RefreshToken(state))
                    return state;
            }

            if (!authRefreshOnly && authFunction != null)
            {
                Uri authUri = client.RequestUserAuthorization(state);

                string authResult = authFunction(authUri);

                var result = client.ProcessUserAuthorization(authResult, state);
                StoreRefreshToken(state);
                return result;
            }
            else
            {
                return null;
            }
        }

        private static string LoadRefreshToken()
        {
            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.RefreshToken))
                return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(Properties.Settings.Default.RefreshToken), aditionalEntropy, DataProtectionScope.CurrentUser));
            else
                return null;
        }

        private static void StoreRefreshToken(IAuthorizationState state)
        {
            Properties.Settings.Default.RefreshToken = Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(state.RefreshToken), aditionalEntropy, DataProtectionScope.CurrentUser));
            Properties.Settings.Default.Save();
        }

        private static void DestroyRefreshToken()
        {
            Properties.Settings.Default.RefreshToken = null;
            Properties.Settings.Default.Save();
        }
    }
}
