using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementTriggerLayer : MonoBehaviour
{
    public Action<Element> ElementEffectTransferEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherElement = other.transform.parent.GetComponent<Element>();
        if (otherElement != null)
        {
            ElementEffectTransferEvent?.Invoke(otherElement);
        }
    }
}
