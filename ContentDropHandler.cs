using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentDropHandler : MonoBehaviour, IDropHandler
{
    [Header("Settings")]
    public Transform content;

    [Header("Visual")]
    public Color highlightColor = new Color(1f, 1f, 0f, 0.8f);

    private GameObject placeholder;
    private int insertIndex = 0;
    private Camera uiCamera;

    void Start()
    {
        if (content == null)
            content = transform;

        uiCamera = Camera.main;
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    public void UpdatePlaceholder(Vector2 screenPosition)
    {
        if (content == null)
        {
            insertIndex = 0;
            return;
        }

        RectTransform contentRect = content.GetComponent<RectTransform>();
        Vector3[] contentCorners = new Vector3[4];
        contentRect.GetWorldCorners(contentCorners);

        // Проверяем что курсор внутри content
        if (screenPosition.y > contentCorners[1].y || screenPosition.y < contentCorners[0].y)
        {
            // Если выше всего content - вставляем в начало
            if (screenPosition.y > contentCorners[1].y)
            {
                insertIndex = 0;
                ShowHighlight(0);
            }
            // Если ниже всего content - вставляем в конец
            else if (screenPosition.y < contentCorners[0].y)
            {
                insertIndex = content.childCount;
                ShowHighlight(insertIndex);
            }
            return;
        }

        if (content.childCount == 0)
        {
            insertIndex = 0;
            ShowHighlight(0);
            return;
        }

        // Находим ближайшую команду
        insertIndex = 0;
        bool found = false;

        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform child = content.GetChild(i).GetComponent<RectTransform>();
            if (child == null) continue;

            Vector3[] childCorners = new Vector3[4];
            child.GetWorldCorners(childCorners);

            float childTop = childCorners[1].y;
            float childBottom = childCorners[0].y;
            float childCenter = (childTop + childBottom) / 2f;

            // Если курсор выше середины команды - вставляем ПЕРЕД ней
            if (screenPosition.y > childCenter)
            {
                insertIndex = i;
                found = true;
                break;
            }
        }

        // Если не нашли - вставляем в конец
        if (!found)
        {
            insertIndex = content.childCount;
        }

        ShowHighlight(insertIndex);
    }

    void ShowHighlight(int index)
    {
        HideHighlight();

        if (index < 0 || index > content.childCount)
            return;

        placeholder = new GameObject("InsertPlaceholder");
        placeholder.transform.SetParent(content);
        placeholder.transform.SetSiblingIndex(index);

        RectTransform placeholderRT = placeholder.AddComponent<RectTransform>();
        placeholderRT.anchorMin = new Vector2(0, 0.5f);
        placeholderRT.anchorMax = new Vector2(1, 0.5f);
        placeholderRT.pivot = new Vector2(0.5f, 0.5f);
        placeholderRT.sizeDelta = new Vector2(-10, 5);
        placeholderRT.localPosition = Vector3.zero;
        placeholderRT.localRotation = Quaternion.identity;
        placeholderRT.localScale = Vector3.one;

        Image placeholderImg = placeholder.AddComponent<Image>();
        placeholderImg.color = highlightColor;
        placeholderImg.raycastTarget = false;
    }

    void HideHighlight()
    {
        if (placeholder != null)
        {
            Destroy(placeholder);
            placeholder = null;
        }
    }

    public int GetInsertIndex()
    {
        return insertIndex;
    }

    public void ForceClearAllPlaceholders()
    {
        HideHighlight();
    }

    public void ClearAllPlaceholders()
    {
        HideHighlight();
    }
}