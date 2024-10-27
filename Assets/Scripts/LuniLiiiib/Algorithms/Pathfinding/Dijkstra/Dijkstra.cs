﻿using System;
using System.Collections.Generic;

namespace LuniLiiiib.Algorithms.Pathfinding.Dijkstra
{
    public static class Dijkstra
    {
        public static List<TNode> GetClosestNode<TNode>(TNode originNode,
                                                        Func<TNode, TNode, float> getDistance,
                                                        Func<DijkstraNode<TNode>, TNode[]> getNeighbours,
                                                        Func<DijkstraNode<TNode>, bool> condition,
                                                        float maxDistance = float.MaxValue)
        {
            HashSet<DijkstraNode<TNode>> openList = new() {new DijkstraNode<TNode>(originNode, null, 0f)};
            HashSet<TNode> closedList = new() {originNode};

            DijkstraNode<TNode> currentNode = null;

            while (openList.Count > 0)
            {
                // Get shortest distance node
                float minDistance = float.MaxValue;
                
                foreach (DijkstraNode<TNode> node in openList)
                {
                    if (node.DistanceFromStart < minDistance)
                    {
                        minDistance = node.DistanceFromStart;
                        currentNode = node;
                    }
                }

                // Check condition
                if (condition(currentNode))
                    return ComputePath(currentNode);

                openList.Remove(currentNode);
                
                // Add neighbours
                TNode[] neighbours = getNeighbours(currentNode);
                foreach (TNode neighbour in neighbours)
                {
                    if (closedList.Add(neighbour))
                    {
                        float distance = getDistance(currentNode!.Node, neighbour) + currentNode.DistanceFromStart;
                        if (distance < maxDistance)
                            openList.Add(new DijkstraNode<TNode>(neighbour, currentNode, distance));
                    }
                }
            }

            return new List<TNode>();
        }

        private static List<TNode> ComputePath<TNode>(DijkstraNode<TNode> currentNode)
        {
            List<TNode> path = new();
            while (currentNode != null)
            {
                path.Add(currentNode.Node);
                currentNode = currentNode.Parent;
            }
            
            path.Reverse();
            return path;
        }
    }
}