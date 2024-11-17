using System.Collections.Generic;
using System.Linq;
using LuniLiiiib.Algorithms.Pathfinding.Dijkstra;
using UnityEngine;

namespace Gameplay
{
    public class PathFinder
    {
        public List<Tile> GetPath(Tile currentTile, Tile clickedTile)
        {
            List<Tile> path = Dijkstra.GetClosestNode(currentTile, 
                GetDistanceBetweenTiles,
                (currentNode) => GetNeighbours(currentNode.Node),
                (currentNode) => currentNode.Node == clickedTile);

            return path;
        }

        private Tile[] GetNeighbours(Tile currentNode)
        {
            return currentNode.Neighbours.Values.Where(neighbor => neighbor != null).ToArray();
        }

        private float GetDistanceBetweenTiles(Tile origin, Tile destination)
        {
            return Vector3.Distance(origin.transform.position, destination.transform.position);
        }
    }
}