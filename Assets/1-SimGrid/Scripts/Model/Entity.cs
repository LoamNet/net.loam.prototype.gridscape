using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public string id;
    public int xPos;
    public int yPos;

    public double secondsAlive;
    [System.NonSerialized] public bool firstUpdateOfNewSecond;

    public Entity Clone()
    {
        Entity clone = new Entity();
        clone.id = this.id;
        clone.xPos = this.xPos;
        clone.yPos = this.yPos;

        // Reset seconds alive and related, that doesn't copy over.
        clone.secondsAlive = 0;
        clone.firstUpdateOfNewSecond = false;
        return clone;
    }
}
