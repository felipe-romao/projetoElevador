using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorSimulator.ExternalService
{
    /// <summary>
    /// Classe que simula um repositório dos andares, fornecendo os métodos para Add, GetAll, e Delete.
    /// </summary>
    /// 
    public class ExternalCallRepository
    {
        private static List<int> calledFloors = new List<int>();

        public void AddCalledFloor(int floor)
        {
            ExternalCallRepository.calledFloors.Add(floor);
        }

        public List<int> GetAllCalledFloors()
        {
            return ExternalCallRepository.calledFloors.ToList();
        }

        public void DeleteFloors(List<int> list)
        {
            foreach(var floor in list)
                ExternalCallRepository.calledFloors.Remove(floor);
        }
    }
}
