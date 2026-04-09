using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotProgramExecutor : MonoBehaviour
{
    [Header("References")]
    public GameObject robotBody;
    public RobotMotor robotMotor;

    [Header("Settings")]
    public float moveSpeed = 100f;
    public float turnSpeed = 180f;
    public float betweenCommandDelay = 0.2f;
    public float robotRadius = 0.5f;

    [Header("Arena Bounds")]
    public bool enableBoundsClamping = true;
    public float arenaLeft = -10f;
    public float arenaRight = 10f;
    public float arenaBottom = -10f;
    public float arenaTop = 10f;

    private Vector3 startPosition;
    private float startRotation;
    private bool startPositionSaved = false;
    private bool isRunning = false;
    private Coroutine currentRoutine;

    void Start()
    {
        InitializeReferences();

        // ✅ Проверка координат при старте
        if (robotBody != null)
        {
            Debug.Log($"📍 Робот на позиции: {robotBody.transform.position}");
            Debug.Log($"📏 Границы арены: [{arenaLeft}, {arenaRight}] x [{arenaBottom}, {arenaTop}]");
        }
    }

    void InitializeReferences()
    {
        if (robotMotor == null)
            robotMotor = FindFirstObjectByType<RobotMotor>();

        if (robotBody == null && robotMotor != null)
            robotBody = robotMotor.gameObject;

        SaveStartPosition();

        if (robotBody == null)
            Debug.LogError("❌ robotBody НЕ НАЙДЕН!");
    }

    void SaveStartPosition()
    {
        if (robotBody != null)
        {
            startPosition = robotBody.transform.position;
            startRotation = robotBody.transform.eulerAngles.z;
            startPositionSaved = true;
            Debug.Log($"💾 Стартовая позиция: {startPosition}");
        }
    }

    public void ExecuteProgram(List<RobotCommand> commands)
    {
        Debug.Log("🚀 Запуск программы!");

        if (commands == null || commands.Count == 0)
        {
            Debug.LogWarning("⚠️ Программа пуста!");
            return;
        }

        if (robotBody == null)
        {
            Debug.LogError("❌ robotBody НЕ назначен!");
            return;
        }

        if (isRunning) StopProgram();

        isRunning = true;
        currentRoutine = StartCoroutine(RunProgramCoroutine(commands));
    }

    public void StopProgram()
    {
        Debug.Log("⏹ Остановка программы!");
        isRunning = false;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }

    private IEnumerator RunProgramCoroutine(List<RobotCommand> commands)
    {
        for (int i = 0; i < commands.Count; i++)
        {
            if (!isRunning) yield break;

            RobotCommand cmd = commands[i];
            Debug.Log($"[{i + 1}/{commands.Count}] {cmd.name} (значение: {cmd.value})");

            if (cmd.type == RobotCommand.CommandType.LoopStart)
            {
                yield return StartCoroutine(ExecuteLoop(cmd));
            }
            else
            {
                yield return StartCoroutine(ExecuteSingleCommand(cmd));
            }

            if (i < commands.Count - 1 && isRunning)
            {
                yield return new WaitForSeconds(betweenCommandDelay);
            }
        }

        Debug.Log("✅ Программа выполнена!");
        isRunning = false;
        currentRoutine = null;
    }

    private IEnumerator ExecuteLoop(RobotCommand loopCommand)
    {
        Debug.Log($"🔁 ЦИКЛ: {loopCommand.loopCount} повторений");

        if (loopCommand.nestedCommands == null || loopCommand.nestedCommands.Count == 0)
        {
            Debug.LogWarning("⚠️ Цикл пуст!");
            yield break;
        }

        for (int iteration = 0; iteration < loopCommand.loopCount && isRunning; iteration++)
        {
            Debug.Log($"  🔁 Итерация {iteration + 1}/{loopCommand.loopCount}");

            for (int j = 0; j < loopCommand.nestedCommands.Count && isRunning; j++)
            {
                RobotCommand nestedCmd = loopCommand.nestedCommands[j];
                Debug.Log($"    → {nestedCmd.name} (значение: {nestedCmd.value})");
                yield return StartCoroutine(ExecuteSingleCommand(nestedCmd));
            }
        }

        Debug.Log("  ✅ Цикл завершён!");
    }

    private IEnumerator ExecuteSingleCommand(RobotCommand command)
    {
        switch (command.type)
        {
            case RobotCommand.CommandType.MoveForward:
                Debug.Log($"  ⬆️ ВПЕРЁД на {command.value} пикселей");
                yield return StartCoroutine(MoveDistance(command.value));
                break;

            case RobotCommand.CommandType.MoveBackward:
                Debug.Log($"  ⬇️ НАЗАД на {command.value} пикселей");
                yield return StartCoroutine(MoveDistance(-command.value));
                break;

            case RobotCommand.CommandType.TurnLeft:
                Debug.Log($"  ↺ ВЛЕВО на {command.value}°");
                yield return StartCoroutine(RotateAngle(command.value));
                break;

            case RobotCommand.CommandType.TurnRight:
                Debug.Log($"  ↻ ВПРАВО на {command.value}°");
                yield return StartCoroutine(RotateAngle(-command.value));
                break;
        }
    }

    // ✅ ИСПРАВЛЕНО: используем transform.up (локальное направление робота)
    IEnumerator MoveDistance(float distance)
    {
        if (robotBody == null)
        {
            Debug.LogError("❌ robotBody is NULL в MoveDistance!");
            yield break;
        }

        float elapsed = 0f;
        float duration = Mathf.Abs(distance) / moveSpeed;
        Vector3 startPos = robotBody.transform.position;

        // ✅ ИСПОЛЬЗУЕМ ЛОКАЛЬНОЕ НАПРАВЛЕНИЕ РОБОТА
        Vector3 direction = robotBody.transform.up;
        Vector3 targetPos = startPos + direction * distance;

        // ✅ Проверяем границы арены
        if (enableBoundsClamping)
        {
            targetPos = ClampToArenaBounds(targetPos);
        }

        Debug.Log($"  📍 Движение: от {startPos} к {targetPos} (направление: {direction})");

        while (elapsed < duration && isRunning)
        {
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, elapsed / duration);

            if (enableBoundsClamping)
            {
                currentPos = ClampToArenaBounds(currentPos);
            }

            robotBody.transform.position = currentPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        robotBody.transform.position = targetPos;
        Debug.Log($"✅ Движение завершено: {distance} пикселей (позиция: {robotBody.transform.position})");
    }

    // ✅ Ограничивает позицию границами арены
    Vector3 ClampToArenaBounds(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, arenaLeft + robotRadius, arenaRight - robotRadius);
        float clampedY = Mathf.Clamp(position.y, arenaBottom + robotRadius, arenaTop - robotRadius);

        return new Vector3(clampedX, clampedY, position.z);
    }

    IEnumerator RotateAngle(float angle)
    {
        if (robotBody == null)
        {
            Debug.LogError("❌ robotBody is NULL в RotateAngle!");
            yield break;
        }

        float elapsed = 0f;
        float duration = Mathf.Abs(angle) / turnSpeed;

        float startAngle = robotBody.transform.eulerAngles.z;
        if (startAngle < 0) startAngle += 360f;

        float targetAngle = startAngle + angle;

        Debug.Log($"  🔄 Вращение: от {startAngle}° к {targetAngle}°");

        while (elapsed < duration && isRunning)
        {
            float t = elapsed / duration;
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            robotBody.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        robotBody.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        Debug.Log($"✅ Поворот завершён: от {startAngle}° до {targetAngle}°");
    }

    public void ResetRobotPosition()
    {
        if (startPositionSaved && robotBody != null)
        {
            Vector3 resetPos = startPosition;
            if (enableBoundsClamping)
            {
                resetPos = ClampToArenaBounds(startPosition);
            }

            robotBody.transform.position = resetPos;
            robotBody.transform.rotation = Quaternion.Euler(0, 0, startRotation);
            Debug.Log($"🔄 Робот сброшен на позицию: {resetPos}");
        }
    }

    void OnDestroy()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
    }
}