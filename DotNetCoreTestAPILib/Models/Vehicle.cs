using DotNetCoreTestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreTestAPILib.Models
{
    /// <summary>
    /// A Vehicle record
    /// </summary>
    public class Vehicle : IVehicle
    {
        /// <summary>
        /// Vehicle Unique Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Make of the vehicle
        /// </summary>
        public string Make { get; set; }
        /// <summary>
        /// The Model of the vehicle
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Year the Vehicle was made
        /// </summary>
        public int Year { get; set; }
    }
}
