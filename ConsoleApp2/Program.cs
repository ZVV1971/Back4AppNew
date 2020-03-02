using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib.Interfaces;
using KeePassLib.Serialization;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Collections;
using System.Diagnostics;
using KeePassLib.Security;

namespace ConsoleApp2
{
    class Program
    {
        private static string AWSGroup = "AWS";
        private static string AWSEntry = "AWS-user UZ";

        static void Main(string[] args)
        {
            PwDatabase pwdb = new PwDatabase();
            IOConnectionInfo ioc = IOConnectionInfo.FromPath(@"e:\IIT\Projects\Back4App\ConsoleApp1\ConsoleApp1\Database.kdbx ");
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
            
            CompositeKey compK = new CompositeKey();
            compK.AddUserKey(new KcpPassword(pwd.ReadString()));
            IStatusLogger logger = null;
            pwdb.Open(ioc, compK, logger);
            
            PwEntry pwEntry = null;
            PwObjectList<PwEntry> entries;
            PwObjectList<PwGroup> groups = pwdb.RootGroup.GetGroups(true);
            foreach (PwGroup item in groups)
            {
                Debug.WriteLine(item.Name);
                if (item.Name == AWSGroup)
                {
                    entries = item.GetEntries(true);
                    foreach (PwEntry eitem in entries)
                    {
                        if (eitem.Strings.ReadSafe("Title").Equals(AWSEntry))
                        {
                            Debug.WriteLine(eitem.Strings.ReadSafe("Title"));
                            pwEntry = eitem;
                        }
                    }
                }
            }

            if (pwEntry != null)
            {
                foreach (var item in pwEntry.Strings)
                {
                    Debug.WriteLineIf(item.Key=="Access Key",item.Value.ReadString());
                }
            }
            ;
        }
    }
}