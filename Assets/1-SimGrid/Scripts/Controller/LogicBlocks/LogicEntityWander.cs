using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicEntityWander : LogicBase
{
    public override void Execute<T>(T target, EntityData entities, GridData grid)
    {
        EntityWanderer current = target as EntityWanderer;
        if(current == null)
        {
            return;
        }

        int curX = current.xPos;
        int curY = current.yPos;

        bool canMoveNorth = CanMoveTo(grid, curX, curY + 1);
        bool canMoveSouth = CanMoveTo(grid, curX, curY - 1);
        bool canMoveWest = CanMoveTo(grid, curX - 1, curY);
        bool canMoveEast = CanMoveTo(grid, curX + 1, curY);

        current.timeToNextMove -= Time.deltaTime;
        if (current.timeToNextMove <= 0)
        {
            int action = Random.Range(0, 8);
            if(action < 4)
            {
                // do nothing, idle.
            }
            else
            {
                switch(action)
                {
                    case 4:
                        if (canMoveNorth)
                        {
                            current.yPos += 1;
                            Debug.Log($"{current.id} is moving north!");
                        }
                        break;
                    case 5:
                        if (canMoveEast)
                        {
                            current.xPos += 1;
                            Debug.Log($"{current.id} is moving east!");
                        }
                        break;
                    case 6:
                        if (canMoveSouth)
                        {
                            current.yPos -= 1;
                            Debug.Log($"{current.id} is moving south!");
                        }
                        break;
                    case 7:
                        if (canMoveWest)
                        {
                            current.xPos -= 1;
                            Debug.Log($"{current.id} is moving west!");
                        }
                        break;
                    default:
                        Debug.Log($"{current.id} tried to move, but decided not to (rolled a {action})");
                        return;
                }
            }

            current.timeToNextMove = current.timeBetweenMoves;
        }
    }
}
