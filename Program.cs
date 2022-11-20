using System.Diagnostics;

namespace lab7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread th = new Thread(new ThreadStart(Comp.SinInt));
            Thread tl = new Thread(new ThreadStart(Comp.SinInt));

            th.Priority = ThreadPriority.Highest;
            tl.Priority = ThreadPriority.Lowest;
            th.Name = "High 1";
            tl.Name = "Low 0";

            Console.CursorVisible = false;
            Comp.Signal = new AutoResetEvent(true);

            th.Start();
            tl.Start();

            Comp.isntWorkSign.WaitOne();
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 2);
            Console.WriteLine("Нажмите любую клавишу");
            Console.ReadLine();

            Console.Clear();
            Console.CursorVisible=false;
            for(int i = 0; i < 5; ++i)
            {
                Thread t = new Thread(new ThreadStart(Comp.SinInt));
                t.Name = $"Thread {i}";
                t.Start();
            }
            
        }
    }

    internal static class Comp
    {
        public static int count = 0;
        public static AutoResetEvent isntWorkSign = new AutoResetEvent(false);
        public static AutoResetEvent Signal = new AutoResetEvent(true);
        public static Semaphore sem = new Semaphore(2, 2);
        delegate void F(decimal x, decimal Int);
        static event F ItterationEnd = ItterationEndF;
        public static void SinInt()
        {
            sem.WaitOne();
            ++count;
            isntWorkSign.Reset();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            decimal Int = 0;
            decimal step = 0.00001M;
            for (decimal x = 0; x <= 1; x += step)
            {
                Int += (decimal)Math.Sin((double)x) * step;
                //for (int i = 0; i < 100000; ++i) 
                //{
                //    int a;
                //    a = i * i;
                //}

                ItterationEnd.Invoke(x, Int);
            }

            int id = Convert.ToInt32(Thread.CurrentThread.Name.Where(x => char.IsDigit(x)).First().ToString());
            Signal.WaitOne();
            Console.SetCursorPosition(0, id);
            watch.Stop();
            Console.WriteLine($"Поток {Thread.CurrentThread.Name} заершился с результатом {Int} со временем {watch.Elapsed.TotalSeconds}\n\n\n");
            Signal.Set();
            sem.Release();
            --count;
            if (count == 0) isntWorkSign.Set();
        }

        static void ItterationEndF(decimal x, decimal Int)
        {
            Thread currentThread = Thread.CurrentThread;

            int id = Convert.ToInt32(currentThread.Name.Where(x => char.IsDigit(x)).First().ToString());

            string progress = string.Empty;

            decimal i;
            for (i = 0; i < x-0.1m; i += 0.1m) progress += '=';
            if(x != 1) progress += '>';
            for (i += 0.1m ; i < 1; i += 0.1m) progress += ' ';

            //if (process.Length != 10) throw new Exception("fuck " + progress.Length);
            var percent = Math.Round(x, 2) * 100;
            Signal.WaitOne();
            Console.SetCursorPosition(0, id);
            Console.WriteLine($"Поток {currentThread.Name}: [{progress}] {percent}% Результат: {Math.Round(Int, 3)}");
            Signal.Set();
        }
    }
}

