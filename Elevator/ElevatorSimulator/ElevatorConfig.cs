using System;

namespace ElevatorSimulator
{
    public class ElevatorConfig
    {
        public ElevatorConfig(int floorCount, Double weightLimit)
        {
            this.FloorCount  = floorCount;
            this.WeightLimit = weightLimit;
        }

        public Double FloorCount { get; private set; }

        public Double WeightLimit { get; private set; }
    }
}
