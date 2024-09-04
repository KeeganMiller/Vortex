using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public class GridRenderComponent : Component
{
    public int GridCellsX;                       // How many cells are rendered
    public int GridCellsY;
    public int GridSpacing;                         // The size of each cell
    private GridComponent _gridComp;
    public bool UseGridComponent;
    public Color Tint = Color.White;
    public float LineThickness = 1;


    public override void Draw()
    {
        base.Draw();

        var cellsX = GridCellsX;
        var cellsY = GridCellsY;
        var spacing = GridSpacing;
        var startPos = Owner.Transform.Position;

        if(_gridComp != null)
        {
            cellsX = _gridComp.GridSizeX;
            cellsY = _gridComp.GridSizeY;
            spacing = _gridComp.GridNodeSize;
        }

        var currentPos = startPos;

        for(var y = 0; y < cellsY; ++y)
        {
            for(var x = 0; x < cellsX; ++x)
            {
                var endPosX = currentPos.X + spacing;
                var endPosY = currentPos.Y + spacing;

                if(IsInCameraView(currentPos))
                {
                    Raylib.DrawLineEx(currentPos, new Vector2(endPosX, currentPos.Y), LineThickness, Tint);
                    Raylib.DrawLineEx(currentPos, new Vector2(currentPos.X, endPosY), LineThickness, Tint);
                }

                currentPos.X += spacing;
            }

            currentPos.Y += spacing;
            currentPos.X = startPos.X;
        }
    }

    private bool IsInCameraView(Vector2 positionRef)
    {
        var windowWidth = Game.WindowSettings.WindowWidth + 75;
        var windowHeight = Game.WindowSettings.WindowHeight + 75;
        var cameraPos = Game.CameraRef.Target;

        if(positionRef.X >= (cameraPos.X - 75) && positionRef.X < (cameraPos.X + windowWidth))
        {
            if(positionRef.Y >= (cameraPos.Y - 75) && positionRef.Y < (cameraPos.Y + windowHeight))
            {
                return true;
            }
        }

        return false;   
    }

    public void SetGridComponent(GridComponent grid)
    {
        _gridComp = grid;
        UseGridComponent = true;
    }
}