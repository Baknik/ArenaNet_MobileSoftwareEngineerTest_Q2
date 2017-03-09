using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PriorityQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine("***** Instantiate Priority Queue *****");
            PriorityQueue<object> priorityQueue = new PriorityQueue<object>(255);

            Program.FillObjectQueue(priorityQueue);

            Debug.WriteLine("***** Start Of Priority Queue Tests *****");

            Debug.WriteLine("***** Start Dequeue Min Test *****");
            object dequeueValue;
            Debug.Write("[  ");
            while (priorityQueue.DequeueMin(out dequeueValue))
            {
                Debug.Write(String.Format("{0}  ", dequeueValue));
            }
            Debug.WriteLine("]");
            Debug.WriteLine("----- End Dequeue Min Test -----");

            Program.FillObjectQueue(priorityQueue);

            Debug.WriteLine("***** Start Dequeue Max Test *****");
            Debug.Write("[  ");
            while (priorityQueue.DequeueMax(out dequeueValue))
            {
                Debug.Write(String.Format("{0}  ", dequeueValue));
            }
            Debug.WriteLine("]");
            Debug.WriteLine("----- End Dequeue Max Test -----");

            Program.FillObjectQueue(priorityQueue);

            Debug.WriteLine("***** Start Dequeue Min-Max Test *****");
            bool minStep = true;
            bool result = true;
            Debug.Write("[  ");
            while (result)
            {
                result = (minStep ? priorityQueue.DequeueMin(out dequeueValue) : priorityQueue.DequeueMax(out dequeueValue));
                Debug.Write(String.Format("{0}  ", dequeueValue));
                minStep = !minStep;
            }
            Debug.WriteLine("]");
            Debug.WriteLine("----- End Dequeue Min-Max Test -----");

            Debug.WriteLine("***** Start Enqueue/Dequeue Performance Test *****");
            Stopwatch stopwatch = new Stopwatch();
            int numElements = 999999;
            stopwatch.Start();
            priorityQueue = Program.FillObjectQueueRandom(numElements); // Mass enqueue
            while (priorityQueue.DequeueMax(out dequeueValue)) // Mass dequeue
            {
                // Do nothing, just testing performance of data structure
            }
            stopwatch.Stop();
            Debug.WriteLine(String.Format("Time taken for {0} elements: {1} milliseconds", numElements, stopwatch.ElapsedMilliseconds));
            Debug.WriteLine("----- End Enqueue/Dequeue Performance Test -----");

            Debug.WriteLine("----- End Of Priority Queue Tests -----");
        }

        public static void FillObjectQueue(PriorityQueue<object> queue)
        {
            Debug.WriteLine("***** Begin Fill Queue *****");
            try
            {
                queue.Enqueue("46", 46);
                queue.Enqueue("31", 31);
                queue.Enqueue("51", 51);
                queue.Enqueue("71", 71);
                queue.Enqueue("31", 31);
                queue.Enqueue("10", 10);
                queue.Enqueue("21", 21);
                queue.Enqueue("8", 8);
                queue.Enqueue("13", 13);
                queue.Enqueue("11", 11);
                queue.Enqueue("41", 41);
                queue.Enqueue("16", 16);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            Debug.WriteLine("----- End Fill Queue -----");
        }

        public static PriorityQueue<object> FillObjectQueueRandom(int numElements)
        {
            PriorityQueue<object> queue = new PriorityQueue<object>(numElements);
            Random r = new Random();

            Debug.WriteLine("***** Begin Fill Queue Random *****");
            try
            {
                for (int i = 0; i < numElements; i++)
                {
                    int randomInt = r.Next();
                    queue.Enqueue(randomInt.ToString(), randomInt);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            Debug.WriteLine("----- End Fill Queue Random -----");

            return queue;
        }
    }
}
