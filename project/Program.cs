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
        static double value = 0.2;
        static int currentTimeFrame; //current time
        static int maxTimeFrame; //closing time
        static int peakTime; //Time with peak amount of incoming visitors;
        static int peakNewVisitors; //peak amount of incoming visitors;
        static int visitorsInQueue; //amount of overall visitors waiting in line
        static int numberOfFoodtrucks;
        static Dictionary<int, FoodTruck> layout; //Foodtrucks and their numbered location in the layout;
        static List<Visitor> visitors;
        static ExcelWriter writer;
        static Random r; //random number generator

        static int incomingVisitors() //incoming visitors on current timeframe
        {
            int deltaPeakTime = Math.Abs(currentTimeFrame - peakTime); //how far away from peaktime it is
            return (int)(peakNewVisitors / (1 + deltaPeakTime * value)); //incoming visitors
        }
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
                        if (r.NextDouble() <= v.chanceOfLeaving())
                        {
                            visitors.Remove(v);                     //Leave
                        }

                        //if at a foodtruck
                        if (layout.ContainsKey(v.location))
                        {
                            FoodTruck truck = layout[v.location]; //Get the current food truck
                            double rndValue = r.NextDouble();
                            if (rndValue <= v.chanceQueueing(truck)) //get in line at the food truck
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
    }
    class FoodTruck
    {
        public double appeal;
        public Queue<Visitor> queue;
        int serviceRate; //amount of customers helped in 1 timeframe
        int location;
        int[] queueArray; //array to store the queue length on each timeframe
        public Dictionary<FoodTruck, int> neighbours; //neighbours with the distance to the neighbour
        public double queueTime()
        {
            return (queue.Count / serviceRate);
        }

        public FoodTruck(int timeframes)
        {
            queueArray = new int[timeframes];
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
        int arrivedTime; // arrived time
        int visitedFoodtrucks; // amount of foodtrucks visited
        double maxQueueTime = 3; //maximum acceptable queuetime for a visitor
        public Dictionary<int, bool> locationsVisited; //location and wether or not he has visited it

        public Visitor(int arrivedTime)
        {
            this.arrivedTime = arrivedTime;
            location = 0;
            visitedFoodtrucks = 0;
            waiting = false;
            locationsVisited = new Dictionary<int, bool>();
        }
        public double chanceOfLeaving()
        {
            double res = 1;
            return res;
        }
        public void GetInLine(FoodTruck truck)
        {
            truck.queue.Enqueue(this);
            waiting = true;
            locationsVisited[location] = true;
        }
        public double chanceQueueing(FoodTruck truck)
        {
            return Math.Max(0, (truck.appeal - truck.appeal * (truck.queueTime() / maxQueueTime)));
        }
    }

}
