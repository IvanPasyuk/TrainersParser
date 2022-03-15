using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrainersParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new WebProxy("127.0.0.1:8888");
            var cookieContainer = new CookieContainer();
            var getRequest = new GetRequest($"https://hrmoscow.ru/trainer")
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
                Useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari / 537.36 Edg / 99.0.1150.39",
                Referer = "https://hrmoscow.ru/trainer",
                Host = "hrmoscow.ru",
                Proxy = proxy
            };

            getRequest.Run(cookieContainer);

            var response = getRequest.Response;
            string trainerStartTag = "\"fio-trainer\">";
            string trainerEndTag = "</h3>";

            SaveTrainersList(GetTrainersList(response, trainerStartTag, trainerEndTag));
        }

        private static List<string> GetTrainersList(string sourceResponce, string startTag, string endTag)
        {
            var trainers = new List<string>();
            while (sourceResponce.IndexOf(startTag) != -1)
            {
                var startIndex = sourceResponce.IndexOf(startTag);
                var currentTrainerStart = startIndex + startTag.Length;
                var currentTrainerEnd = sourceResponce.IndexOf(endTag, currentTrainerStart);
                string trainer = sourceResponce.Substring(currentTrainerStart, currentTrainerEnd - currentTrainerStart).Trim().Replace("<br>", " ");

                trainers.Add(trainer);
                sourceResponce = sourceResponce.Remove(startIndex, sourceResponce.IndexOf(endTag) + endTag.Length - startIndex);
            }

            return trainers;
        }

        private static void SaveTrainersList(List<string> trainersList)
        {
            string jsonString = JsonSerializer.Serialize(trainersList);
            string fileName = @"C:\temp\TrainersList.json";
            File.WriteAllText(fileName, jsonString);
        }
    }
}