using Amazon.Runtime;
using Amazon.S3;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeysFromKeePass;
using Mono.Options;
using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string ak = null;
            string sk = null;
            string path = null;
            string group = null;
            string entry = null;
            bool help = false;
            var p = new OptionSet() {
                "This is a sample program to load data from open source DB into my S3 bucket",
                { "p|kbdxpath=", "The {PATH} to the .KBDX file with credentials", v => path = v},
                { "g|group=", "The {GROUP} of credentials where Access and Secret Keys will be taken from", v => group = v},
                { "e|entry=", "The {ENTRY} in the group where needed secret string could be taken from", u => entry = u},
                { "h|?|help",      v => help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (Exception e)
            {
                return;
            }

            if (help || String.IsNullOrEmpty(path)  || !File.Exists(path))
            {
                p.WriteOptionDescriptions(Console.Out);
                Console.ReadKey();
                return;
            }

            Console.Write("Enter password:");
            ProtectedString pwd = new ProtectedString();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd.Remove(pwd.Length - 1, 1);
                    continue;
                }
                pwd = pwd + key.KeyChar.ToString();
            }

            ProtectedStringDictionary dict = KeyFromKeePassClass.GetKeysDict(pwd, path, group, entry, new List<string> { "Access Key", "Secret_ Access Key" }, null);

            if (dict.UCount != 2)
            {
                Console.WriteLine("Provided KeePass database does not contain necessary keys");
                return;
            }

            var myCreds = new BasicAWSCredentials(ak,sk);
            AmazonS3Client S3Cl = new AmazonS3Client(myCreds, Amazon.RegionEndpoint.USEast2);
            var lb= S3Cl.ListBuckets();
 
            // Part 1: start the HandleFile method.
            Task<IEnumerable<ParseObject>> task = HandleFileAsync();
            try
            {
                task.Wait();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            var x = task.Result;
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"D:\DATA\Trainings\Parse\Back4App\WriteLines2.txt", false, new UTF8Encoding(true)))
            {
                file.WriteLine("CityName|CityLatitude|CityLongitude|CountryName|CountryCapital|CountryCode|CountryPhone|CountryCurrency|CountryNativeName");
                foreach (ParseObject o in x)
                {
                    file.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", 
                        o["name"],
                        ((ParseGeoPoint)o["location"]).Latitude,
                        ((ParseGeoPoint)o["location"]).Longitude,
                        ((ParseObject)o["country"])["name"],
                        ((ParseObject)o["country"])["capital"],
                        ((ParseObject)o["country"])["code"],
                        ((ParseObject)o["country"])["phone"],
                        ((ParseObject)o["country"])["currency"],
                        ((ParseObject)o["country"])["native"]
                        );
                }
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