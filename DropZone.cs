using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Settings")]
    public Transform contentTransform; // Content куда дропать

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("📥 Drop в зону: " + gameObject.name);

        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        // Перемещаем в Content
        if (contentTransform != null)
        {
            droppedObject.transform.SetParent(contentTransform);
            droppedObject.transform.SetAsLastSibling();

            // Сбрасываем локальную позицию
            RectTransform rt = droppedObject.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localPosition = Vector3.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                rt.anchorMin = Vector2.up;
                rt.anchorMax = Vector2.up;
                rt.pivot = Vector2.up;
            }

            Debug.Log("✅ Команда добавлена в программу!");
        }
    }
}