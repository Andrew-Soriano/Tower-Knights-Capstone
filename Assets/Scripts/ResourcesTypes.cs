using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Wood,
    Planks,
    Stone,
    Bricks,
    Ore,
    Metal,
    Parts
}

[Serializable]
public class Resources : IEnumerable<int>
{
    [SerializeField] private int wood;        //raw wood
    [SerializeField] private int planks;      //processed wood planks
    [SerializeField] private int stone;       //Raw stone
    [SerializeField] private int bricks;      //Process stone bricks
    [SerializeField] private int ore;         //Raw, unusable metal ore
    [SerializeField] private int metal;       //Smelted, useable metal
    [SerializeField] private int parts;       //Processed gears from useable metal

    public int Wood => wood;
    public int Planks => planks;
    public int Stone => stone;
    public int Bricks => bricks;
    public int Ore => ore;
    public int Metal => metal;
    public int Parts => parts;

    public Resources(int wood = 0, int planks = 0,
                     int stone = 0, int bricks = 0,
                     int ore = 0, int metal = 0, int parts = 0, int plates = 0)
    {
        this.wood = wood;
        this.planks = planks;
        this.stone = stone;
        this.bricks = bricks;
        this.ore = ore;
        this.metal = metal;
        this.parts = parts;
    }

    public IEnumerator<int> GetEnumerator()
    {
        yield return wood;
        yield return planks;
        yield return stone;
        yield return bricks;
        yield return ore;
        yield return metal;
        yield return parts;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int GetAmount(ResourceType type)
    {
        return type switch
        {
            ResourceType.Wood => Wood,
            ResourceType.Planks => Planks,
            ResourceType.Stone => Stone,
            ResourceType.Bricks => Bricks,
            ResourceType.Ore => Ore,
            ResourceType.Metal => Metal,
            ResourceType.Parts => Parts,
            _ => 0
        };
    }

    public bool HasResources(Resources cost)
    {
        // Array of "this" resource values in the same order as GetEnumerator
        int[] current = { wood, planks, stone, bricks, ore, metal, parts};

        int i = 0;
        foreach (int value in cost)
        {
            if (value > current[i])
                return false;
            i++;
        }

        return true;
    }

    public Resources MissingResources(Resources cost)
    {
        Resources missing = new Resources
        {
            wood = Math.Max(0, cost.wood - this.wood),
            planks = Math.Max(0, cost.planks - this.planks),
            stone = Math.Max(0, cost.stone - this.stone),
            bricks = Math.Max(0, cost.bricks - this.bricks),
            ore = Math.Max(0, cost.ore - this.ore),
            metal = Math.Max(0, cost.metal - this.metal),
            parts = Math.Max(0, cost.parts - this.parts)
        };

        return missing;
    }

    public void Pay(Resources cost)
    {
        wood -= cost.wood;
        planks -= cost.planks;
        stone -= cost.stone;
        bricks -= cost.bricks;
        ore -= cost.ore;
        metal -= cost.metal;
        parts -= cost.parts;
    }
    public void Add(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood: wood += amount; break;
            case ResourceType.Planks: planks += amount; break;
            case ResourceType.Stone: stone += amount; break;
            case ResourceType.Bricks: bricks += amount; break;
            case ResourceType.Ore: ore += amount; break;
            case ResourceType.Metal: metal += amount; break;
            case ResourceType.Parts: parts += amount; break;
        }
    }

    public void Subtract(ResourceType type, int amount)
    {
        switch (type)
                {
                    case ResourceType.Wood: wood -= amount; break;
                    case ResourceType.Planks: planks -= amount; break;
                    case ResourceType.Stone: stone -= amount; break;
                    case ResourceType.Bricks: bricks -= amount; break;
                    case ResourceType.Ore: ore -= amount; break;
                    case ResourceType.Metal: metal -= amount; break;
                    case ResourceType.Parts: parts -= amount; break;
                }
    }
}