using ElevatorSimulator;
using ElevatorSimulator.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Test
{
    [TestFixture]
    public class ElevatorTest
    {
        [Test]
        public void CreateElevator_CreatedSocialElevatorWithInitialValues_ResultIsCorrect()
        {
            // Arrange and Act
            var elevator = new Elevator(ElevatorType.SOCIAL, 600, new RouteFactory());

            // Assert
            Assert.AreEqual(0, elevator.CurrentFloor);
            Assert.AreEqual(0, elevator.PeopleWeight);
            Assert.AreEqual(false, elevator.IsDoorClosed);
            Assert.AreEqual(true, elevator.IsStopped);
            Assert.AreEqual(true, elevator.CanReceiveExternalCall);
        }

        [Test]
        public void CreateElevator_CreatedServiceElevatorWithInitialValues_ResultIsCorrect()
        {
            // Arrange and Act
            var elevator = new Elevator(ElevatorType.SERVICE, 1000, new RouteFactory());

            // Assert
            Assert.AreEqual(0, elevator.CurrentFloor);
            Assert.AreEqual(0, elevator.PeopleWeight);
            Assert.AreEqual(false, elevator.IsDoorClosed);
            Assert.AreEqual(true, elevator.IsStopped);
            Assert.AreEqual(false, elevator.CanReceiveExternalCall);
        }

        [Test]
        public void CloseDoor_DoorIsClosed_ResultIsCorrect()
        {
            // Arrange
            var elevator = new Elevator(ElevatorType.SOCIAL, 600, new RouteFactory());
            Assert.AreEqual(false, elevator.IsDoorClosed);

            // Act
            elevator.CloseDoor();

            // Assert
            Assert.AreEqual(true, elevator.IsDoorClosed);
        }

        [Test]
        public void MoveUp_MoveElevatorToUp_ResultIsCorrect()
        {
            // Arrange
            var elevator = new Elevator(ElevatorType.SOCIAL, 600, new RouteFactory());
            Assert.AreEqual(true, elevator.IsStopped);

            // Act
            elevator.MoveUp();

            // Assert
            Assert.AreEqual(true, elevator.IsMovingUp);
        }

        [Test]
        public void MoveDown_MoveElevatorToDown_ResultIsCorrect()
        {
            // Arrange
            var elevator = new Elevator(ElevatorType.SOCIAL, 600, new RouteFactory());
            Assert.AreEqual(true, elevator.IsStopped);

            // Act
            elevator.MoveDown();

            // Assert
            Assert.AreEqual(true, elevator.IsMovingDown);
        }

        [Test]
        public void IncreasePeopleWeight_WeigthIsValid_ResultIsCorrect()
        {
            // Arrange
            var elevator = new Elevator(ElevatorType.SOCIAL, 600, new RouteFactory());
            Assert.AreEqual(0, elevator.PeopleWeight);

            // Act
            elevator.IncreasePeopleWeight(100);
            elevator.IncreasePeopleWeight(150);

            // Assert
            Assert.AreEqual(250, elevator.PeopleWeight);
        }

        [Test]
        public void DecreasePeopleWeight_WeigthIsValid_ResultIsCorrect()
        {
            // Arrange
            var elevator = new Elevator(ElevatorType.SOCIAL, 600, new RouteFactory());
            Assert.AreEqual(0, elevator.PeopleWeight);

            // Act
            elevator.IncreasePeopleWeight(100);
            elevator.DecreasePeopleWeight(60);

            // Assert
            Assert.AreEqual(40, elevator.PeopleWeight);
        }
    }
}
