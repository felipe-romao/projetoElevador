using System;

namespace ElevatorSimulator.Factory
{
    public interface IElevatorFactory
    {
        IElevator Create(ElevatorType type, IRouteFactory routeFactory, Double weigtLimit);
    }
}
