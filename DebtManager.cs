using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace DebtboundFarmer
{
    internal static class DebtManager
    {
        public static FarmersDebt? FarmersDebt { get; set; }
        public static bool IsInitialized { get; set; } = false;

        const int MAXIMUM_PERCENT_SUM = 1000;

        public static bool LoadDebtFromSave(ModData saveData)
        {
            return false;
        }

        public static bool LoadDebtFromConfig(ModConfig config)
        {
            FarmersDebt? newDebt = ModEntry.Instance.Helper.Data.ReadJsonFile<FarmersDebt>("data/" + config.PaymentPlan + ".json");
            if (newDebt == null)
            {
                ModEntry.Instance.Monitor.Log("Mod Error: Attempted to load a json file (" + config.PaymentPlan + ".json) that failed to read.", LogLevel.Error);
                return false;
            }

            if (!ValidateDebtJSON(newDebt))
            {
                return false;
            }
            return false;
        }

        public static bool ValidateDebtJSON(FarmersDebt debtPlan, bool isConsoleValidation = false)
        {
            if (debtPlan == null)
            {
                ModEntry.Instance.Monitor.Log("Attempted to validate a null JSON file. Returning.", LogLevel.Error);
                return false;
            }

            bool valid = true;
            int sumJojaCredits = 0;
            int sumPayment = 0;

            foreach (DebtPayment payment in debtPlan.Payments)
            {
                bool printErrorPayment = false;
                if (payment.PaymentSize <= 0)
                {
                    valid = false;
                    printErrorPayment = true;
                    ModEntry.Instance.Monitor.Log("Error in payment plan: " + payment.PaymentName + " - PaymentSize cannot be <= 0", LogLevel.Warn);
                }

                if (payment.PaymentSize > 1000 && debtPlan.OverrideConfig == false)
                {
                    valid = false;
                    printErrorPayment = true;
                    ModEntry.Instance.Monitor.Log("Error in payment plan: " + payment.PaymentName + " - PaymentSize cannot be > 1000 if OverrideConfig is false", LogLevel.Warn);
                }

                if (payment.JojaDevelopmentCredits < 0)
                {
                    valid = false;
                    printErrorPayment = true;
                    ModEntry.Instance.Monitor.Log("Error in payment plan: " + payment.PaymentName + " - JojaDevelopmentCredits cannot be < 0", LogLevel.Warn);
                }
                if (payment.DaysToPay <= 0)
                {
                    valid = false;
                    printErrorPayment = true;
                    ModEntry.Instance.Monitor.Log("Error in payment plan: " + payment.PaymentName + " - Days to Pay cannot be <= 0", LogLevel.Warn);
                }

                if (printErrorPayment)
                {
                    payment.Report();
                }

                sumJojaCredits += payment.JojaDevelopmentCredits;
                sumPayment += payment.PaymentSize;
            }

            if (!debtPlan.JojaCosts.AreAllCostsValid())
            {
                valid = false;
                ModEntry.Instance.Monitor.Log("Error in JojaCosts - One or more of the JojaCosts is < 0, which is invalid.", LogLevel.Warn);
                debtPlan.JojaCosts.Report();
            }

            if (sumJojaCredits != debtPlan.JojaCosts.GetFullCost())
            {
                valid = false;
                ModEntry.Instance.Monitor.Log("Error in plan: " + debtPlan.PlanName + " - the sum of JojaDevelopmentCredits (" + sumJojaCredits + ") in payments does not match the number of JojaDevelopmentCredits in JojaCosts (" + debtPlan.JojaCosts.GetFullCost() + ")", LogLevel.Warn);
            }

            if (sumPayment != MAXIMUM_PERCENT_SUM && debtPlan.OverrideConfig == false)
            {
                valid = false;
                ModEntry.Instance.Monitor.Log("Error in plan " + debtPlan.PlanName + "- Sum of PaymentSize must equal " + MAXIMUM_PERCENT_SUM + " if not overriding config. This plan has a sum of: " + sumPayment, LogLevel.Warn);
            }

            if (valid && isConsoleValidation)
            {
                ModEntry.Instance.Monitor.Log("Success! Plan: " + debtPlan.PlanName + " is ready for use in a save file.", LogLevel.Info);
            }

            return valid;
        }
    }
}
