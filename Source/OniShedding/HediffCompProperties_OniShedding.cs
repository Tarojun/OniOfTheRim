using Verse;

namespace OniShedding

{
    public class HediffCompProperties_OniShedding : HediffCompProperties
    {

        public float DaysForFeatherShedding = 1f;

        public HediffCompProperties_OniShedding()
        {
            this.compClass = typeof(HediffComp_OniShedding);
        }

        public ThingDef thingToSpawn;

        public int spawnCount = 1;

        public HediffDef PreventedByHediff;
    }
}
