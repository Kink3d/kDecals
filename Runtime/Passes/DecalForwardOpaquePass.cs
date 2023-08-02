using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    sealed class DecalForwardOpaquePass : DecalPass
    {
        public override string passName => "Decal Forward Opaque";

        public DecalForwardOpaquePass()
        {
            // Set data
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void FilterDecals(ref List<Decal> decals)
        {
            decals = DecalSystem.decals
                .Where(x => x.decalData? !x.decalData.isTransparent : false)
                .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0)
                .ToList();
        }
    }
}
