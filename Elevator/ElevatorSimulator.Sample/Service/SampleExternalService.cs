using ElevatorSimulator.ExternalService;
using ElevatorSimulator.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Sample.Service
{
    public class SampleExternalService : IExternalService
    {
        private String host;
        private int port;

        public SampleExternalService(String host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public List<int> GetExternalCall()
        {
            try
            {
                var client = new ExternalServiceClient(this.host, this.port);
                return client.GetCalledFloors();
            }
            catch
            {
                return null;
            }
        }
    }
}
