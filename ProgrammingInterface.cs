using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ProgrammingInterface : MonoBehaviour
{
    [Header("UI References")]
    public Transform commandListContainer;
    public GameObject commandBlockPrefab;
    public GameObject cycleBlockPrefab;
    public Transform programContent;

    [Header("Buttons - Main")]
    public Button btnRun;
    public Button btnStop;
    public Button btnClear;
    public Button btnUndo;

    [Header("Buttons - Movement")]
    public Button btnForward;
    public Button btnBackward;
    public Button btnLeft;
    public Button btnRight;

    [Header("Buttons - Logic")]
    public Button btnLoop;
    public Button btnIf;
    public Button btnCycle;

    [Header("Other")]
    public Button btnBack;
    public Button btnHome;
    public TextMeshProUGUI txtLevel;

    public RobotProgramExecutor robotExecutor;

    private List<RobotCommand> currentProgram;
    private Transform currentContext;
    private int insertIndex = -1;

    private ColorBlock btnRunColors;
    private ColorBlock btnStopColors;
    private ColorBlock btnClearColors;
    private ColorBlock btnUndoColors;

    void Start()
    {
        currentProgram = new List<RobotCommand>();

        SetupButtonColors();

        if (btnRun != null)
            btnRun.onClick.AddListener(OnRunClick);

        if (btnStop != null)
            btnStop.onClick.AddListener(OnStopClick);

        if (btnClear != null)
            btnClear.onClick.AddListener(OnClearClick);

        if (btnUndo != null)
            btnUndo.onClick.AddListener(OnUndoClick);

        FindContentTransform();
        UpdateProgramDisplay();
    }

    void SetupButtonColors()
    {
        SetupButton(btnRun, new Color(0.2f, 0.8f, 0.2f, 1f), ref btnRunColors);
        SetupButton(btnStop, new Color(0.9f, 0.2f, 0.2f, 1f), ref btnStopColors);
        SetupButton(btnClear, new Color(1f, 0.6f, 0.2f, 1f), ref btnClearColors);
        SetupButton(btnUndo, new Color(1f, 1f, 0f, 1f), ref btnUndoColors);
    }

    void SetupButton(Button btn, Color normalColor, ref ColorBlock storedColors)
    {
        if (btn != null)
        {
            ColorBlock colors = btn.colors;
            colors.normalColor = normalColor;
            colors.highlightedColor = normalColor;
            colors.pressedColor = normalColor;
            colors.selectedColor = Color.gray;
            colors.disabledColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0.5f);

            btn.colors = colors;
            storedColors = colors;
        }
    }

    IEnumerator FlashButton(Button btn, ColorBlock originalColors)
    {
        ColorBlock grayColors = originalColors;
        grayColors.normalColor = Color.gray;
        grayColors.highlightedColor = Color.gray;
        grayColors.pressedColor = Color.gray;
        btn.colors = grayColors;

        yield return new WaitForSeconds(1f);

        btn.colors = originalColors;
    }

    public void AddCycleCommand()
    {
        RobotCommand command = new RobotCommand(RobotCommand.CommandType.LoopStart, 0f, "ЦИКЛ", 5);
        command.buttonColor = new Color(0.2f, 0.6f, 0.2f, 1f);
        command.nestedCommands = new List<RobotCommand>();

        if (insertIndex < 0 || insertIndex > currentProgram.Count)
        {
            insertIndex = currentProgram.Count;
        }

        currentProgram.Insert(insertIndex, command);

        UpdateProgramDisplay();

        insertIndex = -1;
    }

    public void UpdateLoopCount(int commandId, int newCount)
    {
        foreach (var cmd in currentProgram)
        {
            if (cmd.uniqueId == commandId)
            {
                cmd.loopCount = newCount;
                return;
            }
        }
    }

    public RobotCommand GetCommandById(int commandId)
    {
        foreach (var cmd in currentProgram)
        {
            if (cmd.uniqueId == commandId)
            {
                return cmd;
            }
        }
        return null;
    }

    void FindContentTransform()
    {
        if (commandListContainer != null)
        {
            Transform viewport = commandListContainer.Find("Viewport");
            if (viewport != null)
            {
                currentContext = viewport.Find("Content");
            }

            if (currentContext == null)
            {
                currentContext = commandListContainer.Find("Content");
            }
        }

        if (programContent != null)
        {
            currentContext = programContent;
        }

        if (currentContext == null)
        {
            Debug.LogError("Content transform not found!");
        }
    }

    public void SetInsertIndex(int index)
    {
        if (index < 0 || index > currentProgram.Count)
        {
            insertIndex = currentProgram.Count;
        }
        else
        {
            insertIndex = index;
        }
    }

    public void AddCommand(RobotCommand.CommandType type, string commandName, Color color)
    {
        RobotCommand command = new RobotCommand(type, 0f, commandName);

        switch (type)
        {
            case RobotCommand.CommandType.MoveForward:
                command.buttonColor = new Color(0.2f, 0.8f, 0.2f, 1f);
                break;
            case RobotCommand.CommandType.MoveBackward:
                command.buttonColor = new Color(0.9f, 0.2f, 0.2f, 1f);
                break;
            case RobotCommand.CommandType.TurnLeft:
            case RobotCommand.CommandType.TurnRight:
                command.buttonColor = new Color(1f, 0.6f, 0.2f, 1f);
                break;
            case RobotCommand.CommandType.LoopStart:
                command.buttonColor = new Color(0.2f, 0.6f, 0.2f, 1f);
                break;
            case RobotCommand.CommandType.IfStart:
                command.buttonColor = new Color(0.2f, 0.6f, 0.8f, 1f);
                break;
            default:
                command.buttonColor = color;
                break;
        }

        if (insertIndex < 0 || insertIndex > currentProgram.Count)
        {
            insertIndex = currentProgram.Count;
        }

        currentProgram.Insert(insertIndex, command);

        UpdateProgramDisplay();

        insertIndex = -1;
    }

    public void MoveCommandWithinProgram(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= currentProgram.Count) return;
        if (newIndex < 0 || newIndex > currentProgram.Count) return;
        if (oldIndex == newIndex) return;

        RobotCommand command = currentProgram[oldIndex];
        currentProgram.RemoveAt(oldIndex);

        if (newIndex > oldIndex)
            newIndex--;

        currentProgram.Insert(newIndex, command);

        UpdateProgramDisplay();
    }

    public void RemoveCommand(int index)
    {
        if (index < 0 || index >= currentProgram.Count)
        {
            return;
        }

        currentProgram.RemoveAt(index);

        if (insertIndex >= index && insertIndex > 0)
            insertIndex--;

        if (insertIndex > currentProgram.Count)
            insertIndex = -1;
    }

    public void RefreshAfterDelete()
    {
        UpdateProgramDisplay();
    }

    public void UpdateProgramDisplay()
    {
        if (currentContext == null) return;

        ClearProgramDisplay();

        for (int i = 0; i < currentProgram.Count; i++)
        {
            CreateCommandBlock(currentProgram[i], i);
        }
    }

    void ClearProgramDisplay()
    {
        if (currentContext == null) return;

        for (int i = currentContext.childCount - 1; i >= 0; i--)
        {
            Transform child = currentContext.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    void CreateCommandBlock(RobotCommand command, int index)
    {
        if (currentContext == null) return;

        GameObject prefabToUse = (command.type == RobotCommand.CommandType.LoopStart)
            ? cycleBlockPrefab
            : commandBlockPrefab;

        if (prefabToUse == null)
        {
            Debug.LogError("Prefab is NULL for command: " + command.name);
            return;
        }

        GameObject block = Instantiate(prefabToUse, currentContext);

        SimpleDragDrop sdd = block.GetComponent<SimpleDragDrop>();
        if (sdd != null)
        {
            sdd.Initialize(command.uniqueId, index);
        }

        if (command.type == RobotCommand.CommandType.LoopStart)
        {
            CycleBlockHandler cbh = block.GetComponent<CycleBlockHandler>();
            if (cbh != null)
            {
                cbh.Initialize(command.uniqueId, command.loopCount, command);
            }
            else
            {
                Debug.LogError("CycleBlockHandler not found on prefab!");
            }
        }

        TextMeshProUGUI[] allTexts = block.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in allTexts)
        {
            if (text.GetComponentInParent<TMP_InputField>() != null)
                continue;

            text.text = command.name;
            break;
        }

        Image blockImage = block.GetComponent<Image>();
        if (blockImage != null)
            blockImage.color = command.buttonColor;

        block.name = "Command_" + index;
    }

    void OnRunClick()
    {
        if (robotExecutor != null)
        {
            robotExecutor.ExecuteProgram(currentProgram);
            StartCoroutine(FlashButton(btnRun, btnRunColors));
        }
    }

    void OnStopClick()
    {
        if (robotExecutor != null)
        {
            robotExecutor.StopProgram();
            StartCoroutine(FlashButton(btnStop, btnStopColors));
        }
    }

    void OnClearClick()
    {
        currentProgram.Clear();
        RobotCommand.ResetIdCounter();
        insertIndex = -1;
        UpdateProgramDisplay();
        StartCoroutine(FlashButton(btnClear, btnClearColors));
    }

    void OnUndoClick()
    {
        if (robotExecutor != null)
        {
            robotExecutor.ResetRobotPosition();
            StartCoroutine(FlashButton(btnUndo, btnUndoColors));
        }
    }

    public int GetCurrentProgramCount()
    {
        return currentProgram.Count;
    }

    public List<RobotCommand> GetCurrentProgram()
    {
        return currentProgram;
    }
}