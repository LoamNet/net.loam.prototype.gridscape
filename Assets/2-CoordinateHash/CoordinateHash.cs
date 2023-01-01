using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateHash : MonoBehaviour
{
    private Dictionary<Vector2, bool> testDict = new Dictionary<Vector2, bool>();

    private void Start()
    {
        Add(new Vector2(1, 2));
        Add(new Vector2(3, 4));
        Add(new Vector2(4, 3));
        Add(new Vector2(6, 6));
        Contains(new Vector2(1, 2));
        Contains(new Vector2(6, 6));
        Contains(Vector2.one * 6);
        Remove(new Vector2(6, 6));
        Contains(new Vector2(6, 6));
        Contains(Vector2.one * 6);
        Contains(new Vector2(1, 2));
        Contains(new Vector2(7, 8));
    }

    private void Add(Vector2 toAdd)
    {
        testDict.Add(toAdd, true);
        Debug.Log($"Added {toAdd}");
    }

    private void Remove(Vector2 toRemove)
    {
        bool retVal = testDict.Remove(toRemove);
        Debug.Log($"Removed {toRemove} (returned {retVal})");

    }

    private void Contains(Vector2 toCheck)
    {
        bool val = testDict.ContainsKey(new Vector2(1, 2));
        Debug.Log($"ContainsKey {toCheck}? {val}");

    }
}
