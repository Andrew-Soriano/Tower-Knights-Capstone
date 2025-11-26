using UnityEngine;

public class TreeResource : ResourceTile
{
    private void Awake()
    {
        type = ResourceType.Wood;
        tileName = "Lumber Yard";
        amountPerRound = 1;
    }
}
