using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonementCalculator
{
    class AtonementTarget
    {
        public float DurationRemaining;
        public float HealingTaken;
        public float HealingCap;

        public AtonementTarget(float durationRemaining, float healingCap)
        {
            HealingTaken = 0;
            DurationRemaining = durationRemaining;
            HealingCap = healingCap;
        }

        public void TakeHealing(float healing)
        {
            if (DurationRemaining > 0)
            {
                HealingTaken += healing;
                if (HealingTaken >= HealingCap)
                {
                    HealingTaken = HealingCap;
                }
            }
        }
    }
}