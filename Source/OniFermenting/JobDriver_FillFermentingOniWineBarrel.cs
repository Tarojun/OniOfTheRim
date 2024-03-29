﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace OniFermenting
{
    public class JobDriver_FillFermentingOniWineBarrel : JobDriver
    {
        protected Building_FermentingOniWineBarrel Barrel
        {
            get
            {
                return (Building_FermentingOniWineBarrel)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        protected Thing DemonBreathWort
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.Barrel, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.DemonBreathWort, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            base.AddEndCondition(delegate
            {
                if (this.Barrel.SpaceLeftForDemonBreathWort > 0)
                {
                    return JobCondition.Ongoing;
                }
                return JobCondition.Succeeded;
            });
            yield return Toils_General.DoAtomic(delegate
            {
                this.job.count = this.Barrel.SpaceLeftForDemonBreathWort;
            });
            Toil reserveWort = Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return reserveWort;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveWort, TargetIndex.B, TargetIndex.None, true, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    this.Barrel.AddDemonBreathWort(this.DemonBreathWort);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }

        private const TargetIndex BarrelInd = TargetIndex.A;

        private const TargetIndex DemonBreathWortInd = TargetIndex.B;

        private const int Duration = 200;
    }
}
