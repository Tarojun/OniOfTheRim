using RimWorld;
using System;
using System.Collections.Generic;
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
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.OniFermentingWineBarrel);
            for (int i = 0; i < list.Count; i++)
            {
                if (((Building_FermentingOniWineBarrel)list[i]).Fermented)
                {
                    return false;
                }
            }
            return true;
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
            return building_FermentingOniWineBarrel != null && building_FermentingOniWineBarrel.Fermented && !t.IsBurning() && !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(JobDefOf.TakeAOniWineOutOfFermentingOniWineBarrel, t);
        }
    }
}
