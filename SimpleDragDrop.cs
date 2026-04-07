using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SimpleDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Settings")]
    public ContentDropHandler targetDropHandler;
    public Transform programContent;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private int originalSiblingIndex;
    private GameObject dragObject;
    private Canvas canvas;
    private Color buttonColor;
    private int dropIndex = -1;
    private int programIndex = -1;
    private int commandId = -1;
    private CycleBlockHandler parentCycle;
    private bool isFromPalette = false;
    private bool isBeingProcessed = false;

    void Start()
    {
        if (programContent == null && targetDropHandler != null)
        {
            programContent = targetDropHandler.content;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Image img = GetComponent<Image>();
        if (img != null)
            buttonColor = img.color;

        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (targetDropHandler == null)
            targetDropHandler = FindFirstObjectByType<ContentDropHandler>();

        parentCycle = GetComponentInParent<CycleBlockHandler>();
    }

    public void SetProgramIndex(int index)
    {
        programIndex = index;
    }

    public void SetCommandId(int id)
    {
        commandId = id;
    }

    public void Initialize(int uniqueId, int index)
    {
        commandId = uniqueId;
        programIndex = index;
        parentCycle = GetComponentInParent<CycleBlockHandler>();
    }

    public void SetParentCycle(CycleBlockHandler cycle)
    {
        parentCycle = cycle;
    }

    public void UpdateProgramIndex()
    {
        if (programContent == null && targetDropHandler != null)
            programContent = targetDropHandler.content;

        if (programContent != null && transform.parent == programContent)
        {
            programIndex = transform.GetSiblingIndex();
        }
        else
        {
            programIndex = -1;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        ProgrammingInterface pi = FindFirstObjectByType<ProgrammingInterface>();
        if (pi == null)
        {
            Debug.LogError("ProgrammingInterface not found!");
            Destroy(gameObject);
            return;
        }

        List<RobotCommand> program = pi.GetCurrentProgram();

        int actualIndex = -1;
        for (int i = 0; i < program.Count; i++)
        {
            if (program[i].uniqueId == commandId)
            {
                actualIndex = i;
                break;
            }
        }

        if (actualIndex >= 0)
        {
            pi.RemoveCommand(actualIndex);
            Destroy(gameObject);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        if (originalParent != null)
            originalSiblingIndex = transform.GetSiblingIndex();

        if (programContent == null)
        {
            programContent = targetDropHandler?.content;
        }

        // ✅ ИСПРАВЛЕНО: определяем палитру по commandId, а не по родителю
        isFromPalette = (commandId == -1);

        UpdateProgramIndex();

        if (canvasGroup != null)
            canvasGroup.alpha = 0.5f;

        if (canvas == null) return;

        if (isFromPalette)
        {
            dragObject = CreateDragCopy();
        }
        else
        {
            dragObject = CreateDragGhost();
        }

        dropIndex = -1;
    }

    GameObject CreateDragCopy()
    {
        GameObject copy = new GameObject("DragCopy_" + gameObject.name);
        copy.transform.SetParent(canvas.transform);
        copy.transform.SetAsLastSibling();

        RectTransform copyRT = copy.AddComponent<RectTransform>();
        RectTransform sourceRT = GetComponent<RectTransform>();
        copyRT.sizeDelta = sourceRT.sizeDelta;
        copyRT.localPosition = Vector3.zero;
        copyRT.localRotation = Quaternion.identity;
        copyRT.localScale = Vector3.one;
        copyRT.anchorMin = Vector2.zero;
        copyRT.anchorMax = Vector2.zero;
        copyRT.pivot = sourceRT.pivot;

        Image sourceImage = GetComponent<Image>();
        if (sourceImage != null)
        {
            Image copyImage = copy.AddComponent<Image>();
            copyImage.color = buttonColor;
            copyImage.sprite = sourceImage.sprite;
            copyImage.type = sourceImage.type;
            copyImage.raycastTarget = false;
        }

        TextMeshProUGUI sourceText = GetComponentInChildren<TextMeshProUGUI>();
        if (sourceText != null)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(copy.transform);

            TextMeshProUGUI copyText = textObj.AddComponent<TextMeshProUGUI>();
            copyText.text = sourceText.text;
            copyText.fontSize = sourceText.fontSize;
            copyText.color = sourceText.color;
            copyText.alignment = sourceText.alignment;
            copyText.font = sourceText.font;

            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.localPosition = Vector3.zero;
            textRT.localRotation = Quaternion.identity;
            textRT.localScale = Vector3.one;
        }

        copy.transform.position = transform.position;
        return copy;
    }

    GameObject CreateDragGhost()
    {
        GameObject ghost = new GameObject("DragGhost");
        ghost.transform.SetParent(canvas.transform);
        ghost.transform.SetAsLastSibling();

        RectTransform ghostRT = ghost.AddComponent<RectTransform>();
        RectTransform sourceRT = GetComponent<RectTransform>();
        ghostRT.sizeDelta = sourceRT.sizeDelta;
        ghostRT.localPosition = Vector3.zero;
        ghostRT.localRotation = Quaternion.identity;
        ghostRT.localScale = Vector3.one;
        ghostRT.anchorMin = Vector2.zero;
        ghostRT.anchorMax = Vector2.zero;
        ghostRT.pivot = sourceRT.pivot;

        Image sourceImage = GetComponent<Image>();
        if (sourceImage != null)
        {
            Image ghostImage = ghost.AddComponent<Image>();
            ghostImage.color = buttonColor;
            ghostImage.sprite = sourceImage.sprite;
            ghostImage.type = sourceImage.type;
            ghostImage.raycastTarget = false;
        }

        TextMeshProUGUI sourceText = GetComponentInChildren<TextMeshProUGUI>();
        if (sourceText != null)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(ghost.transform);

            TextMeshProUGUI ghostText = textObj.AddComponent<TextMeshProUGUI>();
            ghostText.text = sourceText.text;
            ghostText.fontSize = sourceText.fontSize;
            ghostText.color = sourceText.color;
            ghostText.alignment = sourceText.alignment;
            ghostText.font = sourceText.font;

            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.localPosition = Vector3.zero;
            textRT.localRotation = Quaternion.identity;
            textRT.localScale = Vector3.one;
        }

        ghost.transform.position = transform.position;
        return ghost;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragObject != null)
            dragObject.transform.position = eventData.position;

        if (IsOverCycle(eventData.position))
        {
            dropIndex = -1;
            if (targetDropHandler != null)
                targetDropHandler.ForceClearAllPlaceholders();
        }
        else
        {
            if (targetDropHandler != null)
            {
                targetDropHandler.UpdatePlaceholder(eventData.position);
                dropIndex = targetDropHandler.GetInsertIndex();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ✅ ДЛЯ КНОПОК ПАЛИТРЫ (commandId < 0)
        if (commandId < 0)
        {
            if (dragObject != null)
            {
                Destroy(dragObject);
                dragObject = null;
            }

            bool addedToCycle = false;

            CycleBlockHandler targetCycle = FindCycleAtPosition(eventData.position);

            if (targetCycle != null)
            {
                TextMeshProUGUI txt = GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    string cmdName = txt.text;
                    RobotCommand.CommandType type = GetCommandType(cmdName);
                    Color color = GetComponent<Image>().color;

                    if (type == RobotCommand.CommandType.LoopStart)
                    {
                        Debug.LogWarning("⚠️ Нельзя добавить цикл внутрь другого цикла!");
                        if (canvasGroup != null) canvasGroup.alpha = 1f;
                        return;
                    }

                    targetCycle.AddCommandFromPalette(type, cmdName, color);
                    addedToCycle = true;
                }
            }

            if (!addedToCycle && dropIndex >= 0)
            {
                TextMeshProUGUI txt = GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    string cmdName = txt.text;
                    RobotCommand.CommandType type = GetCommandType(cmdName);
                    Color color = GetComponent<Image>().color;

                    ProgrammingInterface pi = FindFirstObjectByType<ProgrammingInterface>();
                    if (pi != null)
                    {
                        int programCount = pi.GetCurrentProgramCount();
                        int insertIndex = dropIndex >= 0 ? dropIndex : programCount;

                        if (type == RobotCommand.CommandType.LoopStart)
                        {
                            pi.AddCycleCommand();
                        }
                        else
                        {
                            pi.SetInsertIndex(insertIndex);
                            pi.AddCommand(type, cmdName, color);
                        }
                    }
                }
            }

            // ✅ Для палитры просто возвращаем прозрачность, а не позицию!
            if (canvasGroup != null) canvasGroup.alpha = 1f;
            return;
        }

        // ✅ ДЛЯ КОМАНД ИЗ ПРОГРАММЫ (commandId >= 0)
        if (targetDropHandler != null)
            targetDropHandler.ForceClearAllPlaceholders();

        if (dragObject != null)
        {
            Destroy(dragObject);
            dragObject = null;
        }

        bool wasHandled = false;

        if (TryDropIntoCycleFromProgram(eventData))
        {
            wasHandled = true;
        }
        else if (TryDropIntoAnotherCycle(eventData))
        {
            wasHandled = true;
        }
        else if (parentCycle != null && !IsOverSameCycle(eventData.position))
        {
            RemoveFromNestedCommands();
            Destroy(gameObject);
            wasHandled = true;
        }
        else if (dropIndex >= 0 && programIndex >= 0 && programIndex != dropIndex)
        {
            ProgrammingInterface pi = FindFirstObjectByType<ProgrammingInterface>();
            if (pi != null)
            {
                pi.MoveCommandWithinProgram(programIndex, dropIndex);
                Destroy(gameObject); // ✅ УДАЛЯЕМ старый объект!
                wasHandled = true;
            }
        }

        if (!wasHandled)
        {
            ReturnToOriginalPosition();
        }

        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    CycleBlockHandler FindCycleAtPosition(Vector2 screenPoint)
    {
        CycleBlockHandler[] cycles = FindObjectsByType<CycleBlockHandler>(FindObjectsSortMode.None);
        foreach (var cycle in cycles)
        {
            RectTransform cycleRect = cycle.GetComponent<RectTransform>();
            if (cycleRect != null && RectTransformUtility.RectangleContainsScreenPoint(cycleRect, screenPoint, null))
            {
                return cycle;
            }
        }

        SimpleDragDrop[] allDrops = FindObjectsByType<SimpleDragDrop>(FindObjectsSortMode.None);
        foreach (var drop in allDrops)
        {
            if (drop.commandId < 0) continue;

            CycleBlockHandler parentCycle = drop.parentCycle;
            if (parentCycle == null) continue;

            RectTransform cmdRect = drop.GetComponent<RectTransform>();
            if (cmdRect != null && RectTransformUtility.RectangleContainsScreenPoint(cmdRect, screenPoint, null))
            {
                return parentCycle;
            }
        }

        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("ЦИКЛ") || obj.name.Contains("Cycle") || obj.name.Contains("CycleBlock"))
            {
                CycleBlockHandler cycle = obj.GetComponent<CycleBlockHandler>();
                if (cycle != null)
                {
                    RectTransform cycleRect = cycle.GetComponent<RectTransform>();
                    if (cycleRect != null && RectTransformUtility.RectangleContainsScreenPoint(cycleRect, screenPoint, null))
                    {
                        return cycle;
                    }
                }
            }
        }

        return null;
    }

    bool TryDropIntoCycleFromProgram(PointerEventData eventData)
    {
        if (commandId < 0) return false;
        if (isBeingProcessed) return false;

        isBeingProcessed = true;

        Vector2 screenPoint = eventData.position;
        CycleBlockHandler[] cycles = FindObjectsByType<CycleBlockHandler>(FindObjectsSortMode.None);

        foreach (var cycle in cycles)
        {
            RectTransform cycleRect = cycle.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(cycleRect, screenPoint, null))
            {
                TextMeshProUGUI txt = GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    string cmdName = txt.text;
                    RobotCommand.CommandType type = GetCommandType(cmdName);

                    if (type == RobotCommand.CommandType.LoopStart)
                    {
                        Debug.LogWarning("⚠️ Нельзя добавить цикл внутрь другого цикла!");
                        isBeingProcessed = false;
                        return false;
                    }
                }

                ProgrammingInterface pi = FindFirstObjectByType<ProgrammingInterface>();
                if (pi == null)
                {
                    isBeingProcessed = false;
                    return false;
                }

                List<RobotCommand> program = pi.GetCurrentProgram();

                int programIndexToRemove = -1;
                RobotCommand commandToMove = null;

                for (int i = 0; i < program.Count; i++)
                {
                    if (program[i].uniqueId == commandId)
                    {
                        programIndexToRemove = i;
                        commandToMove = program[i];
                        break;
                    }
                }

                if (commandToMove == null)
                {
                    isBeingProcessed = false;
                    return false;
                }

                RobotCommand cycleCmd = cycle.GetCycleCommand();
                if (cycleCmd == null)
                {
                    Debug.LogError("❌ cycleCmd is NULL!");
                    isBeingProcessed = false;
                    return false;
                }

                if (cycleCmd.nestedCommands == null)
                {
                    cycleCmd.nestedCommands = new List<RobotCommand>();
                }

                foreach (var nested in cycleCmd.nestedCommands)
                {
                    if (nested.uniqueId == commandToMove.uniqueId)
                    {
                        isBeingProcessed = false;
                        return false;
                    }
                }

                RobotCommand newCommand = new RobotCommand(commandToMove.type, 0f, commandToMove.name);
                newCommand.uniqueId = System.Guid.NewGuid().GetHashCode();
                newCommand.buttonColor = commandToMove.buttonColor;

                cycleCmd.nestedCommands.Add(newCommand);
                Debug.Log("✅ Добавлено в цикл: " + commandToMove.name + " (всего: " + cycleCmd.nestedCommands.Count + ")");

                if (programIndexToRemove >= 0)
                {
                    program.RemoveAt(programIndexToRemove);
                }

                cycle.UpdateNestedCommandsDisplay();
                Destroy(gameObject);

                isBeingProcessed = false;
                return true;
            }
        }

        isBeingProcessed = false;
        return false;
    }

    bool IsOverCycle(Vector2 screenPoint)
    {
        CycleBlockHandler[] cycles = FindObjectsByType<CycleBlockHandler>(FindObjectsSortMode.None);

        foreach (var cycle in cycles)
        {
            RectTransform cycleRect = cycle.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(cycleRect, screenPoint, null))
            {
                return true;
            }
        }

        return false;
    }

    bool IsOverSameCycle(Vector2 screenPoint)
    {
        if (parentCycle == null) return false;

        RectTransform cycleRect = parentCycle.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(cycleRect, screenPoint, null);
    }

    bool TryDropIntoAnotherCycle(PointerEventData eventData)
    {
        Vector2 screenPoint = eventData.position;

        CycleBlockHandler[] cycles = FindObjectsByType<CycleBlockHandler>(FindObjectsSortMode.None);

        foreach (var cycle in cycles)
        {
            if (cycle == parentCycle) continue;

            RectTransform cycleRect = cycle.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(cycleRect, screenPoint, null))
            {
                MoveToAnotherCycle(cycle);
                return true;
            }
        }

        return false;
    }

    void MoveToAnotherCycle(CycleBlockHandler targetCycle)
    {
        TextMeshProUGUI txt = GetComponentInChildren<TextMeshProUGUI>();
        if (txt == null) return;

        string cmdName = txt.text;
        RobotCommand.CommandType type = GetCommandType(cmdName);

        if (type == RobotCommand.CommandType.LoopStart)
        {
            Debug.LogWarning("⚠️ Нельзя добавить цикл внутрь другого цикла!");
            ReturnToOriginalPosition();
            return;
        }

        RobotCommand targetCycleCmd = targetCycle.GetCycleCommand();
        if (targetCycleCmd == null) return;

        if (targetCycleCmd.nestedCommands == null)
            targetCycleCmd.nestedCommands = new List<RobotCommand>();

        RemoveFromNestedCommands();

        RobotCommand newCommand = new RobotCommand(type, 0f, cmdName);
        newCommand.buttonColor = GetComponent<Image>().color;

        targetCycleCmd.nestedCommands.Add(newCommand);

        targetCycle.UpdateNestedCommandsDisplay();

        Destroy(gameObject);
    }

    void RemoveFromNestedCommands()
    {
        if (parentCycle == null) return;

        RobotCommand cycleCmd = parentCycle.GetCycleCommand();
        if (cycleCmd == null || cycleCmd.nestedCommands == null) return;

        for (int i = 0; i < cycleCmd.nestedCommands.Count; i++)
        {
            if (cycleCmd.nestedCommands[i].uniqueId == commandId)
            {
                cycleCmd.nestedCommands.RemoveAt(i);
                break;
            }
        }

        parentCycle.UpdateNestedCommandsDisplay();
    }

    void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(null);
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalSiblingIndex);
        }
    }

    void CreateCommandInProgram()
    {
        if (programIndex >= 0) return;

        ProgrammingInterface pi = FindFirstObjectByType<ProgrammingInterface>();
        if (pi == null) return;

        TextMeshProUGUI txt = GetComponentInChildren<TextMeshProUGUI>();
        if (txt == null) return;

        string cmdName = txt.text;
        RobotCommand.CommandType type = GetCommandType(cmdName);

        int programCount = pi.GetCurrentProgramCount();
        if (dropIndex > programCount) dropIndex = programCount;

        if (type == RobotCommand.CommandType.LoopStart)
        {
            pi.AddCycleCommand();
            return;
        }

        Color cmdColor = GetCommandColor(type);
        pi.SetInsertIndex(dropIndex);
        pi.AddCommand(type, cmdName, cmdColor);
    }

    Color GetCommandColor(RobotCommand.CommandType type)
    {
        switch (type)
        {
            case RobotCommand.CommandType.MoveForward:
                return new Color(0.2f, 0.8f, 0.2f, 1f);
            case RobotCommand.CommandType.MoveBackward:
                return new Color(0.9f, 0.2f, 0.2f, 1f);
            case RobotCommand.CommandType.TurnLeft:
            case RobotCommand.CommandType.TurnRight:
                return new Color(1f, 0.6f, 0.2f, 1f);
            case RobotCommand.CommandType.LoopStart:
                return new Color(0.2f, 0.6f, 0.2f, 1f);
            case RobotCommand.CommandType.IfStart:
                return new Color(0.2f, 0.6f, 0.8f, 1f);
            default:
                return Color.white;
        }
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
            case "ЦИКЛ":
            case "CYCLE":
            case "LOOP":
                return RobotCommand.CommandType.LoopStart;
            case "ЕСЛИ":
            case "IF":
                return RobotCommand.CommandType.IfStart;
            default:
                return RobotCommand.CommandType.MoveForward;
        }
    }

    public bool IsFromPalette()
    {
        return isFromPalette;
    }

    public int GetCommandId()
    {
        return commandId;
    }
}