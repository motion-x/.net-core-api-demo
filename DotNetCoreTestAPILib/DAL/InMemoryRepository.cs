using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreTestAPI.DAL.Interfaces;
using LiteDB;

namespace DotNetCoreTestAPILib.DAL
{
    public class InMemoryRepository : IInMemoryRepository
    {
        private MemoryStream _DbMemoryStream { get; set; }

        public LiteDatabase Client { get; set; }
        public string DBName { get; private set; }

        private string _PersistancePath => $"{DBName}.db";

        /// <summary>
        /// Creates simple in-memory LiteDb client. Data is persisted to disk on Dispose.
        /// </summary>
        /// <param name="dbName">Database name</param>
        public InMemoryRepository(string dbName)
        {
            DBName = dbName;

            try
            {
                _DbMemoryStream = Util.ReadFileToStream(_PersistancePath);
            }
            catch
            {
                _DbMemoryStream = new MemoryStream();
            }

            Client = new LiteDatabase(_DbMemoryStream);
        }


        ~InMemoryRepository() => Dispose();

        /// <summary>
        /// Persist data to disk.
        /// </summary>
        public void PersistData()
        {
            try
            {
                if (_DbMemoryStream != null)
                {
                    // simple persistence, write stream data to disk.
                    File.WriteAllBytes(_PersistancePath, _DbMemoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed To Persist db!");
            }
        }

        /// <summary>
        /// Release resources and persist data to disk.
        /// </summary>
        public void Dispose()
        {
            // persist and clean up;
            PersistData();

            Client?.Dispose();
            Client = null;

            _DbMemoryStream?.Dispose();
            _DbMemoryStream = null;

        }
    }
}
