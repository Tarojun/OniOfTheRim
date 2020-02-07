using RimWorld;
using System;
using Verse;

namespace OniFermenting
{
    [DefOf]
    public static class JobDefOf
    {
        static JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }

        public static JobDef FillFermentingOniWineBarrel;

        public static JobDef TakeAOniWineOutOfFermentingOniWineBarrel;
        
    }
}