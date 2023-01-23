using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid3D : MonoBehaviour
{
    [SerializeField] private GameObject visualRock;
    [SerializeField] private GameObject visualWater;
    [SerializeField] private GameObject visualPlayer;

    [SerializeField] private Data data;
    [SerializeField] private float zoom = 20.0f;

    private Visualizer visuals;

    public enum CellType
    {
        NONE = 0,

        Water,
        Rock
    }

    [System.Serializable]
    public class CellData
    {
        public bool dirty = false;

        public int x;
        public int y;
        public int z;

        public CellType type;
    }

    [System.Serializable]
    public class Data
    {
        public Vector3 playerLocation;
        [SerializeField] private List<CellData> cells;

        private Dictionary<Vector3, CellData> indexLookup;

        public IReadOnlyList<CellData> Cells => cells;

        public Data()
        {
            cells = new List<CellData>();
            indexLookup = new Dictionary<Vector3, CellData>();
        }

        public void CreateCell(int x, int y, int z, CellType type)
        {
            CellData data = new CellData();
            data.dirty = false;
            data.x = x;
            data.y = y;
            data.z = z;
            data.type = type;

            cells.Add(data);
            indexLookup.Add(new Vector3(x, y, z), data);
        }

        public bool TryGetCell(Vector3 position, out CellData cell)
        {
            return indexLookup.TryGetValue(position, out cell);
        }
    }

    [System.Serializable]
    public class CellVisual : IDisposable
    {
        public GameObject visual;

        public void SyncTo(CellData data)
        {
            visual.transform.localPosition = new Vector3(data.x, data.y, data.z);
        }
        public void Dispose()
        {
            if (visual != null)
            {
                GameObject.Destroy(visual);
            }
        }
    }

    [System.Serializable]
    public class Visualizer : IDisposable
    {
        private GameObject playerVisual;
        private Dictionary<CellData, CellVisual> visuals;

        public Visualizer()
        {
             visuals = new Dictionary<CellData, CellVisual>();
        }

        public void Dispose()
        {
            foreach(CellVisual visual in visuals.Values)
            {
                visual.Dispose();
            }
            visuals.Clear();

            if(playerVisual != null)
            {
                Destroy(playerVisual);
            }
        }

        public void Visualize(Data gridData, Transform parent, GameObject visualWater, GameObject visualRock, GameObject visualPlayer)
        {
            if(playerVisual == null)
            {
                playerVisual = GameObject.Instantiate(visualPlayer, parent);
            }
            playerVisual.transform.position = gridData.playerLocation;

            for (int i = 0; i < gridData.Cells.Count; ++i)
            {
                CellData data = gridData.Cells[i];
                if (visuals.TryGetValue(data, out CellVisual visual))
                {
                    if (data.dirty)
                    {
                        visual.SyncTo(data);
                    }
                }
                else
                {
                    CellVisual newVisual = new CellVisual();
                    GameObject obj = null;
                    switch(data.type)
                    {
                        case CellType.Water:
                            obj = GameObject.Instantiate(visualWater, parent);
                            break;
                        case CellType.Rock:
                            obj = GameObject.Instantiate(visualRock, parent);
                            break;
                    }

                    newVisual.visual = obj;
                    newVisual.SyncTo(data);
                    visuals.Add(data, newVisual);
                }
            }
        }
    }

    public void Start()
    {
        data = new Data();

        if(visuals != null)
        {
            visuals.Dispose();
        }
        visuals = new Visualizer();

        System.Random rand = new System.Random(1243);
        float scaled = 10000f * (float)rand.NextDouble();
        int xSize = 30;
        int zSize = 30;
        int ySize = 10;

        for (int x = -xSize / 2; x < xSize / 2; ++x)
        {
            for (int z = -zSize / 2; z < zSize / 2; ++z)
            {
                float rockHeight = Mathf.PerlinNoise(scaled + x * zoom, scaled + z * zoom);
                rockHeight *= ySize;
                for (int y = -ySize / 2; y < ySize / 2; ++y)
                {
                    CellType cell = CellType.Rock;
                    if(y > rockHeight - ySize / 2)
                    {
                        cell = CellType.Water;
                    }

                    data.CreateCell(x, y, z, cell);
                }
            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) { data.playerLocation = EvaluateSlide(data, data.playerLocation, Vector3.forward); }
        if (Input.GetKeyDown(KeyCode.S)) { data.playerLocation = EvaluateSlide(data, data.playerLocation, Vector3.back); }
        if (Input.GetKeyDown(KeyCode.A)) { data.playerLocation = EvaluateSlide(data, data.playerLocation, Vector3.left); }
        if (Input.GetKeyDown(KeyCode.D)) { data.playerLocation = EvaluateSlide(data, data.playerLocation, Vector3.right); }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftShift)) { data.playerLocation = Evaluate(data, data.playerLocation, Vector3.down); }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) { data.playerLocation = Evaluate(data, data.playerLocation, Vector3.up); }

        visuals.Visualize(data, this.transform, visualWater, visualRock, visualPlayer);
        ClearDirty(data.Cells);
    }

    public Vector3 Evaluate(Data data, Vector3 cur, Vector3 offset)
    {
        Vector3 target = cur + offset;
        if (data.TryGetCell(target, out CellData cell))
        {
            if (cell.type == CellType.Rock)
            {
                return cur;
            }
            
            return target;
        }

        return cur;
    }

    public Vector3 EvaluateSlide(Data data, Vector3 cur, Vector3 offset)
    {
        Vector3 target = cur + offset;
        if (data.TryGetCell(target, out CellData cell))
        {
            if (cell.type == CellType.Rock)
            {
                return Evaluate(data, cur, offset + Vector3.up);
            }

            return target;
        }

        return cur; 
    }

    public void ClearDirty(IReadOnlyList<CellData> toProcess)
    {
        for (int i = 0; i < toProcess.Count; ++i)
        {
            CellData current = toProcess[i];
            current.dirty = false;
        }
    }
}
