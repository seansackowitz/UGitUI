using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace UGitUI
{
    public static class DataFile
    {
        public static void SaveDataFile()
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, RepositoryManager.Servers);
                }
            }
            catch (IOException) { }
        }

        public static void LoadDataFile()
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    RepositoryManager.Servers = (List<RepositoryServer>)bin.Deserialize(stream);
                }
            }
            catch (IOException) { }
        }
    }
}
