using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    // Inspector
    [Header("Links")]
    [SerializeField] private AssetProviderLocal _tempProvider;
    [SerializeField] private Visualizer2D _tempVisualizer;
    [SerializeField] private Visualizer3D _tempVisualizer3D;
    [SerializeField] private IVisualizer _explore;

    [Header("Configuration")]
    [SerializeField] [Range(1, 30)] public int _width = 2;
    [SerializeField] [Range(1, 30)] public int _height = 3;
    [SerializeField] private int _noiseSeed = 200;
    [SerializeField] private float _noiseZoom = 0.1f;
    [SerializeField] private GridCell _cellTemplate;

    [Header("Visuals and Contents")]
    
    [Header("Live Data (CLEARED ON PLAY)")]
    [SerializeField] private EntityData _entityData;
    [SerializeField] private GridData _gridData;

    // Internal only
    private List<LogicBase> _logicBlocks;
    private List<IVisualizer> _visualizers;
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
        _entityData = new EntityData();
        _gridData = new GridData();
        _logicBlocks = new List<LogicBase>();
        _visualizers = new List<IVisualizer>();

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

        // Add visualizers
        _visualizers.Add(_tempVisualizer);
        _visualizers.Add(_tempVisualizer3D);

        // Add Entites
        _entityData.entities.Add(new EntityPlayer() { id = "player", xPos = 10, yPos = 7 });
        _entityData.entities.Add(new EntityWanderer() { id = "wanderer", xPos = 3, yPos = 9, timeBetweenMoves = 1 } );
        _entityData.entities.Add(new EntityWanderer() { id = "wanderer", xPos = 4, yPos = 15, timeBetweenMoves = 1 } );

        // Configure logic blocks
        AddLogic<LogicPlayerMove>(_logicBlocks, new System.Type[] { typeof(EntityPlayer) });
        AddLogic<LogicEntityWander>(_logicBlocks, new System.Type[] { typeof(EntityWanderer) });

        // Asset setup
        StartCoroutine(_assetProvider.Initialize(() => { _ready = true; }));
    }

    private void AddLogic<T>(List<LogicBase> logicList, System.Type[] supportedTypes) where T : LogicBase, new()
    {
        LogicBase obj = new T();
        obj.RegisterTypes(supportedTypes);
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

            for(int i = 0; i < _visualizers.Count; ++i)
            {
                _visualizers[i].RequestCompleteRedraw();
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
            if (logicBlocks[i].SupportsType(curEntity.GetType()))
            {
                logicBlocks[i].Execute(curEntity, entityData, gridData);
            }
        }
    }

    private void UpdateVisuals()
    {
        // Perform updates
        for(int i = 0; i < _visualizers.Count; ++i)
        {
            IVisualizer visualizer = _visualizers[i];
            visualizer.Visualize(_assetProvider, _gridData, _entityData);
        }
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
                cell.height = height;

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
