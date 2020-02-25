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
                Console.WriteLine(o["name"]);
            }
            ;
        }

        static async Task<IEnumerable<ParseObject>> HandleFileAsync()
        {
            ParseClient.Initialize(new ParseClient.Configuration
            {
                ApplicationId = "gkkEC0CESNT5L5VVrUi4CydYOiIFVunbKOWT2lPZ",
                WindowsKey = "JtuBujBqkpefaeXOaDmIjX97uncLya5SfYHs9rw9",
                Server = "https://parseapi.back4app.com/"
            }
            );

            IEnumerable<ParseObject> cities = null;
            List<ParseObject> listOfCities = new List<ParseObject>();

            ParseQuery<ParseObject> countryQuery = from country in ParseObject.GetQuery("Continentscountriescities_Country")
                                                   where country.ContainsKey("name")
                                                   select country;

            ParseQuery<ParseObject> query = from city in ParseObject.GetQuery("Continentscountriescities_City")
                                            .Include("country.name")
                                            //join country in countryQuery on city["country"] equals country
                                            where city.ContainsKey("name")
                                            select city;
            int count = await query.CountAsync();
            query.Limit(100);
            cities = await query.FindAsync();
            listOfCities.AddRange(cities.ToList<ParseObject>());
            //for (int i =0; i < count;)
            //{
            //    query.Skip(i);
            //    cities = await query.FindAsync();
            //    listOfCities.AddRange( cities.ToList<ParseObject>());
            //    i += 1000;
            //}
            
            return listOfCities;
        }
    }
}