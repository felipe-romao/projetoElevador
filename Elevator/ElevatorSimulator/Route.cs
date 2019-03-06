using System.Collections.Generic;
using System.Linq;

namespace ElevatorSimulator
{
    /// <summary>
    /// Classe responsável por receber os andares selecionados e gerar a rota,
    /// respeitando o critério do sentido que ele esta se movendo.
    /// </summary>
    public class Route
    {
        private List<int> floorsSelected = new List<int>();
        private List<int> floorsRoute = new List<int>();

        private int lastFloor = 0;
        private bool isUpDirection = true;

        public bool HasNext
        {
            get { return this.floorsRoute.Count != 0; }
        }

        public bool IsUpDirection => this.isUpDirection;

        /// <summary>
        /// Método resposável em armazenar os os andares selecionados.
        /// </summary>
        /// <param name="floor"> andar selecionado</param>
        /// 
        public void AddSelectedFloor(int floor)
        {
            if (!this.floorsSelected.Contains(floor))
               this.floorsSelected.Add(floor);
        }


        /// <summary>
        /// Método responsável por criar uma rota.
        /// A definição da rota leva em conta o sentido da ultima movimentação.
        /// </summary>
        /// <param name="floorCurrent">Recebe o andar que o elevador esta parado</param>
        /// 
        public void Create(int floorCurrent)
        {
            if (this.floorsSelected.Count == 0)
                return;

            this.mergeFloorList();

            if (isUpDirection)
            {
                this.GetAllFloorsUpCurrenctFloor(floorCurrent);
                this.GetAllFloorsDownCurrenctFloor(floorCurrent);
            }
            else
            {
                this.GetAllFloorsDownCurrenctFloor(floorCurrent);
                this.GetAllFloorsUpCurrenctFloor(floorCurrent);
            }
            this.floorsSelected.Clear();
        }

        /// <summary>
        /// Método responsável em juntar a rota existente com uma nova, caso haja. 
        /// </summary>
        /// 
        public void mergeFloorList()
        {
            if (this.floorsRoute.Count() > 0)
            {
                this.floorsSelected.AddRange(this.floorsRoute);
                this.floorsRoute.Clear();
            }
        }

        /// <summary>
        /// Método responsável em informar o próximo destino da rota.
        /// </summary>
        /// <returns>próximo andar da rota</returns>
        /// 
        public int Next()
        {
            int floor = this.floorsRoute[0];
            this.floorsRoute.Remove(floor);

            isUpDirection = true;
            if (lastFloor > floor)
                isUpDirection = false;

            lastFloor = floor;
            return floor;   
        }

        private void GetAllFloorsUpCurrenctFloor(int floorCurrent)
        {
            var list = floorsSelected.OrderBy(i => i).ToList();

            foreach (int floor in list)
            {
                if (floor > floorCurrent)
                    floorsRoute.Add(floor);
            }
        }

        private void GetAllFloorsDownCurrenctFloor(int floorCurrent)
        {
            var list = floorsSelected.OrderByDescending(i => i).ToList();
            foreach (int floor in list)
            {
                if (floor < floorCurrent)
                    floorsRoute.Add(floor);
            }
        }
    }
}
