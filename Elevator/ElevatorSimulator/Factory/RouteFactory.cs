
namespace ElevatorSimulator.Factory
{
    public class RouteFactory : IRouteFactory
    {
        public Route Create()
        {
            return new Route();
        }
    }
}
