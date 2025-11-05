using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;
using VTools.Utility;

namespace Components.ProceduralGeneration.SimpleRoomPlacement
{
    [CreateAssetMenu(menuName = "Procedural Generation Method/Simple Room Placement")]
    public class SimpleRoomPlacement : ProceduralGenerationMethod
    {        
        protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
        {
            // Declare variables here
            // ........

            List<RectInt> roomList = new();

            for (int i = 0; i < _maxSteps; i++)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                int x = RandomService.Range(0, Grid.Width);
                int y = RandomService.Range(0, Grid.Lenght);

                RectInt room = new RectInt(x, y, 10, 10);

                if (!CellAvailable(room))
                    continue;

                PlaceRoom(room);
                roomList.Add(room);

                // Waiting between steps to see the result.
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken : cancellationToken);
            }

            for (int i = 0; i < roomList.Count - 1; i++)
            {
                Vector2Int a = roomList[i].GetCenter();
                Vector2Int b = roomList[i + 1].GetCenter();

                ConnectRooms(a, b);

                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
            }
            
            // Final ground building.
            BuildGround();
        }
        
        private void BuildGround()
        {
            var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Grass");
            
            // Instantiate ground blocks
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int z = 0; z < Grid.Lenght; z++)
                {
                    if (!Grid.TryGetCellByCoordinates(x, z, out var chosenCell))
                    {
                        Debug.LogError($"Unable to get cell on coordinates : ({x}, {z})");
                        continue;
                    }
                    
                    GridGenerator.AddGridObjectToCell(chosenCell, groundTemplate, false);
                }
            }
        }

        private void PlaceRoom(RectInt room)
        {
            for (int ix = room.xMin; ix < room.xMax; ix++)
            {
                for (int iy = room.yMin; iy < room.yMax; iy++)
                {
                    if (!Grid.TryGetCellByCoordinates(ix, iy, out var cell))
                        continue;

                    AddTileToCell(cell, ROOM_TILE_NAME, true);
                }
            }
        }

        private bool CellAvailable(RectInt room)
        {
            for (int x = room.xMin - 1; x < room.xMax + 1; x++)
            {
                for (int y = room.yMin - 1; y < room.yMax + 1; y++)
                {
                    if (!Grid.TryGetCellByCoordinates(x, y, out var cell))
                        return false;

                    if (cell.ContainObject)
                        return false;
                }
            }

            return true;
        }

        private void ConnectRooms(Vector2Int a, Vector2Int b)
        {
            for (int x = Mathf.Min(a.x, b.x); x <= Mathf.Max(a.x, b.x); x++)
            {
                if (!Grid.TryGetCellByCoordinates(x, a.y, out var cell))
                    continue;

                AddTileToCell(cell, CORRIDOR_TILE_NAME, true);
            }

            for (int y = Mathf.Min(a.y, b.y); y <= Mathf.Max(a.y, b.y); y++)
            {
                if (!Grid.TryGetCellByCoordinates(b.x, y, out var cell))
                    continue;

                AddTileToCell(cell, CORRIDOR_TILE_NAME, true);
            }
        }
    }
}