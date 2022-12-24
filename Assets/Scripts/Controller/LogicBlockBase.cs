using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogicBlockBase
{
    /// <summary>
    /// The IDs that will execute the logic specified
    /// </summary>
    private HashSet<string> _relatedIDs = new HashSet<string>();

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

    public abstract void Execute(Entity current, EntityData entities, GridData grid);
}
