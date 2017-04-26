using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhereAmI
{
    public class ApplicationState
    {
        public List<Position> LastFivePositions { get; set; } = new List<Position>();

        public Position LatestPosition { get; set; }

        public TollboothGeofence LastEnteredTollBooth { get; set; }

        public RouteGeofence CurrentRoute { get; set; }

        public string PrintState()
        {
            string stateAsJson = String.Empty;
            try
            {
                stateAsJson = JsonConvert.SerializeObject(this);
            }
            catch { }

            return stateAsJson;
        }
    }
}
