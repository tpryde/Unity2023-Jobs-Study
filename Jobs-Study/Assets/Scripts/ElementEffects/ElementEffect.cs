using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public sealed class ElementEffect
{
    private EffectAttribute[] _attributes;

    public Action<ElementEffect> EffectRemovedEvent;

    public bool RequiredEffect { get; set; }

    public void Update(float deltaTime)
    {
        int count = 0;
        for (int i = 0; i < _attributes.Length; ++i)
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

    public void AddAttribute(params EffectAttribute[] attributes)
    {
        if (_attributes == null)
        {
            _attributes = attributes;
        }
        else
        {
            int originalLength = _attributes.Length;
            Array.Resize(ref _attributes, _attributes.Length + attributes.Length);
            attributes.CopyTo(_attributes, originalLength);
        }
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
        for(int i = 0; i < _attributes.Length; ++i)
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
