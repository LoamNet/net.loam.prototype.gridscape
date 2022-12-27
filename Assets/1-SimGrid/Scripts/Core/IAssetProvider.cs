using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssetProvider
{
    /// <summary>
    /// Performs any initialization the system needs in a coroutine.
    /// Once completed, the OnLoadCompleted
    /// </summary>
    /// <returns></returns>
    IEnumerator Initialize(Action onComplete);

    /// <summary>
    /// Returns a template for a specified ID
    /// </summary>
    /// <param name="id">the unique string ID to look up</param>
    /// <param name="asset">A game object that will either be initialized or null depending on if the asset was found</param>
    /// <returns>true if the asset was found and 'asset' was assigned something other than null, false if not</returns>
    bool TryGetAssetID(string id, out GameObject asset);
}
