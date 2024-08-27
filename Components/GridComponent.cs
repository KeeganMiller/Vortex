using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;

namespace Vortex;

public class GridComponent : Component
{
    private GridNode[,] _grid;
    public int GridSizeX = 10;
    public int GridSizeY = 10;
    public int GridNodeSize = 16;

    public bool DrawDebugGrid = false;

    public override void Start()
    {
        base.Start();

        if(GridSizeX > 0 && GridSizeY > 0 && GridNodeSize > 0)
            CreateGrid();
    }

    private void CreateGrid()
    {
        _grid = new GridNode[GridSizeX, GridSizeY];

        var startPos = Owner.Transform.Position;
        var currentPos = startPos;

        for(var y = 0; y < _grid.GetLength(1); ++y)
        {
            for(var x = 0; x < _grid.GetLength(0); ++x)
            {
                _grid[x, y] = new GridNode(this, x, y, currentPos);
                currentPos.X += GridNodeSize;
            }

            currentPos.Y += GridNodeSize;
            currentPos.X = startPos.X;
        }
    }

    public override void Draw()
    {
        base.Draw();
        if(Debug.DebugEnabled && DrawDebugGrid)
        {
            bool toggleColor = false;
            for(var y = 0; y < _grid.GetLength(1); ++y)
            {
                for(var x = 0; x < _grid.GetLength(0); ++x)
                {
                    var node = _grid[x, y];
                    if(node != null)
                    {
                        Raylib.DrawRectangle((int)node.GridPosition.X, (int)node.GridPosition.Y, GridNodeSize, GridNodeSize, toggleColor ? Color.Red : Color.Green);
                        toggleColor = !toggleColor;
                    }
                }
            }
        }
    }
}

public class GridNode
{
    public int GridPosX { get; }
    public int GridPosY { get; }
    public Vector2 GridPosition { get; }
    public bool IsWalkable = false;
    private GridComponent _gridRef;

    public GridNode(GridComponent gridRef, int posX, int posY, Vector2 pos, bool isWalkable = true)
    {
        _gridRef = gridRef;
        GridPosX = posX;
        GridPosY = posY;
        GridPosition = pos;
        IsWalkable = isWalkable;
    }
}