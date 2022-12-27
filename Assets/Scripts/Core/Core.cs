using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    // Inspector
    [Header("Links")]
    [SerializeField] private AssetProviderLocal _tempProvider;

    [Header("Configuration")]
    [SerializeField] [Range(1, 30)] public int _width = 2;
    [SerializeField] [Range(1, 30)] public int _height = 3;
    [SerializeField] private int _noiseSeed = 200;
    [SerializeField] private float _noiseZoom = 0.1f;
    [SerializeField] private GridCell _cellTemplate;

    [Header("Visuals and Contents")]
    [SerializeField] private Visualizer _visualizer;
    
    [Header("Live Data (can be cleared or live edited)")]
    [SerializeField] private EntityData _entityData;
    [SerializeField] private GridData _gridData;

    // Internal only
    private List<LogicBase> _logicBlocks;
    private IAssetProvider _assetProvider;
    private int _lastWidth;
    private int _lastHeight;
    private float _lastNoise;
    private float _lastNoiseZoom;
    private bool _ready;

    /// <summary>
    /// Create things here
    /// </summary>
    private void Awake()
    {
        _ready = false;
        _gridData = new GridData();
        _logicBlocks = new List<LogicBase>();

        _assetProvider = _tempProvider;
        _lastWidth = _width;
        _lastHeight = _height;
        _lastNoise = _noiseSeed;
        _lastNoiseZoom = _noiseZoom;
    }

    /// <summary>
    /// Initialize things here
    /// </summary>
    private void Start()
    {
        // Grid setup
        _gridData.Initialize(_width, _height, _cellTemplate);
        SetNoise(_gridData, _noiseSeed, _noiseZoom);

        // Configure logic blocks
        AddLogic<LogicPlayer>(_logicBlocks, new string[] { "player" });
        AddLogic<LoigicWanderer>(_logicBlocks, new string[] { "wanderer" });

        // Asset setup
        StartCoroutine(_assetProvider.Initialize(() => { _ready = true; }));
    }

    private void AddLogic<T>(List<LogicBase> logicList, string[] types) where T : LogicBase, new()
    {
        LogicBase obj = new T();
        obj.RegisterIDs(types);
        logicList.Add(obj);
    }

    private void Update()
    {
        // Early exit if we're not ready
        if (!_ready)
        {
            return;
        }

        UpdateGridSizeIfNeeded();
        UpdateAllEntityLifeAmounts();
        UpdateAllEntities();
        UpdateVisuals();
    }

    private void UpdateGridSizeIfNeeded()
    {
        if (_lastWidth != _width || _lastHeight != _height || _lastNoise != _noiseSeed || _lastNoiseZoom != _noiseZoom)
        {
            _gridData.Dispose();
            _gridData = new GridData();
            _gridData.Initialize(_width, _height, _cellTemplate);
            _lastWidth = _width;
            _lastHeight = _height;
            SetNoise(_gridData, _noiseSeed, _noiseZoom);
        }
    }

    /// <summary>
    /// Go through all entities and update their seconds alive number
    /// </summary>
    private void UpdateAllEntityLifeAmounts()
    {
        // Update current time alive so far
        float dt = Time.deltaTime;
        for (int i = 0; i < _entityData.entities.Count; ++i)
        {
            Entity curEntity = _entityData.entities[i];
            double lastSecAlive = curEntity.secondsAlive;
            curEntity.secondsAlive += dt;
            if ((int)curEntity.secondsAlive != (int)lastSecAlive)
            {
                curEntity.firstUpdateOfNewSecond = true;
            }
            else
            {
                curEntity.firstUpdateOfNewSecond = false;
            }
        }
    }

    /// <summary>
    /// Go through all entities and perform logic in order of 
    /// registration order on entities that have the correct ID.
    /// </summary>
    private void UpdateAllEntities()
    {
        // Update each entity with behaviors
        for(int i = 0; i < _entityData.entities.Count; ++i)
        {
            Entity curEntity = _entityData.entities[i];

            UpdateEntity(curEntity, _logicBlocks, _entityData, _gridData);
        }
    }
    private void UpdateEntity(Entity curEntity, List<LogicBase> logicBlocks, EntityData entityData, GridData gridData)
    {
        for (int i = 0; i < logicBlocks.Count; ++i)
        {
            if (logicBlocks[i].SupportsID(curEntity.id))
            {
                logicBlocks[i].Execute(curEntity, entityData, gridData);
            }
        }
    }

    private void UpdateVisuals()
    {
        // Perform updates
        _visualizer.Update(_assetProvider, _gridData, _entityData);
    }

    /// <summary>
    /// Given a grid, fills it with nois.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="seed"></param>
    /// <param name="zoom"></param>
    private static void SetNoise(GridData data, int seed, float zoom)
    {
        System.Random rand = new System.Random(seed);
        float scaled = 10000f * (float)rand.NextDouble();

        for (int h = 0; h < data.height; ++h)
        {
            for (int w = 0; w < data.width; ++w)
            {
                float height = Mathf.PerlinNoise(scaled + w * zoom, scaled + h * zoom);

                GridCell cell = data.GetCell(w, h);
                cell.alpha = height;

                if (height > 0.5f)
                {
                    cell.cellType = SurfaceType.Wall;
                }
                else
                {
                    cell.cellType = SurfaceType.Floor;
                }
            }
        }
    }
}
