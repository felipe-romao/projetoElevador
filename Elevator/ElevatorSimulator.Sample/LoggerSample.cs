﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Sample
{
    public class LoggerSample : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
