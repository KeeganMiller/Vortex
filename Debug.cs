using System;

namespace Vortex;

public enum EPrintMessageType
{
    PRINT_Log,
    PRINT_Error,
    PRINT_Warning,
    PRINT_Custom
}

public static class Debug
{
    public static bool DebugEnabled = true;

    public static void Print(Object message, EPrintMessageType msgType, ConsoleColor customColor = ConsoleColor.White)
    {
        Console.ForegroundColor = msgType == EPrintMessageType.PRINT_Custom ? customColor : GetConsoleColor(msgType);
        Console.WriteLine(message.ToString());
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static ConsoleColor GetConsoleColor(EPrintMessageType msgType)
    {
        switch(msgType)
        {
            case EPrintMessageType.PRINT_Log:
                return ConsoleColor.White;
            case EPrintMessageType.PRINT_Error:
                return ConsoleColor.Red;
            case EPrintMessageType.PRINT_Warning:
                return ConsoleColor.Yellow;
        }

        return ConsoleColor.White;
    }
}