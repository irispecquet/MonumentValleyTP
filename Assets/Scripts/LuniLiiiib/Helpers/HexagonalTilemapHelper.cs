using System;
using UnityEngine;

namespace LuniLib.Helpers
{
    public static class HexagonalTilemapHelper
    {
        public const int DIRECTIONS_COUNT = 6;
        public const int AXES_COUNT = 3; // This represents the possible axes (horizontal, top right / bottom left, top left / bottom right)
        
        private static readonly float SQRT3 = Mathf.Sqrt(3f);
        private static readonly float SQRT3_DIVIDED_BY_3 = SQRT3 / 3f;
        private static readonly float SQRT3_DIVIDED_BY_2 = SQRT3 / 2f;
        
        private const float ONE_THIRD = 1f / 3f;
        private const float TWO_THIRD = 2f / 3f;
        private const float THREE_QUARTERS = 3f / 4f;
        
        public static readonly Vector2Int[] EVEN_OFFSETS =
        {
            new(1, 0),      // Right
            new(0, 1),      // TopRight
            new(-1, 1),     // TopLeft
            new(-1, 0),     // Left
            new(-1, -1),    // BottomLeft
            new(0, -1),     // BottomRight
        };

        public static readonly Vector2Int[] ODD_OFFSETS =
        {
            new(1, 0),      // Right
            new(1, 1),      // TopRight
            new(0, 1),      // TopLeft
            new(-1, 0),     // Left
            new(0, -1),     // BottomLeft
            new(1, -1),     // BottomRight
        };
        
        // Modify this before using any method/property below in order to setup your own tile size.
        public static float TileSize = 1f;
        
        public static float HorizontalSpacing => TileSize * SQRT3_DIVIDED_BY_2;
        public static float HorizontalSpacingOffset => HorizontalSpacing / 2f;
        public static float VerticalSpacing => TileSize * 0.75f;
        
        #region HEXAGON_COORDINATES
        /// <summary> Convert an axial coordinates position to a cube coordinates position : https://www.redblobgames.com/grids/hexagons/#conversions </summary>
        private static Vector3 AxialToCube(Vector2 axialPos)
        {
            return new Vector3(axialPos.x, axialPos.y, -axialPos.x - axialPos.y);
        }
        
        /// <summary> Convert a cube coordinates position to an offset coordinates position : https://www.redblobgames.com/grids/hexagons/#conversions </summary>
        private static Vector2Int CubeToOffset(Vector3Int cubePos)
        {
            int x = cubePos.x + (cubePos.y - (cubePos.y & 1)) / 2;
            return new Vector2Int(x, cubePos.y);
        }

        /// <summary> Convert an offset coordinates position to an axial coordinates position : https://www.redblobgames.com/grids/hexagons/#conversions </summary>
        private static Vector2Int OffsetToAxial(Vector2Int offsetPos)
        {
            return new Vector2Int(offsetPos.x - (offsetPos.y - (offsetPos.y & 1)) / 2, offsetPos.y);
        }

        /// <summary> Rounds a float position (cube coordinates) to an hexagon position (cube coordinates) : https://www.redblobgames.com/grids/hexagons/#conversions </summary>
        private static Vector3Int CubeRound(Vector3 cubePosition)
        {
            Vector3Int roundedPos = new(Mathf.RoundToInt(cubePosition.x), Mathf.RoundToInt(cubePosition.y), Mathf.RoundToInt(cubePosition.z));
            Vector3 posDiff = new(Mathf.Abs(roundedPos.x - cubePosition.x), Mathf.Abs(roundedPos.y - cubePosition.y), Mathf.Abs(roundedPos.z - cubePosition.z));

            if (posDiff.x > posDiff.y && posDiff.x > posDiff.z)
                roundedPos.x = -roundedPos.y - roundedPos.z;
            else if (posDiff.y > posDiff.z)
                roundedPos.y = -roundedPos.x - roundedPos.z;
            else
                roundedPos.z = -roundedPos.x - roundedPos.y;

            return roundedPos;
        }
        
