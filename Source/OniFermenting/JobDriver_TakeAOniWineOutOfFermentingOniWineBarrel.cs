﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace OniFermenting

{
    public class JobDriver_TakeAOniWineOutOfFermentingOniWineBarrel : JobDriver
    {
        protected Building_FermentingOniWineBarrel OniWineBarrel
        {
            get
            {
                return (Building_FermentingOniWineBarrel)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        protected Thing AOniWine
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.OniWineBarrel, this.job, 1, -1, null, errorOnFailed);
		}

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).FailOn(() => !this.OniWineBarrel.Fermented).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = this.OniWineBarrel.TakeOutAOniWine();
                    GenPlace.TryPlaceThing(thing, this.pawn.Position, base.Map, ThingPlaceMode.Near, null, null, default);
                    StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
                    if (StoreUtility.TryFindBestBetterStoreCellFor(thing, this.pawn, base.Map, currentPriority, this.pawn.Faction, out IntVec3 c, true))
                    {
                        this.job.SetTarget(TargetIndex.C, c);
                        this.job.SetTarget(TargetIndex.B, thing);
                        this.job.count = thing.stackCount;
                        return;
                    }
                    base.EndJobWith(JobCondition.Incompletable);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return Toils_Reserve.Reserve(TargetIndex.C, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.C, PathEndMode.ClosestTouch);
            yield return carryToCell;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, carryToCell, true, false);
            yield break;
        }

        private const TargetIndex OniWineBarrelInd = TargetIndex.A;

        private const TargetIndex AOniWineToHaulInd = TargetIndex.B;

        private const TargetIndex StorageCellInd = TargetIndex.C;

        private const int Duration = 200;
    }
}
