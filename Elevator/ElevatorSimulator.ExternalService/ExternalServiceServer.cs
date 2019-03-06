using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ElevatorSimulator.ExternalService
{
    /// <summary>
    /// Classe responsável em fornecer um servidor, que roda em background utilizado a implentação do TcpListener na integração com o client.
    /// </summary>
    /// 
    public class ExternalServiceServer
    {
        private String host;
        private int port;
        private ExternalCallRepository repository;
        private JavaScriptSerializer serializer;

        public ExternalServiceServer(String host, int port, ExternalCallRepository repository, JavaScriptSerializer serializer)
        {
            this.host       = host;
            this.port       = port;
            this.repository = repository;
            this.serializer = serializer;
        }

        public bool Running
        {
            get; private set;
        }

        public async Task Start()
        {
            await Task.Run(() =>
            {
                this.Running = true;
                this.TcpIPServiceExecute();
            });
        }
        public void Stop()
        {
            this.Running = false;
        }

        /// <summary>
        /// Método responsável em escutar as requisições do client e entregar os andares armazendos do repositório.
        /// Após o envio, os andares são excuídos.
        /// </summary>
        /// 
        private void TcpIPServiceExecute()
        {
            var localAddr = IPAddress.Parse(this.host);
            var listener = new TcpListener(localAddr, this.port);

            listener.Start();

            while (this.Running)
            {
                var client = listener.AcceptTcpClient();
                var ns = client.GetStream();
                var floors = this.repository.GetAllCalledFloors();
                var byteTime = Encoding.ASCII.GetBytes(this.serializer.Serialize(floors));

                try
                {
                    ns.Write(byteTime, 0, byteTime.Length);
                    ns.Close();
                    client.Close();

                    this.repository.DeleteFloors(floors);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            listener.Stop();
        }
    }
}
