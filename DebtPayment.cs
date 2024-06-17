using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtboundFarmer
{
    internal class DebtPayment
    {
        public string PaymentName { get; set; }
        public int PaymentSize { get; set; }
        public int PaidTowardsDebt { get; private set; }
        public bool Finished { get; private set; }
        public int DaysToPay { get; set; }
        public int JojaDevelopmentCredits { get; set; }

        public DebtPayment()
        {
            PaymentName = "Unlisted";
            PaymentSize = -1;
            PaidTowardsDebt = 0;
            Finished = false;
            DaysToPay = -1;
            JojaDevelopmentCredits = -1;
        }

        public void Report()
        {
            Utilities.LogDebug("Reporting Debt Payment Package: " + PaymentName, ConsoleColor.Green);
            Utilities.LogDebug("PaymentSize:" + PaymentSize, ConsoleColor.Yellow);
            Utilities.LogDebug("DaysToPay:" + DaysToPay, ConsoleColor.Yellow);
            Utilities.LogDebug("JojaDevelopmentCredits:" + JojaDevelopmentCredits, ConsoleColor.Yellow);
        }
    }
}
