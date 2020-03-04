using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeePassLib.Keys;
using KeePassLib.Interfaces;
using KeePassLib.Collections;

namespace KeysFromKeePass
{
    public static class KeyFromKeePassClass
    {
        public static ProtectedStringDictionary GetKeysDict(ProtectedString pwd, string path, string groupName, string entryName
            , IEnumerable<string> listOfKeys, IStatusLogger logger)
        {
            //Checks
            if (!File.Exists(path))
                throw new IOException("File " + path + " does not exist");
            if (pwd == null)
                throw new ArgumentNullException("pwd", "Password cannot be null");

            IOConnectionInfo ioc = IOConnectionInfo.FromPath(path);

            CompositeKey compK = new CompositeKey();
            compK.AddUserKey(new KcpPassword(pwd.ReadString()));
            PwDatabase pwdb = new PwDatabase();

            ProtectedStringDictionary dict = new ProtectedStringDictionary();

            try
            {
                pwdb.Open(ioc, compK, logger);
                PwObjectList<PwGroup> groups = pwdb.RootGroup.GetGroups(true);
                foreach (PwGroup item in groups)
                {
                    if (item.Name == groupName)
                    {
                        PwObjectList<PwEntry> entries = item.GetEntries(true);
                        foreach (PwEntry eitem in entries)
                        {
                            if (eitem.Strings.ReadSafe("Title").Equals(entryName))
                            {
                                listOfKeys.ToList().ForEach((string x) => 
                                {
                                    if (!string.IsNullOrEmpty(eitem.Strings.GetKeys().FirstOrDefault(u=> u==x)))
                                    {
                                        dict.Set(x,eitem.Strings.GetSafe(x));
                                    }
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            

            return dict;
            }
    }
}