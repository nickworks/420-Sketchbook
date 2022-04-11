using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{

    public Shader shader;
    private Material mat;


    public Texture noiseTexture;

    void Start()
    {
        mat = new Material(shader);

        mat.SetTexture("_NoiseTex", noiseTexture);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, mat);
    }
}
