using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace ConsoleApplication1
{
    internal static class Program
    {
        public static void Main()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            var dbContext = new DatabaseContext(connection);

            var client = new WebClient();
            const string url = "https://www.bbc.com/sport/formula1/drivers-world-championship/standings";

            try
            {
                string htmlContent = client.DownloadString(url);

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var id = "u4068668372419452";
                for (int i = 1; i < 23; i++)
                {
                    Racer racer = new Racer {RacerId = Guid.NewGuid().ToString()};

                    HtmlNode nameNode = htmlDoc.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"{id}\"]/div/div[2]/div/div/div/div[2]/table/tbody/tr[{i}]/td[2]/span/span[1]");
                    if (nameNode != null)
                    {
                        racer.Name = nameNode.InnerText;
                    }

                    HtmlNode teamNode = htmlDoc.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"{id}\"]/div/div[2]/div/div/div/div[2]/table/tbody/tr[{i}]/td[3]/span/span[1]");
                    if (teamNode != null)
                    {
                        racer.Team = teamNode.InnerText;
                    }

                    HtmlNode winsNode = htmlDoc.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"{id}\"]/div/div[2]/div/div/div/div[2]/table/tbody/tr[{i}]/td[4]/span");
                    if (winsNode != null)
                    {
                        racer.Wins = int.Parse(winsNode.InnerText);
                    }

                    HtmlNode pointsNode = htmlDoc.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"{id}\"]/div/div[2]/div/div/div/div[2]/table/tbody/tr[{i}]/td[5]/span");
                    if (pointsNode != null)
                    {
                        racer.Points = int.Parse(pointsNode.InnerText);
                    }

                    dbContext.Racers.Add(racer);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                RunEngine(dbContext);
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void RunEngine(DatabaseContext dbContext)
        {
            RenderHeader("All Racers...");

            var racers = dbContext.Racers.OrderBy(racer => racer.Name).ToList();
            foreach (var racer in racers)
            {
                Console.WriteLine(RenderRacer(racer));
            }

            RenderHeader("Eliminate Racers with zero points or wins...");

            racers = racers.Where(racer => racer.Points > 0 && racer.Wins > 0).ToList();
            foreach (var racer in racers)
            {
                Console.WriteLine(RenderRacer(racer));
            }
        }

        private static void RenderHeader(string allRacers)
        {
            Console.WriteLine("");
            Console.WriteLine(allRacers);
            Console.WriteLine("");
        }

        private static string RenderRacer(Racer racer)
        {
            return $"{racer.Name} / {racer.Team} / W:{racer.Wins} - Points:{racer.Points}";
        }
    }
}