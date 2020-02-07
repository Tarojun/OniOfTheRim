using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace OniShedding
{
    public class ThoughtWorker_AncestralOniNudist : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            { 
                List<Apparel> wornApparel = p.apparel.WornApparel;
                for (int i = 0; i < wornApparel.Count; i++)
                {
                    if (p.def.defName == "Alien_AncestralAOni")
                    { 
                        if (p.apparel.WornApparel.Any(x => x.def.defName == "Apparel_OniWrapNudist"))
                        {
                            return false;
                        }
                        if (p.apparel.WornApparel.Any(x => x.def.defName == "Apparel_OniGrandWrapNudist"))
                        {
                            return false;
                        }
                        Apparel apparel = wornApparel[i];
                        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
                        {
                            if (apparel.def.apparel.bodyPartGroups[j] == BodyPartGroupDefOf.Torso)
                            {
                                return true;
                            }
                            if (apparel.def.apparel.bodyPartGroups[j] == BodyPartGroupDefOf.Legs)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}