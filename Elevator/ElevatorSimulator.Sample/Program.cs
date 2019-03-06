using System;
using System.Collections.Generic;

namespace ElevatorSimulator.Sample
{
    /// <summary>
    /// Aplicação console responsável em simular as operações do elevador.
    /// Basicamente, as opções para uso são:
	/// 		- embarque de passageiros, informando o peso;
	/// 		- criação da(s) rota(s), informando os andares de destino;
	/// 		- desembarque, informando o peso;
    /// </summary>
    /// 
    class Program
    {
        private static ElevatorController elevatorController;

        static void Main(string[] args)
        {
            try
            {
                Bootstrapper bootstrapper = new Bootstrapper();
                elevatorController = bootstrapper.Bootstrap();
                ShowInitialInfo();
                
                while (true)
                {
                    Console.Write("\nElevator info => ");
                    elevatorController.InfoStatus();

                    if (CheckIsHasANewBoardPassengers())
                        DefineBoardPassengersAndRoute();

                    while (elevatorController.HasFloorTarget())
                    {
                        Console.WriteLine($"\nMoving elevator to the next target: ");
                        elevatorController.MoveToNextFloorTarget();

                        elevatorController.InfoStatus();
                        Console.Write($"It arrived at the destination. Please state the weight of passengers disembarking: ");
                        DisembarkPassengersInfo();

                        CheckExistsNewBoardPassengersAtRouteInProgress();

                        if (elevatorController.HasFloorTarget())
                            Console.WriteLine("\nThe elevator will go to the next destination...");
                        else
                            Console.WriteLine("\nThe elevator has no more destination.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while running the sample: {ex.Message}.");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static void DefineBoardPassengersAndRoute()
        {
            Console.Write($"\nInform the weight of the passengers you are boarding: ");
            SetBoardPassengersInfo();

            Console.Write($"Choose a route(s) for example 1,3,5: ");
            SetRoute();
        }
        private static void CheckExistsNewBoardPassengersAtRouteInProgress()
        {
            try
            {
                Console.Write("New passengers? [y/n]: ");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Y)
                    DefineBoardPassengersAndRoute();
            }
            catch
            {
                CheckExistsNewBoardPassengersAtRouteInProgress();
            }
            return;
        }

        private static void SetBoardPassengersInfo()
        {
            try
            {
                elevatorController.BoardPassengers(GetPeopleWeight());
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.Write("Please, enter a valid weight: ");
                SetBoardPassengersInfo();
            }
        }

        private static void SetRoute()
        {
            try
            {
                elevatorController.CreateRoute(GetSelectedFloors());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.Write("Please, enter a valid route: ");
                SetRoute();
            }
        }

        private static void DisembarkPassengersInfo()
        {
            try
            {
                elevatorController.DisembarkPassengers(GetPeopleWeight());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.Write("Please, enter a valid weight: ");
                DisembarkPassengersInfo();
            }
        }

        private static Double GetPeopleWeight()
        {
            try
            {
                return Convert.ToDouble(Console.ReadLine());
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error on inserted weight: {ex.Message}.");
                Console.Write("Please, insert a valid weight: ");
                return GetPeopleWeight();
            }
        }

        private static List<int> GetSelectedFloors()
        {
            var floors = new List<int>();
            try
            {
                var selectedFloors = Console.ReadLine();
                foreach (var floor in selectedFloors.Split(','))
                    floors.Add(Convert.ToInt32(floor.Trim(' ')));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error on inserted floor: {ex.Message}.");
                Console.Write("Please, insert a valid floor: ");
                return GetSelectedFloors();
            }
            return floors;
        }

        private static void ShowInitialInfo()
        {
            Console.WriteLine("WARNNING: For test external service, first run 'ElevatorSimulator.ExternalService.exe' application.\n");
            Console.WriteLine("Sample from the elevator simulator started successfully.");
            Console.WriteLine($"External service Host: {Bootstrapper.GetExternalServiceHostFromAppConfigFile()}");
            Console.WriteLine($"External service Port: {Bootstrapper.GetExternalServicePortFromAppConfigFile()}");
            Console.WriteLine($"Elevator type: {Bootstrapper.GetElevatorTypeFromAppConfigFile()}.");
            Console.WriteLine("\nIf you want to change this info, use the 'ElevatorSimulator.Sample.exe.config' file for this project.");
            Console.WriteLine("\n.............................................................................................................\n\n");
        }

        private static bool CheckIsHasANewBoardPassengers()
        {
            Console.WriteLine("\nElevator stopped, waitting a external call or a new route.");
            Console.WriteLine("Press ESC to finish application or any key to new passengers boarding.");

            while (true)
            {
                // nesse metodo, checa se existem chamadas externas
                if(elevatorController.HasFloorTarget())
                    return false;

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Application finished.");
                        Environment.Exit(0);
                    }
                    return true;
                }
            }
        }
    }
}
