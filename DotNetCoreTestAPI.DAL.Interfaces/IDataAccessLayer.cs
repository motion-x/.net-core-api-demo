using DotNetCoreTestApi.Models;
using LiteDB;
using System;

namespace DotNetCoreTestAPI.DAL.Interfaces
{
    public interface IDataAccessLayer
    {
        LiteCollection<IVehicle> VehiclesCollection { get; }
    }
}