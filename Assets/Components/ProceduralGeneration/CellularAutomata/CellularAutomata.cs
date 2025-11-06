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

    private bool[,] currentBuffer;
    private bool[,] nextBuffer;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        // Initialiser les buffers
        currentBuffer = new bool[Grid.Width, Grid.Lenght];
        nextBuffer = new bool[Grid.Width, Grid.Lenght];

        // Créer le bruit de départ
        CreateNoise();

        // Remplir currentBuffer pour la première passe
        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Lenght; y++)
            {
                currentBuffer[x, y] = SurroundedByGround(x, y);
            }
        }

        for (int i = 0; i < _maxSteps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Appliquer l'algo d'affinage
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    nextBuffer[x, y] = SurroundedByGround(x, y);
                }
            }

            // Update les tiles si il y a changement entre currentBuffer et nextBuffer
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    if (currentBuffer[x, y] == nextBuffer[x, y])
                        continue;

                    if (!Grid.TryGetCellByCoordinates(x, y, out var cell))
                        continue;

                    // Changer le skin de la tile
                    cell.ChangeSkin(GRASS_TILE_NAME);
                }
            }

            // Swap les buffers
            var temp = currentBuffer;
            currentBuffer = nextBuffer;
            nextBuffer = temp;

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
            Vector2Int cellPos = cell.Coordinates;
            
            currentBuffer[cellPos.x, cellPos.y] = (cellType == GRASS_TILE_NAME);
            
            AddTileToCell(cell, cellType, true);
        }
    }

    private bool SurroundedByGround(int ix, int iy)
    {
        int groundSisters = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                if (ix + x < 0 || ix + x > Grid.Width - 1 || iy + y < 0 || iy + y > Grid.Lenght - 1)
                    continue;

                if (currentBuffer[ix + x, iy + y])
                    groundSisters++;
            }
        }

        return groundSisters >= GroundSisterNeeded;
    }
}