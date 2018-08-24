using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using LiteDB;
using System.IO;
using DotNetCoreTestAPILib.Models;
using DotNetCoreTestAPILib.BLL;
using System.Linq;
using Newtonsoft.Json;
using DotNetCoreTestAPI.DAL.Interfaces;
using DotNetCoreTestApi.Models;

namespace DotNetCoreTestAPILib.Tests
{
    public class VehicleOperationsTests
    {
        [Fact]
        public void Should_Return_All_Vehicles()
        {

            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);

                // Act
                var vehicles = vo.GetAllVehicles().ToList();

                // Assert
                Assert.Equal(TestVehicles.Count, vehicles.Count);

                Assert.NotNull(vehicles.Where(x => x.Make == "VW").FirstOrDefault());
            });
        }

        [Theory]
        [MemberData(nameof(GetVehiclesWithFilters_Data))]
        public void Should_Pass_If_Vehicle_Filters_Work(IEnumerable<Vehicle> vehicles, string make, string model, int? year, int recordCount)
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);

                // Act
                var _vehicles = vo.GetAllVehicles();

                var result = vo.FilterVehicles(vehicles, make, model, year).ToList();

                // Assert
                Assert.Equal(recordCount, result.Count);
            });
        }

        [Theory]
        [MemberData(nameof(GetById_Data))]
        public void Should_Return_A_Vehicle_By_Id(int id, Vehicle expVehicle)
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);

                // Act
                var vehicle = vo.GetVehicleById(id);

                // Assert
                Assert.NotNull(vehicle);

                var exp = JsonConvert.SerializeObject(expVehicle);
                var result = JsonConvert.SerializeObject(vehicle);

                Assert.Equal(exp, result);
            });
        }

        [Theory]
        [MemberData(nameof(NullOrEmptyMakeModel_Data))]
        public void AddNewVehicle_Should_Pass_If_Make_And_Model_Null_OR_Empty_AND_Year_Validation_Works(Vehicle testVehicle, string result)
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);

                // Act
                var _result = vo.AddNewVehicle(testVehicle);

                // Assert

                Assert.Equal(result, _result);
            });
        }

        [Theory]
        [MemberData(nameof(NullOrEmptyMakeModel_Data))]
        public void UpdateVehicle_Should_Pass_If_Make_And_Model_Null_OR_Empty_AND_Year_Validation_Works(Vehicle testVehicle, string result)
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);

                // Act
                var _result = vo.UpdateVehicle(testVehicle);

                // Assert

                Assert.Equal(result, _result);
            });
        }

        [Fact]
        public void Should_Pass_If_New_Vehicle_Is_Added()
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);
                var newVehicle = new Vehicle
                {
                    Make = "Porsche",
                    Model = "911 Turbo",
                    Year = 2018
                };

                // Act
                var result = vo.AddNewVehicle(newVehicle);

                // Assert
                Assert.Contains("Success", result);

                Assert.NotNull(db.GetCollection<IVehicle>().Find(v => v.Make == newVehicle.Make && v.Model == newVehicle.Model).FirstOrDefault());
            });
        }

        [Fact]
        public void Should_Pass_If_Fails_To_Add_Record_With_Existing_Id()
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);
                var newVehicle = new Vehicle
                {
                    Id = 2,
                    Make = "Porsche",
                    Model = "911 Turbo",
                    Year = 2018
                };

                // Act
                var result = vo.AddNewVehicle(newVehicle);

                // Assert
                Assert.Equal("Failed to add vehicle!", result);

                Assert.Null(db.GetCollection<IVehicle>().Find(v => v.Make == newVehicle.Make && v.Model == newVehicle.Model && v.Id == newVehicle.Id).FirstOrDefault());
            });
        }

        [Fact]
        public void Should_Pass_If_Vehicle_Record_Is_Updated_Successfully()
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);
                var newVehicle = new Vehicle
                {
                    Id = 2,
                    Make = "Porsche",
                    Model = "911 Turbo",
                    Year = 2018
                };

                // Act
                var result = vo.UpdateVehicle(newVehicle);

                // Assert
                Assert.Contains("Success", result);

                Assert.NotNull(db.GetCollection<IVehicle>().Find(v => v.Make == newVehicle.Make && v.Model == newVehicle.Model && v.Id == newVehicle.Id).FirstOrDefault());
            });
        }

        [Fact]
        public void Should_Pass_If_Fails_To_Update_Non_Existing_Record()
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);
                var newVehicle = new Vehicle
                {
                    Id = TestVehicles.Count * 2,
                    Make = "Porsche",
                    Model = "911 Turbo",
                    Year = 2018
                };

                // Act
                var result = vo.UpdateVehicle(newVehicle);

                // Assert
                Assert.Equal("Failed to update vehicle!", result);

                Assert.Null(db.GetCollection<IVehicle>().Find(v => v.Make == newVehicle.Make && v.Model == newVehicle.Model && v.Id == newVehicle.Id).FirstOrDefault());
            });
        }

        [Fact]
        public void Should_Pass_If_Vehicle_Is_Deleted_Successfully()
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);

                // Act
                var result = vo.DeleteVehicle(1);

                // Assert
                Assert.True(result);

                Assert.Null(db.GetCollection<IVehicle>().Find(v => v.Id == 1).FirstOrDefault());
            });
        }

        [Fact]
        public void Should_Pass_If_Fails_To_Deleted_Non_Existing_Vehicle_Successfully()
        {
            _DataBaseContainer(db =>
            {
                // Arrange
                VehicleOperations vo = GetVehicleOperationsWithData(db, TestVehicles);

                // Act
                var result = vo.DeleteVehicle(TestVehicles.Count * 2);

                // Assert
                Assert.False(result);
            });
        }

        #region Helper Methods

        private void _DataBaseContainer(Action<LiteDatabase> worker)
        {
            using (var _DbMemoryStream = new MemoryStream())
            {
                using (var db = new LiteDatabase(_DbMemoryStream))
                {
                    worker(db);
                }
            }
        }

        private VehicleOperations GetVehicleOperationsWithData(LiteDatabase db, List<Vehicle> data)
        {
            var vehicleCollection = db.GetCollection<IVehicle>();
            data.ForEach(v => vehicleCollection.Insert(v));

            var dal = new Mock<IDataAccessLayer>();
            dal.Setup(d => d.VehiclesCollection).Returns(vehicleCollection);

            var vo = new VehicleOperations(dal.Object);
            return vo;
        }

        #endregion Helper Methods

        #region Test Data

        private static readonly List<Vehicle> TestVehicles = new List<Vehicle>
        {
            new Vehicle
            {
                Id = 1,
                Make = "Ford",
                Model = "Escape",
                Year = 2007
            },
            new Vehicle
            {
                Id = 2,
                Make = "MB",
                Model = "ML350",
                Year = 2010
            },
            new Vehicle
            {
                Id = 3,
                Make = "Infiniti",
                Model = "XXS",
                Year = 1999
            },
            new Vehicle
            {
                Id = 4,
                Make = "VW",
                Model = "Golf",
                Year = 2000
            },
            new Vehicle
            {
                Id = 5,
                Make = "Ford",
                Model = "Aspire",
                Year = 2002
            },
            new Vehicle
            {
                Id = 6,
                Make = "Ford",
                Model = "Explorer",
                Year = 2002
            },
            new Vehicle
            {
                Id = 7,
                Make = "Ford",
                Model = "Aspire",
                Year = 2002
            },
            new Vehicle
            {
                Id = 8,
                Make = "BMW",
                Model = "750",
                Year = 2002
            }
        };

        public static IEnumerable<object[]> GetById_Data =>
        new List<object[]>
        {
            new object[] { 1, TestVehicles[0] },
            new object[] { 2, TestVehicles[1] },
            new object[] { 3, TestVehicles[2] },
            new object[] { 4, TestVehicles[3] },
            new object[] { 5, TestVehicles[4] },
        };

        public static IEnumerable<object[]> GetVehiclesWithFilters_Data =>
        new List<object[]>
        {
            new object[] { TestVehicles, "ford", null, null, 4 },
            new object[] { TestVehicles, "ford", null, 2002, 3 },
            new object[] { TestVehicles, "ford", "aspire", 2002, 2 },
            new object[] { TestVehicles, "", "", null, TestVehicles.Count },

        };

        public static IEnumerable<object[]> NullOrEmptyMakeModel_Data =>
        new List<object[]>
        {
            new object[] { new Vehicle{ Make = null, Model= "SomeModel", Year = 2000 }, "Make and Model cannot be empty!"},
            new object[] { new Vehicle{ Make = "MB", Model= null, Year = 2000 }, "Make and Model cannot be empty!"},
            new object[] { new Vehicle{ Make = null, Model= null, Year = 2000 }, "Make and Model cannot be empty!"},
            new object[] { new Vehicle{ Make = "", Model= "SomeModel", Year = 2000 }, "Make and Model cannot be empty!"},
            new object[] { new Vehicle{ Make = "MB", Model= "", Year = 2000 }, "Make and Model cannot be empty!"},
            new object[] { new Vehicle{ Make = "", Model= "", Year = 2000 }, "Make and Model cannot be empty!"},
            new object[] { new Vehicle{ Make = "SomeMake", Model= "SomeModel", Year = 1949 }, "Vehicle Year must be between 1950 and 2050!"},
            new object[] { new Vehicle{ Make = "SomeMake", Model= "SomeModel", Year = 2051 }, "Vehicle Year must be between 1950 and 2050!"},
            new object[] { null, "Invalid vehicle entry!"},
        };
        #endregion Test Data

    }



}
