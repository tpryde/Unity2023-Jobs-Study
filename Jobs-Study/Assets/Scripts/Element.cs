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

    private ObjectType _objectType = ObjectType.None;

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
        switch(type)
        {
            case ObjectType.Fire:
                {
                    ElementEffect effect = this.gameObject.AddComponent<ElementEffect>();
                    effect.AddAttribute(new BurnEffect(10f, this.GetComponent<SpriteRenderer>()));

                    effect.RequiredEffect = true;
                    effect.EffectRemovedEvent += OnEffectRemoved;
                    SendEffectMask();
                    break;
                }

            case ObjectType.Water:
                {
                    ElementEffect effect = this.gameObject.AddComponent<ElementEffect>();
                    effect.AddAttribute(new WaterEffect(this.GetComponent<SpriteRenderer>()));

                    effect.RequiredEffect = true;
                    effect.EffectRemovedEvent += OnEffectRemoved;
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
        ElementEffect[] elementEffects = this.GetComponents<ElementEffect>();

        for(int i = 0; i < elementEffects.Length; ++i)
        {
            mask |= elementEffects[i].GetEventMask();
        }
        for (int i = 0; i < elementEffects.Length; ++i)
        {
            elementEffects[i].OnEventMaskSent(mask);
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
