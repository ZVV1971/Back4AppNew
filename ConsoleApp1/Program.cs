using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Parse;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Part 1: start the HandleFile method.
            Task<IEnumerable<ParseObject>> task = HandleFileAsync();
            task.Wait();
            var x = task.Result;
            foreach (ParseObject o in x)
            {
                Console.WriteLine("{0} - {1}", o["name"], (o["country"] as ParseObject)["name"]);
            }
            ;
        }

        static async Task<IEnumerable<ParseObject>> HandleFileAsync()
        {
            ParseClient.Initialize(new ParseClient.Configuration
            {
                ApplicationId = "XW0rXBZxcGJcl3zkoWrh3w6bryBpkXnG36CvzOPV",
                WindowsKey = "C3O1Mn02ELUtYpKSBUbfUXf2dOh0snBHR8yoIbx6",
                Server = "https://parseapi.back4app.com/"
            }
            );

            IEnumerable<ParseObject> cities = null;
            List<ParseObject> listOfCities = new List<ParseObject>();

            ParseQuery<ParseObject> query =
                    ParseObject.GetQuery("City")
                    .Include("country");
                    
            int count = await query.CountAsync();
 
            int limit = 1000;
            for (int i = 0; i < count;)
            {
                query =
                    ParseObject.GetQuery("City")
                    .Include("country").Skip(i).Limit(limit);
                cities = await query.FindAsync();
                listOfCities.AddRange(cities.ToList<ParseObject>());
                i += limit;
            }

            return listOfCities;
        }
    }
}