using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using Lab3.Classes;

namespace Lab3
{
    public partial class MainWindow : Window
    {
        public List<MapObject> mapObjects = new List<MapObject>();
        public List<MapObject> secondList = new List<MapObject>();
        public PointLatLng point = new PointLatLng();
        static PointLatLng RStart;
        static PointLatLng RFinish;
        public List<PointLatLng> areapoints = new List<PointLatLng>();
        public List<PointLatLng> routepoints = new List<PointLatLng>();
        int rpointc = 0;
        int apointc = 0;
        bool creationmode = false;
        bool secondact = false;
        int mcar = 0;
        int mhuman = 0;
        int mloc = 0;
        bool selectpas = false;
        bool selectdis = false;
        Human Passanger;
        public PointLatLng destination;
        bool destselected = false;
        public MainWindow()
        {
            InitializeComponent();
            MapLoad();
            Passanger = null;
        }

        private void MapLoad()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = GMapProviders.GoogleMap;
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            Map.Position = new PointLatLng(55.012823, 82.950359);
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;
        }

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            if(selectpas == true)
            {
                var Obj = mapObjects.OrderBy(mapObject => mapObject.getDistance(point));
                foreach (MapObject obj in Obj)
                {
                    if (obj is Human)
                    {
                        Passanger = (Human)obj;
                        selectpas = false;
                        if (destselected == true)
                        {
                            Passanger.DPoint = destination;
                            RStart = Passanger.getFocus();
                            MessageBox.Show("Now you can call taxi");
                            CallT.IsEnabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Your choise is - " + "'" + Passanger.objectName + "'" + ", now choose the Destination");
                        }
                        break;
                    }
                }
            }
            if(selectdis == true)
            {
                var Obj = mapObjects.OrderBy(mapObject => mapObject.getDistance(point));
                foreach (MapObject obj in Obj)
                {
                    if ((obj is Human && obj != Passanger) || obj is Location_c || obj is Area || obj is Route_c)
                    {
                        destselected = true;
                        selectdis = false;
                        destination = obj.getFocus();
                        if (Passanger != null)
                        {
                            Passanger.DPoint = destination;
                            RFinish = Passanger.DPoint;
                            MessageBox.Show("Now you can call taxi");
                            CallT.IsEnabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Сhoose a passenger to travel");
                        }
                        break;
                    }
                }
            }
            if (creationmode == true)
            {
                if (combox.SelectedIndex == 0)
                {
                    createra.IsEnabled = true;
                }
                if (combox.SelectedIndex == 1)
                {
                    createra.IsEnabled = true;
                }
                if (combox.SelectedIndex == 2)
                {
                    createra.IsEnabled = true;
                }
                if (combox.SelectedIndex == 3)
                {
                    routepoints.Add(point);
                    rpointc += 1;
                    if (rpointc >= 2)
                    {
                        createra.IsEnabled = true;
                        clearpoints.IsEnabled = true;
                    }
                    
                }
                if (combox.SelectedIndex == 4)
                {
                    areapoints.Add(point);
                    apointc += 1;
                    if (apointc >= 3)
                    {
                        createra.IsEnabled = true;
                        clearpoints.IsEnabled = true;
                    }
                }
            }
            else
            {
                OList.Items.Clear();
                OList.Items.Add(null);
                secondList = mapObjects.OrderBy(mobject => mobject.getDistance(point)).ToList();
                foreach (MapObject obj in secondList)
                {
                    string mapObjectAndDistanceString = new StringBuilder()
                        .Append(obj.getType())
                        .Append(" - ")
                        .Append(obj.getTitle())
                        .Append(" - ")
                        .Append(obj.getDistance(point).ToString("0.##"))
                        .Append(" м.").ToString();
                    OList.Items.Add(mapObjectAndDistanceString);
                }
                secondact = true;
            }
        }

        public void placepoint(PointLatLng point)
        {
            MapObject mapObject_point = new Location_c(OName.Text, "Location" ,point);
            mapObjects.Add(mapObject_point);
            Map.Markers.Add(mapObject_point.GetMarker());
            OList.Items.Add(mapObjects.Last().objectType + " - " + mapObjects.Last().objectName);
            createra.IsEnabled = false;
            mloc += 1;
            if (mloc > 1 && mcar > 0 && mhuman > 0)
            {
                ChooseH.IsEnabled = true;
                ChooseD.IsEnabled = true;
            }
        }
        public void placecar(PointLatLng point)
        {
            MapObject mapObject_point = new Car(OName.Text, "Car", point);
            mapObjects.Add(mapObject_point);
            Map.Markers.Add(mapObject_point.GetMarker());
            OList.Items.Add(mapObjects.Last().objectType + " - " + mapObjects.Last().objectName);
            createra.IsEnabled = false;
            mcar += 1;
            if (mloc > 1 && mcar > 0 && mhuman > 0)
            {
                ChooseH.IsEnabled = true;
                ChooseD.IsEnabled = true;
            }
        }
        public void placehuman(PointLatLng point)
        {
            MapObject mapObject_point = new Human(OName.Text, "Human", point);
            mapObjects.Add(mapObject_point);
            Map.Markers.Add(mapObject_point.GetMarker());
            OList.Items.Add(mapObjects.Last().objectType + " - " + mapObjects.Last().objectName);
            createra.IsEnabled = false;
            mloc += 1;
            mhuman += 1;
            if (mloc > 1 && mcar > 0 && mhuman > 0)
            {
                ChooseH.IsEnabled = true;
                ChooseD.IsEnabled = true;
            }
        }
        public void createroute(List <PointLatLng>points)
        {
            rpointc = 0;
            MapObject mapObject_path = new Route_c(OName.Text, "Route", points, points[0]);
            mapObjects.Add(mapObject_path);
            Map.Markers.Add(mapObject_path.GetMarker());
            OList.Items.Add(mapObjects.Last().objectType + " - " + mapObjects.Last().objectName);
            createra.IsEnabled = false;
            clearpoints.IsEnabled = false;
            mloc += 1;
            if (mloc > 1 && mcar > 0 && mhuman > 0)
            {
                ChooseH.IsEnabled = true;
                ChooseD.IsEnabled = true;
            }
        }
        public void createarea(List <PointLatLng>points)
        {
            apointc = 0;
            MapObject mapObject_area = new Area(OName.Text, "Area", points, points[0]);
            mapObjects.Add(mapObject_area);
            Map.Markers.Add(mapObject_area.GetMarker());
            OList.Items.Add(mapObjects.Last().objectType + " - " + mapObjects.Last().objectName);
            createra.IsEnabled = false;
            clearpoints.IsEnabled = false;
            mloc += 1;
            if (mloc > 1 && mcar > 0 && mhuman > 0)
            {
                ChooseH.IsEnabled = true;
                ChooseD.IsEnabled = true;
            }
        }

        private void OList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectpas == true)
            {
                try
                {
                    Passanger = (Human)secondList[OList.SelectedIndex - 1];
                    Map.Position = Passanger.getFocus();
                    selectpas = false;
                    secondact = true;
                    if (destselected == true)
                    {
                        Passanger.DPoint = destination;
                        RStart = Passanger.getFocus();
                        MessageBox.Show("Now you can call taxi");
                        CallT.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Your choise is - " + "'" + Passanger.objectName + "'" + ", now choose the Destination");
                    }
                }
                catch { };
            }
            else
            if (selectdis == true)
            {
                try
                {
                    destination = secondList[OList.SelectedIndex - 1].getFocus();
                    Map.Position = secondList[OList.SelectedIndex - 1].getFocus();
                    destselected = true;
                    if (Passanger != null)
                    {
                        Passanger.DPoint = destination;
                        RFinish = Passanger.DPoint;
                        MessageBox.Show("Now you can call taxi");
                        CallT.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Сhoose a passenger to travel");
                    }
                    selectdis = false;
                    secondact = true;
                }
                catch { };
            }
            else
            {
                if (OList.SelectedIndex > 0 && secondact == false)
                {
                    Map.Position = mapObjects[OList.SelectedIndex - 1].getFocus();
                }
                else
                {
                    if (OList.SelectedIndex > 0)
                    {
                        Map.Position = secondList[OList.SelectedIndex - 1].getFocus();
                    }
                }
            }
        }

        private void Createra_Click(object sender, RoutedEventArgs e)
        {
            if (OName.Text == "")
            {
                MessageBox.Show("Object name is null");
            }
            else
            {
                if (combox.SelectedIndex == 0)
                {
                    placepoint(point);
                }
                if (combox.SelectedIndex == 1)
                {
                    placecar(point);
                }
                if (combox.SelectedIndex == 2)
                {
                    placehuman(point);
                }
                if (combox.SelectedIndex == 3)
                {
                    createroute(routepoints);
                }
                if (combox.SelectedIndex == 4)
                {
                    createarea(areapoints);
                }
                OName.Text = "";

                finder.IsEnabled = true;
                objfind.IsEnabled = true;
                clearpoints.IsEnabled = false;
                routepoints = new List<PointLatLng>();
                areapoints = new List<PointLatLng>();
            }

            OList.Items.Clear();
            for (int i = 0; i < mapObjects.Count; i++)
            {
                if (i == 0)
                {
                    OList.Items.Add(null);
                    OList.Items.Add(mapObjects[i].objectType + " - " + mapObjects[i].objectName);
                }
                else
                {
                    OList.Items.Add(mapObjects[i].objectType + " - " + mapObjects[i].objectName);
                }
            }
            secondact = false;
        }

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectpas = false;
            createra.IsEnabled = false;
            clearpoints.IsEnabled = false;
            rpointc = 0;
            apointc = 0;
            areapoints = new List<PointLatLng>();
            routepoints = new List<PointLatLng>();
        }

        private void Findrb_Checked(object sender, RoutedEventArgs e)
        {
            createrb.IsChecked = false;
            creationmode = false;
        }

        private void Createrb_Checked(object sender, RoutedEventArgs e)
        {
            findrb.IsChecked = false;
            creationmode = true; 
        }

        private void Finder_Click(object sender, RoutedEventArgs e)
        {
            OList.Items.Clear();
            if (objfind.Text == "")
            {
                for (int i = 0; i < mapObjects.Count; i++)
                {
                    if (i == 0)
                    {
                        OList.Items.Add(null);
                        OList.Items.Add(mapObjects[i].objectType + " - " + mapObjects[i].objectName);
                    }
                    else
                    {
                        OList.Items.Add(mapObjects[i].objectType + " - " + mapObjects[i].objectName);
                    }
                }
                secondact = false;
            }
            else
            {
                secondact = true;
                secondList.Clear();
                for (int i = 0; i < mapObjects.Count; i++)
                {

                    if (i == 0)
                    {
                        OList.Items.Add(null);
                        if (mapObjects[i].objectName.Contains(objfind.Text))
                        {
                            OList.Items.Add(mapObjects[i].objectType + " - " + mapObjects[i].objectName);
                            secondList.Add(mapObjects[i]);      
                        }
                    }
                    else
                    {
                        if (mapObjects[i].objectName.Contains(objfind.Text))
                        {
                            OList.Items.Add(mapObjects[i].objectType + " - " + mapObjects[i].objectName);
                            secondList.Add(mapObjects[i]);
                        }
                    }
                }
            }
        }

        private void OList_MouseLeave(object sender, MouseEventArgs e)
        {
            OList.SelectedIndex = 0;
        }

        private void Clearpoints_Click(object sender, RoutedEventArgs e)
        {
            rpointc = 0;
            apointc = 0;
            areapoints = new List<PointLatLng>();
            routepoints = new List<PointLatLng>();
            clearpoints.IsEnabled = false;
            createra.IsEnabled = false;
        }

        private void ChooseH_Click(object sender, RoutedEventArgs e)
        {
            CBar.Value = 0;
            selectpas = true;
            selectdis = false;
            createrb.IsChecked = false;
            findrb.IsChecked = false;
            MessageBox.Show("Choose Passenger");
        }

        private void ChooseD_Click(object sender, RoutedEventArgs e)
        {
            CBar.Value = 0;
            selectdis = true;
            selectpas = false;
            createrb.IsChecked = false;
            findrb.IsChecked = false;
            MessageBox.Show("Choose Destination");
        }

        private void Focus_Follow(object sender, EventArgs args)
        {
            Car car = (Car)sender;
            CBar.Maximum = car.route.Points.Count;
            if ((CBar.Value +1 ) == CBar.Maximum)
            {
                (sender as Car).Follow -= Focus_Follow;
                Map.Markers.Remove(Map.Markers.Last());
                Map.Markers.Remove(Map.Markers.Last());
                Passanger = null;
                destselected = false;
                CBar.Value += 1;
            }
            if (CBar.Value != CBar.Maximum)
            {
                CBar.Value += 1;
                Map.Position = car.getFocus();
            }
        }

        private void CallT_Click(object sender, RoutedEventArgs e)
        {
            CBar.Value = 0;
            Car nearestCar = null;
            RStart = Passanger.getFocus();

            var Obj = mapObjects.OrderBy(mapObject => mapObject.getDistance(RStart));
            foreach (MapObject obj in Obj)
            {
                if (obj is Car)
                {
                    nearestCar = (Car)obj;
                    break;
                }
            }
            
            CallT.IsEnabled = false;

            var route = nearestCar.MoveTo(RStart);
            MapObject mapObject_path = new Route_c(OName.Text, "Route", route.Points, route.Points.First());
            Map.Markers.Add(mapObject_path.GetMarker());
            RoutingProvider routingProvider = GMapProviders.OpenStreetMap;

            MapRoute rroute = routingProvider.GetRoute(RStart,RFinish,false,false,15);
            MapObject mapObject_path1 = new Route_c(OName.Text, "Route", rroute.Points, rroute.Points.First());
            Map.Markers.Add(mapObject_path1.GetMarker());

            nearestCar.Arrived += Passanger.CarArrived;
            Passanger.seated += nearestCar.getintocar;
            nearestCar.Follow += Focus_Follow;
        }
    }
}
