using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quick and dirty asset provider implementation that takes a list from the 
/// inspector and use the assets specified that way. Designed to be serializable.
/// </summary>
[System.Serializable]
public class AssetProviderLocal : MonoBehaviour, IAssetProvider
{
    // Inspector 
    [SerializeField] private List<GameObject> _assets;

    // Internal 
    private Dictionary<string, GameObject> _assetLookup;

    /// <summary>
    /// Sorts the provided asset list into an easy-to-access dictionary.
    /// </summary>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public IEnumerator Initialize(Action onComplete)
    {
        yield return null;
        _assetLookup = new Dictionary<string, GameObject>();
        
        for(int i = 0; i < _assets.Count; ++i)
        {
            GameObject asset = _assets[i];
            string id = asset.name;
            _assetLookup.Add(id, asset);
            
        }

        onComplete?.Invoke();
    }

    /// <summary>
    /// Attempts to retrieve an asset with the specified ID
    /// </summary>
    /// <param name="id">Id of the asset to try and get</param>
    /// <param name="asset">the asset, or null on failure</param>
    /// <returns>returns true if the asset was found, false if not</returns>
    public bool TryGetAssetID(string id, out GameObject asset)
    {
        return _assetLookup.TryGetValue(id, out asset);
    }
}
