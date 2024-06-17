using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace DebtboundFarmer
{
    internal class FarmersDebt
    {
        public bool OverrideConfig { get; set; }
        public int TotalDebt { get; set; } = 0;
        public int InterestDebt { get; set; } = 0;
        public List<DebtPayment> Payments { get; set; } = new List<DebtPayment>();
        public JojaDevelopmentForm JojaCosts { get; set; } = new JojaDevelopmentForm();

        public string PlanName { get; set; } = "Undefined Plan";

        public int ObtainTotalDebt()
        {
            int sumDebt = 0;
            foreach (DebtPayment pay in Payments)
            {
                sumDebt += pay.PaymentSize;
            }

            return sumDebt;
        }
        /*
        public static bool ValidateJSONFile(FarmersDebt debtToValidate)
        {
            int sumPercent = 0;
            int sumJojo = 0;
            bool valid = true;

            Utilities.LogDebug("Attempting to parse settings from JSON file.", ConsoleColor.Green);
            foreach (DebtPayment payment in debtToValidate.Payments)
            {
                if (payment.PaymentSize < 1 || payment.PaymentSize > MAXIMUM_PERCENT_SUM)
                {
                    ModEntry.Instance.Monitor.Log("Error in payment: " + payment.PaymentName + "\t Payment size < 1 (.1%) or > " + MAXIMUM_PERCENT_SUM + " (100%)", LogLevel.Warn);
                    valid = false;
                }
                if (payment.DaysToPay < 1)
                {
                    ModEntry.Instance.Monitor.Log("Error in payment: " + payment.PaymentName + "\t Days to pay less than 1", LogLevel.Warn);
                    valid = false;
                }
                if (payment.JojoDevelopmentTokens < 0 || payment.JojoDevelopmentTokens > MAXIMUM_JOJO_TOKENS)
                { 
                    ModEntry.Instance.Monitor.Log("Error in payment: " + payment.PaymentName + "\t JojoDevelopmentTokens < 0 or > " + MAXIMUM_JOJO_TOKENS, LogLevel.Warn);
                    valid = false;
                }

                sumPercent += payment.PaymentSize;
                sumJojo += payment.JojoDevelopmentTokens;
            }

            if  (sumPercent != MAXIMUM_PERCENT_SUM)
            {
                ModEntry.Instance.Monitor.Log("Sum of percentages in payments does not equal " + MAXIMUM_PERCENT_SUM, LogLevel.Warn);
                valid = false;
            }

            if (sumJojo != MAXIMUM_JOJO_TOKENS)
            {
                ModEntry.Instance.Monitor.Log("Sum of JojoDevelopmentTokens does not equal " + MAXIMUM_JOJO_TOKENS, LogLevel.Warn);
                valid = false;
            }

            if (valid)
            {
                Utilities.LogDebug("Good news - JSON file validated for use.", ConsoleColor.Green);
            }

            return valid;
        }
        */
        public void Report()
        {
            Utilities.LogDebug("Reporting on DebtPayment Package", ConsoleColor.Green);
            Utilities.LogDebug("Override Config:" + OverrideConfig, ConsoleColor.Yellow);
            Utilities.LogDebug("TotalDebt:" + TotalDebt, ConsoleColor.Yellow);
            Utilities.LogDebug("InterestDebt:" + InterestDebt, ConsoleColor.Yellow);

            foreach (DebtPayment pay in Payments)
            {
                pay.Report();
            }
        }
    }
}
