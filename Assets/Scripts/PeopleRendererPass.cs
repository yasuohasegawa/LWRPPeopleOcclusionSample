using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

public class PeopleRendererPass : ScriptableRenderPass
{
    private const string Tag = nameof(PeopleRendererPass);

    private RenderTargetIdentifier currentTarget;

    private Material material;

    public bool Active { get; set; }

    public PeopleRendererPass()
    {
        renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public void SetRenderTarget(RenderTargetIdentifier target)
    {
        currentTarget = target;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!Active) { return; }

        if (material == null)
        {
            var shader = Shader.Find("Custom/PeopleOcclusion");
            if (!shader) { return; }
            material = new Material(shader);
        }

        var commandBuffer = CommandBufferPool.Get(Tag);
        var renderTextureId = Shader.PropertyToID("_SampleLWRPScriptableRenderer");
        var cameraData = renderingData.cameraData;
        var w = cameraData.camera.scaledPixelWidth;
        var h = cameraData.camera.scaledPixelHeight;
        int shaderPass = 0;
        
        if (PostProcessHelper.GetInstance() != null)
        {
            PostProcessHelper.GetInstance().UpdateShaderProperty(material);
        }

        commandBuffer.GetTemporaryRT(renderTextureId, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        commandBuffer.Blit(currentTarget, renderTextureId);
        commandBuffer.Blit(renderTextureId, currentTarget, material, shaderPass);

        context.ExecuteCommandBuffer(commandBuffer);
        CommandBufferPool.Release(commandBuffer);
    }
}
