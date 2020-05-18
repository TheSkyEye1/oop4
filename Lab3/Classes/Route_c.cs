using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Lab3.Classes
{
    class Route_c : MapObject
    {
        PointLatLng point = new PointLatLng();
        List<PointLatLng> points = new List<PointLatLng>();
        public GMapMarker marker { get; private set; }
        public Route_c(string name, string type, List<PointLatLng> Points, PointLatLng Point) : base(name, type)
        {
            point = Point;
            points = Points;
        }
        public override string getTitle()
        {
            return objectName;
        }

        public override string getType()
        {
            return objectType;
        }
        public override double getDistance(PointLatLng pointtwo)
        {
            var DC = new DCalculator();
            double distance = DC.GetMinDistance(point, points[1], pointtwo);
            if (points.Count > 2)
            {
                for (int i = 1; i < points.Count; i++)
                {
                    if (points[i] == points.Last())
                    {
                        if (DC.GetMinDistance(points[i], points[0], pointtwo) < distance)
                        {
                            distance = DC.GetMinDistance(points[i], points[0], pointtwo);
                        }
                    }
                    else
                    {
                        if (DC.GetMinDistance(points[i], points[i + 1], pointtwo) < distance)
                        {
                            distance = DC.GetMinDistance(points[i], points[i + 1], pointtwo);
                        }
                    }
                }
            }
            return distance;
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public override GMapMarker GetMarker()
        {
            marker = new GMapRoute(points)
            {
                Shape = new Path()
                {
                    Stroke = Brushes.DarkBlue,
                    Fill = Brushes.DarkBlue,
                    StrokeThickness = 4
                }
            };
            return marker;
        }

        public override DateTime getCreationDate()
        {
            return new DateTime();
        }
    }
}
