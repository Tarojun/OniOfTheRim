using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace OniFermenting
{
	public class WorkGiver_FillFermentingOniWineBarrel : WorkGiver_Scanner
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

		public static void ResetStaticData()
		{
            WorkGiver_FillFermentingOniWineBarrel.TemperatureTrans = "BadTemperature".Translate().ToLower();
            WorkGiver_FillFermentingOniWineBarrel.NoDemonBreathWortTrans = "NoDemonBreathWort".Translate();
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
            Building_FermentingOniWineBarrel building_FermentingOniWineBarrel = t as Building_FermentingOniWineBarrel;
			if (building_FermentingOniWineBarrel == null || building_FermentingOniWineBarrel.Fermented || building_FermentingOniWineBarrel.SpaceLeftForDemonBreathWort <= 0)
			{
				return false;
			}
			float ambientTemperature = building_FermentingOniWineBarrel.AmbientTemperature;
			CompProperties_TemperatureRuinable compProperties = building_FermentingOniWineBarrel.def.GetCompProperties<CompProperties_TemperatureRuinable>();
			if (ambientTemperature < compProperties.minSafeTemperature + 2f || ambientTemperature > compProperties.maxSafeTemperature - 2f)
			{
				JobFailReason.Is(WorkGiver_FillFermentingOniWineBarrel.TemperatureTrans, null);
				return false;
			}
			if (!t.IsForbidden(pawn))
			{
				LocalTargetInfo target = t;
				if (pawn.CanReserve(target, 1, -1, null, forced))
				{
					if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
					{
						return false;
					}
					if (this.FindDemonBreathWort(pawn, building_FermentingOniWineBarrel) == null)
					{
						JobFailReason.Is(WorkGiver_FillFermentingOniWineBarrel.NoDemonBreathWortTrans, null);
						return false;
					}
					return !t.IsBurning();
				}
			}
			return false;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
            Building_FermentingOniWineBarrel oniwinebarrel = (Building_FermentingOniWineBarrel)t;
			Thing t2 = this.FindDemonBreathWort(pawn, oniwinebarrel);
			return new Job(JobDefOf.FillFermentingOniWineBarrel, t, t2);
		}

		private Thing FindDemonBreathWort(Pawn pawn, Building_FermentingOniWineBarrel oniwinebarrel)
		{
			Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false);
			IntVec3 position = pawn.Position;
			Map map = pawn.Map;
			ThingRequest thingReq = ThingRequest.ForDef(ThingDefOf.DemonBreathWort);
			PathEndMode peMode = PathEndMode.ClosestTouch;
			TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
			Predicate<Thing> validator = predicate;
			return GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
		}

		private static string TemperatureTrans;

		private static string NoDemonBreathWortTrans;
	}
}
