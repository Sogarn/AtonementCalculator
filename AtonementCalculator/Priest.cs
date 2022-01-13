using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonementCalculator
{
    class Priest
    {
        public bool Output { get; set; }

        Random RNG;
        public int Seed;
        public float Timer;
        public float Intellect { get; set; }
        public float BaseInt { get; set; }
        public float Health { get; set; }
        public float BaseHealth { get; set; }
        public float CritChance { get; set; }
        public float Haste { get; set; }
        public float Mastery { get; set; }
        public float Versatility { get; set; }
        public int UseTrinket { get; set; }
        public float ManaSpent;
        public float SpiritShellCap;
        public float SpiritShellDuration;
        public bool SpiritShellStarted;
        public float SchismDuration;
        public float TimeUntilNextDotTick;
        public float TimeUntilNextMindbenderAuto;
        public float MindbenderDuration;
        public float DotDuration;
        public float SchismMultiplier = 1.25f;
        public float SpiritShellMultiplier = 1.07f;
        public float SinsOfTheManyMultiplier;
        public List<AtonementTarget> Atonements;
        public Dictionary<string, Spell> Grimoire;

        // Initialize actor (without seed)
        public Priest(float intellect, float health, float critChance, float haste, float mastery, float versatility, int useTrinket = 181, int seed = -1, bool output = false)
        {
            // Use seed if applicable
            Seed = seed;
            RNG = (Seed > -1 ? new Random(Seed) : new Random());

            // Stat sanity check
            CritChance = (critChance >= 5 ? critChance : 5);
            Haste = (haste >= 0 ? haste : 0);
            Mastery = (mastery >= 11.2f ? mastery : 11.2f);
            Versatility = (versatility >= 0 ? versatility : 0);

            Output = output;
            Timer = 0;
            // Flask + food
            BaseInt = intellect + 88;
            // Arcane intellect
            Intellect = BaseInt * 1.05f;
            // Armor kit
            BaseHealth = health + (32 * 39);
            // Power Word: Fortitude
            Health = BaseHealth * 1.05f;

            ManaSpent = 0;
            // Convert stats to multipliers / percentages
            CritChance = critChance / 100f;
            Haste = haste / 100f + 1;
            Mastery = mastery / 100f + 1;
            Versatility = versatility / 100f + 1;
            UseTrinket = useTrinket;
            TimeUntilNextDotTick = 0;
            TimeUntilNextMindbenderAuto = 0;
            DotDuration = 0; // 20 seconds base
            MindbenderDuration = 0; // 12 seconds base
            SchismDuration = 0;
            SpiritShellCap = 0.6f * health;
            SpiritShellDuration = 11f;
            SpiritShellStarted = false;
            Atonements = new List<AtonementTarget>();
            InitializeGrimoire();
        }

        // Count Atonements
        public int AtonementCount()
        {
            int count = 0;
            foreach (AtonementTarget target in Atonements)
            {
                if (target.DurationRemaining > 0)
                {
                    count += 1;
                }
            }
            return count;
        }

        // Update Sins of the Many
        public float UpdateSins()
        {
            switch (AtonementCount())
            {
                case 0:
                    return 1.12f;
                case 1:
                    return 1.12f;
                case 2:
                    return 1.1f;
                case 3:
                    return 1.08f;
                case 4:
                    return 1.07f;
                case 5:
                    return 1.06f;
                case 6:
                    return 1.06f;
                case 7:
                    return 1.05f;
                case 8:
                    return 1.04f;
                case 9:
                    return 1.04f;
                default:
                    return 1.03f;
            }
        }

        #region Spells and Abilities

        // Set up all spells
        public void InitializeGrimoire()
        {
            Grimoire = new Dictionary<string, Spell>();
            Grimoire.Add("Smite", new Spell(0.47f, 1.5f / Haste, 100, 1, true));
            Grimoire.Add("Penance", new Spell(0.4f, (2f / 3f) / Haste, 800f / 3f, 1, true));
            Grimoire.Add("MindBlast", new Spell(0.9792f, 1.5f / Haste, 1250, 1, true));
            Grimoire.Add("Schism", new Spell(1.5f, 1.5f / Haste, 250, 1, true));
            Grimoire.Add("MindGames", new Spell(3f, 1.5f / Haste, 1000, 1, true));
            Grimoire.Add("PTW", new Spell(0.223f, 1.5f / Haste, 900, 1, true));
            Grimoire.Add("PTW Tick", new Spell(0.124f, 0, 0, 1, true));
            Grimoire.Add("MindbenderAuto", new Spell(1.5f * 1.97f / 6f, 0, 0, 1, true));
            Grimoire.Add("Radiance", new Spell(1.05f, 2f / Haste, 3250, 5, false, true, 5, 15f * 0.6f));
            Grimoire.Add("Bubble", new Spell(1.65f, 1.5f / Haste, 1550, 1, false, true, 1, 15f));
        }

        // Cast Smite
        public void Smite()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: Smite");
            }
            Cast(Grimoire["Smite"]);
        }

        // Cast Penance
        public void Penance()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: Penance");
            }
            // Seperate the bolts
            Cast(Grimoire["Penance"]);
            Cast(Grimoire["Penance"]);
            Cast(Grimoire["Penance"]);
        }

        // Cast MindBlast
        public void MindBlast()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: MindBlast");
            }
            Cast(Grimoire["MindBlast"]);
        }

        // Cast Schism
        public void Schism()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: Schism");
            }
            Cast(Grimoire["Schism"]);
            SchismDuration = 9f;
        }

        // Cast MindGames
        public void MindGames()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: MindGames");
            }
            Cast(Grimoire["MindGames"]);
        }

        // Cast Purge the Wicked
        public void PTW()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: PTW");
            }
            if (DotDuration > 0)
            {
                // Capped at 6 bonus seconds
                DotDuration = DotDuration > 6 ? 26 : DotDuration + 20;
            }
            else
            {
                DotDuration = 20;
            }
            TimeUntilNextDotTick = 2f / Haste;
            Cast(Grimoire["PTW"]);
        }

        // Purge the Wicked tick
        public void PTW_Tick()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: PTW Tick");
            }
            Cast(Grimoire["PTW Tick"]);
        }

        // Cast Mindbender / trinket / Spirit Shell
        public void OneButtonMacro()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: OneButtonMacro");
            }
            MindbenderDuration = 12f;
            TimeUntilNextMindbenderAuto = 1.5f / Haste;
            SpiritShellStarted = true;
            foreach (AtonementTarget target in Atonements)
            {
                if (target.DurationRemaining > 0)
                {
                    target.DurationRemaining += 3;
                }
            }
            Intellect += UseTrinket;
            UpdateTimers(1.5f / Haste, true);
        }

        // Mindbender auto
        public void MindbenderAuto()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: MindbenderAuto");
            }
            SchismMultiplier = 1f;
            Cast(Grimoire["MindbenderAuto"]);
            SchismMultiplier = 1.25f;

            // Gain mana
            ManaSpent -= 265;
        }

        // Cast Power Word: Radiance
        public void Radiance()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: Radiance");
            }
            Cast(Grimoire["Radiance"]);
        }

        // Cast Power Word: Shield
        public void Bubble()
        {
            if (Output)
            {
                Console.WriteLine(Math.Round(Timer, 1) + "s: Bubble");
            }
            Cast(Grimoire["Bubble"]);
        }

        #endregion

        // Cast a spell
        public void Cast(Spell spell)
        {
            float castHealing = 0; // For checking
            ManaSpent += spell.ManaCost;
            UpdateTimers(spell.CastTime, spell.CastTime > 0);
            SinsOfTheManyMultiplier = UpdateSins();

            // Base output for all spells
            float baseOutput = spell.Coefficient * Intellect * Versatility;

            // Set up final healing for damage abilities (healing is half of damage)
            float baseDamageHealing = baseOutput * 0.5f * SinsOfTheManyMultiplier * SpiritShellMultiplier * Mastery;
            if (SchismDuration > 0)
            {
                baseDamageHealing *= SchismMultiplier;
            }
            float finalHealing = baseDamageHealing;

            // Damage spell
            if (spell.GivesAtonementHealing)
            {
                // Don't even track healing before Spirit Shell
                if (SpiritShellStarted)
                {
                    foreach (AtonementTarget target in Atonements)
                    {
                        // Target needs an active atonement
                        if (SpiritShellStarted && target.DurationRemaining > 0)
                        {
                            finalHealing = TestCrit(baseDamageHealing);
                            target.TakeHealing(finalHealing);
                            castHealing += finalHealing;
                        }
                    }
                    if (Output)
                    {
                        Console.WriteLine("Total healing from cast: {0}, per atonement {1}", castHealing, Math.Round(castHealing / AtonementCount()));
                    }
                }
            }
            // Healing Spell
            else if (spell.GivesAtonement)
            {
                for (int i = 0; i < spell.AtonementTargets; i++)
                {
                    Atonements.Add(new AtonementTarget(spell.AtonementDuration, SpiritShellCap));
                }
            }
        }

        // Update all timers
        public void UpdateTimers(float time, bool GCD)
        {
            if (GCD)
            {
                // Add 30 ms for realistic delays to spells on the GCD
                time += 0.03f;
            }

            // Calculate DoT and Mindbender ticks
            while (DotDuration > 0 && TimeUntilNextDotTick < 0)
            {
                TimeUntilNextDotTick += 2f / Haste;
                PTW_Tick();
            }
            while (MindbenderDuration > 0 && TimeUntilNextMindbenderAuto < 0)
            {
                TimeUntilNextMindbenderAuto += 1.5f / Haste;
                MindbenderAuto();
            }

            Timer += time;
            // Iterate through atonement targets to update remaining time
            foreach (AtonementTarget target in Atonements)
            {
                target.DurationRemaining -= (time);
            }

            // Update remaining times
            DotDuration -= time;
            TimeUntilNextDotTick -= time;
            MindbenderDuration -= time;
            TimeUntilNextMindbenderAuto -= time;

            SchismDuration -= time;
            if (SpiritShellStarted)
            {
                SpiritShellDuration -= time;
            }
        }

        // Test crit
        public float TestCrit(float baseValue)
        {
            if (RNG.NextDouble() <= CritChance)
            {
                return baseValue * 2;
            }
            else
            {
                return baseValue;
            }
        }

        // Get actual healing
        public float ActualHealing()
        {
            float output = 0;
            foreach (AtonementTarget target in Atonements)
            {
                output += target.HealingTaken;
            }
            return output;
        }
    }
}
