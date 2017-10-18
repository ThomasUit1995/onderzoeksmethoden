using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class Program
    {
        int currentTimeFrame; //current time
        int maxTimeFrame; //closing time
        int peakTime; //Time with peak amount of incoming visitors;
        int peakNewVisitors; //peak amount of incoming visitors;
        int currentVisitors; //amount of current visitors
        int visitorsInQueue;
        int numberOfFoodtrucks;


        double value = 0.2;
        int incomingVisitors() //incoming visitors on current timeframe
        {
            int deltaPeakTime = Math.Abs(currentTimeFrame - peakTime); //how far away from peaktime it is
            return (int)(peakNewVisitors / (1 + deltaPeakTime * value)); //incoming visitors
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    class FoodTruck
    {
        public double appeal;
        public int queue;
        int serviceRate; //amount of customers helped in 1 timeframe
        int location;

        public double queueTime()
        {
            return (queue / serviceRate);
        }

        public FoodTruck()
        {

        }

    }
    class Visitor
    {
        bool waiting;
        int location;
        int arrivedTime; // arrived time
        int visitedFoodtrucks; // amount of foodtrucks visited
        double maxQueueTime = 3; //maximum acceptable queuetime for a visitor

        public Visitor()
        {

        }
        double chanceOfLeaving()
        {
            double res = 1;
            return res;
        }

        public double chanceQueueing(FoodTruck truck)
        {
            return Math.Max(0, (truck.appeal - truck.appeal * (truck.queueTime() / maxQueueTime)));
        }
    }

}
