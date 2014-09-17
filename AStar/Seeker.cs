using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seeker : MonoBehaviour
{

    PathNode startNode, endNode;
    List<Expansion> open = new List<Expansion>();
    HashSet<PathNode> closed = new HashSet<PathNode>();
    Expansion startingExpansion;
    Expansion best;
    private int distance, currentDistance;

    public bool[,] cellVisited;

    #region Pathfinding
    public Path CalculatePath(Vector2 startGridPosition, Vector2 endGridPosition)
    {
        PathNode startNode = GridGraph.instance.pathNodes[(int)startGridPosition.x, (int)startGridPosition.y];
        PathNode endNode = GridGraph.instance.pathNodes[(int)endGridPosition.x, (int)endGridPosition.y];

        cellVisited = new bool[GridGraph.instance.width, GridGraph.instance.depth];
        open = new List<Expansion>();
        closed = new HashSet<PathNode>();

        startingExpansion = new Expansion(null, startNode, 0f, 0f);
        open.Add(startingExpansion);
        best = null;

        for (;;)
        {
            if (open.Count == 0 || best != null && best.current == endNode)
            {
                Path foundPath = new Path();
                foundPath.pathNodes = new List<PathNode>();
                Expansion iterator = best;
                while (iterator != null)
                {
                    foundPath.pathNodes.Add(iterator.current);
                    iterator = iterator.previous;
                }

                foundPath.pathNodes.Reverse();

                return foundPath;
            }

            best = null;
            foreach (Expansion expansion in open)
            {
                if (best == null || expansion.ToltalCost < best.ToltalCost)
                    best = expansion;
            }

            ExpandPath();
        }
    }

    private void ExpandPath()
    {
        closed.Add(best.current);
        open.Remove(best);

        if (GridGraph.instance.gridType == 0)
        {
            PathNode pnTop = best.current;
            pnTop.gridNodeIndex.y -= 1;
            if (inBounds(pnTop))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnTop.gridNodeIndex.x, (int)pnTop.gridNodeIndex.y]);

            PathNode pnBottom = best.current;
            pnBottom.gridNodeIndex.y += 1;
            if (inBounds(pnBottom))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnBottom.gridNodeIndex.x, (int)pnBottom.gridNodeIndex.y]);

            PathNode pnLeft = best.current;
            pnLeft.gridNodeIndex.x -= 1;
            if (inBounds(pnLeft))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnLeft.gridNodeIndex.x, (int)pnLeft.gridNodeIndex.y]);

            PathNode pnRight = best.current;
            pnRight.gridNodeIndex.x += 1;
            if (inBounds(pnRight))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnRight.gridNodeIndex.x, (int)pnRight.gridNodeIndex.y]);
        }
        else
        {
            PathNode pnNorth = best.current;
            pnNorth.gridNodeIndex.x -= 1;
            if (inBounds(pnNorth))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnNorth.gridNodeIndex.x, (int)pnNorth.gridNodeIndex.y]);

            PathNode pnSouth = best.current;
            pnSouth.gridNodeIndex.x += 1;
            if (inBounds(pnSouth))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnSouth.gridNodeIndex.x, (int)pnSouth.gridNodeIndex.y]);

            PathNode pnNorthEast = best.current;
            pnNorthEast.gridNodeIndex.x += 1;
            pnNorthEast.gridNodeIndex.y -= 1;
            if (pnNorthEast.gridNodeIndex.y % 2 != 0)
                pnNorthEast.gridNodeIndex.x -= 1;
            if (inBounds(pnNorthEast))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnNorthEast.gridNodeIndex.x, (int)pnNorthEast.gridNodeIndex.y]);


            PathNode pnNorthWest = best.current;
            pnNorthWest.gridNodeIndex.x -= 1;
            pnNorthWest.gridNodeIndex.y -= 1;
            if (pnNorthWest.gridNodeIndex.y % 2 == 0)
                pnNorthWest.gridNodeIndex.x += 1;
            if (inBounds(pnNorthWest))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnNorthWest.gridNodeIndex.x, (int)pnNorthWest.gridNodeIndex.y]);

            PathNode pnSouthWest = best.current;
            pnSouthWest.gridNodeIndex.x -= 1;
            pnSouthWest.gridNodeIndex.y += 1;
            if (pnSouthWest.gridNodeIndex.y % 2 == 0)
                pnSouthWest.gridNodeIndex.x += 1;
            if (inBounds(pnSouthWest))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnSouthWest.gridNodeIndex.x, (int)pnSouthWest.gridNodeIndex.y]);

            PathNode pnSouthEast = best.current;
            pnSouthEast.gridNodeIndex.x += 1;
            pnSouthEast.gridNodeIndex.y += 1;
            if (pnSouthEast.gridNodeIndex.y % 2 != 0)
                pnSouthEast.gridNodeIndex.x -= 1;
            if (inBounds(pnSouthEast))
                AddToExpansionPath(GridGraph.instance.pathNodes[(int)pnSouthEast.gridNodeIndex.x, (int)pnSouthEast.gridNodeIndex.y]);
        }
    }

    private void AddToExpansionPath(PathNode pathNode)
    {
        if (!cellVisited[(int)pathNode.gridNodeIndex.x, (int)pathNode.gridNodeIndex.y] && pathNode.isWalkable)
        {
            cellVisited[(int)pathNode.gridNodeIndex.x, (int)pathNode.gridNodeIndex.y] = true;
            float costToGoal = Vector2.Distance(pathNode.gridNodeIndex, endNode.gridNodeIndex);
            float costToNextNode = Vector2.Distance(pathNode.gridNodeIndex, pathNode.gridNodeIndex);
            float costFromStart = best.costFromStart;
            Expansion expansion = new Expansion(best, pathNode, costFromStart + costToNextNode, costToGoal);
            open.Add(expansion);
        }
    }
    #endregion
    #region FloodFills

    public void GenerateMovementGrid(Vector2 pathNode, int distance)
    {
        GridGraph.instance.Scan();
        GenerateGrid(pathNode, distance,0, false);

        //Make All Path Nodes not in flood fill unwalkable and renderer off
        for (int x = 0; x < GridGraph.instance.width; x++)
            for (int z = 0; z < GridGraph.instance.depth; z++)
                if (!closed.Contains(GridGraph.instance.pathNodes[x, z]))
                    GridGraph.instance.pathNodes[x, z].isWalkable =  false;
                else
                {
                    GridGraph.instance.pathNodes[x, z].cell.renderer.enabled = true;
                    GridGraph.instance.pathNodes[x, z].cell.renderer.material.SetTexture(0, GridGraph.instance.walkableTexture);
                }

        //Make shure that we can walk were standing, not walk
        for (int i = 0; i < ObjectPool.units.Count; ++i)
        {
            Vector2 unitGridPosition = ObjectPool.units[i].GetComponent<Mover>().gridPosition;
            if (GameLoop.unitSelected.playerNumb != ObjectPool.units[i].playerNumb && closed.Contains(GridGraph.instance.pathNodes[(int)unitGridPosition.x, (int)unitGridPosition.y]))
                GridGraph.instance.pathNodes[(int)unitGridPosition.x,(int)unitGridPosition.y].cell.renderer.material.SetTexture(0, GridGraph.instance.notWalkableTexture);
        }
    }

    public void GenerateAttackGrid(Vector2 pathNode, int distance, bool expandDiagonal)
    {
        GenerateGrid(pathNode, distance, 1, expandDiagonal);

        //Make All Path Nodes not in flood fill unwalkable and renderer off
        for (int x = 0; x < GridGraph.instance.width; x++)
            for (int z = 0; z < GridGraph.instance.depth; z++)
                GridGraph.instance.pathNodes[x, z].cell.renderer.enabled = false;

        //Get Attack Tiles
        for (int i = 0; i < ObjectPool.units.Count; ++i)
        {
            Vector2 unitGridPosition = ObjectPool.units[i].GetComponent<Mover>().gridPosition;
            if (GameLoop.unitSelected.playerNumb != ObjectPool.units[i].playerNumb && closed.Contains(GridGraph.instance.pathNodes[(int)unitGridPosition.x, (int)unitGridPosition.y]))
            {
                GridGraph.instance.pathNodes[(int)unitGridPosition.x, (int)unitGridPosition.y].cell.renderer.enabled = true;
                GridGraph.instance.pathNodes[(int)unitGridPosition.x, (int)unitGridPosition.y].isAttackable = true;
                GridGraph.instance.pathNodes[(int)unitGridPosition.x, (int)unitGridPosition.y].cell.renderer.material.SetTexture(0, GridGraph.instance.notWalkableTexture);
            }
        }

    }

    private void GenerateGrid(Vector2 pathNode, int distance, int type, bool expandDiagonal)
    {
        
        this.distance = distance;
        cellVisited = new bool[GridGraph.instance.width, GridGraph.instance.depth];
        currentDistance = 0;
        open.Clear();
        closed.Clear();
        PathNode startNode = GridGraph.instance.pathNodes[(int)pathNode.x, (int)pathNode.y];
        Expansion startingExpansion = new Expansion(startNode, currentDistance);
        open.Add(startingExpansion);

        // Type is 0 so we want movement grid
        if (type == 0)
        {
            while (open.Count > 0)
                ExpandFloodFill(open[0]);

        }

        //Type is 1 so we want attack grid
        else if (GridGraph.instance.gridType == 0)
        {
            while (open.Count > 0)
                ExpandFloodFillSingleDirection(open[0], 0);

            open.Add(startingExpansion);
            while (open.Count > 0)
                ExpandFloodFillSingleDirection(open[0], 1);

            currentDistance = 0;
            open.Add(startingExpansion);
            while (open.Count > 0)
                ExpandFloodFillSingleDirection(open[0], 2);

            currentDistance = 0;
            open.Add(startingExpansion);
            while (open.Count > 0)
                ExpandFloodFillSingleDirection(open[0], 3);

            if (expandDiagonal)
            {
                currentDistance = 0;
                open.Add(startingExpansion);
                while (open.Count > 0)
                    ExpandFloodFillSingleDirection(open[0], 4);

                currentDistance = 0;
                open.Add(startingExpansion);
                while (open.Count > 0)
                    ExpandFloodFillSingleDirection(open[0], 5);

                currentDistance = 0;
                open.Add(startingExpansion);
                while (open.Count > 0)
                    ExpandFloodFillSingleDirection(open[0], 6);

                currentDistance = 0;
                open.Add(startingExpansion);
                while (open.Count > 0)
                    ExpandFloodFillSingleDirection(open[0], 7);
            }
        }
        else
        {
            while (true)
            {
                if (open.Count > 0)
                    ExpandFloodFill(open[0]);
                else
                    break;
            }
        }
    }

    private void ExpandFloodFill(Expansion current)
    {
        open.Remove(current);
        closed.Add(current.current);
        currentDistance = current.distance + 1;
        cellVisited[(int)current.current.gridNodeIndex.x, (int)current.current.gridNodeIndex.y] = true;


        if (GridGraph.instance.gridType == 0)
        {
            PathNode pnTop = current.current;
            pnTop.gridNodeIndex.y -= 1;
            if (inBounds(pnTop))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnTop.gridNodeIndex.x, (int)pnTop.gridNodeIndex.y], currentDistance);

            PathNode pnBottom = current.current;
            pnBottom.gridNodeIndex.y += 1;
            if (inBounds(pnBottom))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnBottom.gridNodeIndex.x, (int)pnBottom.gridNodeIndex.y], currentDistance);

            PathNode pnLeft = current.current;
            pnLeft.gridNodeIndex.x -= 1;
            if (inBounds(pnLeft))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnLeft.gridNodeIndex.x, (int)pnLeft.gridNodeIndex.y], currentDistance);

            PathNode pnRight = current.current;
            pnRight.gridNodeIndex.x += 1;
            if (inBounds(pnRight))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnRight.gridNodeIndex.x, (int)pnRight.gridNodeIndex.y], currentDistance);
        }
        else
        {
            PathNode pnNorth = current.current;
            pnNorth.gridNodeIndex.x -= 1;
            if (inBounds(pnNorth))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnNorth.gridNodeIndex.x, (int)pnNorth.gridNodeIndex.y], currentDistance);

            PathNode pnSouth = current.current;
            pnSouth.gridNodeIndex.x += 1;
            if (inBounds(pnSouth))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnSouth.gridNodeIndex.x, (int)pnSouth.gridNodeIndex.y], currentDistance);

            PathNode pnNorthEast = current.current;
            pnNorthEast.gridNodeIndex.x += 1;
            pnNorthEast.gridNodeIndex.y -= 1;
            if (pnNorthEast.gridNodeIndex.y % 2 != 0)
                pnNorthEast.gridNodeIndex.x -= 1;
            if (inBounds(pnNorthEast))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnNorthEast.gridNodeIndex.x, (int)pnNorthEast.gridNodeIndex.y], currentDistance);


            PathNode pnNorthWest = current.current;
            pnNorthWest.gridNodeIndex.x -= 1;
            pnNorthWest.gridNodeIndex.y -= 1;
            if (pnNorthWest.gridNodeIndex.y % 2 == 0)
                pnNorthWest.gridNodeIndex.x += 1;
            if (inBounds(pnNorthWest))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnNorthWest.gridNodeIndex.x, (int)pnNorthWest.gridNodeIndex.y], currentDistance);

            PathNode pnSouthWest = current.current;
            pnSouthWest.gridNodeIndex.x -= 1;
            pnSouthWest.gridNodeIndex.y += 1;
            if (pnSouthWest.gridNodeIndex.y % 2 == 0)
                pnSouthWest.gridNodeIndex.x += 1;
            if (inBounds(pnSouthWest))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnSouthWest.gridNodeIndex.x, (int)pnSouthWest.gridNodeIndex.y], currentDistance);

            PathNode pnSouthEast = current.current;
            pnSouthEast.gridNodeIndex.x += 1;
            pnSouthEast.gridNodeIndex.y += 1;
            if (pnSouthEast.gridNodeIndex.y % 2 != 0)
                pnSouthEast.gridNodeIndex.x -= 1;
            if (inBounds(pnSouthEast))
                AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pnSouthEast.gridNodeIndex.x, (int)pnSouthEast.gridNodeIndex.y], currentDistance);

        }
    }

    private void ExpandFloodFillSingleDirection(Expansion current, int direction)
    {
        open.Remove(current);
        closed.Add(current.current);
        currentDistance = current.distance + 1;
        
        PathNode pn = current.current;
        int addDistance = 0;

        switch (direction)
        {
            case 0: pn.gridNodeIndex.y -= 1; break;
            case 1: pn.gridNodeIndex.y += 1; break;
            case 2: pn.gridNodeIndex.x -= 1; break;
            case 3: pn.gridNodeIndex.x += 1; break;
            case 4: pn.gridNodeIndex.y += 1; pn.gridNodeIndex.x += 1; addDistance++; break;
            case 5: pn.gridNodeIndex.y -= 1; pn.gridNodeIndex.x -= 1; addDistance++; break;
            case 6: pn.gridNodeIndex.y += 1; pn.gridNodeIndex.x -= 1; addDistance++; break;
            case 7: pn.gridNodeIndex.y -= 1; pn.gridNodeIndex.x += 1; addDistance++; break;
        }

        if (inBounds(pn))
            AddToExpansionFloodFill(GridGraph.instance.pathNodes[(int)pn.gridNodeIndex.x, (int)pn.gridNodeIndex.y], currentDistance + addDistance);   
    }

    private void AddToExpansionFloodFill(PathNode pathNode, int dis)
    {
        if (!cellVisited[(int)pathNode.gridNodeIndex.x, (int)pathNode.gridNodeIndex.y] && dis < distance + 1)
        {
            cellVisited[(int)pathNode.gridNodeIndex.x, (int)pathNode.gridNodeIndex.y] = true;
            if (pathNode.isWalkable)
            {
                open.Add(new Expansion(pathNode, dis));
                GridGraph.instance.pathNodes[(int)pathNode.gridNodeIndex.x, (int)pathNode.gridNodeIndex.y].isWalkable = true;
            }
            else
                closed.Add(pathNode);
        }
    }

    #endregion

    private bool inBounds(PathNode pathNode)
    {
        return pathNode.gridNodeIndex.x >= 0 && pathNode.gridNodeIndex.y >= 0 && pathNode.gridNodeIndex.x < GridGraph.instance.width && pathNode.gridNodeIndex.y < GridGraph.instance.depth;
    }

    private class Expansion
    {
        public PathNode current;
        public Expansion previous;
        public float costFromStart;
        public float estimatedCostToGoal;
        public int distance;

        public Expansion(Expansion previous, PathNode current, float costFromStart, float costToGoal)
        {
            this.previous = previous;
            this.current = current;
            this.costFromStart = costFromStart;
            this.estimatedCostToGoal = costToGoal;
        }

        public Expansion(PathNode pathNode, int distance)
        {
            current = pathNode;
            this.distance = distance;
        }

        public float ToltalCost
        {
            get { return costFromStart + estimatedCostToGoal; }
        }
    }
}
