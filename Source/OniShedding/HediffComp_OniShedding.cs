using System;
using Verse;
using RimWorld;

namespace OniShedding
{
    public class HediffComp_OniShedding : HediffComp
    {

        private int SheddingTicker = 0;

        public HediffCompProperties_OniShedding Props
        {
            get
            {
                return (HediffCompProperties_OniShedding)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            ShedFeather();
        }

        public void ShedFeather()
        {
            if (SheddingTicker < (this.Props.DaysForFeatherShedding * 60000))
            {
                SheddingTicker += 1;
            }
            else
            {
                if ((this.parent.pawn.Map != null) &&  ((this.parent.pawn.Downed) == false) && ((this.parent.pawn.Faction == Faction.OfPlayer) || ((this.parent.pawn.IsPrisoner) && (this.parent.pawn.Map.IsPlayerHome))))
                {
                    Hediff hediff = this.parent.pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == this.Props.PreventedByHediff);
                    if (hediff == null)
                    {
                        Thing thing = ThingMaker.MakeThing(this.Props.thingToSpawn, null);
                        thing.stackCount = this.Props.spawnCount;
                        Thing t;
                        GenPlace.TryPlaceThing(thing, this.parent.pawn.Position, this.parent.pawn.Map, ThingPlaceMode.Near, out t, null, null);
                    }
                    else SheddingTicker = 0;
                }
                SheddingTicker = 0;

            }

        }


    }
}
