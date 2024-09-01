using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
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

    /// <summary>
    /// Creates a new grid based on the properties specified
    /// </summary>
    private void CreateGrid()
    {
        _grid = new GridNode[GridSizeX, GridSizeY];

        var startPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y);

        for(var y = 0; y < GridSizeY; ++y)
        {
            for(var x = 0; x < GridSizeX; ++x)
            {
                var currentPos = new Vector2(startPosition.X + x * GridNodeSize, startPosition.Y + y * GridNodeSize);
                _grid[x, y] = new GridNode(this, x, y, currentPos, false);

                //Debug.Print($"GridNode -> X: {x} Y: {y}, Position: {currentPos.X}, {currentPos.Y}", EPrintMessageType.PRINT_Log);
            }
        }
    }

    public override void Draw()
    {
        base.Draw();
        if(Debug.DebugEnabled && DrawDebugGrid && _grid != null)
        {
            for(var y = 0; y < _grid.GetLength(1); ++y)
            {
                bool toggleColor = (y % 2 == 0);
                for(var x = 0; x < _grid.GetLength(0); ++x)
                {
                    var node = _grid[x, y];
                    if(node != null)
                    {
                        var position = node.GridPosition;
                        Raylib.DrawRectangle((int)position.X, (int)position.Y, GridNodeSize, GridNodeSize, toggleColor ? Color.Red : Color.Green);
                        toggleColor = !toggleColor;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets a grid node based on indexes
    /// </summary>
    /// <param name="x">X index</param>
    /// <param name="y">Y Index</param>
    /// <returns>Grid node at index</returns>
    public GridNode GetGridNode(int x, int y)
    {
        if(_grid == null)
            return null;

        if(x > 0 && x < _grid.GetLength(0))
        {
            if(y > 0 && y < _grid.GetLength(1))
            {
                return _grid[x, y];
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a grid node based on the position
    /// </summary>
    /// <param name="position">Position To find node at</param>
    /// <returns>Grid node within position</returns>
    public GridNode GetGridNode(Vector2 position)
    {
        if(_grid == null)
            return null;

        for(var y = 0; y < _grid.GetLength(1); ++y)
        {
            for(var x = 0; x < _grid.GetLength(0); x++)
            {
                var node = _grid[x, y];
                if(node != null)
                {
                    if(node.IsWithinGridPosition(position))
                        return node;
                }
            }
        }

        return null;
    }

    public Vector2 ClampWithinBoundsPosition(Vector2 position)
    {
        return Raymath.Vector2Clamp(position, _grid[0, 0].GridPosition, _grid[GridSizeX - 1, GridSizeY - 1].GridPosition);
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

    public bool IsWithinGridPosition(Vector2 position)
    {
        if(position.X >= GridPosition.X && position.X < (GridPosition.X  + _gridRef.GridNodeSize))
        {
            if(position.Y >= GridPosition.Y && position.Y < (GridPosition.Y + _gridRef.GridNodeSize))
            {
                return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return $"GridNode -> X: {GridPosX} Y: {GridPosY}";
    }
}