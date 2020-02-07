using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace OniShedding
{
    public class ThoughtWorker_OniNudistWrap : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            List<Apparel> wornApparel = p.apparel.WornApparel;
            for (int i = 0; i < wornApparel.Count; i++)
            {
                if (p.def.defName == "AOni")
                {
                    if (p.apparel.WornApparel.Any(x => x.def.defName == "Apparel_OniWrapNudist"))
                    { 
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
