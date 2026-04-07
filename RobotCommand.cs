using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class RobotCommand
{
    public enum CommandType
    {
        MoveForward,
        MoveBackward,
        TurnLeft,
        TurnRight,
        LoopStart,
        LoopEnd,
        IfStart,
        IfEnd,
        Condition,
        CheckDistance,
        CheckObstacle,
        CheckLine,
        CheckColor,
        FunctionCall
    }

    public CommandType type;
    public float value;
    public string name;
    public int loopCount;
    public string functionName;
    public Color buttonColor;
    public int uniqueId;

    [NonSerialized]
    public List<RobotCommand> nestedCommands;

    private static int nextId = 0;

    public RobotCommand(CommandType type, float value, string name)
    {
        this.type = type;
        this.value = value;
        this.name = name;
        this.loopCount = 1;
        this.functionName = "";
        this.buttonColor = Color.white;
        this.uniqueId = nextId++;
        this.nestedCommands = new List<RobotCommand>();
    }

    public RobotCommand(CommandType type, float value, string name, int loopCount)
    {
        this.type = type;
        this.value = value;
        this.name = name;
        this.loopCount = loopCount;
        this.functionName = "";
        this.buttonColor = Color.white;
        this.uniqueId = nextId++;
        this.nestedCommands = new List<RobotCommand>();
    }

    public static void ResetIdCounter()
    {
        nextId = 0;
    }
}