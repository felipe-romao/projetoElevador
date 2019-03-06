using ElevatorSimulator.Factory;
using ElevatorSimulator.Sample.Service;
using ElevatorSimulator.Service;
using System;
using System.Configuration;

namespace ElevatorSimulator.Sample
{
    public class Bootstrapper
    {
        public ElevatorController Bootstrap()
        {
            ILogger logger                   = new LoggerSample();
            ElevatorConfig config            = GetElevatorConfig();
            IElevatorFactory elevatorFactory = new ElevatorFactory();
            IRouteFactory routeFactory       = new RouteFactory();
            IExternalService externalService = new SampleExternalService(GetExternalServiceHostFromAppConfigFile(), GetExternalServicePortFromAppConfigFile());
            ElevatorType elevatorType        = ElevatorType.FromValue(GetElevatorTypeFromAppConfigFile());

            ElevatorController elevatorController = new ElevatorController(config, elevatorType, elevatorFactory, routeFactory, externalService, logger);

            return elevatorController;
        }

        private ElevatorConfig GetElevatorConfig()
        {
            var type = GetElevatorTypeFromAppConfigFile();
            switch (type)
            {
                case "SOCIAL":
                    return new ElevatorConfig(30, 600);
                case "SERVICE":
                    return new ElevatorConfig(30, 1000);
                default:
                    throw new ArgumentException($"Elevator type not supported: '{type}'. Please enter with SOCIAL or SERVICE.");
            }
        }

        public static string GetElevatorTypeFromAppConfigFile()
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings["ElevatorType"] ?? "Not Found";
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
    }
}
