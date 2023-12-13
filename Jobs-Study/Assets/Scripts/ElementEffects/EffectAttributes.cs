using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EffectAttribute
{
    public const int FIRE_MASK = 1 << 1;
    public const int WATER_MASK = 1 << 2;

    bool Tick(float delta);
    void End();
    void Reset();
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

    public bool Tick(float delta)
    {
        bool isActive = _remainingLifeTime > 0f;
        if (isActive)
        {
            _remainingLifeTime -= delta;

            float pct = _remainingLifeTime / _lifeTime;
            Color col = Color.red;
            col.a = pct;
            _spriteRenderer.color = col;
        }
        return isActive;
    }

    public void End()
    {
        _remainingLifeTime = 0;
    }

    public void Reset()
    {
        _remainingLifeTime = _lifeTime;
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

    public bool Tick(float delta) { return true; }

    public void End() { }

    public void Reset() { }

    public int GetEffectMask()
    {
        return EffectAttribute.WATER_MASK;
    }

    public int GetCancelationMask()
    {
        return EffectAttribute.FIRE_MASK;
    }
}