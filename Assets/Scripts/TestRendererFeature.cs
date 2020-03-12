using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LWRP;

public class TestRendererFeature : ScriptableRendererFeature
{
    private PeopleRendererPass peopleRendererPass = null;

    public override void Create()
    {
        peopleRendererPass = peopleRendererPass ?? new PeopleRendererPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        peopleRendererPass.SetRenderTarget(renderer.cameraColorTarget);
        peopleRendererPass.Active = true;
        renderer.EnqueuePass(peopleRendererPass);
    }
}
