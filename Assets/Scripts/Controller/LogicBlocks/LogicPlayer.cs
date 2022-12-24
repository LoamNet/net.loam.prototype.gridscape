using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicPlayer : LogicBlockBase
{
    public override void Execute(Entity current, EntityData entities, GridData grid)
    {
        int curX = current.xPos;
        int curY = current.yPos;

        bool canMoveNorth = CanMoveTo(grid, curX, curY + 1);
        bool canMoveSouth = CanMoveTo(grid, curX, curY - 1);
        bool canMoveWest = CanMoveTo(grid, curX - 1, curY);
        bool canMoveEast = CanMoveTo(grid, curX + 1, curY);

        if(canMoveNorth && Input.GetKeyDown(KeyCode.W))
        {
            current.yPos += 1;
        }
        if (canMoveSouth && Input.GetKeyDown(KeyCode.S))
        {
            current.yPos -= 1;
        }
        if (canMoveWest && Input.GetKeyDown(KeyCode.A))
        {
            current.xPos -= 1;
        }
        if (canMoveEast && Input.GetKeyDown(KeyCode.D))
        {
            current.xPos += 1;
        }
    }

    private bool CanMoveTo(GridData grid, int x, int y)
    {
        if (grid.TryGetCell(x, y, out GridCell cellW))
        {
            if (cellW.cellType == SurfaceType.Floor)
            {
                return true;
            }
        }

        return false;
    }
}
