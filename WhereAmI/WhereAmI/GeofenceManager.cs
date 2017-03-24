using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Buffer;
using System;
using System.Collections.Generic;

namespace WhereAmI
{
    public class GeofenceManager
    {
        public List<Geofence> Geofences => mGeofences;
        List<Geofence> mGeofences { get; set; } = new List<Geofence>();

        public void SubscribeGeofence(Geofence newGeofence) => mGeofences.Add(newGeofence);

        public void UnsubscribeGeofence(Geofence geofence) => mGeofences.Remove(geofence);

        public event EventHandler<GeofenceEnteredEventArgs> OnEnteredGeofence;
        public event EventHandler<GeofenceExitedEventArgs> OnExitedGeofence;

        public void UpdateGeofences(Plugin.Geolocator.Abstractions.Position position)
        {
            this.UpdateGeofences(position.Latitude, position.Longitude, position.Timestamp.LocalDateTime.AddHours(1));
            //this.UpdateGeofences(position.Latitude, position.Longitude, TimeZoneInfo.ConvertTime(position.Timestamp.LocalDateTime, TimeZoneInfo.Local));
        }

        public void UpdateGeofences(double lat, double lng, Nullable<DateTime> triggerTime = null)
        {
            Point location = new Point(lng, lat);

            foreach (Geofence geofence in mGeofences)
            {
                if (geofence.Geometry.Contains(location))
                {
                    if (!geofence.IsInside)
                    {
                        geofence.IsInside = true;
                        OnEnteredGeofence.Invoke(this, new GeofenceEnteredEventArgs(geofence, triggerTime ?? DateTime.Now));
                    }
                }
                else
                {
                    if (geofence.IsInside)
                    {
                        geofence.IsInside = false;
                        OnExitedGeofence.Invoke(this, new GeofenceExitedEventArgs(geofence, triggerTime ?? DateTime.Now));
                    }
                }
            }
        }

    }

    public class Geofence
    {
        readonly Polygon mGeometry;
        public Polygon Geometry => mGeometry;

        readonly string mName;
        public string Name => mName;

        public bool IsInside { get; set; }

        private double metersToBufferDistanceConversionFactor = 1.0 / 213000;

        public Geofence(string name, Polygon geometry)
        {
            mGeometry = geometry;
            mName = name;
        }

        public Geofence(string name, string geometryWellKnownText)
        {
            mName = name;

            WKTReader reader = new WKTReader();
            GeoAPI.Geometries.IGeometry geometry = reader.Read(geometryWellKnownText);

            if (geometry is Polygon)
            {
                mGeometry = (Polygon)geometry;
            }
            else
            {
                // for now, can extrude with default settings later
                throw new ArgumentException("Well Known Text does not represent a polygon");
            }
        }

        public Geofence(string name, string lineStringWellKnownText, double bufferDistance)
        {
            mName = name;

            WKTReader reader = new WKTReader();
            LineString lineString = (LineString)reader.Read(lineStringWellKnownText);
            BufferParameters bufferParameters = new BufferParameters(8, GeoAPI.Operation.Buffer.EndCapStyle.Round, GeoAPI.Operation.Buffer.JoinStyle.Round, 5);
            mGeometry = (Polygon)lineString.Buffer(bufferDistance * metersToBufferDistanceConversionFactor, bufferParameters);
        }

        public Geofence(string name, GeoAPI.Geometries.Coordinate[] polygonBorderCoordinates)
        {
            mName = name;

            mGeometry = new Polygon(new LinearRing(polygonBorderCoordinates));
        }

        public Geofence(string name, double lat, double lng, double meterRadius)
        {
            mName = name;

            Point point = new Point(lng, lat);
            BufferParameters bufferParams = new BufferParameters(16, GeoAPI.Operation.Buffer.EndCapStyle.Round, GeoAPI.Operation.Buffer.JoinStyle.Round, meterRadius);
            Polygon polygon = (Polygon)point.Buffer(meterRadius * metersToBufferDistanceConversionFactor, bufferParams);
            mGeometry = polygon;
        }

        public Geofence(string name, Point location, double meterRadius)
        {
            mName = name;

            BufferParameters bufferParams = new BufferParameters(16, GeoAPI.Operation.Buffer.EndCapStyle.Round, GeoAPI.Operation.Buffer.JoinStyle.Round, meterRadius);
            Polygon polygon = (Polygon)location.Buffer(meterRadius * metersToBufferDistanceConversionFactor, bufferParams);
            mGeometry = polygon;
        }

        public Geofence(string name, GeoAPI.Geometries.Coordinate coordinate, double meterRadius)
        {
            mName = name;

            Point point = new Point(coordinate.X, coordinate.Y);
            BufferParameters bufferParams = new BufferParameters(16, GeoAPI.Operation.Buffer.EndCapStyle.Round, GeoAPI.Operation.Buffer.JoinStyle.Round, meterRadius);
            Polygon polygon = (Polygon)point.Buffer(meterRadius * metersToBufferDistanceConversionFactor, bufferParams);
            mGeometry = polygon;
        }
    }

    public class GeofenceEnteredEventArgs : EventArgs
    {
        public Geofence Geofence => mGeofence;
        Geofence mGeofence { get; set; }
        public DateTime DateTimeEntered => mDateTimeEntered;
        DateTime mDateTimeEntered { get; set; }

        public GeofenceEnteredEventArgs(Geofence geofence, DateTime dateTimeEntered)
        {
            mGeofence = geofence;
            mDateTimeEntered = dateTimeEntered;
        }
    }

    public class GeofenceExitedEventArgs : EventArgs
    {
        public Geofence Geofence => mGeofence;
        Geofence mGeofence { get; set; }
        public DateTime DateTimeExited => mDateTimeExited;
        DateTime mDateTimeExited { get; set; }

        public GeofenceExitedEventArgs(Geofence geofence, DateTime dateTimeExited)
        {
            mGeofence = geofence;
            mDateTimeExited = dateTimeExited;
        }
    }
}
