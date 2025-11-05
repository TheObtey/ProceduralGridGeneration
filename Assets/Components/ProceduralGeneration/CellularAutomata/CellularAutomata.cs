using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTools.Grid;

[CreateAssetMenu(menuName = "Procedural Generation Method/CellularAutomata")]
public class CellularAutomata : ProceduralGenerationMethod
{
    [SerializeField] private float GroundRatio = 0.5f;
    [SerializeField] private int GroundSisterNeeded = 4;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        // Créer le bruit de départ
        CreateNoise();

        for (int i = 0; i < _maxSteps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Créer une grille temporaire
            bool[,] tempGrid = new bool[Grid.Width, Grid.Lenght];

            // Populate la temp grid
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    if (!Grid.TryGetCellByCoordinates(x, y, out var cell))
                        continue;

                    tempGrid[x, y] = false;

                    if (SurroundedByGround(cell))
                    {
                        tempGrid[x, y] = true;
                    }
                }
            }

            // Ajouter les tiles
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    if (!Grid.TryGetCellByCoordinates(x, y, out var cell))
                        continue;

                    if (tempGrid[x, y])
                        AddTileToCell(cell, GRASS_TILE_NAME, true);
                    else
                        AddTileToCell(cell, WATER_TILE_NAME, true);
                }
            }

            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }
    }

    private void CreateNoise()
    {
        for (int i = 0; i < Grid.Cells.Count; i++)
        {
            string cellType = WATER_TILE_NAME;

            if (RandomService.Chance(GroundRatio))
                cellType = GRASS_TILE_NAME;

            Cell cell = Grid.Cells[i];
            AddTileToCell(cell, cellType, true);
        }
    }

    private bool SurroundedByGround(Cell cell)
    {
        Vector2Int pos = cell.Coordinates;
        Cell sisterCell;
        int groundSisters = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                if (!Grid.TryGetCellByCoordinates(pos.x + x, pos.y + y, out sisterCell))
                    continue;

                if (sisterCell.GridObject.Template.Name == GRASS_TILE_NAME)
                    groundSisters++;
            }
        }

        Debug.Log(groundSisters);
        return groundSisters >= GroundSisterNeeded;
    }
}