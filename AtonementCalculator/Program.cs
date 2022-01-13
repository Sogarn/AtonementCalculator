using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonementCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            bool output = false;

            /* Atonement sim = 1
             * Statweight sim = 2
             * Gearcompare sim = 3
             * Individual sim = 4*/

            int mode = 2;

            int iterations = 200;

            int statVariance = 250;
            int statStep = 50;

            // Minimum number of power word: shields to cast for ramp
            int bubbleAtonements = 4;

            string filePath = @"C:\Temp\AtonementOutput.csv";
            string weightsFilePath = @"C:\Temp\AtonementStatWeights.csv";

            // Sim automatically adds flask, int food and armor kit
            float intellect = 1467;
            float health = 35600;
            float crit = 23.77f;
            float haste = 19.21f;
            float mastery = 26.85f;
            float vers = 4.8f;
            int trinket = 181;

            //For gear compare
            float netInt = 0;
            float netStam = 0;
            float netCrit = 0;
            float netHaste = 0;
            float netMastery = 0;
            float netVers = 0;
            int netTrinket = 0;

            // Multipliers stat to % or actual value
            float stamMod = 39;
            float critMod = 35;
            float hasteMod = 33;
            float masteryMod = 35 * 1.35f;
            float versMod = 40;

            // Generate seed for RNG so every iteration has the same "fights"
            Random generator = new Random();
            int[] seed = new int[iterations];
            for (int i = 0; i < iterations; i++)
            {
                seed[i] = generator.Next(0, 2147483647);
            }
            generator = null;

            // Int, Health, Crit, Haste, Mastery, Vers
            Priest MyStats = new Priest(intellect, health, crit, haste, mastery, vers, trinket, seed[0], output);

            // Run numbers for healing vs atonements
            if (mode == 1)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine("AtonementCount,Haste%,TotalHealing,AverageShell%");

                    double convertedHaste;
                    double convertedHealing;
                    double convertedAverage;
                    float totalHealing;
                    float averageTotalHealing;
                    Priest Reldarus;

                    // Test loop for haste differences (in this case, my base haste and power infusion)
                    for (float j = haste; j < 60f; j += 25f)
                    {
                        // Test loop for extra atonements
                        for (int i = 0; i < 21; i++)
                        {
                            totalHealing = 0;
                            averageTotalHealing = 0;
                            // Average out the runs
                            for (int k = 0; k < iterations; k++)
                            {
                                Reldarus = new Priest(intellect, health, crit, j, mastery, vers, trinket, seed[k]);
                                // Calculate total healing done
                                totalHealing += RunSim(Reldarus, i).ActualHealing();
                            }
                            averageTotalHealing = (float)Math.Round(totalHealing / iterations, 1);
                            convertedHaste = Math.Round(j, 1);
                            convertedHealing = Math.Round(averageTotalHealing, 1);
                            convertedAverage = Math.Round((averageTotalHealing / (i + 10f) / MyStats.SpiritShellCap * 100), 1);
                            // Atonements, Haste, Healing, Shell%
                            sw.WriteLine("{0},{1}%,{2},{3}%", (i + 10), convertedHaste, convertedHealing, convertedAverage);
                        }
                    }
                }
            }

            // Run numbers for stat weights
            if (mode == 2)
            {
                if (File.Exists(weightsFilePath))
                {
                    File.Delete(weightsFilePath);
                }
                using (StreamWriter sw = new StreamWriter(weightsFilePath, true))
                {
                    Priest Reldarus;
                    int tempAtonements;
                    float[] baselineOutput = new float[iterations];

                    for (int i = 0; i < iterations; i++)
                    {
                        // Create new actor
                        Reldarus = new Priest(intellect, health, crit, haste, mastery, vers, trinket, seed[i]);
                        // Calculate optimal atonement count for this actor
                        tempAtonements = OptimalAtonementCount(intellect, health, crit, haste, mastery, vers,
                            trinket, seed[i], bubbleAtonements);
                        // Run sim
                        baselineOutput[i] = RunSim(Reldarus, tempAtonements).ActualHealing();
                    }

                    // Hold data for later
                    float[] yArray = new float[(2 * statVariance / statStep) + 1];
                    float[] xArray = new float[yArray.Length];
                    float[] intArray = new float[yArray.Length];
                    float[] intArrayAtonements = new float[yArray.Length];
                    float[] stamArray = new float[yArray.Length];
                    float[] stamArrayAtonements = new float[yArray.Length];
                    float[] critArray = new float[yArray.Length];
                    float[] critArrayAtonements = new float[yArray.Length];
                    float[] hasteArray = new float[yArray.Length];
                    float[] hasteArrayAtonements = new float[yArray.Length];
                    float[] mastArray = new float[yArray.Length];
                    float[] mastArrayAtonements = new float[yArray.Length];
                    float[] versArray = new float[yArray.Length];
                    float[] versArrayAtonements = new float[yArray.Length];

                    // Set up X values
                    for (int i = 0; i < xArray.Length; i++)
                    {
                        xArray[i] = -statVariance + (i * statStep);
                    }

                    double tempOutput;
                    float result;
                    float atonementResult;
                    int count = 0;

                    Console.WriteLine("Starting Int");
                    // Int
                    for (int i = -statVariance; i < statVariance + 1; i += statStep)
                    {
                        tempOutput = 0;
                        atonementResult = 0;
                        for (int k = 0; k < iterations; k++)
                        {
                            // Create actor
                            Reldarus = new Priest(intellect + i, health, crit, haste, mastery, vers, trinket, seed[k]);
                            // Calculate optimal atomenets
                            tempAtonements = OptimalAtonementCount(intellect + i, health, crit, haste, mastery, vers,
                                trinket, seed[k], bubbleAtonements);
                            // Save the difference between the run and the baseline
                            tempOutput += (RunSim(Reldarus, tempAtonements).ActualHealing() - baselineOutput[k]);
                            atonementResult += Reldarus.Atonements.Count();
                        }
                        // Store result in array
                        result = (float)Math.Round((tempOutput / iterations));
                        intArray[count] = result;
                        intArrayAtonements[count] = (float)Math.Round(atonementResult / iterations);
                        count += 1;
                    }

                    count = 0;
                    Console.WriteLine("Starting Health");
                    // Health
                    for (int i = -statVariance; i < statVariance + 1; i += statStep)
                    {
                        tempOutput = 0;
                        atonementResult = 0;
                        for (int k = 0; k < iterations; k++)
                        {
                            Reldarus = new Priest(intellect, health + (i * stamMod), crit, haste, mastery, vers, trinket, seed[k]);
                            tempAtonements = OptimalAtonementCount(intellect, health + (i * stamMod), crit, haste, mastery, vers,
                                trinket, seed[k], bubbleAtonements);
                            tempOutput += (RunSim(Reldarus, tempAtonements).ActualHealing() - baselineOutput[k]);
                            atonementResult += Reldarus.Atonements.Count();
                        }
                        result = (float)Math.Round((tempOutput / iterations));
                        stamArray[count] = result;
                        stamArrayAtonements[count] = (float)Math.Round(atonementResult / iterations);
                        count += 1;
                    }

                    count = 0;
                    Console.WriteLine("Starting Crit");
                    // Crit
                    for (float i = -statVariance; i < statVariance + 1; i += statStep)
                    {
                        tempOutput = 0;
                        atonementResult = 0;
                        for (int k = 0; k < iterations; k++)
                        {
                            Reldarus = new Priest(intellect, health, crit + (i / critMod), haste, mastery, vers, trinket, seed[k]);
                            tempAtonements = OptimalAtonementCount(intellect, health, crit + (i / critMod), haste, mastery, vers,
                                trinket, seed[k], bubbleAtonements);
                            tempOutput += (RunSim(Reldarus, tempAtonements).ActualHealing() - baselineOutput[k]);
                            atonementResult += Reldarus.Atonements.Count();
                        }
                        result = (float)Math.Round((tempOutput / iterations));
                        critArray[count] = result;
                        critArrayAtonements[count] = (float)Math.Round(atonementResult / iterations);
                        count += 1;
                    }

                    count = 0;
                    Console.WriteLine("Starting Haste");
                    // Haste
                    for (float i = -statVariance; i < statVariance + 1; i += statStep)
                    {
                        tempOutput = 0;
                        atonementResult = 0;
                        for (int k = 0; k < iterations; k++)
                        {
                            Reldarus = new Priest(intellect, health, crit, haste + (i / hasteMod), mastery, vers, trinket, seed[k]);
                            tempAtonements = OptimalAtonementCount(intellect, health, crit, haste + (i / hasteMod), mastery, vers,
                                trinket, seed[k], bubbleAtonements);
                            tempOutput += (RunSim(Reldarus, tempAtonements).ActualHealing() - baselineOutput[k]);
                            atonementResult += Reldarus.Atonements.Count();
                        }
                        result = (float)Math.Round((tempOutput / iterations));
                        hasteArray[count] = result;
                        hasteArrayAtonements[count] = (float)Math.Round(atonementResult / iterations);
                        count += 1;
                    }

                    count = 0;
                    Console.WriteLine("Starting Mastery");
                    // Mastery
                    for (float i = -statVariance; i < statVariance + 1; i += statStep)
                    {
                        tempOutput = 0;
                        atonementResult = 0;
                        for (int k = 0; k < iterations; k++)
                        {
                            Reldarus = new Priest(intellect, health, crit, haste, mastery + (i / masteryMod), vers, trinket, seed[k]);
                            tempAtonements = OptimalAtonementCount(intellect, health, crit, haste, mastery + (i / masteryMod), vers,
                                trinket, seed[k], bubbleAtonements);
                            tempOutput += (RunSim(Reldarus, tempAtonements).ActualHealing() - baselineOutput[k]);
                            atonementResult += Reldarus.Atonements.Count();
                        }
                        result = (float)Math.Round((tempOutput / iterations));
                        mastArray[count] = result;
                        mastArrayAtonements[count] = (float)Math.Round(atonementResult / iterations);
                        count += 1;
                    }

                    count = 0;
                    Console.WriteLine("Starting Vers");
                    // Vers
                    for (float i = -statVariance; i < statVariance + 1; i += statStep)
                    {
                        tempOutput = 0;
                        atonementResult = 0;
                        for (int k = 0; k < iterations; k++)
                        {
                            Reldarus = new Priest(intellect, health, crit, haste, mastery, vers + (i / versMod), trinket, seed[k]);
                            tempAtonements = OptimalAtonementCount(intellect, health, crit, haste, mastery, vers + (i / versMod),
                                trinket, seed[k], bubbleAtonements);
                            tempOutput += (RunSim(Reldarus, tempAtonements).ActualHealing() - baselineOutput[k]);
                            atonementResult += Reldarus.Atonements.Count();
                        }
                        result = (float)Math.Round((tempOutput / iterations));
                        versArray[count] = result;
                        versArrayAtonements[count] = (float)Math.Round(atonementResult / iterations);
                        count += 1;
                    }

                    // Main output
                    //Console.WriteLine("Output,Int,Stam,Crit,Haste,Mastery,Vers");
                    sw.WriteLine("Output,Int,Stam,Crit,Haste,Mastery,Vers");
                    for (int i = 0; i < xArray.Length; i++)
                    {
                        sw.WriteLine("{0},{1},{2},{3},{4},{5},{6}", xArray[i],
                            intArray[i], stamArray[i], critArray[i], hasteArray[i], mastArray[i], versArray[i]);
                    }
                    sw.WriteLine();
                    // Atonements
                    sw.WriteLine("Atonements,Int,Stam,Crit,Haste,Mastery,Vers");
                    for (int i = 0; i < xArray.Length; i++)
                    {
                        sw.WriteLine("{0},{1},{2},{3},{4},{5},{6}", xArray[i],
                            intArrayAtonements[i], stamArrayAtonements[i], critArrayAtonements[i],
                            hasteArrayAtonements[i], mastArrayAtonements[i], versArrayAtonements[i]);
                    }
                    sw.WriteLine();
                    // Not-normalized stat weights
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Weights,");
                    float intWeight = SolveStatWeight(xArray, intArray);
                    float stamWeight = SolveStatWeight(xArray, stamArray);
                    float critWeight = SolveStatWeight(xArray, critArray);
                    float hasteWeight = SolveStatWeight(xArray, hasteArray);
                    float mastWeight = SolveStatWeight(xArray, mastArray);
                    float versWeight = SolveStatWeight(xArray, versArray);

                    sb.Append(intWeight + ",");
                    sb.Append(stamWeight + ",");
                    sb.Append(critWeight + ",");
                    sb.Append(hasteWeight + ",");
                    sb.Append(mastWeight + ",");
                    sb.Append(versWeight + ",");
                    sw.WriteLine(sb);
                    // Normalized stat weights
                    sb = new StringBuilder();
                    sb.Append("Normalized,");
                    sb.Append(Math.Round(intWeight / intWeight, 2) + ",");
                    sb.Append(Math.Round(stamWeight / intWeight, 2) + ",");
                    sb.Append(Math.Round(critWeight / intWeight, 2) + ",");
                    sb.Append(Math.Round(hasteWeight / intWeight, 2) + ",");
                    sb.Append(Math.Round(mastWeight / intWeight, 2) + ",");
                    sb.Append(Math.Round(versWeight / intWeight, 2) + ",");
                    sw.WriteLine(sb);
                }
            }

            // Run a gear compare
            if (mode == 3)
            {
                // We already have the base gear so lets make a copy to compare
                float tempInt = intellect + netInt;
                float tempHealth = health + (netStam * stamMod);
                float tempCrit = crit + (netCrit / critMod);
                float tempHaste = haste + (netHaste / hasteMod);
                float tempMastery = mastery + (netMastery / masteryMod);
                float tempVers = vers + (netVers / versMod);
                int tempTrinket = trinket + netTrinket;

                Priest NewStats;

                float oldGearOutput = 0;
                int oldBubbles = bubbleAtonements;
                float newGearOutput = 0;
                int newBubbles = bubbleAtonements;

                // Iterate through all seeds on both
                for (int i = 0; i < iterations; i++)
                {
                    // Old
                    MyStats = new Priest(intellect, health, crit, haste, mastery, vers, trinket, seed[i]);
                    oldBubbles = OptimalAtonementCount(intellect, health, crit, haste, mastery, vers, trinket, seed[i], bubbleAtonements);
                    oldGearOutput += RunSim(MyStats, oldBubbles).ActualHealing();

                    // New
                    NewStats = new Priest(tempInt, tempHealth, tempCrit, tempHaste, tempMastery, tempVers, tempTrinket, seed[i]);
                    newBubbles = OptimalAtonementCount(tempInt, tempHealth, tempCrit, tempHaste, tempMastery, tempVers, tempTrinket, seed[i], bubbleAtonements);
                    newGearOutput += RunSim(NewStats, newBubbles).ActualHealing();
                }

                // Output results
                Console.WriteLine("New Gear / Old Gear: {0}%", Math.Round(newGearOutput / oldGearOutput, 3) * 100);
                Console.ReadLine();
            }

            // Run a lone sim
            if (mode == 4)
            {
                bubbleAtonements = OptimalAtonementCount(intellect, health, crit, haste, mastery, vers,
                    trinket, seed[0], bubbleAtonements);
                RunSim(MyStats, bubbleAtonements);
            }
        }

        // Returns slope of regression line
        public static float SolveStatWeight(float[] xValues, float[] yValues)
        {
            float count = xValues.Length;
            float sumX = 0;
            float xAverage = 0;
            float sumY = 0;
            float yAverage = 0;
            float sumXY = 0;
            float sumXSquared = 0;
            float sumProductDifference = 0;
            float sumSquaresDifference = 0;
            float finalAnswer;

            // get sumX
            foreach (float x in xValues)
            {
                sumX += x;
            }

            // get x average
            xAverage = sumX / count;

            // get sumY
            foreach (float y in yValues)
            {
                sumY += y;
            }

            // get y average
            yAverage = sumY / count;

            // get sum xy
            for (int i = 0; i < count; i++)
            {
                sumXY += (xValues[i] * yValues[i]);
            }

            // get sum x squared
            for (int i = 0; i < count; i++)
            {
                sumXSquared += (xValues[i] * xValues[i]);
            }

            // get sum product difference (sXY)
            sumProductDifference = sumXY - ((sumX * sumY) / count);

            // get sum squares difference (sXX)
            sumSquaresDifference = sumXSquared - ((sumX * sumX) / count);

            finalAnswer = (float)Math.Round((sumProductDifference / sumSquaresDifference), 1);

            return finalAnswer;
        }

        // Calculate optimal atonements for one seed
        public static int OptimalAtonementCount(float intellect, float health, float critChance,
            float haste, float mastery, float versatility, int useTrinket, int seed, int bubbleAtonements)
        {
            // Needs to be a gain of at least 2.5% higher
            float percentToBeat = 1.025f;

            float currentResult = 1;
            float nextResult = 1;
            float percentChange = 2;

            while (percentChange > percentToBeat)
            {
                currentResult = nextResult;
                nextResult = RunSim(
                    new Priest(intellect, health, critChance, haste, mastery, versatility, useTrinket, seed), bubbleAtonements).ActualHealing();

                // Calculate change and remove an atonement if we have lost value
                percentChange = nextResult / currentResult;
                if (percentChange > percentToBeat)
                {
                    bubbleAtonements += 1;
                }
                else
                {
                    if (percentChange < 1)
                    {
                        bubbleAtonements -= 1;
                    }
                }
            }
            return bubbleAtonements;
        }

        // Actual sim
        public static Priest RunSim(Priest Reldarus, int bubbleAtonements)
        {
            Reldarus.PTW();
            for (int k = 0; k < bubbleAtonements; k++)
            {
                Reldarus.Bubble();
            }
            Reldarus.Radiance();
            Reldarus.Radiance();
            Reldarus.OneButtonMacro();
            Reldarus.Schism();
            Reldarus.MindGames();
            Reldarus.MindBlast();
            Reldarus.Penance();
            Reldarus.PTW();
            while (Reldarus.SpiritShellDuration > 0)
            {
                Reldarus.Smite();
            }

            if (Reldarus.Output)
            {
                // Sort atonement list
                Reldarus.Atonements = Reldarus.Atonements.OrderByDescending(o => o.HealingTaken).ToList();

                // Output neat numbers
                float totalCap = 0;
                for (int i = 0; i < Reldarus.Atonements.Count(); i++)
                {
                    totalCap += Reldarus.Atonements[i].HealingTaken;
                    Console.WriteLine("{0} - Healing Recieved: {1} Percent of Cap: {2}%",
                        i + 1,
                        Math.Round(Reldarus.Atonements[i].HealingTaken, 1),
                        Math.Round(Reldarus.Atonements[i].HealingTaken / Reldarus.Atonements[i].HealingCap * 100, 1));
                }
                Console.WriteLine();
                float atonementCount = Reldarus.Atonements.Count();
                Console.WriteLine("Atonement Count: {0}", atonementCount);
                Console.WriteLine("Total Healing: {0}", totalCap);
                float averageCap = totalCap / atonementCount;
                Console.WriteLine("Average Healing: {0} ({1} %)", Math.Round(averageCap, 1), Math.Round(averageCap / Reldarus.SpiritShellCap * 100f, 1));
                Console.ReadLine();
            }
            return Reldarus;
        }
    }
}