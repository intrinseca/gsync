using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSync
{
    public interface ICalendar
    {
        bool EntryExists(CalendarEntry entry);
        void AddEntry(CalendarEntry newEntry);
        List<CalendarEntry> GetEntries();
    }
}
