using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereAmI
{
    public sealed class GeofenceManagerProvider
    {
        /// <summary>
        /// Lazy loaded instance of the Geofence Manager
        /// </summary>
        static readonly Lazy<GeofenceManager> instance = new Lazy<GeofenceManager>(() => new GeofenceManager());
        /// <summary>
        /// Gets the singleton instance of the Geofence Manager
        /// </summary>
        public static GeofenceManager Instance => instance.Value;

        /// <summary>
        /// Prevents a default instance of the <see cref="GeofenceManager"/> class from being created.
        /// </summary>
        private GeofenceManagerProvider() { }
    }
}
