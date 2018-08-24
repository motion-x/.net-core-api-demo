namespace DotNetCoreTestApi.Models
{
    public interface IVehicle
    {

        int Id { get; set; }

        string Make { get; set; }

        string Model { get; set; }

        int Year { get; set; }
    }
}