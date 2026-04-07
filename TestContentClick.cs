using UnityEngine;
using UnityEngine.EventSystems;

public class TestContentClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Start()
    {
        Debug.Log("✅ Content Start: " + gameObject.name);

        var img = GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            Debug.Log("   Image: Raycast Target = " + img.raycastTarget);
            Debug.Log("   Image: Color = " + img.color);
        }
        else
        {
            Debug.LogWarning("   ⚠️ Image НЕ найден! Добавьте Image на Content!");
        }

        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Debug.Log("   BoxCollider2D: " + collider.size);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("✅✅✅ МЫШЬ НА CONTENT!");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("❌ Мышь ушла с Content");
    }
}