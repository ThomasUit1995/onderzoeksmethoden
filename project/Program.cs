using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    static class Program
    {
        const string excelFilePath = "D:\\onderzoeksmethoden\\project\\";
        public const int numberOfFoodtrucks = 8;
        public const int maxQueueTime = 20;
        public const int amountOfSimulations = 100;
        public const int peakNewVisitors = 10; //peak amount of incoming visitors;
        public static int currentsimulation = 0;
        //const string excelFilePath = "C:\\Users\\Thomas\\Documents\\2017-2018\\p1_ondezoeksmethoden_gametech\\results\\";
        public static int currentTimeFrame; //current time
        public static int maxTimeFrame; //closing time
        public static int peakTime; //Time with peak amount of incoming visitors;
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
                    if (!v.waiting)
                    {
                        //Chance of leaving (early/without having passed all foodtrucks)
                        if (r.NextDouble() <= v.ChanceOfLeaving())
                        {
                            visitors.Remove(v);                             //Leave
                        }
                        if (!v.isEating)
                        {
                            FoodTruck truck = layout[v.location];           //Food truck at th current location
                            if (r.NextDouble() <= v.ChanceQueueing(truck) && !v.locationsVisited[v.location] && truck.QueueTime() < maxTimeFrame - currentTimeFrame) //chance to get in line if u haven't passed this food truck yet
                            {
                                v.GetInLine(truck); //get in line at the food truck
                                truck.gotInLine[currentTimeFrame]++;  //got in line because of combination appeal + queuetime
                            }      
                            else
                            {
                                if (!v.locationsVisited[v.location])
                                {
                                    truck.didntGetInLine[currentTimeFrame]++;    //didnt get in line because of combination appeal + queuetime
                                }
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
                        else { v.ProgressEating(); } //eat
                    }
                    

                }
                foreach (KeyValuePair<int, FoodTruck> k in layout)
                {
                    k.Value.ProgressServing();
                }
                currentTimeFrame++;
            }
           
            /*foreach (KeyValuePair<int, FoodTruck> k in layout)
            {
                Console.WriteLine("APPEAL: {0} LOCATION: {1}", k.Value.appeal, k.Value.location);
                for (int i = 0; i < maxTimeFrame; i++)
                {
                    int gotInLine = k.Value.gotInLine[i];
                    int didntGetInLine = k.Value.didntGetInLine[i];
                    double percentage;
                    if (didntGetInLine+gotInLine == 0)
                        percentage = 0;
                    else percentage = (double)gotInLine / (double)(gotInLine+didntGetInLine);
                    Console.WriteLine("GOTINLINE: {0} DIDNTGETINLINE: {1} QUEUETIME:{2} APPEAL: {3}", gotInLine,didntGetInLine, k.Value.queueArray[i],k.Value.actualAppeal[i]);
                }
                Console.WriteLine("-----------------------------------------------------------------------------");
            }*/
            writer.OpenWorksheet();
            foreach(KeyValuePair<int,FoodTruck> k in layout)
            {
                writer.WriteArrays(k.Value);
            }
            currentsimulation++;
            writer.SaveSheet();
            //while (true)
            {

            }
        }

        static void Initialise()
        {
            writer = new ExcelWriter(excelFilePath);
            currentTimeFrame = 0;
            maxTimeFrame = 480;
            peakTime = 280;
            layout = new Dictionary<int, FoodTruck>();
            visitors = new List<Visitor>();
            r = new Random();
            InitialiseFoodTrucks();
        }
        static void InitialiseFoodTrucks()
        {
            for(int i = 0; i < numberOfFoodtrucks; i++)
            {
                FoodTruck truck = new FoodTruck(i, 0.3);
                layout.Add(i, truck);
            }
            InitialiseLayout();
        }
        //Initialise the layout of neighbours
        static void InitialiseLayout()
        {
            layout[0].neighbours.Add(1); layout[0].neighbours.Add(2);
            layout[1].neighbours.Add(0); layout[1].neighbours.Add(3);
            layout[2].neighbours.Add(0); layout[2].neighbours.Add(3); layout[2].neighbours.Add(4); layout[2].neighbours.Add(5);
            layout[3].neighbours.Add(1); layout[3].neighbours.Add(2); layout[3].neighbours.Add(4); layout[3].neighbours.Add(7);
            layout[4].neighbours.Add(2); layout[4].neighbours.Add(3); layout[4].neighbours.Add(6); layout[4].neighbours.Add(7);
            layout[5].neighbours.Add(2); layout[5].neighbours.Add(6);
            layout[6].neighbours.Add(4); layout[6].neighbours.Add(5); layout[6].neighbours.Add(7);
            layout[7].neighbours.Add(3); layout[7].neighbours.Add(4); layout[7].neighbours.Add(6);
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
        public int serviceRate; //time after which customers are helped
        public int servedPerRate; //amount of customers served every service time
        public int location;
        public int[] queueArray; //array to store the queue length on each timeframe
        public List<int> neighbours; //neighbours with the distance to the neighbour
        public int[] gotInLine;
        public int[] didntGetInLine;
        public double[] actualAppeal; //appeal with queuesize taken into account
        public FoodTruck(int location, double appeal)
        {
            queueArray = new int[Program.maxTimeFrame];
            gotInLine = new int[Program.maxTimeFrame];
            didntGetInLine = new int[Program.maxTimeFrame];
            actualAppeal = new double[Program.maxTimeFrame];
            servingProgress = 0;
            this.location = location;
            this.appeal = appeal;
            neighbours = new List<int>();
            serviceRate = 1;
            servedPerRate = 2;
            queue = new Queue<Visitor>();
        }

        public double QueueTime()
        {
            return (queue.Count * (serviceRate/servedPerRate));
        }

        public void AddNeighbour(int trucklocation)
        {
            neighbours.Add(trucklocation);
        }
        public void ProgressServing()
        {
            queueArray[Program.currentTimeFrame] = queue.Count;     //store the queue size
            writeAppeal();                                          //store appeal
            if (queue.Count != 0)                                   //continue serving
            {
                servingProgress++;                      
                if (servingProgress == serviceRate)
                {
                    servingProgress = 0;
                    for (int i = 0; i < servedPerRate; i++)
                    {
                        if (queue.Count > 0)
                        {
                            Visitor v = queue.Dequeue();
                            v.waiting = false;
                            v.isEating = true;
                        }
                    }
                }
            }
        }
        void writeAppeal()
        {
            if (QueueTime() >= Program.maxQueueTime)
                actualAppeal[Program.currentTimeFrame] = 0;
            else if (QueueTime() >= Program.maxQueueTime/2)
            {
                actualAppeal[Program.currentTimeFrame] = Math.Max(0, (appeal - (0.5 * appeal * (QueueTime() / 20))));
            }
            else actualAppeal[Program.currentTimeFrame]= appeal;
        }
    }
    class Visitor
    {
        public bool waiting;
        public int location;
        public int arrivedTime; // arrived time
        public int visitedFoodtrucks; // amount of foodtrucks visited
        public int maxTotalTime; //maximum acceptable totaltime at the festival for a visitor
        public bool[] locationsVisited; //location and wether or not he has visited it
        public bool isEating;
        int eatingProgress;
        const int eatingTime = 7; //takes 7 minutes to eat
        public Visitor(int arrivedTime)
        {
            this.arrivedTime = arrivedTime;
            location = 0;
            visitedFoodtrucks = 0;
            waiting = false;
            locationsVisited = new bool[Program.numberOfFoodtrucks];
            isEating = false;
            eatingProgress = 0;
            maxTotalTime = 240;
        }

        public double ChanceOfLeaving()
        {
            double closingProgress = (Program.currentTimeFrame / Program.maxTimeFrame);
            if (closingProgress > 0.9)
            {
                return (0.5*closingProgress)+((double)visitedFoodtrucks / (double)Program.numberOfFoodtrucks) * ((double)(Program.currentTimeFrame - arrivedTime) / (double)maxTotalTime);
            }
            return ((double)visitedFoodtrucks / (double)Program.numberOfFoodtrucks);
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
            if (truck.QueueTime() >= Program.maxQueueTime)
                return 0;
            if (truck.QueueTime() >= 10)
            {
                return Math.Max(0, (truck.appeal - (0.5*truck.appeal * (truck.QueueTime() / Program.maxQueueTime))));
            }
            return truck.appeal;
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