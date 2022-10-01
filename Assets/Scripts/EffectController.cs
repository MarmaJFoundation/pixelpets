using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public ParticleSystem particle;
    private EffectType effectType;
    public void Setup(EffectType effectType, Vector3 offset)
    {
        this.effectType = effectType;
        //gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        transform.localPosition += offset;
        particle.Play();
        BaseUtils.activeEffects.Add(this);
    }
    private void Update()
    {
        if (!particle.isPlaying)
        {
            Dispose(true);
        }
    }
    public void Dispose(bool fromDeath)
    {
        if (particle == null)
        {
            return;
        }
        //gameObject.SetActive(false);
        if (!fromDeath)
        {

            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            BaseUtils.activeEffects.Remove(this);
        }
        transform.SetParent(BaseUtils.restingCanvas);
        BaseUtils.effectPool[effectType].Enqueue(this);
    }
}
