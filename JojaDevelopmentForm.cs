using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtboundFarmer
{
    internal class JojaDevelopmentForm
    {
        public int FullCost { get; set; }
        public int GreenhouseCost { get; set; }
        public int BridgeCost { get; set; }
        public int BusCost { get; set; }
        public int BoulderCost { get; set; }
        public int MinecartCost { get; set; }

        public JojaDevelopmentForm()
        {
            GreenhouseCost = 200;
            BridgeCost = 200;
            BusCost = 200;
            BoulderCost = 200;
            MinecartCost = 200;

            FullCost = GetFullCost();
        }

        public int GetFullCost()
        {
            return GreenhouseCost + BridgeCost + BusCost + BoulderCost + MinecartCost;
        }

        public bool AreAllCostsValid()
        {
            return (GreenhouseCost >= 0 && BridgeCost >= 0 && BusCost >= 0 && BoulderCost >= 0 && MinecartCost >= 0);
        }

        public void Report()
        {
            Utilities.LogDebug("Reporting on Joja Costs", ConsoleColor.Green);
            Utilities.LogDebug("GreenhouseCost: " + GreenhouseCost, ConsoleColor.White);
            Utilities.LogDebug("BridgeCost: " + BridgeCost, ConsoleColor.White);
            Utilities.LogDebug("BusCost: " + BusCost, ConsoleColor.White);
            Utilities.LogDebug("BoulderCost: " + BoulderCost, ConsoleColor.White);
            Utilities.LogDebug("MinecartCost: " + MinecartCost, ConsoleColor.White);
            Utilities.LogDebug("Total Cost: " + GetFullCost(), ConsoleColor.White);
        }
    }
}
