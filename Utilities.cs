﻿using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtboundFarmer
{
    internal static class Utilities
    {
        [Conditional("DEBUG")]
        public static void LogDebug(string message, ConsoleColor foreColor = ConsoleColor.DarkGray)
        {
            Console.ForegroundColor = foreColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
