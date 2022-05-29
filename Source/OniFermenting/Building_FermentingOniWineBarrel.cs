using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace OniFermenting
{
    [StaticConstructorOnStartup]
    public class Building_FermentingOniWineBarrel : Building
    {
		public float Progress
		{
			get
			{
				return this.progressInt;
			}
			set
			{
				if (value == this.progressInt)
				{
					return;
				}
				this.progressInt = value;
				this.barFilledCachedMat = null;
			}
		}

        private Material BarFilledMat
        {
            get
            {
                if (this.barFilledCachedMat == null)
                {
                    this.barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(Building_FermentingOniWineBarrel.BarZeroProgressColor, Building_FermentingOniWineBarrel.BarFermentedColor, this.Progress), false);
                }
                return this.barFilledCachedMat;
            }
        }

        public int SpaceLeftForDemonBreathWort
        {
            get
            {
                if (this.Fermented)
                {
                    return 0;
                }
                return 25 - this.demonbreathwortCount;
            }
        }

        private bool Empty
        {
            get
            {
                return this.demonbreathwortCount <= 0;
            }
        }

        public bool Fermented
        {
            get
            {
                return !this.Empty && this.Progress >= 1f;
            }
        }

        private float CurrentTempProgressSpeedFactor
		{
			get
			{
				CompProperties_TemperatureRuinable compProperties = this.def.GetCompProperties<CompProperties_TemperatureRuinable>();
				float ambientTemperature = base.AmbientTemperature;
				if (ambientTemperature < compProperties.minSafeTemperature)
				{
					return 0.1f;
				}
				if (ambientTemperature < 7f)
				{
					return GenMath.LerpDouble(compProperties.minSafeTemperature, 7f, 0.1f, 1f, ambientTemperature);
				}
				return 1f;
			}
		}

        private float ProgressPerTickAtCurrentTemp
        {
            get
            {
                return 2.77777781E-06f * this.CurrentTempProgressSpeedFactor;
            }
        }

        private int EstimatedTicksLeft
        {
            get
            {
                return Mathf.Max(Mathf.RoundToInt((1f - this.Progress) / this.ProgressPerTickAtCurrentTemp), 0);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.demonbreathwortCount, "demonbreathwortCount", 0, false);
            Scribe_Values.Look<float>(ref this.progressInt, "progress", 0f, false);
        }

        public override void TickRare()
        {
            base.TickRare();
            if (!this.Empty)
            {
                this.Progress = Mathf.Min(this.Progress + 250f * this.ProgressPerTickAtCurrentTemp, 1f);
            }
        }

        public void AddDemonBreathWort(int count)
        {
            base.GetComp<CompTemperatureRuinable>().Reset();
            if (this.Fermented)
            {
                Log.Warning("Tried to add Oni smelly jelly to a Oni wine barrel full of wine. Colonists should take the wine first.");
                return;
            }
            int num = Mathf.Min(count, 25 - this.demonbreathwortCount);
            if (num <= 0)
            {
                return;
            }
            this.Progress = GenMath.WeightedAverage(0f, (float)num, this.Progress, (float)this.demonbreathwortCount);
            this.demonbreathwortCount += num;
        }

        protected override void ReceiveCompSignal(string signal)
        {
            if (signal == "RuinedByTemperature")
            {
                this.Reset();
            }
        }

        private void Reset()
        {
            this.demonbreathwortCount = 0;
            this.Progress = 0f;
        }

        public void AddDemonBreathWort(Thing demonbreathwort)
        {
            int num = Mathf.Min(demonbreathwort.stackCount, 25 - this.demonbreathwortCount);
            if (num > 0)
            {
                this.AddDemonBreathWort(num);
                demonbreathwort.SplitOff(num).Destroy(DestroyMode.Vanish);
            }
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length != 0)
            {
                stringBuilder.AppendLine();
            }
            CompTemperatureRuinable comp = base.GetComp<CompTemperatureRuinable>();
            if (!this.Empty && !comp.Ruined)
            {
                if (this.Fermented)
                {
                    stringBuilder.AppendLine("ContainsAOniWine".Translate(this.demonbreathwortCount, 25));
                }
                else
                {
                    stringBuilder.AppendLine("ContainsDemonBreathWort".Translate(this.demonbreathwortCount, 25));
                }
            }
            if (!this.Empty)
            {
                if (this.Fermented)
                {
                    stringBuilder.AppendLine("Fermented".Translate());
                }
                else
                {
                    stringBuilder.AppendLine("FermentationProgress".Translate(this.Progress.ToStringPercent(), this.EstimatedTicksLeft.ToStringTicksToPeriod(true, false, true, true)));
                    if (this.CurrentTempProgressSpeedFactor != 1f)
                    {
                        stringBuilder.AppendLine("FermentationBarrelOutOfIdealTemperature".Translate(this.CurrentTempProgressSpeedFactor.ToStringPercent()));
                    }
                }
            }
            stringBuilder.AppendLine("Temperature".Translate() + ": " + base.AmbientTemperature.ToStringTemperature("F0"));
            stringBuilder.AppendLine("IdealFermentingTemperature".Translate() + ": " + 7f.ToStringTemperature("F0") + " ~ " + comp.Props.maxSafeTemperature.ToStringTemperature("F0"));
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public Thing TakeOutAOniWine()
        {
            if (!this.Fermented)
            {
                Log.Warning("Tried to get wine but it's not yet fermented.");
                return null;
            }
            Thing thing = ThingMaker.MakeThing(ThingDefOf.AOniWine, null);
            thing.stackCount = this.demonbreathwortCount;
            this.Reset();
            return thing;
        }
        public override void Draw()
        {
            base.Draw();
            if (!this.Empty)
            {
                Vector3 drawPos = this.DrawPos;
                drawPos.y += 0.046875f;
                drawPos.z += 0.25f;
                GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest
                {
                    center = drawPos,
                    size = Building_FermentingOniWineBarrel.BarSize,
                    fillPercent = (float)this.demonbreathwortCount / 25f,
                    filledMat = this.BarFilledMat,
                    unfilledMat = Building_FermentingOniWineBarrel.BarUnfilledMat,
                    margin = 0.1f,
                    rotation = Rot4.North
                });
            }
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (Prefs.DevMode && !this.Empty)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Debug: Set progress to 1",
                    action = delegate ()
                    {
                        this.Progress = 1f;
                    }
                };
            }
            yield break;
        }

        private int demonbreathwortCount;

        private float progressInt;

        private Material barFilledCachedMat;

        public const int MaxCapacity = 25;

        private const int BaseFermentationDuration = 360000;

        public const float MinIdealTemperature = 7f;

        private static readonly Vector2 BarSize = new Vector2(0.55f, 0.1f);

        private static readonly Color BarZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);

        private static readonly Color BarFermentedColor = new Color(0.9f, 0.85f, 0.2f);

        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
    }
}