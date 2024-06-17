using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtboundFarmer
{
    public sealed class ModConfig
    {
        /*
         * Difficulty modifier - .5, 1, 1.5, 2, 3
         * Starting Debt from drop-down (500k, 1000k, 2000k, 3000k, 5000k)
         * Interest Rate from drop-down (4%, 6%, 8%, 10%, 12%)
         * Escalating Interest - Missed payments increase interest by 2% each time until caught up.
         * Difficulty Modifier is a multiplier on Starting Debt, Interest Rate.
         * Be explicitly clear how difficulty works.
         */

        public string DifficultyModifier;
        public int StartingDebt;
        public int InterestRate;
        public bool EscalatingInterest;
        public string PaymentPlan;

        public ModConfig()
        {
            DifficultyModifier = "Normal";
            StartingDebt = 1000000;
            InterestRate = 6;
            EscalatingInterest = false;
            PaymentPlan = "PlanOne";
        }
    }
}
