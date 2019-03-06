using System;

namespace ElevatorSimulator
{
    public interface IElevator
    {
        ElevatorType Type { get; }

        DoorStatus DoorStatus { get; }

        ElevatorStatus Status { get; }

        Route Route { get; }

        int CurrentFloor { get; }

        Double PeopleWeight { get; }

        Double WeightLimit { get; }

        bool IsDoorClosed { get; }

        bool IsStopped { get; }

        bool IsMovingUp { get; }

        bool IsMovingDown { get; }

        bool CanReceiveExternalCall { get; }

        bool IsValidWeight { get; }

        void CloseDoor();

        void OpenDoor();

        void MoveUp();

        void MoveDown();

        void Stop();

        void IncreasePeopleWeight(Double weight);

        void DecreasePeopleWeight(Double weight);
    }
}
