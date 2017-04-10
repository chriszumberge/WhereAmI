//
// RouteGeofence.cs
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
    public class RouteGeofence : Geofence
    {
        public RouteGeofence(string name, Polygon geometry) : base(name, geometry) { }

        public RouteGeofence(string name, string geometryWellKnownText) : base(name, geometryWellKnownText) { }

        public RouteGeofence(string name, string lineStringWellKnownText, double bufferDistance) : base(name, lineStringWellKnownText, bufferDistance) { }

        public RouteGeofence(string name, GeoAPI.Geometries.Coordinate[] polygonBorderCoordinates) : base(name, polygonBorderCoordinates) { }

        public RouteGeofence(string name, double lat, double lng, double meterRadius) : base(name, lat, lng, meterRadius) { }

        public RouteGeofence(string name, Point location, double meterRadius) : base(name, location, meterRadius) { }

        public RouteGeofence(string name, GeoAPI.Geometries.Coordinate coordinate, double meterRadius) : base(name, coordinate, meterRadius) { }
    }
}
