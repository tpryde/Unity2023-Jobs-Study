using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public sealed class ElementEffect
{
    private List<EffectAttribute> _attributes = new List<EffectAttribute>();

    public Action<ElementEffect> EffectRemovedEvent;

    public bool RequiredEffect { get; set; }

    public void Update(float deltaTime)
    {
        int count = 0;
        for (int i = 0; i < _attributes.Count; ++i)
        {
            EffectAttribute attribute = _attributes[i];
            if (!attribute.AttributeExpired())
            {
                attribute.Tick(deltaTime);
                ++count;
            }
        }
        if (count == 0)
        {
            EffectRemovedEvent?.Invoke(this);
        }
    }

    public void AddAttribute(EffectAttribute attribute)
    {
        _attributes.Add(attribute);
    }

    public int GetEventMask()
    {
        int mask = 0;
        foreach (EffectAttribute attribute in _attributes)
        {
            mask |= attribute.GetEffectMask();
        }
        return mask;
    }

    public void OnEventMaskSent(int mask)
    {
        int count = 0;
        for(int i = 0; i < _attributes.Count; ++i)
        {
            EffectAttribute attribute = _attributes[i];
            int cancelationMask = attribute.GetCancelationMask();
            if ((mask & cancelationMask) == cancelationMask)
            {
                attribute.End();
            }
            else
            {
                ++count;
            }
        }
        if (count == 0)
        {
            EffectRemovedEvent?.Invoke(this);
        }
    }
}
