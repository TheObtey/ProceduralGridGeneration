using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural Generation Method/NoiseGeneration")]
public class NoiseGeneration : ProceduralGenerationMethod
{
    [SerializeField][Range(-1.0f, 1.0f)] private float WaterHeight = -0.6f;
    [SerializeField][Range(-1.0f, 1.0f)] private float SandHeight = -0.3f;
    [SerializeField][Range(-1.0f, 1.0f)] private float GroundHeight = 0.8f;
    [SerializeField][Range(-1.0f, 1.0f)] private float HillHeight = 1f;


    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        FastNoiseLite noise = new FastNoiseLite(RandomService.Seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Lenght; y++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!Grid.TryGetCellByCoordinates(x, y, out var cell))
                    continue;

                float noiseValue = noise.GetNoise(x, y);
                string tileName = "";

                if (noiseValue <= WaterHeight)
                    tileName = WATER_TILE_NAME;
                else if (noiseValue > WaterHeight && noiseValue <= SandHeight)
                    tileName = SAND_TILE_NAME;
                else if (noiseValue > SandHeight && noiseValue <= GroundHeight)
                    tileName = GRASS_TILE_NAME;
                else
                    tileName = ROCK_TILE_NAME;

                AddTileToCell(cell, tileName, true);
            }
        }

        await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
    }
}
