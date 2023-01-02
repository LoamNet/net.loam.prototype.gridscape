using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityData
{
    public List<Entity> entities = new List<Entity>();

    public List<Entity> GetEntitiesAtPosition(int x, int y)
    {
        List<Entity> entities = new List<Entity>();
        for(int i = 0; i < entities.Count; ++i)
        {
            Entity curEntity = entities[i];
            if(curEntity.xPos == x && curEntity.yPos == y)
            {
                entities.Add(curEntity);
            }
        }

        return entities;
    }
}
