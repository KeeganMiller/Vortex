using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public static class Input
{
    private static List<InputAction> _inputActions = new List<InputAction>();
    public static Vector2 GetVectorInput(string positiveX, string negativeX, string positiveY, string negativeY)
    {
        var inputValue = Vector2.Zero;

        if(GetAction(positiveX).IsKeyDown())
            inputValue.X += 1;
        if(GetAction(negativeX).IsKeyDown())
            inputValue.X -= 1;
        if(GetAction(positiveY).IsKeyDown())
            inputValue.Y += 1;
        if(GetAction(negativeY).IsKeyDown())
            inputValue.Y -= 1;

        return inputValue;
    }

    public static InputAction GetAction(string actionName)
    {
        foreach(var action in _inputActions)
        {
            if(action.ActionName == actionName)
                return action;
        }

        return null;
    }
}

public class InputAction
{
    public string ActionName;
    public KeyboardKey KeyValue;

    public System.Action KeyPressed;
    public System.Action KeyReleased;

    public bool IsKeyDown() => Raylib.IsKeyDown(KeyValue);
    public bool IsKeyUp() => Raylib.IsKeyUp(KeyValue);
    public bool IsKeyPressed()
    {
        if(Raylib.IsKeyPressed(KeyValue))
        {
            KeyPressed?.Invoke();
            return true;
        }

        return false;
    }
    public bool IsKeyReleased() 
    {
        if(Raylib.IsKeyReleased(KeyValue))
        {
            KeyReleased?.Invoke();
            return true;
        }

        return false;
    }
}