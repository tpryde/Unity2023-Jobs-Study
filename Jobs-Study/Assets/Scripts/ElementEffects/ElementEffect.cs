using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ElementEffect : MonoBehaviour
{
    private Element _contextTarget;
    private List<EffectAttribute> _attributes = new List<EffectAttribute>();

    public Action<ElementEffect> EffectRemovedEvent;

    public bool RequiredEffect { get; set; }

    private void Start()
    {
        _contextTarget = this.transform.GetComponent<Element>();
        if(_contextTarget == null)
        {
            Destroy(this); // Remove the component
        } 
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < _attributes.Count; ++i)
        {
            EffectAttribute attribute = _attributes[i];
            if (attribute.AttributeExpired())
            {
                 _attributes.RemoveAt(i);
                --i;
            }
            else
            {
                attribute.Tick(deltaTime);
            }
        }
        TryResolveElementEffect();
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
        for(int i = 0; i < _attributes.Count; ++i)
        {
            EffectAttribute attribute = _attributes[i];
            int cancelationMask = attribute.GetCancelationMask();
            if ((mask & cancelationMask) == cancelationMask)
            {
                _attributes.RemoveAt(i);
                --i;
            }
        }
        TryResolveElementEffect();
    }

    private bool TryResolveElementEffect()
    {
        bool resolved = _attributes.Count == 0;
        if(resolved)
        {
            EffectRemovedEvent?.Invoke(this);
            Destroy(this); // Remove the component
        }
        return resolved;
    }
}
