using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace ElevatorSimulator.ExternalService
{
    /// <summary>
    /// Classe client utilizada para fazer a conexão com o server.
    /// Possui apenas um método que retorna todos os andares das chamadas externas.
    /// 
    /// </summary>
    /// 
    public class ExternalServiceClient
    {
        private JavaScriptSerializer serializer;
        private String host;
        private int port;
        public ExternalServiceClient(String host, int port)
        {
            this.host       = host;
            this.port       = port;
            this.serializer = new JavaScriptSerializer();
        }
        public List<int> GetCalledFloors()
        {
            var client = new TcpClient(this.host, this.port);
            var ns = client.GetStream();

            var bytes = new byte[1024];
            var bytesRead = ns.Read(bytes, 0, bytes.Length);
            var data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
            client.Close();

            return this.serializer.Deserialize<List<int>>(data);
        }
    }
}
