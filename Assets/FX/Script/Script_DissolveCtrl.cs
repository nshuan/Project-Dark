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
    private bool isPlay;

    [Range(0, 1)]
    public float initialDissolveValue = 0f;

    void ResetEffects()
    {
        // Reset Particle System
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particleSystem.Play();
        isPlay = true;

        // Reset Dissolve Value in Shader
        if (dissolveMaterial != null)
        {
            dissolveMaterial.SetFloat("_Disolve_Value", initialDissolveValue);
        }
    }

    void Update ()
    {
        if (dissolveMaterial!=null) dissolveMaterial.SetFloat(variable, anm.Evaluate(Time.time * Speed)*Mul);
    }
}
