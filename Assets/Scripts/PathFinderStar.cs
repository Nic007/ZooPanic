using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PriorityQueueDemo;
using UnityEngine;

namespace Assets.Scripts
{
    public class PathFinderStar
    {
        public TileIndex[] FindShortestPath(GameObject startNode, GameObject endNode)
        {
            var nodesToVisit = new PriorityQueue<int, GameObject>();
            var nodesToVisitSet = new HashSet<GameObject>();

            var visitedNodes = new HashSet<GameObject>();
            var nodesOrigin = new Dictionary<GameObject, GameObject>();

            var visitedCost = new Dictionary<GameObject, int>();
            visitedCost[startNode] = 0;

            nodesToVisit.Add(
                new KeyValuePair<int, GameObject>(
                    visitedCost[startNode] + heuristic(startNode.GetComponent<BasicTileComponent>().CurrentLocation,
                        endNode.GetComponent<BasicTileComponent>().CurrentLocation), startNode));
            nodesToVisitSet.Add(startNode);

            while (!nodesToVisit.IsEmpty)
            {
                var currentNode = nodesToVisit.Dequeue().Value;
                if (currentNode == endNode)
                {
                    var path = new LinkedList<TileIndex>();
                    path.AddFirst(currentNode.GetComponent<BasicTileComponent>().CurrentLocation);

                    while (currentNode != startNode && nodesOrigin.ContainsKey(currentNode))
                    {
                        currentNode = nodesOrigin[currentNode];
                        path.AddFirst(currentNode.GetComponent<BasicTileComponent>().CurrentLocation);
                    }

                    return path.ToArray();
                }

                nodesToVisitSet.Remove(currentNode);
                visitedNodes.Add(currentNode);

                if (!visitedCost.ContainsKey(currentNode))
                {
                    visitedCost[currentNode] = int.MaxValue;
                }

                var tileComponent = currentNode.GetComponent<BasicTileComponent>();
                for (var i = 0; i < tileComponent.NeighborsObjects.Length; ++i)
                {
                    var neighbor = tileComponent.NeighborsObjects[i];
                    if (neighbor == null || /*tileComponent.NeighborsState[i] != BasicTileComponent.PathState.Available ||*/ visitedNodes.Contains(neighbor))
                    {
                        continue;
                    }

                    if (!visitedCost.ContainsKey(neighbor))
                    {
                        visitedCost[neighbor] = int.MaxValue;
                    }

                    var possibleVisitedCost = visitedCost[currentNode] + 1;
                    if (!nodesToVisitSet.Contains(neighbor) || possibleVisitedCost < visitedCost[neighbor])
                    {
                        nodesOrigin[neighbor] = currentNode;
                        visitedCost[neighbor] = possibleVisitedCost;
                        var estimatedCost = visitedCost[neighbor] +
                                                   heuristic(
                                                       neighbor.GetComponent<BasicTileComponent>().CurrentLocation,
                                                       endNode.GetComponent<BasicTileComponent>().CurrentLocation);

                        if (!nodesToVisitSet.Contains(neighbor))
                        {
                            nodesToVisit.Add(new KeyValuePair<int, GameObject>(estimatedCost, neighbor));
                            nodesToVisitSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private int heuristic(TileIndex currentNode, TileIndex endNode)
        {
            // Distance de Mannathan
            return Math.Abs(currentNode.x - endNode.x) + Math.Abs(currentNode.y - endNode.y);
        }

        public int Cost()
        {
            return 0;
        }
    }
}
