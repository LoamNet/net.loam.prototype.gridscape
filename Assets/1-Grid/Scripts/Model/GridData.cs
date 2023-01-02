using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData : IDisposable
{
    public int width;
    public int height;
    public List<GridCell> cells;

    public void Initialize(int width, int height, GridCell template)
    {
        this.width = width;
        this.height = height;

        cells = new List<GridCell>(this.width * this.height);
        for(int i = 0; i < this.width * this.height; ++i)
        {
            cells.Add(template.Clone());
        }
    }

    public void Dispose()
    {
        
    }

    public GridCell GetCell(int xPos, int yPos)
    {
        // Bounds checking
        if (xPos < 0 || xPos >= width || yPos < 0 || yPos >= height)
        {
            return null;
        }

        int index = width * yPos + xPos;
        return cells[index];
    }

    public GridCell GetCell(Vector2 pos)
    {
        return GetCell((int)pos.x, (int)pos.y);
    }

    public bool TryGetCell(int xPos, int yPos, out GridCell data)
    {
        data = GetCell(xPos, yPos);
        return data != null;
    }

    public bool TryGetCell(Vector2 pos, out GridCell data)
    {
        data = GetCell(pos);
        return data != null;
    }
}