        /// <summary> Convert a world position to an hexagon position : https://www.redblobgames.com/grids/hexagons/#rounding </summary>
        public static Vector2Int WorldToCell(Vector3 pos)
        {
            // Convert mouse pos to axial coordinates position : https://www.redblobgames.com/grids/hexagons/#pixel-to-hex
            Vector2 axialPos = new((SQRT3_DIVIDED_BY_3 * pos.x - ONE_THIRD * pos.z) * 2f / TileSize, TWO_THIRD * pos.z * 2f / TileSize);
            // Convert axial coordinates position to cube coordinates position
            Vector3 cubePos = AxialToCube(axialPos);
            // Round to get hexagon position (cube coordinates)
            Vector3Int cubePosRounded = CubeRound(cubePos);
            // Convert to offset coordinates position
            Vector2Int offsetPos = CubeToOffset(cubePosRounded);
            
            return offsetPos;
        }

        /// <summary> Convert an hexagon position to a world position (x,z) : https://www.redblobgames.com/grids/hexagons/#hex-to-pixel </summary>
        public static Vector3 CellToWorld(Vector2Int offsetPos)
        {
            float xPos = TileSize * SQRT3_DIVIDED_BY_2 * (offsetPos.x + 0.5f * (offsetPos.y & 1));
            float yPos = TileSize * THREE_QUARTERS * offsetPos.y;
            return new Vector3(xPos, 0f, yPos);
        }
        
        /// <summary> Convert an hexagon position to a world position in 2D (x,y) : https://www.redblobgames.com/grids/hexagons/#hex-to-pixel </summary>
        public static Vector2 CellToWorld2D(Vector2Int offsetPos)
        {
            float xPos = TileSize * SQRT3_DIVIDED_BY_2 * (offsetPos.x + 0.5f * (offsetPos.y & 1));
            float yPos = TileSize * THREE_QUARTERS * offsetPos.y;
            return new Vector2(xPos, yPos);
        }

        /// <summary> Compute the distance in tiles between 2 hexagon positions (in offset coordinates) </summary>
        public static int TileDistance(Vector2Int originHex, Vector2Int endHex)
        {
            Vector2Int axialOriginHex = OffsetToAxial(originHex);
            Vector2Int axialEndHex = OffsetToAxial(endHex);
            int xDiff = axialEndHex.x - axialOriginHex.x;
            int yDiff = axialEndHex.y - axialOriginHex.y;

            int distance = (Mathf.Abs(xDiff) + Mathf.Abs(xDiff + yDiff) + Mathf.Abs(yDiff)) / 2;
            return distance;
        }
        
        /// <summary> Compute the distance in world coordinates between 2 hexagon positions (in offset coordinates) </summary>
        public static float WorldDistance(Vector2Int originHex, Vector2Int endHex)
        {
            return Vector3.Distance(OffsetToWorldPosition(originHex), OffsetToWorldPosition(endHex));
        }
        #endregion // HEXAGON_COORDINATES
        
        public static float GetXOffset(int yPosition)
        {
            return yPosition % 2 == 0 ? 0 : HorizontalSpacingOffset;
        }
        
        /// <summary>
        /// Offsets depend on the tile position in Unity's tilemaps. This returns the right offsets depending on the tile's position.
        /// </summary>
        /// <param name="yPosition">The Y position of the tile.</param>
        /// <returns>The offsets, starting by right and rotating counterclockwise.</returns>
        public static Vector2Int[] GetTileOffsets(int yPosition)
        {
            return yPosition % 2 == 0 ? EVEN_OFFSETS : ODD_OFFSETS;
        }

        /// <summary>
        /// Offsets depend on the tile position in Unity's tilemaps. This returns the right offset depending on the tile's position.
        /// </summary>
        /// <param name="yPosition">The Y position of the tile.</param>
        /// <param name="directionIndex">The index of the direction for the offset</param>
        public static Vector2Int GetTileOffset(int yPosition, int directionIndex)
        {
            return (yPosition % 2 == 0 ? EVEN_OFFSETS : ODD_OFFSETS)[directionIndex];
        }

