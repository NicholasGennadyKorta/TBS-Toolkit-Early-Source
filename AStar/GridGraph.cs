using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGraph : MonoBehaviour
{
    public static GridGraph instance;
    public PathNode[,] pathNodes;
    public float nodeSize;
    public int width, depth;
    public int gridType = 1;

    public Texture normalTexture;
    public Texture walkableTexture;
    public Texture notWalkableTexture;

    public void Awake()
    {
        instance = this;
    }

    public void Intialize()
    {
        pathNodes = new PathNode[width, depth];

        // Create a grid depending on what type of grid type it is
        if (gridType == 0)
            IntalizeSquareGrid();
        else
            IntalizeHexGrid();
        
    }

    void IntalizeSquareGrid()
    {
        //Create our Grid Square Grid
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                PathNode pathNode = new PathNode();
                pathNode.gridNodeIndex = new Vector2(x, z);
                pathNode.position = new Vector3(transform.position.x + x * nodeSize, transform.position.y, transform.position.z + z * nodeSize);
                pathNode.isWalkable = true;

                pathNode.cell = GameObject.CreatePrimitive(PrimitiveType.Plane);
                pathNode.cell.transform.name = "cell";
                pathNode.cell.transform.parent = transform;
                pathNode.cell.transform.position = pathNode.position;
                pathNode.cell.transform.localScale = new Vector3(nodeSize * 0.1f, 1, nodeSize * 0.1f);

                pathNode.cell.renderer.enabled = false;
                pathNode.cell.renderer.castShadows = false;
                pathNode.cell.renderer.receiveShadows = false;
                pathNode.cell.renderer.material.shader = Shader.Find("Unlit/Transparent");
                Destroy(pathNode.cell.collider);

                RaycastHit hit;
                Vector3 rayShootPosition = pathNode.position + new Vector3(0, 1000, 0);

                if (Physics.Raycast(rayShootPosition, Vector3.down, out hit, 1000))
                    pathNode.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                //THIS LAYS OUT THE CELLS OVER THE TERRAIN 
                Mesh mesh = ((MeshFilter)pathNode.cell.GetComponent(typeof(MeshFilter))).mesh as Mesh;
                Vector3[] vertices = mesh.vertices;
                Vector3 position = new Vector3(pathNode.cell.transform.position.x + (pathNode.cell.transform.localScale.x * 10 / 2), 1000, pathNode.cell.transform.position.z + (pathNode.cell.transform.localScale.z * 10 / 2));
                float xStep = pathNode.cell.transform.localScale.x;
                float zStep = pathNode.cell.transform.localScale.z;
                int squaresize = 10 + 1;
                for (int n = 0; n < squaresize; n++)
                {
                    for (int i = 0; i < squaresize; i++)
                    {
                        if (Physics.Raycast(position, -Vector3.up, out hit, 1000.0F))
                        {
                            vertices[(n * squaresize) + i].y = Terrain.activeTerrain.SampleHeight(position);
                            position.x -= xStep;
                        }
                    }
                    position.x += (((float)squaresize) * xStep);
                    position.z -= zStep;
                }

                mesh.vertices = vertices;
                mesh.RecalculateBounds();
                pathNode.cell.transform.position += new Vector3(0, 0.3f, 0);

                pathNodes[x, z] = pathNode;
            }
        }

        walkableTexture = Resources.Load<Texture>("TRPG/BattleGrid/Texture_GridSquareWalkable");
        notWalkableTexture = Resources.Load<Texture>("TRPG/BattleGrid/Texture_GridSquareNotWalkable");
    }

    void IntalizeHexGrid()
    {
        //Create our Grid Hex Grid

        normalTexture = Resources.Load<Texture>("TRPG/BattleGrid/Texture_GridHexNormal");
        walkableTexture = Resources.Load<Texture>("TRPG/BattleGrid/Texture_GridHexWalkable");
        notWalkableTexture = Resources.Load<Texture>("TRPG/BattleGrid/Texture_GridHexNotWalkable");

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                PathNode pathNode = new PathNode();
                pathNode.gridNodeIndex = new Vector2(x, z);
                pathNode.isWalkable = true;

                Vector2 gridPos = new Vector2(x, z);
                float offset = 0;
                if (z % 2 != 0)
                    offset = nodeSize * 0.5f;
                pathNode.position = new Vector3(offset + gridPos.x * nodeSize, 0, gridPos.y * nodeSize);

                pathNode.cell = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Plane);

                pathNode.cell.transform.name = "cell";
                pathNode.cell.transform.position = pathNode.position;
                pathNode.cell.transform.localScale = new Vector3(nodeSize * 0.1175f, 1, nodeSize * 0.135f);
                pathNode.cell.transform.parent = transform;


                pathNode.cell.renderer.enabled = false;
                pathNode.cell.renderer.castShadows = false;
                pathNode.cell.renderer.receiveShadows = false;
                pathNode.cell.renderer.material.shader = Shader.Find("Unlit/Transparent");
                pathNode.cell.renderer.material.SetTexture(0, normalTexture);

                RaycastHit hit;
                Vector3 rayShootPosition = pathNode.position + new Vector3(0, 1000, 0);

                if (Physics.Raycast(rayShootPosition, Vector3.down, out hit, 1000))
                    pathNode.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);



                //THIS LAYS OUT THE CELLS OVER THE TERRAIN 
                Mesh mesh = ((MeshFilter)pathNode.cell.GetComponent(typeof(MeshFilter))).mesh as Mesh;
                Vector3[] vertices = mesh.vertices;
                Vector3 position = new Vector3(pathNode.cell.transform.position.x + (pathNode.cell.transform.localScale.x * 10 / 2), 1000, pathNode.cell.transform.position.z + (pathNode.cell.transform.localScale.z * 10 / 2));
                float xStep = pathNode.cell.transform.localScale.x;
                float zStep = pathNode.cell.transform.localScale.z;
                int squaresize = 10 + 1;
                for (int n = 0; n < squaresize; n++)
                {
                    for (int i = 0; i < squaresize; i++)
                    {
                        if (Physics.Raycast(position, -Vector3.up, out hit, 1000.0F))
                        {
                            vertices[(n * squaresize) + i].y = Terrain.activeTerrain.SampleHeight(position);
                            position.x -= xStep;
                        }
                    }
                    position.x += (((float)squaresize) * xStep);
                    position.z -= zStep;
                }
                
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
                pathNode.cell.transform.position += new Vector3(0, 0.3f, 0);
                
                pathNodes[x, z] = pathNode;
            }
        }
    }

    public void ClearFloodFill()
    {
        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
                pathNodes[x, z].cell.renderer.enabled = false;
                //pathNodes[x,z].cell.renderer.material.SetTexture(0, normalTexture);
    }

    public bool CellWalkable(Vector2 position)
    {
        return pathNodes[(int)position.x,(int)position.y].isWalkable;
    }

    public Vector3[] GetCellVertices(Vector2 gridPosition)
    {
        return ((MeshFilter)pathNodes[(int)gridPosition.x, (int)gridPosition.y].cell.GetComponent(typeof(MeshFilter))).mesh.vertices;
    }

    public void Scan()
    {
        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
            {
                pathNodes[x, z].isWalkable = true;
                pathNodes[x, z].isAttackable = false;
            }

            for (int i = 0; i < ObjectPool.units.Count;++i)
                if (GameLoop.unitSelected.playerNumb != ObjectPool.units[i].playerNumb)
                    pathNodes[(int)ObjectPool.units[i].GetComponent<Mover>().gridPosition.x, (int)ObjectPool.units[i].GetComponent<Mover>().gridPosition.y].isWalkable = false;
                else
                    pathNodes[(int)ObjectPool.units[i].GetComponent<Mover>().gridPosition.x, (int)ObjectPool.units[i].GetComponent<Mover>().gridPosition.y].isWalkable = true;
    }
}