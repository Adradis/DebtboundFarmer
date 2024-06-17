using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace DebtboundFarmer
{
    internal class ModData
    {
        public string CurrentVersion;
        public const string FileVersion = "0.0.1";

        public ModData()
        {
            CurrentVersion = FileVersion;
        }

        public void Report()
        {
            Utilities.LogDebug("Reporting all Mod Data variables.", ConsoleColor.Green);
            Utilities.LogDebug("Data version: " + CurrentVersion + "\t Latest version: " + FileVersion, ConsoleColor.Magenta);


            Utilities.LogDebug("End of Mod Data", ConsoleColor.DarkGreen);
        }
    }
}
