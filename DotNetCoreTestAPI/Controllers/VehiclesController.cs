using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreTestAPILib.Models;
using DotNetCoreTestAPI.BLL.Interfaces;

namespace DotNetCoreTestAPI.Controllers
{
    [Route("api/[controller]")]
    public class VehiclesController : Controller
    {
        // Get an instance of VehicleOperations through DI
        private IVehicleOperations _VehicleOperations { get; set; }
        public VehiclesController(IVehicleOperations bal)
        {
            _VehicleOperations = bal;
        }

        // GET api/values
        /// <summary>
        /// Fetches all vehicles stored in the db.
        /// </summary>
        /// <param name="model">Optional Query parameter used to filter by model</param>
        /// <param name="make">Optional Query parameter used to filter by make</param>
        /// <param name="year">Optional Query parameter used to filter by year</param>
        /// <returns>All vehicles filtered by provided criteria >> IEnumerable&lt;Vehicle&gt;</returns>
        [HttpGet]
        public IActionResult Get(string model, string make, int? year)
        {
            var allVehicles = _VehicleOperations.GetAllVehicles();
            var filteredVehicles = _VehicleOperations.FilterVehicles(allVehicles, make, model, year);
            return Ok(filteredVehicles);
        }

        // GET api/vehicles/5
        /// <summary>
        /// Fetches a particular vehicle by its id.
        /// </summary>
        /// <param name="id">The id of the vehicle to be fetched</param>
        /// <returns>The vehicle for the given id. >> Vehicle</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var vehicle = _VehicleOperations.GetVehicleById(id);
            if (vehicle != null)
            {
                return Ok(vehicle);
            }
            else
            {
                return NoContent();
            }

        }

        // POST api/vehicles
        /// <summary>
        /// Adds a new vehicle entry
        /// </summary>
        /// <param name="vehicle">The vehicle to be added</param>
        /// <returns>The vehicle with it's assigned id >> Vehicle</returns>
        [HttpPost]
        public IActionResult Post([FromBody]Vehicle vehicle)
        {
            var msg = _VehicleOperations.AddNewVehicle(vehicle);

            if (msg.Contains("Success"))
            {
                try
                {
                    // parse the vehicle id and grab the record.
                    vehicle.Id = int.Parse(msg.Split(':')[1]);

                    return Created($"api/vehicle/{vehicle.Id}", vehicle);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return BadRequest(msg);
        }

        // PUT api/vehicles/5
        /// <summary>
        /// Uptades an existing vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to be updated</param>
        /// <returns>"Success" if update was successful</returns>
        [HttpPut]
        public IActionResult Put([FromBody]Vehicle vehicle)
        {
            var msg = _VehicleOperations.UpdateVehicle(vehicle);

            if (msg.Contains("Success"))
            {
                return Ok(msg);
            }
            else
            {
                return BadRequest(msg);
            }
        }

        // DELETE api/vehicles/5
        /// <summary>
        /// Deletes a vehicle with the given id
        /// </summary>
        /// <param name="id">The id of the vehicle to be deleted</param>
        /// <returns>True if deleted successfully</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_VehicleOperations.DeleteVehicle(id))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
