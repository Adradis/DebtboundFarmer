using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DebtboundFarmer;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace DebtboundFarmer
{
    internal class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public IModHelper ModHelper { get; private set; }
        public ModConfig Config { get; private set; }
        public ModData Data { get; private set; }
        public Harmony Harmony { get; private set; }

        public const string ModID = "DebtboundFarmer";

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModHelper = helper;
            Config = ModHelper.ReadConfig<ModConfig>();

            ModHelper.Events.GameLoop.GameLaunched += RegisterConfigWithGMCM;
            ModHelper.Events.GameLoop.SaveLoaded += SetupMod;

            RegisterDebugCommands();
        }

        public void SetupMod(object? sender, SaveLoadedEventArgs args)
        {
            Data = ModHelper.Data.ReadSaveData<ModData>(ModID);
            bool loadFromSave = true;

            if (Data == null)
            {
                if (Game1.stats.DaysPlayed > 1)
                {
                    // TODO: Add warnings to ReadMe, Nexus Page
                    Monitor.Log("Mod Error: Attempting to setup " + ModID + " on an in-progress save.", LogLevel.Error);
                    Monitor.Log(ModID + " requires a fresh save to run. " + ModID + " will not be running on this save.", LogLevel.Error);
                    return;
                }
                else
                {
                    Data = new ModData();
                    loadFromSave = false;
                    Utilities.LogDebug("Created new Mod Data", ConsoleColor.Cyan);
                }
            }

#if DEBUG
            Utilities.LogDebug("Successfully loaded saved data, version: " + Data.CurrentVersion, ConsoleColor.Green);
            if (Data.CurrentVersion != ModData.FileVersion)
            {
                Utilities.LogDebug("Attempting to load mismatched mod data. This may cause issues.", ConsoleColor.Red);
            }
#endif

            bool successfulLoad;
            if (loadFromSave)
            {
                successfulLoad = DebtManager.LoadDebtFromSave(Data);
            }
            else
            {
                successfulLoad = DebtManager.LoadDebtFromConfig(Config);
            }

            if (successfulLoad)
            {
                /// TODO: There will be other registered event handlers here - The day to day managing of the mod will be registered here.
                ModHelper.Events.GameLoop.DayEnding += SaveData;
            }
        }

        public void RegisterHarmonyPatches()
        {
            /// TODO: Implement Harmony patches.
            // Need to replace Jojo Development Form onClick
            // Need to register Furniture.CheckForAction
        }

        public void RegisterConfigWithGMCM(object? sender, GameLaunchedEventArgs args)
        {
            var GMCMConfig = ModHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (GMCMConfig == null)
            {
                Monitor.Log("Unable to register mod with Generic Mod Config Menu. If it is not installed, you may ignore this warning.", LogLevel.Warn);
                Monitor.Log("It is highly recommended you install Generic Mod Config Menu.", LogLevel.Warn);
                return;
            }

            GMCMConfig.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => ModHelper.WriteConfig(Config)
                );

            GMCMConfig.AddTextOption(
                mod: ModManifest,
                name: () => "Payment Plan",
                tooltip: () => "The payment schedule that will be used for the debt.",
                getValue: () => Config.PaymentPlan,
                setValue: value => Config.PaymentPlan = value,
                allowedValues: new string[] { "PlanOne", "PlanTwo", "PlanThree", "PlanFour", "Custom" }
                );

            GMCMConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Starting Debt",
                tooltip: () => "The starting debt you owe. Minimum: 1000",
                getValue: () => Config.StartingDebt,
                setValue: value =>
                    {
                        if (value < 1000)
                        {
                            value = 1000;
                        }
                        Config.StartingDebt = value;
                    }
                );

            GMCMConfig.AddSectionTitle(
                mod: ModManifest,
                text: () => "Modifiers"
                );

            GMCMConfig.AddTextOption(
                mod: ModManifest,
                name: () => "Difficulty",
                tooltip: () => "Difficulty modifier - Adjusts debt and interest rate. Easy: 0.5x, Normal: 1x, Hard: 1.5x, Very Hard: 2x, Extreme: 3x",
                getValue: () => Config.DifficultyModifier,
                setValue: value => Config.DifficultyModifier = value,
                allowedValues: new string[] { "Easy", "Normal", "Hard", "Very Hard", "Extreme" }
                );

            GMCMConfig.AddBoolOption(
                mod: ModManifest,
                name: () => "Escalating Interest",
                tooltip: () => "If true, missed payments will increase the interest rate on future missed payments.",
                getValue: () => Config.EscalatingInterest,
                setValue: value => Config.EscalatingInterest = value
                );

            /*
            Config.AddTextOption(
            mod: this.ModManifest,
            name: () => "Harvest Limit Method",
            tooltip: () => "How the maximum harvests for a crop should be determined.",
            getValue: () => this.Config.HarvestCountMethod,
            setValue: value => this.Config.HarvestCountMethod = value,
            allowedValues: new string[] { "Fixed", "Harvests per Season", "(Hard) Harvests Per Season" }
            );
            */

            Utilities.LogDebug("Successfully loaded and registered configuration", ConsoleColor.Green);
        }

        public void RegisterDebugCommands()
        {
            ModHelper.ConsoleCommands.Add("DebtboundSaveData", "Reports the saved data for the current file", ConsoleReportSaveData);
            ModHelper.ConsoleCommands.Add("DebtboundMilestones", "Lists all debt payment milestones for this save", ConsoleReportMilestones);
            ModHelper.ConsoleCommands.Add("DebtboundJSON", "Runs verification on debt payments for a given filename (no path)", ConsoleJSONVerification);
            /// TODO: Implement commands to rip out all mod data from save.
            /// Users will need to sleep, then close the game and remove the mod.
        }

        public void SaveData(object? sender, DayEndingEventArgs args)
        {
            if (Data != null)
            {
                ModHelper.Data.WriteSaveData(ModID, Data);
            }
            else
            {
                Monitor.Log("Error: Tried to save null data - This should never happen. Please report this to the developer if you see it.", LogLevel.Error);
            }
        }

        public void ConsoleReportSaveData(string command, string[] args)
        {
            if (Data != null)
            {
                Data.Report();
            }
            else
            {
                Monitor.Log("No mod data available to report.", LogLevel.Alert);
            }
        }

        public void ConsoleReportMilestones(string command, string[] args)
        {
            Utilities.LogDebug("This console command is currently not implemented", ConsoleColor.White);
        }
        
        public void ConsoleJSONVerification(string command, string[] args)
        {
            FarmersDebt? debtDebug = new FarmersDebt();
            if (args.Length < 2)
            {
                Monitor.Log("Called JSON Verification without an argument. Aborting.", LogLevel.Debug);
                return;
            }

            if (Helper != null)
            {
                debtDebug = ModHelper.Data.ReadJsonFile<FarmersDebt>("data/" + args[0]);
            }
            if (debtDebug != null)
            {
                DebtManager.ValidateDebtJSON(debtDebug, isConsoleValidation: true);
                if (args[1] == "true")
                {
                    debtDebug.Report();
                    debtDebug.JojaCosts.Report();
                }
            }
            else
            {
                Monitor.Log("Called JSON Verification on a file that failed to load", LogLevel.Debug);
            }
        }
    }
}


