using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace WhereAmI
{
    public class DrawableMap : Map
    {
        public List<MapShape> Shapes { get; set; } = new List<MapShape>();
        public GeometryDrawableOptions DrawableOptions { get; set; }

        public DrawableMap()
        {

        }
    }

    public class MapShape
    {
        public List<Position> ShapeCoordinates { get; set; } = new List<Position>();
        public MapShape()
        {

        }
    }

    public class GeometryDrawableOptions
    {
        public Color FillColor { get; set; } = Color.Red;
        public Color StrokeColor { get; set; } = Color.White;
        public double LineWidth { get; set; } = 9.0;
        public double Alpha { get; set; } = 0.4;
    }
}
