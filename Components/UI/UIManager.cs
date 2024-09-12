using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public static class UIManager
{
    public static List<UIComponent> CurrentlyOver = new List<UIComponent>();

    public static void Update(float dt)
    {
        if(CurrentlyOver.Count > 0)
        {
            if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
            {
                var comp = SceneManager.GetTopUiComponent(CurrentlyOver);
                if(comp != null)
                    comp.OnClick?.Invoke();
            }
        }
    }
}