using System;

namespace ElevatorSimulator
{
    public sealed class DoorStatus
    {
        public static DoorStatus FromValue(string value)
        {
            if (OPENED.Value.Equals(value))
            {
                return OPENED;
            }
            throw new ArgumentException($"Status {value} not found.");
        }


        public static readonly DoorStatus OPENED = new DoorStatus("OPENED");
        public static readonly DoorStatus CLOSED = new DoorStatus("CLOSED");

        private DoorStatus(string value)
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