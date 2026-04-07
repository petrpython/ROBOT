using UnityEngine;
using UnityEngine.EventSystems;

public class TestContentDebug : MonoBehaviour, IPointerEnterHandler, IDropHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("✅✅✅ МЫШЬ НА Content!");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("🎯 DROP на Content!");
    }
}