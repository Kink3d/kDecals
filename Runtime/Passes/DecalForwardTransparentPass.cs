using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    sealed class DecalForwardTransparentPass : DecalPass
    {
        public override string passName => "Decal Forward Transparent";
        public override string passTag => "DecalForward";

        public DecalForwardTransparentPass() : base()
        {
            // Set data
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void FilterDecals(ref List<Decal> decals)
        {
            decals = DecalSystem.decals
                .Where(x => x.decalData? x.decalData.isTransparent : true)
                .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0)
                .ToList();
        }
    }
}
