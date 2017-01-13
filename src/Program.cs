using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using NearestNeighborhood.Algorithm;
using NearestNeighborhood.Data;
using Newtonsoft.Json;

namespace NearestNeighborhood
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            var dataPath = ConfigurationSettings.AppSettings["dataPath"];
            var resultPath = ConfigurationSettings.AppSettings["resultPath"];

            var dataLoader = new DataLoader(dataPath, 1000);

            stopwatch.Start();

            var userToSongMap = dataLoader.Data;

            stopwatch.Stop();

            Console.WriteLine($"Loaded {nameof(userToSongMap)}  in {stopwatch.ElapsedMilliseconds} ms.");

            Console.ReadKey();

            var knearest = new NearestNeighborhoodAlgorithm(userToSongMap);

            var result = knearest.Find(100);

            WriteResults(result, resultPath);
        }

        private static void WriteResults(Dictionary<string, SortedSet<SimilarUser>> result, string resultPath)
        {
            var jsonResult = JsonConvert.SerializeObject(result);

            using (var stream = File.Create(resultPath))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(jsonResult);
                }
            }
        }
    }
}