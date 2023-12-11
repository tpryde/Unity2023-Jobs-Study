using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EffectAttribute
{
    public const int FIRE_MASK = 1 << 1;
    public const int WATER_MASK = 1 << 2;

    void Tick(float delta);
    void Reset();
    bool AttributeExpired();
    int GetEffectMask();
    int GetCancelationMask();
}

public struct BurnEffect : EffectAttribute
{
    private float _lifeTime;
    private float _remainingLifeTime;

    private SpriteRenderer _spriteRenderer;

    public BurnEffect(float lifeTime, SpriteRenderer spriteRenderer)
    {
        _lifeTime = Mathf.Clamp(lifeTime, 1f, float.MaxValue);
        _remainingLifeTime = lifeTime;
        _spriteRenderer = spriteRenderer;
    }

    public void Tick(float delta)
    {
        _remainingLifeTime -= delta;

        float pct = _lifeTime / _remainingLifeTime;
        Color col = Color.red;
        col.a = pct;
        _spriteRenderer.color = col;
    }

    public void Reset()
    {
        _remainingLifeTime = _lifeTime;
    }

    public bool AttributeExpired()
    {
        return _remainingLifeTime <= 0f;
    }

    public int GetEffectMask()
    {
        return EffectAttribute.FIRE_MASK;
    }

    public int GetCancelationMask() 
    {
        return EffectAttribute.WATER_MASK;
    }
}

public struct WaterEffect : EffectAttribute
{

    public WaterEffect(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = Color.blue;
    }

    public void Tick(float delta) { }

    public void Reset() { }

    public bool AttributeExpired() { return false; }

    public int GetEffectMask()
    {
        return EffectAttribute.WATER_MASK;
    }

    public int GetCancelationMask()
    {
        return EffectAttribute.FIRE_MASK;
    }
}