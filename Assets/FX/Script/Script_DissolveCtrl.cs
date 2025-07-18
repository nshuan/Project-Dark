using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Script_DissolveCtrl : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Material dissolveMaterial; // Hoặc lấy từ SpriteRenderer
    public string variable;
    public AnimationCurve anm;
    public float Mul=1;
    public float Speed=1;
    private float startTime;

    [Range(0, 1)]
    public float initialDissolveValue = 0f;

    void ResetEffects()
    {
        // Reset Particle System
        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Play();
        }

        // Reset Dissolve Value in Shader
        if (dissolveMaterial != null)
        {
            dissolveMaterial.SetFloat("_Disolve_Value", initialDissolveValue);
        }
        startTime = Time.time;
    }

    void Update()
{
    if (dissolveMaterial != null)
    {
        float elapsed = Time.time - startTime;
        float dissolveValue = anm.Evaluate(elapsed * Speed) * Mul;
        dissolveMaterial.SetFloat(variable, dissolveValue);
    }
}
    
}
