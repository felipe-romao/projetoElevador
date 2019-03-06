using ElevatorSimulator.Factory;
using System;

namespace ElevatorSimulator
{
    public class Elevator: IElevator
    {
        public Elevator(ElevatorType type, Double weightLimit, IRouteFactory routeFactory)
        {
            this.CurrentFloor   = 0;
            this.PeopleWeight   = 0;
            this.Type           = type;
            this.WeightLimit    = weightLimit;
            this.Route          = routeFactory.Create();
            this.OpenDoor();
            this.Stop();
        }

        public ElevatorType Type { get; private set; }

        public DoorStatus DoorStatus { get; private set; }

        public ElevatorStatus Status { get; private set; }

        public Route Route { get; private set; }

        public int CurrentFloor { get; private set; }

        public double PeopleWeight { get; private set; }

        public Double WeightLimit { get; private set; }

        public bool IsDoorClosed => DoorStatus.CLOSED.Equals(this.DoorStatus);

        public bool IsStopped => ElevatorStatus.STOPPED.Equals(this.Status);

        public bool IsMovingUp => ElevatorStatus.MOVING_UP.Equals(this.Status);

        public bool IsMovingDown => ElevatorStatus.MOVING_DOWN.Equals(this.Status);

        public bool CanReceiveExternalCall => !ElevatorType.SERVICE.Equals(this.Type);

        public bool IsValidWeight => this.PeopleWeight <= this.WeightLimit;

        public void CloseDoor()
        {
            this.DoorStatus = DoorStatus.CLOSED;
        }

        public void OpenDoor()
        {
            this.DoorStatus = DoorStatus.OPENED;
        }

        public void MoveDown()
        {
            this.Status = ElevatorStatus.MOVING_DOWN;
            this.CurrentFloor--;
        }

        public void MoveUp()
        {
            this.Status = ElevatorStatus.MOVING_UP;
            this.CurrentFloor++;
        }

        public void Stop()
        {
            this.Status = ElevatorStatus.STOPPED;
        }

        public void IncreasePeopleWeight(double weight)
        {
            this.PeopleWeight += weight;
        }

        public void DecreasePeopleWeight(double weight)
        {
            this.PeopleWeight -= weight;
        }
    }
}
