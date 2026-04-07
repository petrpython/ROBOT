using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NestedContentDropHandler : MonoBehaviour, IDropHandler
{
    private CycleBlockHandler parentCycle;

    void Start()
    {
        parentCycle = GetComponentInParent<CycleBlockHandler>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("✅ Drop into nested content!");

        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        ProgrammingInterface pi = FindFirstObjectByType<ProgrammingInterface>();
        if (pi == null) return;

        TextMeshProUGUI txt = droppedObject.GetComponentInChildren<TextMeshProUGUI>();
        if (txt == null) return;

        string cmdName = txt.text;
        RobotCommand.CommandType type = GetCommandType(cmdName);

        if (parentCycle != null && parentCycle.GetCycleCommand() != null)
        {
            RobotCommand cycleCmd = parentCycle.GetCycleCommand();

            if (cycleCmd.nestedCommands == null)
                cycleCmd.nestedCommands = new List<RobotCommand>();

            RobotCommand newCommand = new RobotCommand(type, 0f, cmdName);

            UnityEngine.UI.Image img = droppedObject.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                newCommand.buttonColor = img.color;
            else
                newCommand.buttonColor = UnityEngine.Color.white;

            cycleCmd.nestedCommands.Add(newCommand);

            Debug.Log("✅ Added to cycle! Total nested: " + cycleCmd.nestedCommands.Count);

            parentCycle.UpdateNestedCommandsDisplay();
        }
        else
        {
            Debug.LogError("❌ parentCycle or cycleCommand is null!");
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
            default:
                return RobotCommand.CommandType.MoveForward;
        }
    }
}