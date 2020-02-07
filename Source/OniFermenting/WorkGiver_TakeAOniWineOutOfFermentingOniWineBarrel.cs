using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace OniFermenting
{
    public class WorkGiver_TakeAOniWineOutOfFermentingOniWineBarrel : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(ThingDefOf.OniFermentingWineBarrel);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_FermentingOniWineBarrel building_FermentingOniWineBarrel = t as Building_FermentingOniWineBarrel;
            if (building_FermentingOniWineBarrel == null || !building_FermentingOniWineBarrel.Fermented)
            {
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            if (!t.IsForbidden(pawn))
            {
                LocalTargetInfo target = t;
                if (pawn.CanReserve(target, 1, -1, null, forced))
                {
                    return true;
                }
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(JobDefOf.TakeAOniWineOutOfFermentingOniWineBarrel, t);
        }
    }
}
