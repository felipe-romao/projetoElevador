using System;
using System.Configuration;
using System.Web.Script.Serialization;

namespace ElevatorSimulator.ExternalService
{
    /// <summary>
    /// Aplicação console responsável em receber as requisições de chamadas externas e armazena-las em um repositório.
    /// Possui um serviço que roda em background que fornece os andares das chamadas externas para o simulador.
    /// 
    /// Os andares referente a cada chamada externa ficam armazenado em uma lista estática, 
    /// ou seja, disponiveis enquando a aplicação esta ativa.
    /// 
    /// </summary>
    /// 
    public class Program
    {
        private static JavaScriptSerializer serializer;
        private static ExternalCallRepository repository;

        public static void Main(string[] args)
        {
            ExternalServiceServer server = null;
            try
            {
                var host = GetExternalServiceHostFromAppConfigFile();
                var port = GetExternalServicePortFromAppConfigFile();

                serializer = new JavaScriptSerializer();
                repository = new ExternalCallRepository();

                server = new ExternalServiceServer(host, port, repository, serializer);
                server.Start();

                Console.WriteLine("External server service stated successfully.");
                Console.WriteLine($"Host: {host}.");
                Console.WriteLine($"Port: {port}.\n");
                Console.WriteLine("............................................\n\n");

                GetExternalCall();
                server.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while running the external service: {ex.Message}.");
                server.Stop();
            }
            Console.ReadKey();
        }

        public static void GetExternalCall()
        {
            while (true)
            {
                repository.AddCalledFloor(GetSelectedFloor());

                Console.Write("New floor? [y/n]: ");
                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Y)
                {
                    Console.WriteLine("\nApplication finished!");
                    break;
                }
            }
        }

        public static string GetExternalServiceHostFromAppConfigFile()
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings["ExternalServiceHost"] ?? "127.0.0.1";
        }

        public static int GetExternalServicePortFromAppConfigFile()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return Convert.ToInt32(appSettings["ExternalServicePort"] ?? "13000");
            }
            catch
            {
                return 13000;
            }
        }

        private static int GetSelectedFloor()
        {
            try
            {
                Console.Write("\nPlease, insert a valid floor [0..30]: ");
                var floor = Convert.ToInt32(Console.ReadLine());
                CheckIsFloorValid(floor);
                return floor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error, please enter a valid floor: {ex.Message}.");
                return GetSelectedFloor();
            }
        }

        private static void CheckIsFloorValid(int floor)
        {
            if (floor < 0 || floor > 30)
                throw new ArgumentException("Error invalid floor. Please enter with floor between 0 and 30.");
        }
    }
}
