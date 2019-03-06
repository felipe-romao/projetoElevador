using System.Collections.Generic;

namespace ElevatorSimulator.Service
{
    public interface IExternalService
    {
        List<int> GetExternalCall();
    }
}
