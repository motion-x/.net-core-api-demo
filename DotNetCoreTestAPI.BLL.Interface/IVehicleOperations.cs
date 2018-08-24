using DotNetCoreTestApi.Models;
using System;
using System.Collections.Generic;

namespace DotNetCoreTestAPI.BLL.Interfaces
{
    public interface IVehicleOperations
    {
        string AddNewVehicle(IVehicle vehicle);

        bool DeleteVehicle(int id);

        IEnumerable<IVehicle> GetAllVehicles();

        IEnumerable<IVehicle> FilterVehicles(IEnumerable<IVehicle> vehicles, string make, string model, int? year);

        IVehicle GetVehicleById(int id);

        string UpdateVehicle(IVehicle vehicle);
    }
}
