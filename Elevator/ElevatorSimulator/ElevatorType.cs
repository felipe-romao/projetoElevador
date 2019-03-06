using System;

namespace ElevatorSimulator
{
    public sealed class ElevatorType
    {
        public static ElevatorType FromValue(string value)
        {
            if (SOCIAL.Value.Equals(value))
            {
                return SOCIAL;
            }
            if (SERVICE.Value.Equals(value))
            {
                return SERVICE;
            }

            throw new ArgumentException($"elevator type {value} not found.");
        }

        public static readonly ElevatorType SOCIAL = new ElevatorType("SOCIAL");
        public static readonly ElevatorType SERVICE = new ElevatorType("SERVICE");

        private ElevatorType(string value)
        {
            this.Value = value;
        }

        public string Value { get; }
    }
}
