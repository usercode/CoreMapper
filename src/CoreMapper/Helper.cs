using ObjectChangeTracking.Abstractions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ObjectChangeTracking.CoreMapper
{
    class Helper
    {
        public static bool IsChangedProperty(ITrackableObject obj, string property)
        {
            return obj.ChangedProperties.Any(x => x.Name == property);
        }

        public static IEnumerable<T> GetAddedItems<T>(ITrackableCollection<T> collection)
        {
            return collection.Added.ToList();
        }

        public static IEnumerable<T> GetRemovedItems<T>(ITrackableCollection<T> collection)
        {
            return collection.Removed.ToList();
        }
    }
}
