using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceType
{
    NONE = 0,

    Floor,
    Wall
}

[System.Serializable]
public class GridCell
{
    public SurfaceType cellType = SurfaceType.NONE;
    public float alpha = 0;

    public GridCell Clone()
    {
        GridCell clone = new GridCell();
        clone.cellType = this.cellType;
        clone.alpha = this.alpha;

        return clone;
    }

    public void Copy(GridCell toCopyFrom)
    {
        this.cellType = toCopyFrom.cellType;
        this.alpha = toCopyFrom.alpha;
    }
}