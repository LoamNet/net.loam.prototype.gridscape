using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stub. The intent is to eventually use addressables to look up prefabs/assets by ID, and only load as needed.
/// </summary>
public class AssetProviderAddressable : IAssetProvider
{
    // Internal Variables
    private Dictionary<string, GameObject> _assetLookup = new Dictionary<string, GameObject>();

    public IEnumerator Initialize(Action onComplete)
    {
        // Grab addressables asyncronously 
        yield return null;

        // Call when completed
        onComplete?.Invoke();
    }

    public bool TryGetAssetID(string id, out GameObject asset)
    {
         return _assetLookup.TryGetValue(id, out asset);
    }
}
