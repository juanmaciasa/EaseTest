using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EaseChallenge
{
    class Program
    {
        public static int N { get; set; }
        public static int[][] Grid { get; set; }
        public static List<Elevation> GlobalPath { get; set; }
        static void Main(string[] args)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                GlobalPath = new List<Elevation>();
                StreamReader file = new StreamReader("map.txt");
                string bounds = file.ReadLine();
                N = Convert.ToInt32(bounds.Split(' ')[0]);

                Grid = new int[N][];
                string fileText = file.ReadToEnd();
                string[] lines = fileText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                lines = lines.Where(x => x != "").ToArray();
                for (int i = 0; i < N; i++)
                {
                    string line = lines[i];
                    Grid[i] = Array.ConvertAll(line.Split(' '), arr => Convert.ToInt32(arr));
                }
                CalculatePath();
                watch.Stop();
                var elapsed = watch.ElapsedMilliseconds;
                Console.WriteLine("Path Length: " + GlobalPath.Count);
                Console.Write("Path: ");
                foreach (var item in GlobalPath)
                {
                    Console.Write(item.Value + " ");
                }
                Console.WriteLine();
                Console.WriteLine("Drop: " + (GlobalPath.FirstOrDefault().Value - GlobalPath.LastOrDefault().Value));
                Console.WriteLine("Elapsed time: " + Convert.ToDouble(elapsed) / 1000 + "s");
                Console.ReadLine();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Coordinate[] Coordinates =
        {
            new Coordinate() {X = 1, Y= 0 },
            new Coordinate() {X = -1, Y= 0 },
            new Coordinate() {X = 0, Y= 1 },
            new Coordinate() {X = 0, Y= -1 }
        };

        private static void CalculatePath()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    ValidateValue(Grid[i][j], i, j, new List<Elevation>() { new Elevation() { Value = Grid[i][j], X = i, Y = j } });
                }
            }
        }

        private static void ValidateValue(int val, int x, int y, List<Elevation> path)
        {
            for (int i = 0; i < Coordinates.Length; i++)
            {
                if (path.LastOrDefault().Value == val && path.LastOrDefault().X == x && path.LastOrDefault().Y == y)
                {
                    Validation(i, val, x, y, path);
                }
                else
                {
                    int index = path.IndexOf(path.Where(obj => obj.Value == val && obj.X == x && obj.Y == y).FirstOrDefault());
                    if (index > 0)
                    {
                        index++;
                        path.RemoveRange(index, path.Count - index);
                    }
                    else if (index == 0)
                        path.RemoveRange(1, path.Count - 1);

                    Validation(i, val, x, y, path);
                }
            }

            ValidateGlobalPath(path);
        }

        private static void Validation(int i, int val, int x, int y, List<Elevation> path)
        {
            int x2 = x + Coordinates[i].X;
            int y2 = y + Coordinates[i].Y;
            if (x2 >= 0 && y2 >= 0 && x2 < N && y2 < N)
            {
                if (val > Grid[x2][y2])
                {
                    path.Add(new Elevation() { Value = Grid[x2][y2], X = x2, Y = y2 });
                    ValidateValue(Grid[x2][y2], x2, y2, path);
                }
            }
        }

        private static void ValidateGlobalPath(List<Elevation> path)
        {
            if (GlobalPath == null)
            {
                GlobalPath = path.ToList();
            }
            else if (GlobalPath.Count < path.Count)
            {
                GlobalPath = path.ToList();
            }
            else if (GlobalPath.Count == path.Count)
            {
                int firstG = GlobalPath.FirstOrDefault().Value;
                int lastG = GlobalPath.LastOrDefault().Value;
                int firstP = path.FirstOrDefault().Value;
                int lastP = path.LastOrDefault().Value;
                if ((firstG - lastG) < (firstP - lastP))
                {
                    GlobalPath = path.ToList();
                }
            }
        }
    }
}
