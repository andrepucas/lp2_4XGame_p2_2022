// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE_CODE or copy
// at http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
using System.IO;

namespace ImportedGenerator
{
    public class Generator
    {
        private const int maxBitsForTerrain = 4; // i.e. max of 16 terrain types
        private const int bitsInByte = 8;
        private const double probResource = 0.5; // Increased original value = 0.3
        private const double centerPointsDensity = 0.1;
        private const int centerPointsRatio = 10;
        private readonly int terrainMask;
        private readonly Random random;
        private readonly IDictionary<int, string> terrains;
        private readonly IDictionary<int, string> resources;
        private readonly IList<int> resourceList;

        public Generator(IEnumerable<string> terrainCollection,
            IEnumerable<string> resourceCollection)
        {
            int terrainCount = 0;
            int resourceBit = maxBitsForTerrain + 1;

            random = new Random();

            terrainMask =
                (int)(0xFFFFFFFF >> (sizeof(int) * bitsInByte - maxBitsForTerrain));

            terrains = new Dictionary<int, string>();
            resources = new Dictionary<int, string>();

            foreach (string terrain in terrainCollection)
            {
                terrains.Add(terrainCount, terrain);
                terrainCount++;
            }

            foreach (string resource in resourceCollection)
            {
                resources.Add(1 << resourceBit, resource);
                resourceBit++;
            }

            resourceList = new List<int>(resources.Keys);
        }

        private void AddRandomResources(ref int tile)
        {
            int resourcesInTile = 0;
            resourceList.Shuffle();

            while (random.NextDouble() < probResource
                && resourcesInTile < resources.Count)
            {
                tile |= resourceList[resourcesInTile];
                resourcesInTile++;
            }
        }

        public Map CreateRandomMap(int rows, int cols)
        {
            Map map = new Map(rows, cols);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int tile = random.Next(terrains.Count);

                    AddRandomResources(ref tile);

                    map[r, c] = tile;
                }
            }

            return map;
        }

        public Map CreatePCGMap(int rows, int cols)
        {
            double Distance((int r, int c) a, (int r, int c) b)
            {
                int dr = Math.Abs(a.r - b.r);
                int dc = Math.Abs(a.c - b.c);

                if (dr > rows / 2.0)
                    dr = rows - dr;
                if (dc > cols / 2.0)
                    dc = cols - dc;

                return Math.Sqrt(dr * dr + dc * dc);
            }

            Map map = new Map(rows, cols);

            int totalTiles = rows * cols;

            if (totalTiles < 10) return CreateRandomMap(rows, cols);

            int numCenterPoints = (totalTiles > 50)
                ? (int)(totalTiles * centerPointsDensity) 
                : (int)(totalTiles * centerPointsDensity * (100/totalTiles));

            IList<(int, int)> centerPoints;

            ISet<(int, int)> unvisitedTiles = new HashSet<(int, int)>();

            IDictionary<(int, int), int> visitedTiles =
                new Dictionary<(int, int), int>();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    unvisitedTiles.Add((i, j));

            while (visitedTiles.Count < numCenterPoints)
            {
                (int, int) tilePos = (random.Next(rows), random.Next(cols));
                if (unvisitedTiles.Contains(tilePos))
                {
                    unvisitedTiles.Remove(tilePos);
                    visitedTiles.Add(tilePos, random.Next(terrains.Count));
                }
            }

            centerPoints = new List<(int, int)>(visitedTiles.Keys);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (unvisitedTiles.Contains((i, j)))
                    {
                        double minDist = double.PositiveInfinity;
                        int closestCenterTile = int.MaxValue;

                        foreach ((int r, int c) point in centerPoints)
                        {
                            double dist = Distance((i, j), point);

                            if (dist < minDist)
                            {
                                minDist = dist;
                                closestCenterTile = visitedTiles[point];
                            }
                        }

                        unvisitedTiles.Remove((i, j));
                        visitedTiles.Add((i, j), closestCenterTile);
                    }
                }
            }

            foreach (KeyValuePair<(int r, int c), int> posTile in visitedTiles)
            {
                int tile = posTile.Value;
                AddRandomResources(ref tile);
                map[posTile.Key.r, posTile.Key.c] = tile;
            }

            return map;
        }

        public void SaveMap(Map map, string file)
        {
            ICollection<string> resourcesInTile = new List<string>();

            using (TextWriter sw = new StreamWriter(file))
            {
                sw.WriteLine($"{map.Rows} {map.Cols}");
                for (int r = 0; r < map.Rows; r++)
                {
                    for (int c = 0; c < map.Cols; c++)
                    {
                        int tile = map[r, c];

                        string terrain = terrains[tile & terrainMask];

                        sw.Write(terrain);

                        resourcesInTile.Clear();

                        for (int i = 0; i < resources.Count; i++)
                        {
                            int resourceBit = maxBitsForTerrain + 1 + i;
                            int resourceMask = 1 << resourceBit;
                            bool resourceExists = (tile & resourceMask) != 0;

                            if (resourceExists)
                            {
                                sw.Write($" {resources[resourceMask]}");
                            }
                        }

                        if (c == 0)
                        {
                            sw.Write($"\t# Start of row {r}");
                        }

                        sw.WriteLine();
                    }
                }
            }
        }
    }
}