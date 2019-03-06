using System;

namespace ElevatorSimulator
{
    public sealed class ElevatorStatus
    {
        public static ElevatorStatus FromValue(string value)
        {
            if (STOPPED.Value.Equals(value))
            {
                return STOPPED;
            }
            if (MOVING_UP.Value.Equals(value))
            {
                return MOVING_UP;
            }
            if (MOVING_DOWN.Value.Equals(value))
            {
                return MOVING_DOWN;
            }
            throw new ArgumentException($"Status {value} not found.");
        }

        public static readonly ElevatorStatus STOPPED     = new ElevatorStatus("STOPPED");
        public static readonly ElevatorStatus MOVING_UP   = new ElevatorStatus("MOVING_UP");
        public static readonly ElevatorStatus MOVING_DOWN = new ElevatorStatus("MOVING_DOWN");
        
        private ElevatorStatus(string value)
        {
            this.Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return this.Value;
        }
    }
}