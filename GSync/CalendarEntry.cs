using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Outlook;
using System.Diagnostics;

namespace GSync
{
    [Serializable()]
    public class CalendarEntry
    {
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string UniqueID { get; set; }

        public DateTime End
        {
            get
            {
                return Start + Duration;
            }
        }

        public static CalendarEntry FromOutlook(AppointmentItem i)
        {
            var e = new CalendarEntry();
            e.Start = i.Start;
            e.Duration = TimeSpan.FromMinutes(i.Duration);
            e.Title = i.Subject;
            e.Location = i.Location;
            e.Description = i.Body;
            e.UniqueID = i.EntryID;

            return e;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} ({2})", Start, Title, Location);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((CalendarEntry)obj).UniqueID == UniqueID;
        }
    }
}
