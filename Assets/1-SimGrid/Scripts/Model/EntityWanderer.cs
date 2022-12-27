using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWanderer : Entity
{
    public float timeBetweenMoves;

    // Runtime only
    [System.NonSerialized] public float timeToNextMove;
}
