using ElevatorSimulator;
using NUnit.Framework;
using System.Collections.Generic;

namespace ElevatorSimulator.Test
{
    [TestFixture]
    public class RouteTest
    {
        [Test]
        public void CreateRouteAtZeroFloor_AddSelectedFloorThreeFloors_ResultIsCorrect()
        {
            Route route = new Route();
            route.AddSelectedFloor(1);
            route.AddSelectedFloor(5);
            route.AddSelectedFloor(4);

            route.Create(0);

            List<int> floorsExpected = new List<int> { 1, 4, 5 };
            int pos = 0;

            while (route.HasNext)
            {
                Assert.AreEqual(floorsExpected[pos], route.Next());
                pos++;
            }

            Assert.False(route.HasNext);
        }

        [Test]
        public void CreateRouteAtThreeFloorAndUpDirection_SelectedTwoFloorsUpAndOneDown_ResultIsCorrect()
        {
            Route route = new Route();
            route.AddSelectedFloor(1);
            route.AddSelectedFloor(7);
            route.AddSelectedFloor(6);

            route.Create(3);

            List<int> floorsExpected = new List<int> { 6, 7, 1 };
            int pos = 0;

            while (route.HasNext)
            {
                Assert.AreEqual(floorsExpected[pos], route.Next());
                pos++;
            }

            Assert.False(route.HasNext);
        }

        [Test]
        public void CreateRouteAtThreeFloorAndDownDirection_SelectedTwoFloorsUpAndOneDown_ResultIsCorrect()
        {
            // elevador sobe do andar 0 para o 5, com sentido 'para cima'
            var route = new Route();
            route.AddSelectedFloor(5);
            route.Create(0);
            while (route.HasNext)
                route.Next();
            Assert.AreEqual(true, route.IsUpDirection);

            // elevador desce do andar 5 para o 4, mundando o sentido 'para baixo'
            route.AddSelectedFloor(4);
            route.Create(5);
            while (route.HasNext)
                route.Next();
            Assert.AreEqual(false, route.IsUpDirection);

            // como o sentido esta para baixo, o elevador irá descer para o andar 1 e depois subir para 6 e 7, terminando com o sentido para cima.
            route.AddSelectedFloor(1);
            route.AddSelectedFloor(7);
            route.AddSelectedFloor(6);
            route.Create(4);

            var floorsExpected = new List<int> { 1, 6, 7 };
            var pos = 0;
            while (route.HasNext)
            {
                Assert.AreEqual(floorsExpected[pos], route.Next());
                pos++;
            }
            Assert.AreEqual(true, route.IsUpDirection);
            Assert.False(route.HasNext);
        }

        [Test]
        public void IncludeNewAtExistingRoute_MaintainDirectionFromRoute_ResultIsCorrect()
        {
            var route = new Route();
            route.AddSelectedFloor(2);
            route.AddSelectedFloor(7);
            route.AddSelectedFloor(4);
            route.Create(0);
            Assert.AreEqual(2, route.Next());
            Assert.AreEqual(4, route.Next());
            Assert.AreEqual(true, route.HasNext);

            route.AddSelectedFloor(6);
            route.Create(4);
            Assert.AreEqual(6, route.Next());
            Assert.AreEqual(7, route.Next());
            Assert.AreEqual(false, route.HasNext);
        }

        [Test]
        public void IncludeNewAtExistingRoute_ChangeDirectionFromRoute_ResultIsCorrect()
        {
            var route = new Route();
            route.AddSelectedFloor(2);
            route.AddSelectedFloor(7);
            route.AddSelectedFloor(4);
            route.Create(0);
            Assert.AreEqual(2, route.Next());
            Assert.AreEqual(4, route.Next());
            Assert.AreEqual(true, route.HasNext);

            route.AddSelectedFloor(1);
            route.Create(4);
            Assert.AreEqual(7, route.Next());
            Assert.AreEqual(1, route.Next());
            Assert.AreEqual(false, route.HasNext);
        }
    }
}
