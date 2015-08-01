using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CTABusTrackerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string busKey;
            string stop;
            string apiWebsite;
            int count;
            string stopName;
            string route;
            string destination;
            string direction;
            string estArrivalTime;
            List<Bus> buses = new List<Bus>();


            //Add your CTA API key here.
            busKey = null;

            //Add stop here.
            stop = "16142";

            apiWebsite = "http://www.ctabustracker.com/bustime/api/v1/getpredictions?key=" + busKey + "&stpid=" + stop;

            //Imports XML data.
            XmlDocument APIDoc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader("http://www.ctabustracker.com/bustime/api/v1/getpredictions?key=MmcntgzUwyMKJrC72nVtabrvC&stpid=16142");
            APIDoc.Load(reader);
            count = APIDoc.ChildNodes[1].SelectNodes("prd").Count;
            reader.Close();

            //Extracts data from XML and stroes in a Bus object
            for (int i = 0; i < count; i++)
            {
                stopName = APIDoc.ChildNodes[1].ChildNodes[i].ChildNodes[2].ChildNodes[0].Value;
                route = APIDoc.ChildNodes[1].ChildNodes[i].ChildNodes[6].ChildNodes[0].Value;
                destination = APIDoc.ChildNodes[1].ChildNodes[i].ChildNodes[8].ChildNodes[0].Value;
                direction = APIDoc.ChildNodes[1].ChildNodes[i].ChildNodes[7].ChildNodes[0].Value;
                estArrivalTime = APIDoc.ChildNodes[1].ChildNodes[i].ChildNodes[9].ChildNodes[0].Value;

                Bus bus = new Bus(stopName, route, destination, direction);

                bus.ArrivalInMinutes = bus.CalculateMinutesToArrival(estArrivalTime);
                bus.PrintString = bus.PrintData();

                buses.Add(bus);

            }

            WriteListToConsole(buses);

            Console.ReadLine();
        }

            //Gets all arriving buses.
            public static void WriteListToConsole(List<Bus> buses)
            {
                foreach (Bus bus in buses)
                {
                    Console.WriteLine(bus.PrintString);
                }
            }

        
    }

    class Bus
    {
        public string Stop { get; set; }
        public string Route { get; set; }
        public string Destination { get; set; }
        public string Direction { get; set; }
        public int ArrivalInMinutes { get; set; }
        public bool IsDelayed { get; set; }
        public string PrintString { get; set; }

        //Constructor
        public Bus(string stop, string route, string destination, string direction)
        {
            Stop = stop;
            Route = route;
            Destination = destination;
            Direction = direction;
            ArrivalInMinutes = 0;
            IsDelayed = false;
        }

        //Calculates the estimated minutes to bus arrival
        public int CalculateMinutesToArrival(string estArrivalTime)
        {
            int estHour;
            int estMinute;
            int curHour;
            int curMinute;

            //Separates hour and minute
            DateTime now = DateTime.Now;
            curHour = Convert.ToInt32(now.ToString("HH"));
            curMinute = now.Minute;

            estHour = Convert.ToInt32(estArrivalTime.Substring(9, 2));
            estMinute = Convert.ToInt32(estArrivalTime.Substring(12, 2));

            if (estHour > curHour)
                estMinute = +60;

            return estMinute - curMinute;
        }

        //Checks if bus is delayed
        public bool IsBusDelayed(int isDelayed)
        {
            if (isDelayed == 1)
                return true;
            return false;
        }

        //Creates a string of train data to display
        public string PrintData()
        {
            return Stop + '\n' + Route + "-" + Direction + '\n' + "To " + Destination + '\n' +
                "Will arrive in " + ArrivalInMinutes + " minutes" + '\n';

        }


    }
}
