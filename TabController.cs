using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [Header("Tabs")]
    public Button tabMovement;
    public Button tabLogic;

    [Header("Content Panels")]
    public GameObject movementContentPanel;
    public GameObject logicContentPanel;

    [Header("Colors")]
    public Color activeTabColor = new Color(0.3f, 0.6f, 0.3f, 1f);
    public Color inactiveTabColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    private Color originalMovementColor;
    private Color originalLogicColor;

    private void Start()
    {
        // Сохраняем оригинальные цвета
        if (tabMovement != null && tabMovement.GetComponent<Image>() != null)
            originalMovementColor = tabMovement.GetComponent<Image>().color;

        if (tabLogic != null && tabLogic.GetComponent<Image>() != null)
            originalLogicColor = tabLogic.GetComponent<Image>().color;

        // Добавляем слушатели событий
        if (tabMovement != null)
            tabMovement.onClick.AddListener(() => SwitchTab("movement"));

        if (tabLogic != null)
            tabLogic.onClick.AddListener(() => SwitchTab("logic"));

        // Показываем первую вкладку по умолчанию
        SwitchTab("movement");
    }

    public void SwitchTab(string tabName)
    {
        Debug.Log("📍 Переключение на вкладку: " + tabName);

        // Сбрасываем цвета вкладок
        if (tabMovement != null && tabMovement.GetComponent<Image>() != null)
            tabMovement.GetComponent<Image>().color = inactiveTabColor;

        if (tabLogic != null && tabLogic.GetComponent<Image>() != null)
            tabLogic.GetComponent<Image>().color = inactiveTabColor;

        // Скрываем все панели
        if (movementContentPanel != null)
            movementContentPanel.SetActive(false);

        if (logicContentPanel != null)
            logicContentPanel.SetActive(false);

        // Показываем нужную панель
        switch (tabName)
        {
            case "movement":
                if (tabMovement != null && tabMovement.GetComponent<Image>() != null)
                    tabMovement.GetComponent<Image>().color = activeTabColor;
                if (movementContentPanel != null)
                    movementContentPanel.SetActive(true);
                Debug.Log("✅ Показана панель: Движение");
                break;

            case "logic":
                if (tabLogic != null && tabLogic.GetComponent<Image>() != null)
                    tabLogic.GetComponent<Image>().color = activeTabColor;
                if (logicContentPanel != null)
                    logicContentPanel.SetActive(true);
                Debug.Log("✅ Показана панель: Логика");
                break;
        }
    }

    private void OnDestroy()
    {
        // Очищаем слушатели
        if (tabMovement != null)
            tabMovement.onClick.RemoveAllListeners();
        if (tabLogic != null)
            tabLogic.onClick.RemoveAllListeners();
    }
}