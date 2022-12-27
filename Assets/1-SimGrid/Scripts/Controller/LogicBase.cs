using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogicBase
{
    /// <summary>
    /// The IDs that will execute the logic specified
    /// </summary>
    private HashSet<System.Type> _registeredTypes = new HashSet<System.Type>();
    public abstract void Execute<T>(T target, EntityData entities, GridData grid) where T : Entity;

    public void RegisterTypes(System.Type[] toRegister)
    {
        for(int i = 0; i < toRegister.Length; ++i)
        {
            RegisterType(toRegister[i]);
        }
    }

    public void RegisterType(System.Type type)
    {
        bool added = _registeredTypes.Add(type);
        if (!added)
        {
            Debug.LogWarning($"Tried to add a duplicate type: {type.Name}");
        }
    }

    public bool SupportsType(System.Type type)
    {
        return _registeredTypes.Contains(type);
    }

    public void RegisterType<T>() where T : Entity
    {
        RegisterType(typeof(T));
    }

    public bool SupportsType<T>()
    {
        return SupportsType(typeof(T));
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
