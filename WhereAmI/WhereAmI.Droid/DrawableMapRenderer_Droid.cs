using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using WhereAmI;
using WhereAmI.Droid;

[assembly:ExportRenderer (typeof(DrawableMap), typeof(CustomMapRenderer))]
namespace WhereAmI.Droid
{
    public class DrawableMapRenderer_Droid : MapRenderer, IOnMapReadyCallback
    {
        GoogleMap map;
        DrawableMap formsMap;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // unsubscribe
            }

            if (e.NewElemnt != null)
            {
                formsMap = (DrawableMap)e.NewElement;

                ((MapView)Control).GetMapAsync(this);
            }
        }

        public void OnMapready(GoogleMap googleMap)
        {
            map = googleMap;

            foreach (var shape in formsMap.Shapes)
            {
                var polygonOptions = new PolygonOptions();
                polygonOptions.InvokeFillColor(formsMap.FillColor.ToString("X"));
                polygonOptions.InvokeStrokeColor(formsMap.FillColor.ToString("X"));
                polygonOptions.InvokeStrokeWidth((float)formsMap.StrokeWidth);

                foreach (var coordinate in shape.ShapeCoordinates)
                {
                    polygonOptions.Add(new LatLng(coordinate.Latitude, coordinate.Longitude));
                }

                map.AddPolygon(polygonOptions);
            }
        }
    }
}