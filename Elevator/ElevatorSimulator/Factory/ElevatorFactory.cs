
namespace ElevatorSimulator.Factory
{
    public class ElevatorFactory : IElevatorFactory
    {
        public IElevator Create(ElevatorType type, IRouteFactory routeFactory, double weigtLimit)
        {
            return new Elevator(type, weigtLimit, routeFactory);
        }
    }
}
