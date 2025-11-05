using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using VTools.RandomService;

[CreateAssetMenu(menuName = "Procedural Generation Method/BSP")]
public class BSP : ProceduralGenerationMethod
{
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        List<Node> nodesList = new();
        var allGrid = new RectInt(0, 0, Grid.Width, Grid.Lenght);
        var root = new Node(allGrid, RandomService, this, nodesList, 10);

        await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
    }

    public void PlaceRoom(RectInt room)
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
}

public class Node
{
    private RandomService _randomService;
    private BSP _bsp;
    private readonly RectInt _bounds;
    private Node _child1, _child2 = null;

    public Node(RectInt bounds, RandomService randomservice, BSP bsp, List<Node> nodesList, int maxNodes)
    {
        _bounds = bounds;
        _randomService = randomservice;
        _bsp = bsp;

        // Do shit
    }

    public RectInt Bounds {
        get => _bounds;
    }
}