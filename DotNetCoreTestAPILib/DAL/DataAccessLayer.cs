using DotNetCoreTestApi.Models;
using DotNetCoreTestAPI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreTestAPILib.DAL
{
    public class DataAccessLayer : IDataAccessLayer
    {
        private const string _VEHICLE_COLLECTION_NAME = "vehicles";

        public DataAccessLayer(IInMemoryRepository repo)
        {
            Repository = repo;
        }

        private IInMemoryRepository Repository { get; set; }

        public LiteDB.LiteCollection<IVehicle> VehiclesCollection =>
                Repository.Client.GetCollection<IVehicle>(_VEHICLE_COLLECTION_NAME);

    }
}
