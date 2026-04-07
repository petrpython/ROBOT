using UnityEngine;
using UnityEngine.UI;

public class ButtonCommand : MonoBehaviour
{
    public string commandName;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            Color color = GetComponent<Image>().color;
            btn.onClick.AddListener(() => OnClick(color));
        }
    }

    void OnClick(Color color)
    {
        var programmingInterface = FindFirstObjectByType<ProgrammingInterface>();

        if (programmingInterface != null)
        {
            var type = GetCommandType(commandName);
            Color cmdColor = GetCommandColor(type);
            programmingInterface.AddCommand(type, commandName, cmdColor);
        }
        else
        {
            Debug.LogError("ProgrammingInterface not found!");
        }
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
            case "FORWARD":
            case "ВПЕРЕД":
                return RobotCommand.CommandType.MoveForward;
            case "BACKWARD":
            case "НАЗАД":
                return RobotCommand.CommandType.MoveBackward;
            case "LEFT":
            case "ВЛЕВО":
                return RobotCommand.CommandType.TurnLeft;
            case "RIGHT":
            case "ВПРАВО":
                return RobotCommand.CommandType.TurnRight;
            case "CYCLE":
            case "ЦИКЛ":
            case "LOOP":
                return RobotCommand.CommandType.LoopStart;
            case "IF":
            case "ЕСЛИ":
                return RobotCommand.CommandType.IfStart;
            default:
                return RobotCommand.CommandType.MoveForward;
        }
    }
}