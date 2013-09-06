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

    class GoogleCalendar
    {
        private Func<Uri, string> authFunction;
        private CalendarService service;

        public GoogleCalendar(Func<Uri, string> _authFunction)
        {
            authFunction = _authFunction;

            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, ClientCredentials.ClientID, ClientCredentials.ClientSecret);
            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, getAuthorisation);

            // Create the service.
            service = new CalendarService(new BaseClientService.Initializer()
            {
                Authenticator = auth,
                ApplicationName = "GSync",
            });
        }

        public IList<CalendarListEntry> GetCalendarList()
        {
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
            Event e = new Event();
            e.Start = new EventDateTime() { DateTime = newEntry.Start.ToUniversalTime().ToString("O"), TimeZone="UTC" };
            e.End = new EventDateTime() { DateTime = newEntry.End.ToUniversalTime().ToString("O"), TimeZone = "UTC" };
            e.Summary = newEntry.Title;
            e.Description = newEntry.Description;
            e.Location = newEntry.Location;

            service.Events.Insert(e, calendarID).Execute();
        }

        private IAuthorizationState getAuthorisation(NativeApplicationClient arg)
        {
            IAuthorizationState state = new AuthorizationState(new[] { CalendarService.Scopes.Calendar.GetStringValue() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            string authResult = authFunction(authUri);

            return arg.ProcessUserAuthorization(authResult, state);
        }
    }
}
