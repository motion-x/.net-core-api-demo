using DotNetCoreTestApi.Models;
using DotNetCoreTestAPI.BLL.Interfaces;
using DotNetCoreTestAPI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCoreTestAPILib.BLL
{
    public class VehicleOperations : IVehicleOperations
    {
        private IDataAccessLayer _DAL { get; set; }
        public VehicleOperations(IDataAccessLayer dal)
        {
            _DAL = dal;
        }

        /// <summary>
        /// Fetches all vehicles
        /// </summary>
        /// <returns>A sorted collection of all vehicles</returns>
        public IEnumerable<IVehicle> GetAllVehicles() =>
            _DAL.VehiclesCollection.FindAll()
            .OrderBy(v => v.Make)
            .ThenBy(v => v.Model)
            .ThenBy(v => v.Year);

        /// <summary>
        /// Filters collection based on the given filters (keeps vehicles matching the filterconditions)
        /// </summary>
        /// <param name="vehicles">Vehicle collection</param>
        /// <param name="make">The make of the vehicle (not case sensitive)</param>
        /// <param name="model">The model of the vehicle (not case sensitive)</param>
        /// <param name="year">The year the vehicle was made</param>
        /// <returns>A collection of vehicles matching the provided criteria</returns>
        public IEnumerable<IVehicle> FilterVehicles(IEnumerable<IVehicle> vehicles, string make, string model, int? year)
        {
            // if no filters are present return original collection.
            if (string.IsNullOrEmpty(make) && string.IsNullOrEmpty(model) && year == null)
            {
                return vehicles;
            }

            // filter collection by appling only the filters that are not null
            return vehicles.Where(v =>
                                    v.Make.ToLower() == (make ?? v.Make).ToLower()
                                    && v.Model.ToLower() == (model ?? v.Model).ToLower()
                                    && v.Year == (year ?? v.Year));

        }

        /// <summary>
        /// Fetches a Vehicle with the given Id
        /// </summary>
        /// <param name="id">Id of vehicle to be fetched</param>
        /// <returns></returns>
        public IVehicle GetVehicleById(int id) =>
            _DAL.VehiclesCollection.Find(v => v.Id == id).FirstOrDefault();

        /// <summary>
        /// Add a new vehicle entry. Make and Model cannot be empty or null.
        /// </summary>
        /// <param name="vehicle">Valid vehicle obj</param>
        /// <returns>"Success:id" if insert is successful, error msg otherwise</returns>
        public string AddNewVehicle(IVehicle vehicle)
        {
            return ValidateVehicle(vehicle, (v, msg) =>
            {
                try
                {
                    int id = _DAL.VehiclesCollection.Insert(vehicle);
                    return id > 0 ? $"{msg}:{id}" : "Failed to add vehicle!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return "Failed to add vehicle!";
                }
            });
        }

        /// <summary>
        /// Updates an existing vehicle entry. Make and Model cannot be empty or null.
        /// </summary>
        /// <param name="vehicle">Valid vehicle obj</param>
        /// <returns>"Success:id" if insert is successful, error msg otherwise</returns>
        public string UpdateVehicle(IVehicle vehicle)
        {
            return ValidateVehicle(vehicle, (v, msg) =>
            {
                try
                {
                    return _DAL.VehiclesCollection.Update(vehicle) ? msg : "Failed to update vehicle!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return "Failed to update vehicle!";
                }
            });
        }

        /// <summary>
        /// Delete an existing vehicle.
        /// </summary>
        /// <param name="id">Vehicle id</param>
        /// <returns>True if deleted successfully, False otherwise</returns>
        public bool DeleteVehicle(int id) =>
            _DAL.VehiclesCollection.Delete(id);

        private static string ValidateVehicle(IVehicle vehicle, Func<IVehicle, string, string> doIfValid)
        {
            var message = "Success";

            if (vehicle != null)
            {
                if (string.IsNullOrEmpty(vehicle.Make) || string.IsNullOrEmpty(vehicle.Model))
                {
                    message = "Make and Model cannot be empty!";
                }
                else if (vehicle.Year < 1950 || vehicle.Year > 2050)
                {
                    message = "Vehicle Year must be between 1950 and 2050!";
                }
                else
                {
                    message = doIfValid(vehicle, message);
                }
            }
            else
            {
                message = "Invalid vehicle entry!";
            }

            return message;
        }
    }
}
