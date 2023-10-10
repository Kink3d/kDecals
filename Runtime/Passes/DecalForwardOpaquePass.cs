using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cathei.LinqGen;

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
            if (renderingMode == RenderingMode.Deferred)
            {
                var items = DecalSystem.decals.Gen()
                    .Where(x => x.decalData? (!x.decalData.isTransparent && (!x.decalData.supportsDeferred || x.decalData.forceForward)) : false)
                    .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0);

                foreach (var item in items)
                {
                    decals.Add(item);
                }
            }
            else
            {
                var items = DecalSystem.decals.Gen()
                    .Where(x=> x.decalData? (!x.decalData.isTransparent) : false)
                    .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0);

                foreach (var item in items)
                {
                    decals.Add(item);
                }
            }
        }
    }
}
