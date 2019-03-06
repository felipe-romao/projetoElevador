using ElevatorSimulator.Factory;
using ElevatorSimulator.Service;
using System;
using System.Collections.Generic;

namespace ElevatorSimulator
{
    /// <summary>
    /// Classe responsável por controlar as acões do elevador.
    /// </summary>
    /// 
    public class ElevatorController
    {
        private ElevatorConfig config;
        private IElevator elevator;
        private IExternalService externalService;
        private ILogger logger;

        /// <summary>
        /// Construtor recebe os parametros por injeção de dependencia.
        /// Praticamente todos os parametros são interfaces, o que facilida a implementação e a construção dos testes.
        /// </summary>
        /// <param name="config">objeto com as cofigurações</param>
        /// <param name="elevatorType">tipo do elevador</param>
        /// <param name="elevatorFactory">interface de um factory para criar um elevador</param>
        /// <param name="routeFactory">interface de um factory para criar uma rota</param>
        /// <param name="externalService">interface para simular um serviço externo que gera chamadas externas para o elevador</param>
        /// <param name="logger">interface para logar informações</param>
        /// 
        public ElevatorController(ElevatorConfig config, ElevatorType elevatorType, IElevatorFactory elevatorFactory, IRouteFactory routeFactory, IExternalService externalService, ILogger logger)
        {
            this.config             = config;
            this.elevator           = elevatorFactory.Create(elevatorType, routeFactory, config.WeightLimit);
            this.externalService    = externalService;
            this.logger             = logger;
            this.InfoExternalService();
        }

        /// <summary>
        /// Método responsável em checar se existe uma próximo destino de uma rota.
        /// </summary>
        /// <returns>true ou false</returns>
        /// 
        public bool HasFloorTarget()
        {
            var floorExternalCall = this.GetFloorFromExternallCall();

            if (floorExternalCall != null)
                this.CreateRoute(floorExternalCall);

            if (!this.elevator.Route.HasNext)
                return false;

            return true;
        }

        /// <summary>
        /// Método responsável em receber o peso das pessoas que embarcaram no elevador.
        /// Este método valida se o peso é válido.
        /// </summary>
        /// <param name="peopleWeight">peso das pessoas</param>
        /// 
        public void BoardPassengers(Double peopleWeight)
        {
            CheckStatusFromElevatorAndDoorToBoardAndDisembarkPassengers();
            this.elevator.IncreasePeopleWeight(peopleWeight);

            if (!this.elevator.IsValidWeight)
            {
                var weightError = this.elevator.PeopleWeight;
                this.elevator.DecreasePeopleWeight(peopleWeight);
                throw new ArgumentException($"Weight {weightError} exceeded the limit of {config.WeightLimit}.");
            }
        }

        /// <summary>
        /// Método responsável em criar uma rota.
        /// </summary>
        /// <param name="selectedFloors">Recebe os andares selecionados</param>
        /// 
        public void CreateRoute(List<int> selectedFloors)
        {
            this.CheckIsSelectedFloorsAreValid(selectedFloors);
            foreach (var selectedFloor in selectedFloors)
                this.elevator.Route.AddSelectedFloor(selectedFloor);

            this.elevator.Route.Create(this.elevator.CurrentFloor);
        }

        /// <summary>
        /// Método responsável em movimentar o elevador para o próximo destino da rota.
        /// Valida se o peso é valido e o status da porta e do elevador antes de move-lo.
        /// </summary>
        /// 
        public void MoveToNextFloorTarget()
        {
            if (!this.elevator.IsValidWeight)
                throw new InvalidOperationException($"Weight {this.elevator.PeopleWeight} exceeded the limit of {config.WeightLimit}.");

            this.elevator.CloseDoor();

            var floorTarget = this.elevator.Route.Next();
            while (this.elevator.CurrentFloor != floorTarget)
            {
                var floorBeforeMove = this.elevator.CurrentFloor;

                if (floorBeforeMove < floorTarget)
                    this.elevator.MoveUp();
                else
                    this.elevator.MoveDown();
                
                this.logger.Log($"Door Status: {this.elevator.DoorStatus} - " +
                                $"Elevator Status: {this.elevator.Status} " +
                                $"from {floorBeforeMove} to {this.elevator.CurrentFloor} floor.");
            }

            this.elevator.Stop();
            this.elevator.OpenDoor();
        }

        /// <summary>
        /// Método responsável em receber o peso das pessoas que desembarcaram do elevador.
        /// Este método valida se o peso é válido.
        /// </summary>
        /// <param name="peopleWeight"></param>
        /// 
        public void DisembarkPassengers(Double peopleWeight)
        {
            CheckStatusFromElevatorAndDoorToBoardAndDisembarkPassengers();
            this.elevator.DecreasePeopleWeight(peopleWeight);
            if (this.elevator.PeopleWeight < 0)
            {
                var weightError = this.elevator.PeopleWeight;
                this.elevator.IncreasePeopleWeight(peopleWeight);
                throw new ArgumentException($"The weight disembark passengers {peopleWeight} is larger than the current weight {this.elevator.PeopleWeight}.");
            }
        }

        /// <summary>
        /// Método responsável em logar as informações de status do elevador. 
        /// </summary>

        public void InfoStatus()
        {
            var direction = "DOWN";
            if (this.elevator.Route.IsUpDirection)
                direction = "UP";

            this.logger.Log($"Door Status: {this.elevator.DoorStatus} - " +
                $"Elevator Status: {this.elevator.Status} - " +
                $"Floor: {this.elevator.CurrentFloor} - " +
                $"Route Direction: {direction} - " +
                $"People Weight: {this.elevator.PeopleWeight}.");
        }

        private void CheckStatusFromElevatorAndDoorToBoardAndDisembarkPassengers()
        {
            if (!this.elevator.IsStopped)
                throw new InvalidOperationException("The elevator is not stopped.");

            if (this.elevator.IsDoorClosed)
                throw new InvalidOperationException("The elevator door is not open.");
        }

        private void CheckIsSelectedFloorsAreValid(List<int> selectedFloors)
        {
            if (selectedFloors == null)
                return;

            foreach(var floor in selectedFloors)
            {
                if (floor > this.config.FloorCount)
                    throw new ArgumentException($"The selected floor '{floor}' exceeds the supported limit '{config.FloorCount}'.");
            }
        }

        private List<int> GetFloorFromExternallCall()
        {
            if (!this.elevator.CanReceiveExternalCall)
                return null;

            var floorExternalCall = externalService.GetExternalCall();
            if (floorExternalCall == null || floorExternalCall.Count <= 0)
            {
                return null;
            }

            return floorExternalCall;
        }

        private void InfoExternalService()
        {
            if (!this.elevator.CanReceiveExternalCall)
                this.logger.Log($"WARNNING: This elevator type can not be receive external call.");
        }
    }
}
