using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonementCalculator
{
    class Spell
    {
        public float Coefficient;
        public float CastTime;
        public float ManaCost;
        public int Targets;
        public bool GivesAtonementHealing;
        public bool GivesAtonement;
        public int AtonementTargets;
        public float AtonementDuration;

        public Spell(float coefficient, float castTime, float manaCost, int targets,
            bool givesAtonementHealing, bool givesAtonement = false, int atonementTargets = 0, float atonementDuration = 0)
        {
            Coefficient = coefficient;
            CastTime = castTime;
            ManaCost = manaCost;
            Targets = targets;
            GivesAtonementHealing = givesAtonementHealing;
            GivesAtonement = givesAtonement;
            AtonementTargets = atonementTargets;
            AtonementDuration = atonementDuration;
        }
    }
}
