using System.Security.Policy;

namespace KodeKandy.Mapnificent.Tests.TestEntities
{
    public class SimpleFrom
    {
        public string Name { get; set; }
        public int Age;
    }

    public class VehicleFrom
    {
        public string Name { get; set; }
    }

    public class CarFrom : VehicleFrom
    {
        public int NoSeats { get; set; }
    }

    public class VehicleTo
    {
        public string Name { get; set; }
    }

    public class CarTo : VehicleTo
    {
        public int NoSeats { get; set; }
    }
}