using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandPalette : MonoBehaviour
{
    [Header("References")]
    public ProgrammingInterface programmingInterface;

    [Header("Command Buttons")]
    public Button btnForward;
    public Button btnBackward;
    public Button btnLeft;
    public Button btnRight;
    public Button btnLoop;
    public Button btnIf;
    public Button btnFunction;
    public Button btnSensor;

    void Awake()
    {
        // Если кнопки не назначены — ищем по именам автоматически
        if (btnForward == null) btnForward = FindButton("Forward", "ВПЕРЕД");
        if (btnBackward == null) btnBackward = FindButton("Backward", "НАЗАД");
        if (btnLeft == null) btnLeft = FindButton("Left", "ВЛЕВО");
        if (btnRight == null) btnRight = FindButton("Right", "ВПРАВО");
        if (btnLoop == null) btnLoop = FindButton("Loop", "ЦИКЛ");
        if (btnIf == null) btnIf = FindButton("If", "ЕСЛИ");
        if (btnFunction == null) btnFunction = FindButton("Function", "ФУНКЦИЯ");
        if (btnSensor == null) btnSensor = FindButton("Sensor", "ДАТЧИК");
    }

    void Start()
    {
        Debug.Log("=== CommandPalette Start ===");

        if (programmingInterface == null)
        {
            programmingInterface = FindFirstObjectByType<ProgrammingInterface>();
            Debug.Log("ProgrammingInterface: " + (programmingInterface != null ? "НАЙДЕН" : "НЕ НАЙДЕН"));
        }

        if (btnForward != null)
        {
            Color color = btnForward.GetComponent<Image>().color;
            btnForward.onClick.RemoveAllListeners();
            btnForward.onClick.AddListener(() => OnCommandClick("ВПЕРЕД", color));
            Debug.Log("Кнопка ВПЕРЕД назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnForward = NULL!");

        if (btnBackward != null)
        {
            Color color = btnBackward.GetComponent<Image>().color;
            btnBackward.onClick.RemoveAllListeners();
            btnBackward.onClick.AddListener(() => OnCommandClick("НАЗАД", color));
            Debug.Log("Кнопка НАЗАД назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnBackward = NULL!");

        if (btnLeft != null)
        {
            Color color = btnLeft.GetComponent<Image>().color;
            btnLeft.onClick.RemoveAllListeners();
            btnLeft.onClick.AddListener(() => OnCommandClick("ВЛЕВО", color));
            Debug.Log("Кнопка ВЛЕВО назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnLeft = NULL!");

        if (btnRight != null)
        {
            Color color = btnRight.GetComponent<Image>().color;
            btnRight.onClick.RemoveAllListeners();
            btnRight.onClick.AddListener(() => OnCommandClick("ВПРАВО", color));
            Debug.Log("Кнопка ВПРАВО назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnRight = NULL!");

        if (btnLoop != null)
        {
            Color color = btnLoop.GetComponent<Image>().color;
            btnLoop.onClick.RemoveAllListeners();
            btnLoop.onClick.AddListener(() => OnCommandClick("ЦИКЛ", color));
            Debug.Log("Кнопка ЦИКЛ назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnLoop = NULL!");

        if (btnIf != null)
        {
            Color color = btnIf.GetComponent<Image>().color;
            btnIf.onClick.RemoveAllListeners();
            btnIf.onClick.AddListener(() => OnCommandClick("ЕСЛИ", color));
            Debug.Log("Кнопка ЕСЛИ назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnIf = NULL!");

        if (btnFunction != null)
        {
            Color color = btnFunction.GetComponent<Image>().color;
            btnFunction.onClick.RemoveAllListeners();
            btnFunction.onClick.AddListener(() => OnCommandClick("ФУНКЦИЯ", color));
            Debug.Log("Кнопка ФУНКЦИЯ назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnFunction = NULL!");

        if (btnSensor != null)
        {
            Color color = btnSensor.GetComponent<Image>().color;
            btnSensor.onClick.RemoveAllListeners();
            btnSensor.onClick.AddListener(() => OnCommandClick("ДАТЧИК", color));
            Debug.Log("Кнопка ДАТЧИК назначена, цвет: " + color);
        }
        else Debug.LogWarning("btnSensor = NULL!");

        Debug.Log("=== CommandPalette End ===");
    }

    Button FindButton(params string[] names)
    {
        foreach (var name in names)
        {
            var btn = transform.Find(name)?.GetComponent<Button>();
            if (btn != null) return btn;

            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if (child.name.ToUpper().Contains(name.ToUpper()))
                {
                    var b = child.GetComponent<Button>();
                    if (b != null) return b;
                }
            }
        }
        return null;
    }

    void OnCommandClick(string commandName, Color color)
    {
        Debug.Log("=== OnCommandClick ===");
        Debug.Log("Команда: " + commandName);
        Debug.Log("Цвет: " + color);
        Debug.Log("ProgrammingInterface: " + (programmingInterface != null ? "OK" : "NULL"));

        if (programmingInterface == null)
        {
            Debug.LogError("ProgrammingInterface НЕ НАЙДЕН!");
            return;
        }

        RobotCommand.CommandType type = GetCommandType(commandName);
        Debug.Log("Тип команды: " + type);

        programmingInterface.AddCommand(type, commandName, color);
        Debug.Log("AddCommand вызван");
    }

    RobotCommand.CommandType GetCommandType(string name)
    {
        string cleanName = name.Replace("🔁", "").Replace("❓", "")
                               .Replace("⚡", "").Replace("📡", "").Trim();

        switch (cleanName)
        {
            case "ВПЕРЕД": return RobotCommand.CommandType.MoveForward;
            case "НАЗАД": return RobotCommand.CommandType.MoveBackward;
            case "ВЛЕВО": return RobotCommand.CommandType.TurnLeft;
            case "ВПРАВО": return RobotCommand.CommandType.TurnRight;
            case "ЦИКЛ": return RobotCommand.CommandType.LoopStart;
            case "ЕСЛИ": return RobotCommand.CommandType.IfStart;
            case "ФУНКЦИЯ": return RobotCommand.CommandType.FunctionCall;
            case "ДАТЧИК": return RobotCommand.CommandType.CheckDistance;
            default: return RobotCommand.CommandType.MoveForward;
        }
    }
}