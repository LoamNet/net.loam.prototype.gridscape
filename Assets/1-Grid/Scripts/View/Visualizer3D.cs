using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer3D : MonoBehaviour, IVisualizer
{
    [SerializeField] private string _assetPrefix = "3D";
    [SerializeField] private Transform _visualsParent;
    [SerializeField] private float _squareSize = 1;
    [SerializeField] private float _padding = 0.01f;

    private List<GameObject> _visualsActive = new List<GameObject>();
    private List<GameObject> _visualsPool = new List<GameObject>();
    private List<GameObject> _entityList = new List<GameObject>(); // not a pool

    Dictionary<Vector2, bool> _coordDirty = new Dictionary<Vector2, bool>();
    private bool _redraw;

    public void RequestCompleteRedraw()
    {
        _redraw = true;
    }

    private void CreateEntry()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.SetActive(true);
        obj.transform.position = Vector3.zero;
        obj.SetActive(false);

        _visualsPool.Add(obj);
    }

    private void Pool(GameObject toPool)
    {
        toPool.SetActive(false);
        toPool.transform.position = Vector3.zero;

        _visualsPool.Add(toPool);
    }

    private GameObject GetObjectFromPool()
    {
        if (_visualsPool.Count == 0)
        {
            CreateEntry();
        }

        GameObject obj = _visualsPool[0];
        _visualsPool.RemoveAt(0);

        obj.transform.SetParent(_visualsParent);
        return obj;
    }

    public void Visualize(IAssetProvider assetProvider, GridData gridData, EntityData entityData)
    {
        DisplayCells(gridData);
        DisplayEntities(assetProvider, gridData, entityData);
    }

    private void DisplayEntities(IAssetProvider assetProvider, GridData gridData, EntityData entityData)
    {
        // Clear entity list
        for (int i = 0; i < _entityList.Count; ++i)
        {
            GameObject.Destroy(_entityList[i]);
        }
        _entityList.Clear();

        for (int i = 0; i < entityData.entities.Count; ++i)
        {
            Entity entity = entityData.entities[i];
            CalculateXY(gridData, entity.xPos, entity.yPos, out float x, out float y);

            string id = _assetPrefix + entity.id;
            if (assetProvider.TryGetAssetID(id, out GameObject entityTemplate))
            {
                DisplayEntity(entity, gridData, x, y, entityTemplate);
            }
            else
            {
                Debug.LogError($"Couldn't find entity with ID {id}");
            }
        }
    }

    private void DisplayCells(GridData gridData)
    {
        int gridWidth = gridData.width;
        int gridHeight = gridData.height;

        // Clear everything if we resized
        if (_redraw)
        {
            // First, pool everything we *do* have, then clear that out.
            // Not efficient but do it anyways.
            for (int i = 0; i < _visualsActive.Count; ++i)
            {
                Pool(_visualsActive[i]);
            }
            _visualsActive.Clear();
            _coordDirty.Clear();
        }

        // Place everything, we can assume we're the right size now
        for (int curHeight = 0; curHeight < gridHeight; ++curHeight)
        {
            for (int curWidth = 0; curWidth < gridWidth; ++curWidth)
            {
                if (gridData.TryGetCell(curWidth, curHeight, out GridCell entry))
                {
                    if (IsDirty(curWidth, curHeight))
                    {
                        CalculateXY(gridData, curWidth, curHeight, out float x, out float y);
                        DisplayCell(x, y, entry);
                        SetDirty(curWidth, curHeight, false);
                    }
                }
            }
        }
    }

    private bool IsDirty(int x, int y)
    {
        Vector2 coord = new Vector2(x, y);
        if (_coordDirty.TryGetValue(coord, out bool isDirty))
        {
            return isDirty;
        }
        else
        {
            return true;
        }
    }

    private void SetDirty(int x, int y, bool val)
    {
        Vector2 coord = new Vector2(x, y);
        if (_coordDirty.ContainsKey(coord))
        {
            _coordDirty[coord] = val;
        }
        else
        {
            _coordDirty.Add(coord, val);
        }
    }

    private void CalculateXY(GridData gridData, int gridX, int gridY, out float xPos, out float yPos)
    {
        float gridSize = (_squareSize + _padding * 2);
        float widthOffset = gridSize * gridData.width - gridSize;
        float heightOffset = gridSize * gridData.height - gridSize;

        xPos = gridX * gridSize - (widthOffset / 2f);
        yPos = gridY * gridSize - (heightOffset / 2f);
    }

    private void DisplayCell(float x, float y, GridCell entry)
    {
        GameObject newItem = GetObjectFromPool();
        newItem.SetActive(true);

        float height = GetVisualHeight(entry);
        newItem.transform.localScale = new Vector3(_squareSize, height, _squareSize);
        newItem.transform.localPosition = new Vector3(x + _padding, height / 2, y + _padding);

        _visualsActive.Add(newItem);
    }

    private float GetVisualHeight(GridData data, int x, int y)
    {
        GridCell cell = data.GetCell(x, y);
        return GetVisualHeight(cell);
    }

    private float GetVisualHeight(GridCell cell)
    {
        float scalar = 4f;
        switch (cell.cellType)
        {
            case SurfaceType.Floor:
                return (0.4f * cell.height) * scalar;
            
            case SurfaceType.Wall:
                return (cell.height + 0.05f) * scalar;

            case SurfaceType.NONE:
            default:
                Debug.LogError("Invalid contents");
                return 0;
        }
    }

    private void DisplayEntity(Entity entity, GridData data, float xVal, float yVal, GameObject entityTemplate)
    {
        GameObject newItem = GameObject.Instantiate(entityTemplate);
        newItem.transform.SetParent(_visualsParent);
        newItem.SetActive(true);

        float x = xVal + _padding;
        float y = GetVisualHeight(data, (int)entity.xPos, (int)entity.yPos) + 0.5f;
        float z = yVal + _padding;
        newItem.transform.localPosition = new Vector3(xVal, y, yVal);

        _entityList.Add(newItem);
    }

    public void Dispose()
    {
        for (int i = 0; i < _visualsActive.Count; ++i)
        {
            GameObject.Destroy(_visualsActive[i]);
        }
        _visualsActive.Clear();

        for (int i = 0; i < _visualsPool.Count; ++i)
        {
            GameObject.Destroy(_visualsPool[i]);
        }
        _visualsPool.Clear();

        for (int i = 0; i < _entityList.Count; ++i)
        {
            GameObject.Destroy(_entityList[i]);
        }
        _entityList.Clear();
    }
}
