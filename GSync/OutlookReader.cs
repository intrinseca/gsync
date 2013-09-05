using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Outlook;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace GSync
{
    class OutlookReader
    {
        public ObservableCollection<CalendarEntry> Entries { get; private set; }

        public OutlookReader()
        {
            Entries = new ObservableCollection<CalendarEntry>();
        }

        public void Refresh()
        {
            Entries.Clear();

            var outlook = new Microsoft.Office.Interop.Outlook.Application();
            var mapiNamespace = outlook.GetNamespace("MAPI"); ;
            var folder = mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);
            var items = folder.Items;
            items.Sort("[Start]");
            items.IncludeRecurrences = true;

            var startDate = DateTime.Today;
            var endDate = startDate.AddMonths(1);

            var currentEvent = items.Find(String.Format("[Start] >= \"{0:D}\" and [Start] <= \"{1:D}\"", startDate, endDate));

            while (currentEvent != null)
            {
                Entries.Add(CalendarEntry.FromOutlook((AppointmentItem)currentEvent));
                currentEvent = items.FindNext();
            }
        }
    }
}
