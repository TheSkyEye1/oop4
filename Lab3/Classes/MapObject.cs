using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace Lab3
{
    public abstract class MapObject
    {
        public string objectName;
        public string objectType;
        public DateTime creationTime;
        public MapObject(string name, string type)
        {
            this.objectType = type;
            this.objectName = name;
            creationTime = DateTime.Now;
        }

        public abstract string getTitle();

        public abstract string getType();

        public abstract DateTime getCreationDate();

        public abstract double getDistance(PointLatLng pointtwo);

        public abstract PointLatLng getFocus();

        public abstract GMapMarker GetMarker();

        
    }
}
