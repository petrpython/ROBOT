using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CycleBlockHandler : MonoBehaviour
{
    public TMP_InputField loopCountInput;
    public Transform nestedContent;
    public TextMeshProUGUI titleText;
    public GameObject commandBlockPrefab; // ✅ Добавляем ссылку на префаб

    private int commandId = -1;
    private ProgrammingInterface programInterface;
    private RobotCommand cycleCommand;
    private bool isUpdatingDisplay = false;

    void Start()
    {
        programInterface = FindFirstObjectByType<ProgrammingInterface>();

        if (loopCountInput != null)
        {
            loopCountInput.onEndEdit.AddListener(OnLoopCountChanged);
        }

        if (nestedContent == null)
        {
            Debug.LogError("❌ nestedContent НЕ НАЗНАЧЕН в Inspector!");
        }
    }

    public void Initialize(int uniqueId, int loopCount, RobotCommand command)
    {
        if (command == null)
        {
            Debug.LogError("❌ command is NULL!");
            return;
        }

        commandId = uniqueId;
        cycleCommand = command;

        if (cycleCommand.nestedCommands == null)
        {
            cycleCommand.nestedCommands = new List<RobotCommand>();
        }

        if (loopCountInput != null)
            loopCountInput.text = loopCount.ToString();

        if (titleText != null)
            titleText.text = command.name;

        UpdateNestedCommandsDisplay();
    }

    public void AddCommandFromPalette(RobotCommand.CommandType type, string cmdName, Color color)
    {
        if (cycleCommand == null)
        {
            Debug.LogWarning("⚠️ cycleCommand is NULL! Пытаемся найти в программе...");
            if (programInterface != null && commandId >= 0)
            {
                RobotCommand foundCmd = programInterface.GetCommandById(commandId);
                if (foundCmd != null)
                {
                    cycleCommand = foundCmd;
                    Debug.Log("✅ cycleCommand найден и восстановлен! ID=" + commandId);
                }
                else
                {
                    Debug.LogError("❌ Не удалось найти цикл в программе! Данные будут потеряны.");
                    return;
                }
            }
            else
            {
                Debug.LogError("❌ programInterface или commandId невалидны!");
                return;
            }
        }

        if (cycleCommand.nestedCommands == null)
        {
            cycleCommand.nestedCommands = new List<RobotCommand>();
        }

        RobotCommand newCmd = new RobotCommand(type, 0f, cmdName);
        newCmd.uniqueId = System.Guid.NewGuid().GetHashCode();
        newCmd.buttonColor = color;

        cycleCommand.nestedCommands.Add(newCmd);
        Debug.Log("✅ Добавлено в цикл: " + cmdName + " (всего: " + cycleCommand.nestedCommands.Count + ")");

        UpdateNestedCommandsDisplay();
    }

    public void UpdateNestedCommandsDisplay()
    {
        if (isUpdatingDisplay)
        {
            Debug.LogWarning("⚠️ UpdateNestedCommandsDisplay уже выполняется! Пропускаем.");
            return;
        }

        isUpdatingDisplay = true;

        try
        {
            if (nestedContent == null)
            {
                Debug.LogError("❌ nestedContent is NULL!");
                return;
            }

            if (cycleCommand == null)
            {
                Debug.LogError("❌ cycleCommand is NULL!");
                return;
            }

            if (cycleCommand.nestedCommands == null)
            {
                cycleCommand.nestedCommands = new List<RobotCommand>();
            }

            int commandCount = cycleCommand.nestedCommands.Count;

            // ✅ Очистка
            for (int i = nestedContent.childCount - 1; i >= 0; i--)
            {
                Transform child = nestedContent.GetChild(i);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }

            // ✅ Создание
            for (int i = 0; i < commandCount; i++)
            {
                RobotCommand cmd = cycleCommand.nestedCommands[i];
                if (cmd == null) continue;

                CreateCommandBlock(cmd, i);
            }

            // ✅ ИСПРАВЛЕНО: Правильный размер nestedContent
            RectTransform nestedRect = nestedContent.GetComponent<RectTransform>();
            if (nestedRect != null)
            {
                // Ширина: -10 (отступы), Высота: команды × 25px + отступ
                nestedRect.sizeDelta = new Vector2(-10, Mathf.Max(commandCount * 25 + 10, 35));
                nestedRect.anchoredPosition = Vector2.zero;
                nestedRect.localRotation = Quaternion.identity;
                nestedRect.localScale = Vector3.one;
            }

            // ✅ УБРАЛИ LayoutRebuilder - он вызывает расширение!
        }
        finally
        {
            isUpdatingDisplay = false;
        }
    }

    void CreateCommandBlock(RobotCommand cmd, int index)
    {
        if (nestedContent == null) return;

        GameObject block = new GameObject("Cmd_" + cmd.name);
        block.transform.SetParent(nestedContent, false);

        RectTransform rt = block.AddComponent<RectTransform>();
        // ✅ ИСПРАВЛЕНО: Правильные якоря и позиционирование
        rt.anchorMin = new Vector2(0, 1);      // ✅ Левый верхний угол
        rt.anchorMax = new Vector2(1, 1);      // ✅ Правый верхний угол
        rt.pivot = new Vector2(0.5f, 1);       // ✅ Якорь сверху
        rt.sizeDelta = new Vector2(-10, 25);   // ✅ Отступы по 5px, высота 25px
        rt.anchoredPosition = new Vector2(0, -index * 25 - 5f); // ✅ Позиция сверху вниз
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        Image bg = block.AddComponent<Image>();
        bg.color = cmd.buttonColor;
        bg.type = Image.Type.Sliced;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(block.transform, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = cmd.name;
        text.fontSize = 10;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;

        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(3, 2);
        textRT.offsetMax = new Vector2(-3, -2);

        SimpleDragDrop sdd = block.AddComponent<SimpleDragDrop>();
        sdd.Initialize(cmd.uniqueId, index);
        sdd.SetParentCycle(this);

        RightClickHandler rightClick = block.AddComponent<RightClickHandler>();
        rightClick.Initialize(cmd.uniqueId, this);
    }

    public void RemoveCommand(int index)
    {
        if (cycleCommand == null || cycleCommand.nestedCommands == null) return;
        if (index < 0 || index >= cycleCommand.nestedCommands.Count) return;

        cycleCommand.nestedCommands.RemoveAt(index);
        UpdateNestedCommandsDisplay();
    }

    RobotCommand.CommandType GetCommandType(string name)
    {
        switch (name)
        {
            case "ВПЕРЕД":
            case "FORWARD":
                return RobotCommand.CommandType.MoveForward;
            case "НАЗАД":
            case "BACKWARD":
                return RobotCommand.CommandType.MoveBackward;
            case "ВЛЕВО":
            case "LEFT":
                return RobotCommand.CommandType.TurnLeft;
            case "ВПРАВО":
            case "RIGHT":
                return RobotCommand.CommandType.TurnRight;
            default:
                return RobotCommand.CommandType.MoveForward;
        }
    }

    public void OnLoopCountChanged(string newValue)
    {
        if (commandId < 0 || programInterface == null) return;
        int count;
        if (int.TryParse(newValue, out count))
        {
            count = Mathf.Clamp(count, 1, 100);
            programInterface.UpdateLoopCount(commandId, count);
            loopCountInput.text = count.ToString();
        }
    }

    public Transform GetNestedContent() => nestedContent;
    public RobotCommand GetCycleCommand() => cycleCommand;
    public int GetCommandId() => commandId;
}

public class RightClickHandler : MonoBehaviour
{
    private int commandId;
    private CycleBlockHandler parentCycle;

    public void Initialize(int id, CycleBlockHandler cycle)
    {
        commandId = id;
        parentCycle = cycle;
    }

    void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && parentCycle != null)
        {
            RobotCommand cmd = parentCycle.GetCycleCommand();
            if (cmd != null && cmd.nestedCommands != null)
            {
                for (int i = 0; i < cmd.nestedCommands.Count; i++)
                {
                    if (cmd.nestedCommands[i].uniqueId == commandId)
                    {
                        parentCycle.RemoveCommand(i);
                        break;
                    }
                }
            }
        }
    }
}