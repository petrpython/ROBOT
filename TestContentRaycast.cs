using UnityEngine;
using UnityEngine.EventSystems;

public class TestContentRaycast : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    void Start()
    {
        Debug.Log("✅✅✅ Content Start: " + gameObject.name);
        Debug.Log("   Image: " + (GetComponent<UnityEngine.UI.Image>() != null ? "Есть" : "НЕТ"));
        Debug.Log("   BoxCollider2D: " + (GetComponent<BoxCollider2D>() != null ? "Есть" : "НЕТ"));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("✅✅✅ МЫШЬ НА CONTENT!");
        Debug.Log("   Позиция: " + eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("❌ Мышь ушла с Content");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("🎯🎯🎯 DROP НА CONTENT!");
    }
}