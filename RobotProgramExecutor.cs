using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotProgramExecutor : MonoBehaviour
{
    [Header("References")]
    public GameObject robotBody;
    public RobotMotor robotMotor;

    [Header("Settings")]
    public float moveForce = 5f;
    public float rotationAngle = 90f;
    public float commandDuration = 0.5f;
    public float betweenCommandDelay = 0.2f;

    private Vector3 startPosition;
    private float startRotation;
    private bool startPositionSaved = false;
    private bool isRunning = false;
    private Coroutine currentRoutine;

    void Start()
    {
        InitializeReferences();
    }

    void InitializeReferences()
    {
        if (robotMotor == null)
        {
            robotMotor = FindFirstObjectByType<RobotMotor>();
        }

        if (robotBody == null && robotMotor != null)
        {
            robotBody = robotMotor.gameObject;
        }

        SaveStartPosition();
    }

    void SaveStartPosition()
    {
        if (robotBody != null)
        {
            startPosition = robotBody.transform.position;
            startRotation = robotBody.transform.eulerAngles.z;
            startPositionSaved = true;
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

        if (robotMotor == null)
        {
            Debug.LogError("❌ Motor НЕ назначен!");
            return;
        }

        if (isRunning) StopProgram();

        isRunning = true;
        robotMotor.SetManualControl(false);

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

        if (robotMotor != null)
        {
            robotMotor.SetManualControl(true);
            robotMotor.Stop();
        }
    }

    private IEnumerator RunProgramCoroutine(List<RobotCommand> commands)
    {
        for (int i = 0; i < commands.Count; i++)
        {
            if (!isRunning) yield break;

            RobotCommand cmd = commands[i];
            Debug.Log("[" + (i + 1) + "/" + commands.Count + "] " + cmd.name);

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

        if (robotMotor != null)
            robotMotor.SetManualControl(true);

        isRunning = false;
        currentRoutine = null;
    }

    private IEnumerator ExecuteLoop(RobotCommand loopCommand)
    {
        Debug.Log("🔁 ЦИКЛ: " + loopCommand.loopCount + " повторений");

        if (loopCommand.nestedCommands == null || loopCommand.nestedCommands.Count == 0)
        {
            Debug.LogWarning("⚠️ Цикл пуст!");
            yield break;
        }

        for (int iteration = 0; iteration < loopCommand.loopCount && isRunning; iteration++)
        {
            Debug.Log("  🔁 Итерация " + (iteration + 1) + "/" + loopCommand.loopCount);

            for (int j = 0; j < loopCommand.nestedCommands.Count && isRunning; j++)
            {
                RobotCommand nestedCmd = loopCommand.nestedCommands[j];
                Debug.Log("    → " + nestedCmd.name);
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
                Debug.Log("  ⬆️ ВПЕРЁД");
                float elapsedForward = 0f;
                while (elapsedForward < commandDuration && isRunning)
                {
                    robotMotor.Move(moveForce);
                    elapsedForward += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                break;

            case RobotCommand.CommandType.MoveBackward:
                Debug.Log("  ⬇️ НАЗАД");
                float elapsedBackward = 0f;
                while (elapsedBackward < commandDuration && isRunning)
                {
                    robotMotor.Move(-moveForce);
                    elapsedBackward += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                break;

            case RobotCommand.CommandType.TurnLeft:
                Debug.Log("  ↺ ВЛЕВО на " + rotationAngle + "°");
                if (robotBody != null)
                {
                    robotBody.transform.Rotate(0, 0, rotationAngle);
                }
                yield return new WaitForSeconds(0.3f);
                break;

            case RobotCommand.CommandType.TurnRight:
                Debug.Log("  ↻ ВПРАВО на " + rotationAngle + "°");
                if (robotBody != null)
                {
                    robotBody.transform.Rotate(0, 0, -rotationAngle);
                }
                yield return new WaitForSeconds(0.3f);
                break;
        }

        if (robotMotor != null)
            robotMotor.Stop();
    }

    public void ResetRobotPosition()
    {
        if (robotMotor != null)
        {
            robotMotor.Stop();
            robotMotor.SetManualControl(true);
        }

        if (startPositionSaved && robotBody != null)
        {
            robotBody.transform.position = startPosition;
            robotBody.transform.rotation = Quaternion.Euler(0, 0, startRotation);
            Debug.Log("🔄 Робот сброшен на позицию: " + startPosition);
        }
    }

    void OnDestroy()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
    }
}