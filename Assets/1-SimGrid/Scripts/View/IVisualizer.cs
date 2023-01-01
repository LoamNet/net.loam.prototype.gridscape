using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisualizer : IDisposable
{
    public void Visualize(IAssetProvider assetProvider, GridData gridData, EntityData entityData);
}
