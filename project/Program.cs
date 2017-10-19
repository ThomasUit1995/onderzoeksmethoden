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
        public const int numberOfFoodtrucks = 7;
        public static readonly double[] appeals = { 0.4,0.8,0.55,0.6,0.7,0.65,0.3 };
        //const string excelFilePath = "C:\\Users\\Thomas\\Documents\\2017-2018\\p1_ondezoeksmethoden_gametech\\results\\";
        public static double value = 0.2;
        public static int currentTimeFrame; //current time
        public static int maxTimeFrame; //closing time
        public static int peakTime; //Time with peak amount of incoming visitors;
        public static int peakNewVisitors; //peak amount of incoming visitors;
        public static int visitorsInQueue; //amount of overall visitors waiting in line
        public static Dictionary<int, FoodTruck> layout; //Foodtrucks and their numbered location in the layout;
        public static List<Visitor> visitors;
        public static ExcelWriter writer;
        public static Random r; //random number generator

        static void Main(string[] args)
        {
            Initialise();
            //TODO: Initialise foodtrucks
            while (currentTimeFrame < maxTimeFrame)
            {
                for (int i = 0; i < incomingVisitors(); i++)
                {
                    visitors.Add(new Visitor(currentTimeFrame));
                }
                foreach (Visitor v in visitors.ToList())
                {
                    //if not waiting
                    if (!v.isEating)
                    {
                        if (!v.waiting)
                        {
                            //Chance of leaving (early/without having passed all foodtrucks)
                            if (r.NextDouble() <= v.ChanceOfLeaving())
                            {
                                visitors.Remove(v);                             //Leave
                            }
                            FoodTruck truck = layout[v.location];           //Food truck at th current location
                            if (r.NextDouble() <= v.ChanceQueueing(truck) && !v.locationsVisited[v.location]) //chance to get in line if u haven't passed this food truck yet
                                v.GetInLine(truck);                 //get in line at the food truck
                            else
                            {
                                v.locationsVisited[v.location] = true;
                                List<int> directDestinations = new List<int>(); // all not visited neighbours
                                List<int> indirectDestinations = new List<int>();// all neighbours with not visited neighbours
                                foreach (int l in truck.neighbours) //for every neighbour
                                {
                                    if (!v.locationsVisited[l])     //if not visited add them to directdestinations
                                        directDestinations.Add(l);
                                    else
                                    {
                                        foreach (int j in layout[l].neighbours) //if neighbour has neighbours that arent visited, add neighbour to indirect destinations
                                        {
                                            if (!v.locationsVisited[j])
                                                indirectDestinations.Add(l);
                                        }
                                    }
                                }
                                if (directDestinations.Count == 0)                               //if all direct neighbours visited, go to a neighbour who has not visited neighbours
                                {
                                    if (indirectDestinations.Count == 0)                        //if no neighbours have not visited neighbours then leave the festival(seen it all(at least for this layout))
                                        visitors.Remove(v);
                                    else
                                    {
                                        int rnd = r.Next(0, indirectDestinations.Count);
                                        v.location = indirectDestinations[rnd];   //move towards indirectly not visited neighbour
                                    }
                                }
                                else
                                {
                                    int rnd = r.Next(0, directDestinations.Count);
                                    v.location = directDestinations[rnd];   //move to directly not visited neighbour
                                }
                            }
                        }
                    }
                    else { v.ProgressEating(); } //eat

                }
                foreach (KeyValuePair<int, FoodTruck> k in layout)
                {
                    k.Value.ProgressServing();
                }
                currentTimeFrame++;
            }
            foreach (KeyValuePair<int, FoodTruck> k in layout)
            {
                Console.WriteLine("APPEAL: {0} LOCATION: {1}", k.Value.appeal,k.Value.location);
                foreach (int i in k.Value.queueArray)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("-----------------------------------------------------------------------------");
            }
            while (true)
            {

            }
        }

        static void Initialise()
        {
            writer = new ExcelWriter(excelFilePath);
            currentTimeFrame = 0;
            maxTimeFrame = 480;
            peakTime = 280;
            peakNewVisitors = 10;
            visitorsInQueue = 0;
            layout = new Dictionary<int, FoodTruck>();
            visitors = new List<Visitor>();
            writer = new ExcelWriter(excelFilePath);
            r = new Random();
            InitialiseFoodTrucks();
        }
        static void InitialiseFoodTrucks()
        {
            bool[] valueAssigned = new bool[numberOfFoodtrucks]; //if appeal was assigned already
            int initialised_trucks = 0;
            double increment = 1 / numberOfFoodtrucks;
            while (initialised_trucks < numberOfFoodtrucks)
            {
                int rnd = r.Next(0, 7);
                if (!valueAssigned[rnd])
                {
                    FoodTruck truck = new FoodTruck(initialised_trucks, appeals[rnd]);
                    layout.Add(initialised_trucks, truck);
                    valueAssigned[rnd] = true;
                    initialised_trucks++;
                }
            }
            InitialiseLayout();
        }
        static void InitialiseLayout()
        {
            layout[0].neighbours.Add(1); layout[0].neighbours.Add(2);
            layout[1].neighbours.Add(0); layout[1].neighbours.Add(3);
            layout[2].neighbours.Add(0); layout[2].neighbours.Add(3); layout[2].neighbours.Add(4); layout[2].neighbours.Add(5);
            layout[3].neighbours.Add(1); layout[3].neighbours.Add(2); layout[3].neighbours.Add(4);
            layout[4].neighbours.Add(2); layout[4].neighbours.Add(3); layout[4].neighbours.Add(6);
            layout[5].neighbours.Add(2); layout[5].neighbours.Add(6);
            layout[6].neighbours.Add(4); layout[6].neighbours.Add(5);
        }
        static int incomingVisitors() //incoming visitors on current timeframe
        {
            int deltaPeakTime = Math.Abs(currentTimeFrame - peakTime); //how far away from peaktime it is
            return (int)(peakNewVisitors / (1 + deltaPeakTime * 0.0125)); //incoming visitors
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
        int servingProgress;
        public int serviceRate; //amount of customers helped in 1 timeframe
        public int location;
        public int[] queueArray; //array to store the queue length on each timeframe
        public List<int> neighbours; //neighbours with the distance to the neighbour

        public FoodTruck(int location, double appeal)
        {
            queueArray = new int[Program.maxTimeFrame];
            servingProgress = 0;
            this.location = location;
            this.appeal = appeal;
            neighbours = new List<int>();
            serviceRate = 1;
            queue = new Queue<Visitor>();
        }

        public double QueueTime()
        {
            return (queue.Count * serviceRate);
        }

        public void AddNeighbour(int trucklocation)
        {
            neighbours.Add(trucklocation);
        }
        public void ProgressServing()
        {
            queueArray[Program.currentTimeFrame] = queue.Count;     //store the queue size
            if (queue.Count != 0)                                   //continue serving
            {
                servingProgress++;                      
                if (servingProgress == serviceRate)
                {
                    servingProgress = 0;
                    Visitor v = queue.Dequeue();
                    v.waiting = false;
                    v.isEating = true;
                }
            }
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
        public bool[] locationsVisited; //location and wether or not he has visited it
        public bool isEating;
        int eatingProgress;
        const int eatingTime = 5; //takes 5 minutes to eat
        public Visitor(int arrivedTime)
        {
            this.arrivedTime = arrivedTime;
            location = 0;
            visitedFoodtrucks = 0;
            waiting = false;
            maxQueueTime = MaxTime();
            locationsVisited = new bool[Program.numberOfFoodtrucks];
            isEating = false;
            eatingProgress = 0;
            maxTotalTime = 240;
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
        public void ProgressEating()
        {
            if (isEating)
            {
                eatingProgress++;
                if (eatingProgress == eatingTime)
                {
                    eatingProgress = 0;
                    isEating = false;
                }
            }
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

            if (i <= a40Left || i > a40Right) return 40;
            else if (i <= a10Left || i > a10Right) return 10;
            else if (i <= a20Left || i > a20Right) return 20;
            else return 30;

            //Anders

            /*int i = Program.r.Next(1, 101);

            if (i <= 20) return 1;
            else if (i <= 50) return 2;
            else if (i <= 90) return 3;
            else return 4;*/
        }
    }
}