using CoreLocation;
using MapKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using WhereAmI;
using WhereAmI.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer (typeof(DrawableMap), typeof(DrawableMapRenderer_iOS))]
namespace WhereAmI.iOS
{
    public class DrawableMapRenderer_iOS : MapRenderer
    {
        MKPolygonRenderer polygonRenderer;

        DrawableMap formsMap;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.OverlayRenderer = null;
            }

            if (e.NewElement != null)
            {
                formsMap = (DrawableMap)e.NewElement;
                var nativeMap = Control as MKMapView;

                nativeMap.OverlayRenderer = GetOverlayRenderer;

                foreach (var shape in formsMap.Shapes)
                {
                    CLLocationCoordinate2D[] shapeCoordArray = shape.ShapeCoordinates.Select(c => new CLLocationCoordinate2D(c.Latitude, c.Longitude)).ToArray();
                    MKPolygon shapePolygon = MKPolygon.FromCoordinates(shapeCoordArray);
                    nativeMap.AddOverlay(shapePolygon);
                }
            }
        }

        private MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (polygonRenderer == null)
            {
                polygonRenderer = new MKPolygonRenderer(overlay as MKPolygon);
                polygonRenderer.FillColor = formsMap.DrawableOptions.FillColor.ToUIColor();
                polygonRenderer.StrokeColor = formsMap.DrawableOptions.StrokeColor.ToUIColor();
                polygonRenderer.Alpha = (float)formsMap.DrawableOptions.Alpha;
                polygonRenderer.LineWidth = (float)formsMap.DrawableOptions.LineWidth;
            }
            return polygonRenderer;
        }
    }
}
