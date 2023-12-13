using System;
using System.Collections.Generic;
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
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody;
    private ObjectType _objectType = ObjectType.None;

    public void Update()
    {
        if(!Spawner.s_hasUpdatedDeltaTime)
        {
            Spawner.s_sharedDeltaTime = Time.deltaTime;
            Spawner.s_hasUpdatedDeltaTime = true;
        }

        foreach (ElementEffect effect in _elementEffects)
        {
            effect?.Update(Spawner.s_sharedDeltaTime);
        }
    }

    private void Activate()
    {
        this.enabled = true;

        _physicsLayer.layer = 6; // Physics
        _triggerLayer.gameObject.layer = 7; // Trigger
        _spriteRenderer.enabled = true;
        _rigidBody.simulated = true;
    }

    private void Disable()
    {
        this.enabled = false;

        _elementEffects = new ElementEffect[8];

        _physicsLayer.layer = 8; // None
        _triggerLayer.gameObject.layer = 8; // None
        _spriteRenderer.enabled = false;
        _rigidBody.simulated = false;
    }

    public void Init(ObjectType type)
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        Activate();

        _objectType = type;
        ApplyObjectTypeEffect(type);
        _triggerLayer.ElementEffectTransferEvent += OnObjectTrigger;
    }

    private void ApplyObjectTypeEffect(ObjectType type)
    {
        ElementEffect effect = new ElementEffect();
        switch (type)
        {
            case ObjectType.Fire:
                {   
                    effect.AddAttribute(new BurnEffect(10f, this.GetComponent<SpriteRenderer>()));
                    effect.EffectRemovedEvent += OnEffectRemoved;
                    effect.RequiredEffect = true;
                    break;
                }

            case ObjectType.Water:
                {
                    effect.AddAttribute(new WaterEffect(this.GetComponent<SpriteRenderer>()));
                    effect.EffectRemovedEvent += OnEffectRemoved;
                    effect.RequiredEffect = true;
                    break;
                }

            default:
                break;
        }

        int nextIndex = -1;
        int mask = effect.GetEventMask();
        for(int i = 0; i < _elementEffects.Length; ++i)
        {
            if(_elementEffects[i] == null)
            {
                nextIndex = i;
            }
            else if ((mask & _elementEffects[i].GetEventMask()) == _elementEffects[i].GetEventMask())
            {
                return;
            }
        }

        if (nextIndex >= 0)
        {
            _elementEffects[nextIndex] = effect;
            SendEffectMask();
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
        {
            Disable();
        }
        else
        {
            int count = 0;
            for (int i = 0; i < _elementEffects.Length; ++i)
            {
                if (_elementEffects[i] == effect)
                {
                    _elementEffects[i] = null;
                }
                else if (_elementEffects[i] != null)
                {
                    ++count;
                }
            }

            if (count == 0)
            {
                Disable();
            }
        }
    }
}
