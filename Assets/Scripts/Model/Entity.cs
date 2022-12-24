using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public string id;
    public int xPos;
    public int yPos;

    public Entity Clone()
    {
        Entity clone = new Entity();
        clone.id = this.id;
        clone.xPos = this.xPos;
        clone.yPos = this.yPos;

        return clone;
    }
}
