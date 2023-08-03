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
        public override string passTag => "DecalForward";

        public RenderingMode renderingMode { get; set; }

        public DecalForwardOpaquePass() : base()
        {
            // Set data
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void FilterDecals(ref List<Decal> decals)
        {
            bool Filter(DecalData decalData)
            {
                return renderingMode == RenderingMode.Deferred ?
                    (!decalData.isTransparent && (!decalData.supportsDeferred || decalData.forceForward)) :
                    (!decalData.isTransparent);
            }

            decals = DecalSystem.decals
                .Where(x => x.decalData? Filter(x.decalData) : false)
                .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0)
                .ToList();
        }
    }
}
