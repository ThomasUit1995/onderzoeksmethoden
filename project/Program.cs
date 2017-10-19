using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    static class Program
    {
        const string excelFilePath = "C:\\Users\\Timo\\Documents\\onderzoeksmethoden\\";
        //const string excelFilePath = "C:\\Users\\Thomas\\Documents\\2017-2018\\p1_ondezoeksmethoden_gametech\\results\\";
        public static double value = 0.2;
        public static int currentTimeFrame; //current time
        public static int maxTimeFrame; //closing time
        public static int peakTime; //Time with peak amount of incoming visitors;
        public static int peakNewVisitors; //peak amount of incoming visitors;
        public static int visitorsInQueue; //amount of overall visitors waiting in line
        public static int numberOfFoodtrucks;
        public static int currentVisitors;
        public static Dictionary<int, FoodTruck> layout; //Foodtrucks and their numbered location in the layout;
        public static List<Visitor> visitors;
        public static ExcelWriter writer;
        public static Random r; //random number generator

        static void Main(string[] args)
        {
            Initialise();
            //TODO: Initialise foodtrucks
            while (currentTimeFrame<maxTimeFrame)
            {
                for (int i = 0; i < incomingVisitors(); i++)
                {
                    visitors.Add(new Visitor(currentTimeFrame));
                }
                foreach(Visitor v in visitors)
                {
                    if (!v.waiting)
                    {
                        //Chance of leaving
                        if (r.NextDouble() <= v.ChanceOfLeaving())
                        {
                            visitors.Remove(v);                     //Leave
                        }

                        //if at a foodtruck
                        if (layout.ContainsKey(v.location))
                        {
                            FoodTruck truck = layout[v.location]; //Get the current food truck
                            double rndValue = r.NextDouble();
                            if (rndValue <= v.ChanceQueueing(truck)) //get in line at the food truck
                                v.GetInLine(truck);
                            else                                     //move somewhere else
                            {
                                double rnd = r.NextDouble();

                            }
                        }
                    }

                }
                currentTimeFrame++;
            }
            Console.ReadLine();
        }

        static void Initialise()
        {
            writer = new ExcelWriter(excelFilePath);
            currentTimeFrame = 0;
            maxTimeFrame = 54;
            peakTime = 30;
            peakNewVisitors = 50;
            currentVisitors = 0;
            visitorsInQueue = 0;
            numberOfFoodtrucks = 10;
            layout = new Dictionary<int, FoodTruck>();
            visitors = new List<Visitor>();
            writer = new ExcelWriter(excelFilePath);
            r = new Random();

        }

        static int incomingVisitors() //incoming visitors on current timeframe
        {
            int deltaPeakTime = Math.Abs(currentTimeFrame - peakTime); //how far away from peaktime it is
            return (int)(peakNewVisitors / (1 + deltaPeakTime * value)); //incoming visitors
        }

        public static double RandomValue()
        {
            return r.Next(1, 101) / 100.0;
        }
    }

    class FoodTruck
    {
        public double appeal;
        public Queue<Visitor> queue;
        public int serviceRate; //amount of customers helped in 1 timeframe
        public int location;
        public int[] queueArray; //array to store the queue length on each timeframe
        public Dictionary<FoodTruck, int> neighbours; //neighbours with the distance to the neighbour

        public FoodTruck(int timeframes)
        {
            queueArray = new int[timeframes];
        }

        public double QueueTime()
        {
            return (queue.Count / serviceRate);
        }

        public void AddNeighbour(FoodTruck truck, int distance)
        {
            neighbours.Add(truck, distance);
        }

    }
    class Visitor
    {
        public bool waiting;
        public int location;
        public int arrivedTime; // arrived time
        public int visitedFoodtrucks; // amount of foodtrucks visited
        public double maxQueueTime; //maximum acceptable queuetime for a visitor
        public int maxTotalTime; //maximum acceptable totaltime at the festival for a visitor
        public Dictionary<int, bool> locationsVisited; //location and wether or not he has visited it

        public Visitor(int arrivedTime)
        {
            this.arrivedTime = arrivedTime;
            location = 0;
            visitedFoodtrucks = 0;
            waiting = false;
            maxQueueTime = MaxTime();
            locationsVisited = new Dictionary<int, bool>();
        }

        public double ChanceOfLeaving()
        {
            return ((double)visitedFoodtrucks / (double)Program.numberOfFoodtrucks) * ((double)(Program.currentTimeFrame - arrivedTime) / (double)maxTotalTime);
        }

        public void GetInLine(FoodTruck truck)
        {
            truck.queue.Enqueue(this);
            waiting = true;
            locationsVisited[location] = true;
        }

        public double ChanceQueueing(FoodTruck truck)
        {
            return Math.Max(0, (truck.appeal - truck.appeal * (truck.QueueTime() / maxQueueTime)));
        }

        public void GoToFoodtruck(FoodTruck truck)
        {
            location = truck.location;
            waiting = true;
            visitedFoodtrucks++;
            //truck.queue++;
        }

        int MaxTime()
        {
            int a10 = 20; // maximum queue times for 10 min: 20%     20 min: 30%
            int a20 = 30; //                         30 min: 40%     40 min: 10%
            int a40 = 10;

            int a40Left = a40 / 2;
            int a40Right = 100 - a40 / 2;
            int a10Left = a40Left + a10 / 2;
            int a10Right = a40Right - a10 / 2;
            int a20Left = a10Left + a20 / 2;
            int a20Right = a10Right - a20 / 2;

            int i = Program.r.Next(1, 101);

            if (i <= a40Left || i > a40Right) return 4;
            else if (i <= a10Left || i > a10Right) return 1;
            else if (i <= a20Left || i > a20Right) return 2;
            else return 3;

            //Anders

            /*int i = Program.r.Next(1, 101);

            if (i <= 20) return 1;
            else if (i <= 50) return 2;
            else if (i <= 90) return 3;
            else return 4;*/
        }
    }
}