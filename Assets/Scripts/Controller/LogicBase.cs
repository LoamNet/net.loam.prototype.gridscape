using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogicBase
{
    /// <summary>
    /// The IDs that will execute the logic specified
    /// </summary>
    private HashSet<string> _relatedIDs = new HashSet<string>();
    public abstract void Execute(Entity current, EntityData entities, GridData grid);

    public void RegisterIDs(string[] collection)
    {
        for(int i = 0; i < collection.Length; ++i)
        {
            string toAdd = collection[i];
            RegisterID(toAdd);
        }
    }

    public void RegisterID(string toAdd)
    {
        bool added = _relatedIDs.Add(toAdd);
        if (!added)
        {
            Debug.LogWarning($"Tried to add a duplicate term: {toAdd}");
        }
    }

    public bool SupportsID(string id)
    {
        return _relatedIDs.Contains(id);
    }


    ///////////////////////
    // Utility Functions //
    ///////////////////////
    
    /// <summary>
    /// Determine if a grid allows a 'walking' entity (player, NPC, etc) to walk to a specific square, etc
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected static bool CanMoveTo(GridData grid, int x, int y)
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
