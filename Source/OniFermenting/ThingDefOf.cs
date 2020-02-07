using RimWorld;
using System;
using Verse;

namespace OniFermenting
{
    [DefOf]
    public static class ThingDefOf
    {
        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }

        public static ThingDef DemonBreathWort;

        public static ThingDef AOniWine;

        public static ThingDef OniFermentingWineBarrel;
        
    }
}