/*

        Config.Register(
            mod: this.ModManifest,
            reset: () => this.Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(this.Config)
        );

        Config.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Gentle Junimos",
            tooltip: () => "If enabled, Junimo Huts will not cause damage to the crop when harvesting.",
            getValue: () => this.Config.GentleJunimos,
            setValue: value => this.Config.GentleJunimos = value
        );

        Config.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Allow Harvest Override",
            tooltip: () => "If enabled, custom crops & Content Patcher patches can set their own harvest limits for crops",
            getValue: () => this.Config.AllowOverride,
            setValue: value => this.Config.AllowOverride = value
            );

        Config.AddTextOption(
            mod: this.ModManifest,
            name: () => "Harvest Limit Method",
            tooltip: () => "How the maximum harvests for a crop should be determined.",
            getValue: () => this.Config.HarvestCountMethod,
            setValue: value => this.Config.HarvestCountMethod = value,
            allowedValues: new string[] { "Fixed", "Harvests per Season", "(Hard) Harvests Per Season" }
            );

        Config.AddTextOption(
            mod: this.ModManifest,
            name: () => "Crop Randomization",
            getValue: () => this.Config.RandomizationSetting,
            setValue: value => this.Config.RandomizationSetting = value,
            allowedValues: new string[] { "None", "Per Crop", "Total" }
            );

        Config.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Harvest Values",
            tooltip: () => "Sets harvest values for crop limits. Crop Random means all of a crop planted that day will share a harvest limit. Full Random means each individual plant can vary."
            );

        Config.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Fixed Number of Harvests",
            tooltip: () => "If Harvest Limit Method is set to fixed, the base number of times a crop will regrow.",
            getValue: () => this.Config.BaseHarvests,
            setValue: value =>
            {
                if (value < 0)
                {
                    this.Config.BaseHarvests = 0;
                    return;
                }

                this.Config.BaseHarvests = value;
            }
            );

        Config.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Lower Randomization Limit",
            tooltip: () => "The maximum number of harvests (inclusive) below the expected harvest that randomization can produce. Will never push a crop below 1 harvest.",
            getValue: () => this.Config.LowerRandModifier,
            setValue: value =>
            {
                if (value < 0)
                {
                    this.Config.LowerRandModifier = 0;
                    return;
                }

                this.Config.LowerRandModifier = value;
            }
            );

        Config.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Upper Randomization Limit",
            tooltip: () => "The maximum number of harvests (inclusive) above the expected harvest that randomization can produce.",
            getValue: () => this.Config.UpperRandModifier,
            setValue: value =>
            {
                if (value < 0)
                {
                    this.Config.UpperRandModifier = 0;
                    return;
                }

                this.Config.UpperRandModifier = value;
            }
            );
    }
        }
    }
*/