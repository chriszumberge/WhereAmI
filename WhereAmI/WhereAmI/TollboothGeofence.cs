//
// TollboothGeofence.cs
//
//
// Author:
//       Chris Zumberge <chriszumberge@gmail.com>
//
// Copyright (c) 2017 Christopher Zumberge
//
// All rights reserved
//
using System;
using NetTopologySuite.Geometries;

namespace WhereAmI
{
    public class TollboothGeofence : Geofence
    {
        public TollboothGeofence(string name, Polygon geometry) : base(name, geometry) { }

        public TollboothGeofence(string name, string geometryWellKnownText) : base(name, geometryWellKnownText) { }

        public TollboothGeofence(string name, string lineStringWellKnownText, double bufferDistance) : base(name, lineStringWellKnownText, bufferDistance) { }

        public TollboothGeofence(string name, GeoAPI.Geometries.Coordinate[] polygonBorderCoordinates) : base(name, polygonBorderCoordinates) { }

        public TollboothGeofence(string name, double lat, double lng, double meterRadius) : base(name, lat, lng, meterRadius) { }

        public TollboothGeofence(string name, Point location, double meterRadius) : base(name, location, meterRadius) { }

        public TollboothGeofence(string name, GeoAPI.Geometries.Coordinate coordinate, double meterRadius) : base(name, coordinate, meterRadius) { }
    }
}
