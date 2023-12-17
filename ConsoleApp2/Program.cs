using System;
using System.Diagnostics;
using System.Threading;
using System.Text;



public class Program
{
    static int countSymbols = 50;
    static int countThreads = 200;

    static string symbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    static Random r = new Random();
    static char GetRandomChar()
    {
        var index = r.Next(symbols.Length);
        return symbols[index];
    }
    static void Main()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        runThreads();
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result1 = "RunTimeBasic: " + elapsedTime;

        stopWatch.Restart();
        stopWatch.Start();
        runThreadsMutex();
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result2 = "RunTimeMutex: " + elapsedTime;

        stopWatch.Restart();
        stopWatch.Start();
        runThreadsSemaphore();
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result3 = "RunTimeSemaphore: " + elapsedTime;

        stopWatch.Restart();
        stopWatch.Start();
        runThreadsSemaphoreSlim();
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result4 = "RunTimeSemaphoreSlim: " + elapsedTime;

        stopWatch.Restart();
        stopWatch.Start();
        runThreadsBarrier();
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result5 = "RunTimeBarrier: " + elapsedTime;

        stopWatch.Restart();
        stopWatch.Start();
        runThreadsSpinLock();
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result6 = "RunTimeSpinLock: " + elapsedTime;

        stopWatch.Restart();
        stopWatch.Start();
        runThreadsSpinWait();
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result7 = "RunTimeSpinWait: " + elapsedTime;

        stopWatch.Restart();
        stopWatch.Start();
        runThreadsMonitor();
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        string result8 = "RunTimeMonitor: " + elapsedTime;

        Console.WriteLine(result1 + "\n" + result2 + "\n" + result3 + "\n" + result4 + "\n" + result5 + "\n" + result6 + "\n" + result7 + "\n" + result8);
    }



    static object syncObject = new object();
    static void runThreadsMonitor()
    {
        Console.WriteLine("Гонка c монитором");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRaceMonitor1);

            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка с монитором завершена");
    }

    static void RunRaceMonitor1()
    {
        Monitor.Enter(syncObject);
        lock (syncObject)
        {

            int distance = 0;

            char now = GetRandomChar();
            while (distance < countSymbols)
            {
                Console.Write(now);
                distance++;
            }

            Console.WriteLine();


        }
        Monitor.Exit(syncObject);
    }



    static void runThreadsSpinWait()
    {
        Console.WriteLine("Гонка с спинВеит");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRaceSpinWait);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка с спинВеит завершена");
    }

    static void RunRaceSpinWait()
    {
        var spin = new SpinWait();
        int distance = 0;

        char now = GetRandomChar();
        while (distance < countSymbols)
        {
            Console.Write(now);
            distance++;
        }

        Console.WriteLine();
        spin.SpinOnce();
    }





    private static SpinLock spinLock = new SpinLock();
    //private static int sharedVariable = 0;
    static void runThreadsSpinLock()
    {

        Console.WriteLine("Гонка c спинлок");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRaceSpinLock);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка с спинлок завершена");
    }

    static void RunRaceSpinLock
()
    {
        while (true)
        {
            bool lockTaken = false;
            try
            {
                spinLock.Enter(ref lockTaken); // Захват SpinLock

                int distance = 0;

                char now = GetRandomChar();
                while (distance < countSymbols)
                {
                    Console.Write(now);
                    distance++;
                }

                Console.WriteLine();
                break;
            }
            finally
            {
                if (lockTaken)
                {
                    spinLock.Exit(); // Освобождение SpinLock
                }
            }
        }

    }



    static Barrier barrier = new Barrier(countThreads, (bar) =>
    {
        //можно что то написать
    });
    static void runThreadsBarrier()
    {
        Console.WriteLine("Гонка c барьерами");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRaceBarrier
);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка с барьерами завершена");
    }

    static void RunRaceBarrier
()
    {
        int distance = 0;

        char now = GetRandomChar();
        while (distance < countSymbols)
        {
            barrier.SignalAndWait();
            Console.Write(now);
            distance++;
        }

        Console.WriteLine();
    }



    private static SemaphoreSlim semaphore;
    static void runThreadsSemaphoreSlim()
    {
        semaphore = new SemaphoreSlim(3);
        Console.WriteLine("Гонка c семафорами слим");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRaceSemaphoreSlim);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка с семафорами слим завершена");
    }

    static void RunRaceSemaphoreSlim()
    {
        semaphore.Wait();
        int distance = 0;

        char now = GetRandomChar();
        while (distance < countSymbols)
        {
            Console.Write(now);
            distance++;
        }

        Console.WriteLine();
        semaphore.Release();
    }




    static void runThreads()
    {
        Console.WriteLine("Гонка");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRace);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка завершена");
    }

    static void RunRace()
    {
        int distance = 0;

        char now = GetRandomChar();
        while (distance < countSymbols)
        {
            Console.Write(now);
            distance++;
        }

        Console.WriteLine();
    }



    private static Mutex mut = new Mutex();
    static void runThreadsMutex()
    {
        Console.WriteLine("Гонка c мьютексами");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRaceMutex);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка c мьютексами завершена");
    }

    static void RunRaceMutex()
    {
        mut.WaitOne();
        int distance = 0;

        char now = GetRandomChar();
        while (distance < countSymbols)
        {
            Console.Write(now);
            distance++;
        }

        Console.WriteLine();
        mut.ReleaseMutex();
    }




    private static Semaphore _pool;
    static void runThreadsSemaphore()
    {
        _pool = new Semaphore(2, 2);
        Console.WriteLine("Гонка c семафорами");

        int numberOfThreads = countThreads;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i] = new Thread(RunRaceSemaphore);
            threads[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("Гонка c семафорами завершена");
    }

    static void RunRaceSemaphore()
    {
        _pool.WaitOne();
        int distance = 0;
        char now = GetRandomChar();
        while (distance < countSymbols)
        {
            Console.Write(now);
            distance++;
        }

        Console.WriteLine();
        _pool.Release();
    }

}