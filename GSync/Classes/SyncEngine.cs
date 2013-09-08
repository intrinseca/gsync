using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GSync
{
    public class SyncEngine
    {
        public HashSet<CalendarEntry> SyncedEntries { get; private set; }

        private ICalendar source;
        private ICalendar dest;

        public SyncEngine(ICalendar _source, ICalendar _dest)
        {
            SyncedEntries = new HashSet<CalendarEntry>();

            source = _source;
            dest = _dest;
        }

        public void SaveSyncedEntries(Stream target)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(target, SyncedEntries);
        }

        public void LoadSyncedEntries(Stream source)
        {
            BinaryFormatter deserializer = new BinaryFormatter();
            SyncedEntries = (HashSet<CalendarEntry>)deserializer.Deserialize(source);
        }

        public void Sync()
        {
            //Get entries from the Source
            var sourceEntries = new HashSet<CalendarEntry>(source.GetEntries());

            //Remove entries already synced
            sourceEntries.ExceptWith(SyncedEntries);

            //Add to destination
            foreach (var entry in sourceEntries)
            {
                dest.AddEntry(entry);
                Debug.Print("Adding {0}", entry);
            }

            //Update synced list
            SyncedEntries.UnionWith(sourceEntries);
        }
    }
}
