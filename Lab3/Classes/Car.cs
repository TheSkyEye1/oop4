using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Threading;
using Lab3.Classes;

namespace Lab3.Classes
{
    class Car : MapObject
    {
        PointLatLng point = new PointLatLng();

        public MapRoute route { get; private set; }
        private Human human;
        private List<Human> passengers = new List<Human>();
        private GMapMarker marker;
        public event EventHandler Arrived;
        public event EventHandler Follow;

        public Car(string name, string type, PointLatLng Point) : base(name, type)
        {
            this.point = Point;
        }

        public override double getDistance(PointLatLng pointtwo)
        {
            GeoCoordinate geo1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate geo2 = new GeoCoordinate(pointtwo.Lat, pointtwo.Lng);
            return geo1.GetDistanceTo(geo2);
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public override DateTime getCreationDate()
        {
            return creationTime;
        }

        public override GMapMarker GetMarker()
        {
            marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32,
                    Height = 32,
                    ToolTip = objectName,
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/car.png"))
                }
            };
            return marker;
        }

        public override string getTitle()
        {
            return objectName;
        }

        public override string getType()
        {
            return objectType;
        }
        public void getintocar(object sender, EventArgs args)
        {
            human = (Human)sender;
            MoveTo(human.DPoint);
            human.point = point;
            (sender as Human).seated -= getintocar;
        }
        public MapRoute MoveTo(PointLatLng endpoint)
        {
            RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
            route = routingProvider.GetRoute(point, endpoint, false, false, 15);
            Thread ridingCar = new Thread(MoveByRoute);
            ridingCar.Start();
            return route;
        }

        private void MoveByRoute()
        {
            try
            {
                foreach (var point in route.Points)
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        this.point = point;
                        marker.Position = point;
                        if (human != null)
                        {
                            human.marker.Position = point;
                            human.point = point;
                            Follow?.Invoke(this, null);
                        }
                    });
                    Thread.Sleep(1000);
                }
                if (human == null)
                {
                    Arrived?.Invoke(this, null);
                }
                else
                {
                    MessageBox.Show("Passenger arrived");
                    human = null;
                    Arrived?.Invoke(this, null);
                }
            }
            catch { };
        }
    }
}
