using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace OniShedding
{
    public class HediffGiver_OniShedCheckerSilent : HediffGiver
    {

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            float x = (float)pawn.ageTracker.AgeBiologicalYears / pawn.RaceProps.lifeExpectancy;
            if (Rand.MTBEventOccurs(this.ageFractionMtbDaysCurve.Evaluate(x), 60000f, 60f))
            {
                if (this.minPlayerPopulation > 0 && pawn.Faction == Faction.OfPlayer && PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Count<Pawn>() < this.minPlayerPopulation)
                {
                    return;
                }
                if (base.TryApply(pawn, null))
                {
                   ;
                }
            }
        }
        public SimpleCurve ageFractionMtbDaysCurve;

        public int minPlayerPopulation;
    }
}