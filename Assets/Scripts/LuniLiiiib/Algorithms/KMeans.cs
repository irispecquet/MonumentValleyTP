using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LuniLiiiib.Algorithms
{
    public static class KMeans
    {
        private static readonly System.Random RANDOM = new();
        
        /// <summary>
        /// This method is used to get the best clusters possible through repetitions and a validator. The algorithm will be called multiple times, and will return the most "optimized" clusters formation found.
        /// </summary>
        /// <param name="data">What data we want to cluster, aka positions.</param>
        /// <param name="k">The desired cluster count.</param>
        /// <param name="repetitions">Number of times the algorithm will be repeated. Not linked to maxIterations, which impacts cluster quality. Algorithm repetitions only impacts the randomness.</param>
        /// <param name="maxIterations">Max count of algorithm iteration. Each iteration improves the result, automatically stops when the algorithm doesn't see any improvement left.</param>
        /// <param name="initialDataIndexCentroids">Initial expected centroids for clusters. Must be of size k. Optional, if not given, data centroids are randomly assigned. If initial centroids are given, then the algorithm will never be random, hence repetitions should be set at 1.</param>
        /// <param name="clusterValidator">Optional, is used to manipulate clusters validity. Validation is more important than total distance in clusters.</param>
        /// <returns>The most "optimized" and valid clusters formation found.</returns>
        public static KMeansClustersInfo GetBestClusters(List<Vector2> data, int k, int repetitions = 1, int maxIterations = 50, int[] initialDataIndexCentroids = null, System.Func<KMeansClustersInfo, bool> clusterValidator = null)
        {
            KMeansClustersInfo bestClusters = null;
            bool? bestClustersValidation = null;
            
            for (int i = 0; i < repetitions; i++)
            {
                KMeansClustersInfo clusters = GetClusters(data, k, maxIterations, initialDataIndexCentroids);
                bool? clustersValidation = clusterValidator?.Invoke(clusters);
                
                if (bestClusters == null || bestClustersValidation != true || clustersValidation == true)
                {
                    if (bestClusters == null // no clusters saved yet
                        || clustersValidation == true && bestClustersValidation != true // newly found clusters are valid and current saved ones are not
                        || clustersValidation != false && clusters.TotalDistance < bestClusters.TotalDistance) // newly found clusters are valid and are more optimized than current saved ones
                    {
                        bestClusters = clusters;
                        bestClustersValidation = clustersValidation;
                    }
                }

                if (bestClusters.TotalDistance == 0) // cannot improved the best clusters
                    break;
            }
            
            return bestClusters;
        }
        
        /// <summary>
        /// Almost the same as GetBestClusters method, but only apply the algorithm once, and does not "validates" the clusters in any way.
        /// </summary>
        /// <param name="data">What data we want to cluster, aka positions.</param>
        /// <param name="k">The desired cluster count.</param>
        /// <param name="maxIterations">Max count of algorithm iteration. Each iteration improves the result, automatically stops when the algorithm doesn't see any improvement left.</param>
        /// <param name="initialDataIndexCentroids">Initial expected centroids for clusters. Must be of size k. Optional, if not given, data centroids are randomly assigned.</param>
        /// <returns>An "optimized" cluster formation (First found).</returns>
        public static KMeansClustersInfo GetClusters(List<Vector2> data, int k, int maxIterations, int[] initialDataIndexCentroids = null)
        {
            KMeansClustersInfo kMeansClustersInfo = new(data.Count, k);
            bool clustersChanged = true;

            // If initial centroids were given, assign clusters to the data regarding the given centroids 
            // Else, assign random clusters
            if (initialDataIndexCentroids != null && initialDataIndexCentroids.Length == k)
            {
                kMeansClustersInfo.DataIndexCentroids = initialDataIndexCentroids;
                AssignClusters(data, kMeansClustersInfo, k);
            }
            else
            {
                kMeansClustersInfo.ClusterIdByData = RandomlyAssignClusters(data.Count, k);
            }
            
            while (clustersChanged && ++kMeansClustersInfo.Iterations < maxIterations)
            {
                UpdateClustersInfo(data, kMeansClustersInfo, k);
                clustersChanged = AssignClusters(data, kMeansClustersInfo, k);
            }

            return kMeansClustersInfo;
        }

        private static bool AssignClusters(List<Vector2> data, KMeansClustersInfo kMeansClustersInfo, int clusterCount)
        {
            bool changed = false;
            
            for (int i = 0; i < data.Count; i++)
            {
                float smallestDistance = float.MaxValue;
                int closestClusterIndex = -1;

                // find the closest cluster
                for (int k = 0; k < clusterCount; k++)
                {
                    float distance = (data[i] - kMeansClustersInfo.Means[k]).magnitude;
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        closestClusterIndex = k;
                    }
                } 

                // Re-assign the cluster for datapoint if needed
                if (closestClusterIndex != -1 && kMeansClustersInfo.ClusterIdByData[i] != closestClusterIndex)
                {
                    changed = true;
                    kMeansClustersInfo.ClusterIdByData[i] = closestClusterIndex;
                }
            }

            // If changed is true, it means at lease one datapoint was assigned to a new cluster
            // Therefore, we might still improve the result
            // If false, it means the algorithm won't be able to find a better result than the current one
            return changed;
        }
        
        private static void UpdateClustersInfo(List<Vector2> data, KMeansClustersInfo kMeansClustersInfo, int clusterCount)
        {
            kMeansClustersInfo.ClusterSizes = new int[clusterCount]; // resetting the clusterSizes
            kMeansClustersInfo.MaxDistanceByCluster = new Vector2[clusterCount]; // resetting the maxDistances
            kMeansClustersInfo.MinDistanceByCluster = new Vector2[clusterCount].Select(x => Vector2.one * float.MaxValue).ToArray(); // resetting the minDistances
            kMeansClustersInfo.Means = new Vector2[clusterCount]; // resetting the means
            kMeansClustersInfo.TotalDistance = 0;

            // Calculate the means for each cluster
            // Sum of all the positions
            for (int i = 0; i < data.Count; i++)
            {
                Vector2 v = data[i];
                int clusterId = kMeansClustersInfo.ClusterIdByData[i];
                ++kMeansClustersInfo.ClusterSizes[clusterId];
                kMeansClustersInfo.Means[clusterId] += v;
            }

            // Get average position (mean)
            for (int k = 0; k < clusterCount; k++)
            {
                int itemCount = kMeansClustersInfo.ClusterSizes[k];
                if (itemCount > 0)
                    kMeansClustersInfo.Means[k] /= itemCount;
            }
            
            // calculate totalDistance and update dataCentroid
            for (int i = 0; i < data.Count; i++)
            {
                int clusterId = kMeansClustersInfo.ClusterIdByData[i]; 
                Vector2 distance = data[i] - kMeansClustersInfo.Means[clusterId];
                kMeansClustersInfo.TotalDistance += distance.magnitude;
                
                // update dataCentroid if the current data is closer to the mean than the previous one
                if (distance.magnitude < kMeansClustersInfo.MinDistanceByCluster[clusterId].magnitude)
                {
                    kMeansClustersInfo.MinDistanceByCluster[clusterId] = distance;
                    kMeansClustersInfo.DataIndexCentroids[clusterId] = i;
                }

                if (Mathf.Abs(distance.x) > kMeansClustersInfo.MaxDistanceByCluster[clusterId].x)
                    kMeansClustersInfo.MaxDistanceByCluster[clusterId].x = Mathf.Abs(distance.x);
                if (Mathf.Abs(distance.y) > kMeansClustersInfo.MaxDistanceByCluster[clusterId].y)
                    kMeansClustersInfo.MaxDistanceByCluster[clusterId].y = Mathf.Abs(distance.y);
            }
        }
        
        private static int[] RandomlyAssignClusters(int dataCount, int clusterCount)
        {
            int[] clusterIdByData = new int[dataCount];

            for (int i = 0; i < dataCount; ++i)
                clusterIdByData[i] = RANDOM.Next(0, clusterCount);

            return clusterIdByData;
        }
    }

    public record KMeansClustersInfo
    {
        public KMeansClustersInfo(int dataCount, int clusterCount)
        {
            this.ClusterIdByData = new int[dataCount];
            this.ClusterSizes = new int[clusterCount];
            this.DataIndexCentroids = new int[clusterCount];
        }

        /// <summary>
        /// Cluster index assigned to each data.
        /// </summary>
        public int[] ClusterIdByData;

        /// <summary>
        /// Data count for each cluster.
        /// </summary>
        public int[] ClusterSizes;
        
        /// <summary>
        /// For each cluster, contains the index of the data element which is closest of being the center.
        /// </summary>
        public int[] DataIndexCentroids;
        
        /// <summary>
        /// Contains the max distance for each cluster.
        /// </summary>
        public Vector2[] MaxDistanceByCluster;
        
        /// <summary>
        /// Contains the min distance for each cluster.
        /// </summary>
        public Vector2[] MinDistanceByCluster;

        /// <summary>
        /// Number of iterations applied to this clusters formation. Helps to know if it could be improved or not.
        /// </summary>
        public int Iterations = -1;
        
        /// <summary>
        /// Mean for each cluster.
        /// </summary>
        public Vector2[] Means;

        /// <summary>
        /// Total distance between each data and their cluster means.
        /// </summary>
        public float TotalDistance;
    }
}