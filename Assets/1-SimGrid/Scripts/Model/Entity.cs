using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public string id = "default";
    public int xPos = 0;
    public int yPos = 0;

    //public List<DataBase> data = new List<DataBase>();

    public virtual Entity Clone()
    {
        Entity clone = new Entity();
        clone.id = this.id;
        clone.xPos = this.xPos;
        clone.yPos = this.yPos;

        /*
        for(int i = 0; i < this.data.Count; ++i)
        {
            clone.data.Add(this.data[i].Clone());
        }
        */

        return clone;
    }
}
