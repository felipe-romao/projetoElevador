using ElevatorSimulator;
using ElevatorSimulator.Factory;
using ElevatorSimulator.Service;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ElevatorSimulator.Test
{
    [TestFixture]
    public class ElevatorControllerTest
    {
        private static int FLOOR_COUNT = 30;
        private static int ELEVATOR_SOCIAL_WEIGHT_LIMIT  = 600;
        private static int ELEVATOR_SERVICE_WEIGHT_LIMIT = 1000;

        private IElevator elevator;
        private ILogger logger;
        private ElevatorConfig config;
        private RouteFactory routeFactory;
        private IElevatorFactory elevatorFactory;
        private IExternalService externalService;
        private ElevatorController elevatorController;
        
        [SetUp]
        public void SetUp()
        {
            this.logger          = Substitute.For<ILogger>();
            this.elevatorFactory = Substitute.For<IElevatorFactory>();
            this.externalService = Substitute.For<IExternalService>();
            this.config          = new ElevatorConfig(FLOOR_COUNT, ELEVATOR_SOCIAL_WEIGHT_LIMIT);
            this.routeFactory    = new RouteFactory();
        }

        /// <summary>
        /// Teste para validar o embarque de pessoas com elevador parado.
        /// Experado uma InvalidOperationException.
        /// </summary>
        /// 
        [Test]
        public void BoardPassengers_ElevatorIsNotStopped_ThrowException()
        {
            // Arrange
            this.elevator = Substitute.For<IElevator>();
            this.elevator.IsStopped.Returns(false);

            this.elevatorFactory.Create(Arg.Any<ElevatorType>(), Arg.Any<RouteFactory>(), Arg.Any<Double>())
                                .Returns(this.elevator);

            this.elevatorController = new ElevatorController(this.config, ElevatorType.SOCIAL, this.elevatorFactory, this.routeFactory, this.externalService, this.logger);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => this.elevatorController.BoardPassengers(100));

            // Assert
            Assert.AreEqual("The elevator is not stopped.", ex.Message);
        }

        /// <summary>
        /// Teste para validar o embarque de pessoas com a porta aberta.
        /// Experado uma InvalidOperationException.
        /// </summary>
        /// 
        [Test]
        public void BoardPassengers_ElevatorDoorIsNotOpened_ThrowException()
        {
            // Arrange
            this.elevator = Substitute.For<IElevator>();
            this.elevator.IsDoorClosed.Returns(true);
            this.elevator.IsStopped.Returns(true);

            this.elevatorFactory.Create(Arg.Any<ElevatorType>(), Arg.Any<RouteFactory>(), Arg.Any<Double>())
                                .Returns(this.elevator);

            this.elevatorController = new ElevatorController(this.config, ElevatorType.SOCIAL, this.elevatorFactory, this.routeFactory, this.externalService, this.logger);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => this.elevatorController.BoardPassengers(100));

            // Assert
            Assert.AreEqual("The elevator door is not open.", ex.Message);
        }

        /// <summary>
        /// Teste para validar o desembarque de pessoas com elevador parado.
        /// Experado uma InvalidOperationException.
        /// </summary>
        /// 
        [Test]
        public void DisembarkPassengers_ElevatorIsNotStopped_ThrowException()
        {
            // Arrange
            this.elevator = Substitute.For<IElevator>();
            this.elevator.IsStopped.Returns(false);

            this.elevatorFactory.Create(Arg.Any<ElevatorType>(), Arg.Any<RouteFactory>(), Arg.Any<Double>())
                                .Returns(this.elevator);

            this.elevatorController = new ElevatorController(this.config, ElevatorType.SOCIAL, this.elevatorFactory, this.routeFactory, this.externalService, this.logger);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => this.elevatorController.DisembarkPassengers(100));

            // Assert
            Assert.AreEqual("The elevator is not stopped.", ex.Message);
        }

        /// <summary>
        /// Teste para validar o desembarque de pessoas com a porta aberta.
        /// Experado uma InvalidOperationException.
        /// </summary>
        /// 
        [Test]
        public void DisembarkPassengers_ElevatorDoorIsNotOpened_ThrowException()
        {
            // Arrange
            this.elevator = Substitute.For<IElevator>();
            this.elevator.IsDoorClosed.Returns(true);
            this.elevator.IsStopped.Returns(true);

            this.elevatorFactory.Create(Arg.Any<ElevatorType>(), Arg.Any<RouteFactory>(), Arg.Any<Double>())
                                .Returns(this.elevator);

            this.elevatorController = new ElevatorController(this.config, ElevatorType.SOCIAL, this.elevatorFactory, this.routeFactory, this.externalService, this.logger);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => this.elevatorController.DisembarkPassengers(100));

            // Assert
            Assert.AreEqual("The elevator door is not open.", ex.Message);
        }

        /// <summary>
        /// Teste para validar a criação de rota selecionado um andar acima do esperado.
        /// Experado uma ArgumentException.
        /// </summary>
        /// 
        [Test]
        public void CreateRoute_SelectedFloorIsInvalid_ThrowException()
        {
            // Arrange
            this.InitializeElevatorSocial();

            // Act
            var ex = Assert.Throws<ArgumentException>(() => this.elevatorController.CreateRoute(new List<int> { 1, 50 }));

            // Assert
            Assert.AreEqual($"The selected floor '50' exceeds the supported limit '{config.FloorCount}'.", ex.Message);
        }

        /// <summary>
        /// Teste para validar o limite de peso.
        /// Experado uma ArgumentException.
        /// </summary>
        /// 
        [Test]
        public void BoardPassengers_PeopleWeightIsInvalid_ThrowException()
        {
            // Arrange
            this.InitializeElevatorSocial();

            // act
            var ex = Assert.Throws<ArgumentException>(() => this.elevatorController.BoardPassengers(1500));

            // Assert
            Assert.AreEqual($"Weight 1500 exceeded the limit of 600.", ex.Message);
        }

        /// <summary>
        /// Teste para validar se o peso do desembarque não é inferior a zero.
        /// Experado uma ArgumentException.
        /// </summary>
        /// 
        [Test]
        public void DisembarkPassengers_PeopleWeightIsInvalid_ThrowException()
        {
            // Arrange
            this.InitializeElevatorSocial();
            this.elevatorController.BoardPassengers(200);

            // act
            var ex = Assert.Throws<ArgumentException>(() => this.elevatorController.DisembarkPassengers(1500));

            // Assert
            Assert.AreEqual($"The weight disembark passengers 1500 is larger than the current weight 200.", ex.Message);
        }

        /// <summary>
        /// Teste para validar a entrada de três rotas no elevador social parado no terreo.
        /// O elevador esta no andar 0, entram três pessoas sendo com rotas diferentes (2, 7, 4 andares).
        /// Esperado que ele vá até o andar 2, depois 4 e por ultimo o andar 7.
        /// </summary>
        /// 
        [Test]
        public void MoveElevator_ElevatorAtZeroFloorAndReceiveThreeRoutes_ResultIsCorrect()
        {
            this.InitializeElevatorSocial();

            // Verifica que o elevador está no andar zero, parado e porta aberta.
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 0, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // embarca os passagerios, totalizando 180 kg.
            this.elevatorController.BoardPassengers(180);
            Assert.AreEqual(true, this.elevator.IsValidWeight);

            // cria a rota para os andares selecionados.
            this.elevatorController.CreateRoute(new List<int> { 2, 7, 4 });
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());

            // elevador se move para o primeiro destino: do andar 0 para 2.
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 0 to 1 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 1 to 2 floor.");
            });

            // desembarca os passageiros do primeiro destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(120);
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 2, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // elevador segue para o proximo destino: do andar 2 para 4.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 2 to 3 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 3 to 4 floor.");
            });

            // desembarca os passageiros do segundo destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(60);
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 4, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // elevador segue para o proximo destino: do andar 4 para 7.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 4 to 5 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 5 to 6 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 6 to 7 floor.");
            });

            // elevador nao tem mais rotas. Esta no andar 7, parado e com as portas abertas.
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 7, ElevatorStatus.STOPPED, DoorStatus.OPENED);
        }


        /// <summary>
        /// Teste para validar a entrada de três rotas de diferentes sentidos no elevador social parado no andar 7.
        /// O elevado subiu para o andar 7, entram três pessoas com rotas de sentidos diferentes (3, 10, 9 andares).
        /// esperado que ele vá para os andares 9, 10 e volte para o 3. 
        /// </summary>
        [Test]
        public void MoveElevator_ElevatorAtSevenFloorAndReceiveThreeRoutesDifferentsDirection_ResultIsCorrect()
        {
            this.InitializeElevatorSocial();

            // elevador subiu para o 7 andar e esta parado com a porta aberta. 
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 0, ElevatorStatus.STOPPED, DoorStatus.OPENED);
            this.elevatorController.BoardPassengers(80);
            this.elevatorController.CreateRoute(new List<int> { 7 });
            this.elevatorController.MoveToNextFloorTarget();
            this.elevatorController.DisembarkPassengers(80);
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 7, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // embarca as pessoas e criam a rota com sentidos diferentes
            this.elevatorController.BoardPassengers(200);
            this.elevatorController.CreateRoute(new List<int> { 3, 10, 9 });

            // elevador segue para o proximo destino considerando que estava subindo: do andar 7 para 9.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 7 to 8 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 8 to 9 floor.");
            });

            // desembarca os passageiros do segundo destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(60);
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 9, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // elevador segue para o proximo destino considerando que estava subindo: do andar 9 para 10.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 9 to 10 floor.");
            });

            // desembarca os passageiros do segundo destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(70);
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 10, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // elevador desce para o ultimo destino: do andar 10 para 3.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_DOWN} from 10 to 9 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_DOWN} from 9 to 8 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_DOWN} from 8 to 7 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_DOWN} from 7 to 6 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_DOWN} from 6 to 5 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_DOWN} from 5 to 4 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_DOWN} from 4 to 3 floor.");
            });

            // desembarca os passageiros do segundo destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(70);
            
            // elevador nao tem mais rotas. Esta no andar 3, parado e com as portas abertas.
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 3, ElevatorStatus.STOPPED, DoorStatus.OPENED);
        }


        /// <summary>
        /// Teste para validar uma atualização da rota em curso.
        /// O elevado parado no andar 0 recebe uma 3 rotas (2, 5, 6 andares). No andar 2 entra mais uma pessoa com destino ao andar 4. 
        /// esperado que ele vá para os andares 2, 4, 5 e 6. 
        /// </summary>
        /// 
        [Test]
        public void MoveElevator_UpdateRouteDuringARouteOnCourse_ResultIsCorrect()
        {
            this.InitializeElevatorSocial();

            // elevador esta parado com a porta aberta no andar 0. 
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 0, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // embarca as pessoas e a rota é criada: andares 2, 5, 6.
            this.elevatorController.BoardPassengers(300);
            this.elevatorController.CreateRoute(new List<int> { 2, 5, 6 });

            // elevador segue para o proximo destino considerando que estava subindo: do andar 7 para 9.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 0 to 1 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 1 to 2 floor.");
            });

            // desembarca os passageiros do segundo destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(60);
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 2, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // embarca mais um pessoa e a rota é criada novamente.
            this.elevatorController.BoardPassengers(75);
            this.elevatorController.CreateRoute(new List<int> { 4 });

            // elevador segue para o proximo destino considerando agora a nova rota: do andar 7 para 9.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 2 to 3 floor.");
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 3 to 4 floor.");
            });

            // desembarca os passageiros do segundo destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(75);
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 4, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // elevador segue para o proximo destino: do andar 4 para 5.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 4 to 5 floor.");
            });

            // desembarca os passageiros do terceiro destino e revalida o peso.
            this.elevatorController.DisembarkPassengers(100);
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 5, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // elevador segue para o proximo destino: do andar 5 para 6.
            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.MoveToNextFloorTarget();
            Received.InOrder(() =>
            {
                logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 5 to 6 floor.");
            });

            // desembarca os passageiros do ultimo destino.
            this.elevatorController.DisembarkPassengers(140);

            // elevador nao tem mais rotas. Esta no andar 6, parado e com as portas abertas.
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 6, ElevatorStatus.STOPPED, DoorStatus.OPENED);
        }

        /// <summary>
        /// Teste para validar uma chamada external de um elevador social.
        /// O elevador esta no andar 0, parado e com a porta aberta. A chamada é para o andar 1.
        /// O elevador cria rota, peso é valido, fecha a porta, e move para o andar chamado, atualizando seu status para 'movendo para cima'.
        /// </summary>
        /// 
        [Test]
        public void GetExternalCall_ElevatorStoppedAtZeroFloorAndCalledOnFirstFloor_ResultIsCorrect()
        {
            // Mock para criar uma chamada externa para o andar 1.
            var floors = new List<int> { 1 };
            this.externalService.GetExternalCall().Returns(floors);

            this.InitializeElevatorSocial();

            // Verifica que o elevador está no andar zero, parado e porta aberta.
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 0, ElevatorStatus.STOPPED, DoorStatus.OPENED);

            // Elevador recebe a chamada, cria a rota, valida peso, fecha a porta e move para o andar 1, atualizando o status para 'movendo para cima'.
            Assert.AreEqual(true, this.elevatorController.HasFloorTarget());
            this.elevatorController.BoardPassengers(0);

            Assert.AreEqual(true, this.elevator.IsValidWeight);
            Assert.AreEqual(true, this.elevator.Route.HasNext);

            this.elevatorController.MoveToNextFloorTarget();
            logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 0 to 1 floor.");

            // Elevador chegou no andar 2, status parado, porta aberta e não possui mais nenhuma rota.
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 1, ElevatorStatus.STOPPED, DoorStatus.OPENED);
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());
        }

        /// <summary>
        /// Teste para validar se elevador de serviço rejeita uma chamada externa.
        /// Elevador esta no andar zero, uma pessoa embarca para o andar 1. Alguém no andar 0 chama o elevador de volta.
        /// Esperado que o elevador não atenda o chamado externo.
        /// </summary>
        /// 
        [Test]
        public void MoveElevatorService_ElevatorIgnoreExternalCall_ResultIsCorrect()
        {
            // Mock para criar uma chamada externa para o andar 0 quando o elevador estiver no andar 1.
            var floors = new List<int> { 0 };
            this.externalService.GetExternalCall().Returns(x => null, x => floors);

            this.elevator = new Elevator(ElevatorType.SERVICE, ELEVATOR_SERVICE_WEIGHT_LIMIT, routeFactory);
            this.elevatorFactory.Create(Arg.Any<ElevatorType>(), Arg.Any<RouteFactory>(), Arg.Any<Double>())
                                .Returns(this.elevator);

            this.elevatorController = new ElevatorController(this.config, ElevatorType.SERVICE, this.elevatorFactory, this.routeFactory, 
                                                            this.externalService, this.logger);

            // Verifica que o elevador está no andar zero, parado e porta aberta.
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 0, ElevatorStatus.STOPPED, DoorStatus.OPENED);
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());

            // Elevador cria a rota, valida peso, fecha a porta e move para o andar 1, atualizando o status para 'movendo para cima'.
            this.elevatorController.BoardPassengers(140);
            this.elevatorController.CreateRoute(new List<int> { 1 });

            Assert.AreEqual(true, this.elevator.IsValidWeight);
            this.elevatorController.MoveToNextFloorTarget();
            logger.Received().Log($"Door Status: {DoorStatus.CLOSED} - Elevator Status: {ElevatorStatus.MOVING_UP} from 0 to 1 floor.");

            // Elevador chegou no andar 1, status parado, porta aberta e não possui mais nenhuma rota ignorando a chamada externa.
            AssertFloorAndElevatorStatusAndDoorStatusFromElevator(this.elevator, 1, ElevatorStatus.STOPPED, DoorStatus.OPENED);
            Assert.AreEqual(false, this.elevatorController.HasFloorTarget());
        }

        private void InitializeElevatorSocial()
        {
            this.elevator = new Elevator(ElevatorType.SOCIAL, ELEVATOR_SOCIAL_WEIGHT_LIMIT, routeFactory);
            this.elevatorFactory.Create(Arg.Any<ElevatorType>(), Arg.Any<RouteFactory>(), Arg.Any<Double>())
                                .Returns(this.elevator);

            this.elevatorController = new ElevatorController(this.config, ElevatorType.SOCIAL, this.elevatorFactory, 
                                                            this.routeFactory, this.externalService, this.logger);
        }

        private void AssertFloorAndElevatorStatusAndDoorStatusFromElevator(IElevator elevator, int currentFloor, ElevatorStatus elevatorStatus, DoorStatus doorStatus)
        {
            Assert.AreEqual(currentFloor    , elevator.CurrentFloor);
            Assert.AreEqual(elevatorStatus  , elevator.Status);
            Assert.AreEqual(doorStatus      , elevator.DoorStatus);
        }
    }
}
