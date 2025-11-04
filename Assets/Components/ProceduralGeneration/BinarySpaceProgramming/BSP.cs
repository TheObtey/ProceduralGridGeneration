using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
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
        var root = new Node(allGrid, RandomService, nodesList, 5);

        for (int i = 0;  i < nodesList.Count; i++)
        {
            Node node = nodesList[i];
            var nodeBound = node.Bounds;

            PlaceRoom(nodeBound);

            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
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
}

public class Node
{
    private RandomService _randomService;
    private readonly RectInt _bounds;
    private int _spacing = 15;
    private Node _child1, _child2;

    public Node(RectInt bounds, RandomService randomservice, List<Node> nodesList, int maxNodes)
    {
        _bounds = bounds;
        _randomService = randomservice;

        int childWidth, childHeight;
        RectInt boundA, boundB;

        if (randomservice.Chance(0.5f))
        { // Vertical slice
            childWidth = _bounds.width / 2;
            childHeight = _bounds.height;

            boundA = new(bounds.x + _spacing, bounds.y + _spacing, childWidth, childHeight);
            boundB = new(bounds.x + childWidth + _spacing, bounds.y + _spacing, childWidth, childHeight);
        }
        else
        { // Horizontal slice
            childWidth = _bounds.width;
            childHeight = _bounds.height / 2;

            boundA = new(bounds.x + _spacing, bounds.y + _spacing, childWidth, childHeight);
            boundB = new(bounds.x + _spacing, bounds.y + _spacing + childHeight, childWidth, childHeight);
        }
        
        if (nodesList.Count < maxNodes)
        {
            nodesList.Add(this);
        }

        if (nodesList.Count < maxNodes)
        {
            _child1 = new Node(boundA, randomservice, nodesList, maxNodes);
        }

        if (nodesList.Count < maxNodes)
        {
            _child2 = new Node(boundB, randomservice, nodesList, maxNodes);
        }
    }

    public RectInt Bounds {
        get => _bounds;
    }
}