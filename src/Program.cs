using NearestNeighborhood.Algorithm;
using NearestNeighborhood.Data;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

            var listenHistory = dataLoader.Data;

            stopwatch.Stop();

            Console.WriteLine($"Loaded {nameof(listenHistory)}  in {stopwatch.Elapsed.Seconds} s.");

            var similarUseres = 100;

            var nearestNeighborhoodAlgorithm = new NearestNeighborhoodAlgorithm(listenHistory);

            stopwatch.Start();

            var result = nearestNeighborhoodAlgorithm.FindNearest(similarUseres);

            stopwatch.Stop();

            Console.WriteLine($"Found {100} similar users for {similarUseres} users in {stopwatch.Elapsed.Seconds} s.");

            Console.WriteLine($"Write results to file {resultPath}");

            WriteResults(result, resultPath);

            Console.Beep(80, 500);

            Console.ReadKey();
        }

        private static void WriteResults(SimilarUsers result, string resultPath)
        {
            var sortedObject = result.SimilarUsersForUser.ToDictionary(pair => pair.Key,
                pair => pair.Value.OrderByDescending(user => user.Similarity).ToList());

            var jsonResult = JsonConvert.SerializeObject(sortedObject, Formatting.Indented);

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