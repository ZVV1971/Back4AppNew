using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Interfaces;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeysFromKeePass
{
    public static class KeyFromKeePassClass
    {
        /// <summary>
        /// Opens KeePass database file .kdbx, using password pwd;
        /// searchs for groupName, finds the entry 
        /// and then iterates through all strings in the entry
        /// to find keys, listed in the listOfKeys
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="path"></param>
        /// <param name="groupName"></param>
        /// <param name="entryName"></param>
        /// <param name="listOfKeys"></param>
        /// <param name="logger"></param>
        /// <returns>dictionary of protected strings</returns>
        public static ProtectedStringDictionary GetKeysDict(
            ProtectedString pwd,
            string path,
            string groupName,
            string entryName,
            IEnumerable<string> listOfKeys,
            IStatusLogger logger)
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
                            //if required entry is found
                            if (eitem.Strings.ReadSafe("Title").Equals(entryName))
                            {
                                //for each requested key try to find hit among the strings
                                listOfKeys.ToList().ForEach((string x) => 
                                {
                                    //and if there's such non-empty coincidence
                                    //add it to the protected dictionary
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