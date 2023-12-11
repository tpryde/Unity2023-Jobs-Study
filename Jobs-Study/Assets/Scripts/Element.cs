using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public enum ObjectType
{
    None = 0,
    Fire,
    Water
};

public sealed class Element : MonoBehaviour
{
    [SerializeField] private GameObject _physicsLayer;
    [SerializeField] private ElementTriggerLayer _triggerLayer;

    private ElementEffect[] _elementEffects = new ElementEffect[8];
    private ObjectType _objectType = ObjectType.None;
    private int _nextIndex = 0;

    public void Update()
    {
        foreach (var effect in _elementEffects)
        {
            effect?.Update();
        }
    }

    public void Init(ObjectType type)
    {
        _objectType = type;
        ApplyObjectTypeEffect(type);

        if (type == ObjectType.None)
        {
            _triggerLayer.gameObject.SetActive(false);
        }
        else
        {
            _triggerLayer.ElementEffectTransferEvent += OnObjectTrigger;
        }
    }

    private void ApplyObjectTypeEffect(ObjectType type)
    {
        ElementEffect effect = new ElementEffect();
        _elementEffects[_nextIndex++] = effect;

        if(_nextIndex == _elementEffects.Length)
        {
            Array.Sort(_elementEffects, (x, y) =>
            {
                if (x != null && y != null)
                    return 0;
                else if (x != null)
                    return -1;
                
                return 0;
            });

            for(int i = 0; i < _elementEffects.Length; ++i)
            {
                if (_elementEffects[i] == null)
                {
                    _nextIndex = i;
                    break;
                }
            }
        }

        switch (type)
        {
            case ObjectType.Fire:
                {   
                    effect.AddAttribute(new BurnEffect(10f, this.GetComponent<SpriteRenderer>()));
                    effect.EffectRemovedEvent += OnEffectRemoved;
                    effect.RequiredEffect = true;
                    SendEffectMask();
                    break;
                }

            case ObjectType.Water:
                {
                    effect.AddAttribute(new WaterEffect(this.GetComponent<SpriteRenderer>()));
                    effect.EffectRemovedEvent += OnEffectRemoved;
                    effect.RequiredEffect = true;
                    SendEffectMask();
                    break;
                }

            default:
                break;
        }
    }

    private void SendEffectMask()
    {
        int mask = 0;
        foreach (var effect in _elementEffects)
        {
            if (effect == null)
                continue;

            mask |= effect.GetEventMask();
        }
        foreach (var effect in _elementEffects)
        {
            effect?.OnEventMaskSent(mask);
        }
    }

    private void OnObjectTrigger(Element other)
    {
        if (other._objectType != _objectType)
        {
            ApplyObjectTypeEffect(other._objectType);
        }
    }

    private void OnEffectRemoved(ElementEffect effect)
    {
        if (effect.RequiredEffect)
            Destroy(this.gameObject);
    }
}