        public static int GetOppositeDirectionIndex(int index)
        {
            return (index + 3) % DIRECTIONS_COUNT;
        }

        /// <summary>
        /// <para>Gives all the direction indices from one side. Imagine placing an axe of symmetry and picking the 2 directions of one side.</para>
        /// <para>Example : axe index is 0, we "cut" horizontally and return top right and top left (1 and 2) if clockwise, bottom right and bottom left (6 and 5) if counterclockwise.</para>
        /// </summary>
        /// <param name="axeDirectionIndex">The direction of the imaginary axe.</param>
        /// <param name="clockwise">Whether we want the directions on the clockwise side or the other.</param>
        public static int[] GetSideIndices(int axeDirectionIndex, bool clockwise)
        {
            int[] sideIndices = new int[2];
            sideIndices[0] = GetNextDirectionIndex(axeDirectionIndex, clockwise);
            sideIndices[1] = GetNextDirectionIndex(sideIndices[0], clockwise);

            return sideIndices;
        }

        public static int GetNextDirectionIndex(int index, bool clockwise)
        {
            index += clockwise ? 1 : -1;
            if (index < 0)
                index += DIRECTIONS_COUNT;
            else
                index %= DIRECTIONS_COUNT;

            return index;
        }

        /// <summary>
        /// Check if at least one neighbour satisfies the given condition.
        /// </summary>
        /// <param name="tilePosition">The position of the tile.</param>
        /// <param name="condition">The condition to check on the neighbours.</param>
        /// <returns>True if at least one neighbour satisfies the given condition.</returns>
        public static bool AnyNeighbour(Vector2Int tilePosition, Func<Vector2Int, bool> condition)
        {
            Vector2Int[] offsets = GetTileOffsets(tilePosition.y);
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2Int offsetPosition = tilePosition + offsets[i];

                if (condition(offsetPosition))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Check if x neighbours satisfy the given condition.
        /// </summary>
        /// <param name="tilePosition">The position of the tile.</param>
        /// <param name="condition">The condition to check on the neighbours.</param>
        /// <param name="neighboursCount">The number of neighbours needed to satisfy the requirements.</param>
        /// <returns>True if at least x neighbours satisfy the given condition.</returns>
        public static bool DoXNeighbours(Vector2Int tilePosition, Func<Vector2Int, bool> condition, int neighboursCount)
        {
            int x = 0;
            Vector2Int[] offsets = GetTileOffsets(tilePosition.y);
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2Int offsetPosition = tilePosition + offsets[i];

                if (condition(offsetPosition) && ++x >= neighboursCount) // Here we check the neighbours count
                    return true;
            }

            return false;
        }
        
        public static Vector2Int? GetCellFromScreenPoint(Vector3 screenPosition, Camera camera, Plane tilemapPlane)
        {
            Ray ray = camera.ScreenPointToRay(screenPosition);
            if (tilemapPlane.Raycast(ray, out float dist))
            {
                Vector3 position = ray.GetPoint(dist);
                return WorldToCell(position);
            }

            return null;
        }
        
        // public static Vector2Int? GetCellFromViewportPoint(Vector3 screenPosition, Camera camera, Plane tilemapPlane)
        // {
        //     Vector3? mapWorldPoint = IshLib.UnityUtils.CameraUtils.GetPlaneIntersectionFromViewportRay(screenPosition, camera, tilemapPlane);
        //     if (mapWorldPoint.HasValue)
        //         return WorldToCell(mapWorldPoint.Value);
        //
        //     return null;
        // }

        public static Vector2 GetViewportPointFromCell(Vector2Int cellPosition, Camera camera)
        {
            Vector3 worldPosition = CellToWorld(cellPosition);
            return camera.WorldToViewportPoint(worldPosition);
        }

        public static Vector3 OffsetToWorldPosition(Vector2Int cellPosition)
        {
            float xPos = cellPosition.x * HorizontalSpacing + GetXOffset(cellPosition.y);
            float yPos = cellPosition.y * VerticalSpacing;
            return new Vector3(xPos, 0f, yPos);
        }
    }
}
