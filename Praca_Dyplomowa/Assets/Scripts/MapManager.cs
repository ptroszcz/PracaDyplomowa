using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public TerrainType[] regions;

    public GameObject gameBarrierTile;

    public GameObject[] stoneTiles;

    public GameObject[] mountainTiles;

    public GameObject[] sandTiles;

    public GameObject[] treeTiles;

    public GameObject[] positiveObjects;

    public GameObject[] neutralObjects;

    public GameObject waterEdgeTile;

    public GameObject dungeonEnter;

    public GameObject dungeonColumn;

    public GameObject dungeonChest;

    public Quantity positiveObjectsRange;

    public Quantity neutralObjectsRange;

    public Quantity enemiesRange;

    public int mapWidth;

    public int mapHeight;

    public float noiseScale;

    public int octaves;

    [Range(0, 1)]
    public float persistance;

    public float lacunarity;

    [HideInInspector]
    public int seed;

    private Vector3 playerStartPosition;

    private Vector3 dungeonTempleEntry;

    public Vector2 offset;

    private Transform mapHolder;

    private List<Vector3> freeTiles = new List<Vector3>();

    private GameObject[,] tileMap;

    public void GenerateMap()
    {
        seed = Random.Range(0, (int) Mathf.Pow(2, 20));
        Debug.Log("Seed: " + seed);
        float[,] noiseMap =
            PerlinNoise
                .GenerateNoiseMap(mapWidth,
                mapHeight,
                seed,
                noiseScale,
                octaves,
                persistance,
                lacunarity,
                offset);

        TerrainType[,] map = new TerrainType[mapWidth, mapHeight];
        tileMap = new GameObject[mapWidth, mapHeight];
        for (int x = mapWidth - 1; x >= 0; --x)
        {
            for (int y = mapHeight - 1; y >= 0; --y)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; ++i)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        map[x, y] = regions[i];
                        GameObject[] possibleSprites = map[x, y].tileSprites;
                        switch (map[x, y].type)
                        {
                            case MapTileType.Water:
                                tileMap[x, y] = possibleSprites[0];
                                if (
                                    y == mapHeight - 1 ||
                                    (
                                    y != mapHeight - 1 &&
                                    map[x, y + 1].type != MapTileType.Water
                                    )
                                )
                                {
                                    tileMap[x, y] = possibleSprites[1];
                                }
                                break;
                            case MapTileType.Tree:
                            case MapTileType.Mountain:
                                tileMap[x, y] =
                                    possibleSprites[Random
                                        .Range(0,
                                        regions[i].tileSprites.Length)];
                                if (x != mapWidth - 1 && y != mapHeight - 1)
                                {
                                    if (map[x + 1, y].type == map[x, y].type)
                                    {
                                        tileMap[x, y] = tileMap[x + 1, y];
                                    }
                                    else if (
                                        map[x, y + 1].type == map[x, y].type
                                    )
                                    {
                                        tileMap[x, y] = tileMap[x, y + 1];
                                    }
                                }
                                else if (x != mapWidth - 1 && y == mapHeight - 1
                                )
                                {
                                    if (map[x + 1, y].type == map[x, y].type)
                                    {
                                        tileMap[x, y] = tileMap[x + 1, y];
                                    }
                                }
                                else if (x == mapWidth - 1 && y != mapHeight - 1
                                )
                                {
                                    if (map[x, y + 1].type == map[x, y].type)
                                    {
                                        tileMap[x, y] = tileMap[x, y + 1];
                                    }
                                }
                                break;
                            default:
                                tileMap[x, y] =
                                    possibleSprites[Random
                                        .Range(0, possibleSprites.Length)];
                                break;
                        }
                        break;
                    }
                }
            }
        }
        generatePlayerStartPoint (map, seed);
        if (playerStartPosition.x < 0 || playerStartPosition.y < 0)
        {
            GenerateMap();
        }
        else
        {
            generateDungeonTempleWithEntry (map);
            drawMap();
        }
    }

    private void generateDungeonTempleWithEntry(TerrainType[,] map)
    {
        int distance = (mapWidth + mapHeight) / 2 - 17;
        int x = (int) getPlayerStartPostition().x;
        int y = (int) getPlayerStartPostition().y;
        int i = x > mapWidth / 2 ? -1 : 1;
        int j = y > mapHeight / 2 ? -1 : 1;
        List<MapTileType> moveableTiles =
            new List<MapTileType> {
                MapTileType.Sand,
                MapTileType.Grass,
                MapTileType.Tree
            };

        while (distance > 0)
        {
            if ((i == -1 && x > 4) || (i == 1 && x < mapWidth - 5))
            {
                if (moveableTiles.Contains(map[x + i, y].type))
                {
                    x += i;
                    distance--;
                }
                else
                {
                    if (
                        moveableTiles.Contains(map[x, y + j].type) &&
                        ((j == -1 && y > 9) || (j == 1 && y < mapHeight - 10))
                    )
                    {
                        y += j;
                        distance--;
                    }
                    else
                    {
                        x += i;
                        distance--;
                        switch (map[x, y].type)
                        {
                            case MapTileType.Water:
                                map[x, y].type = MapTileType.Sand;
                                map[x, y].tileSprites = sandTiles;
                                tileMap[x, y] =
                                    sandTiles[Random
                                        .Range(0, sandTiles.Length)];
                                if (
                                    y != 0 &&
                                    map[x, y - 1].type == MapTileType.Water
                                )
                                {
                                    tileMap[x, y - 1] = waterEdgeTile;
                                }
                                break;
                            case MapTileType.Mountain:
                                map[x, y].type = MapTileType.Tree;
                                map[x, y].tileSprites = treeTiles;
                                tileMap[x, y] =
                                    treeTiles[Random
                                        .Range(0, treeTiles.Length)];
                                break;
                        }
                    }
                }
            }
            if (distance <= 0)
            {
                break;
            }
            if ((j == -1 && y > 9) || (j == 1 && y < mapHeight - 10))
            {
                if (moveableTiles.Contains(map[x, y + j].type))
                {
                    y += j;
                    distance--;
                }
                else
                {
                    if (
                        moveableTiles.Contains(map[x + i, y].type) &&
                        ((i == -1 && x > 4) || (i == 1 && x < mapWidth - 5))
                    )
                    {
                        x += i;
                        distance--;
                    }
                    else
                    {
                        y += j;
                        distance--;
                        switch (map[x, y].type)
                        {
                            case MapTileType.Water:
                                map[x, y].type = MapTileType.Sand;
                                map[x, y].tileSprites = sandTiles;
                                tileMap[x, y] =
                                    sandTiles[Random
                                        .Range(0, sandTiles.Length)];
                                if (
                                    y != 0 &&
                                    map[x, y - 1].type == MapTileType.Water
                                )
                                {
                                    tileMap[x, y - 1] = waterEdgeTile;
                                }
                                break;
                            case MapTileType.Mountain:
                                map[x, y].type = MapTileType.Tree;
                                map[x, y].tileSprites = treeTiles;
                                tileMap[x, y] =
                                    treeTiles[Random
                                        .Range(0, treeTiles.Length)];
                                break;
                        }
                    }
                }
            }
            if (
                ((i == -1 && y <= 9) || (i == 1 && y >= mapHeight - 10)) &&
                ((i == -1 && x <= 4) || (i == 1 && x >= mapWidth - 5))
            )
            {
                break;
            }
        }
        this.dungeonTempleEntry = new Vector3(x, y, 0f);
        generateDungeonTemple (map);
    }

    private void generateDungeonTemple(TerrainType[,] map)
    {
        int j = dungeonTempleEntry.y > playerStartPosition.y ? 1 : -1;
        int startX = (int) dungeonTempleEntry.x - 4;
        int startY = (int) dungeonTempleEntry.y + j;
        for (int x = 0; x < 9; ++x)
        {
            for (int y = 0; y < 9; ++y)
            {
                if (y == 0 || y == 8)
                {
                    if (x >= 2 && x <= 6)
                    {
                        map[startX + x, startY + y * j].type =
                            MapTileType.Mountain;
                        map[startX + x, startY + y * j].tileSprites =
                            mountainTiles;
                        tileMap[startX + x, startY + y * j] = mountainTiles[1];
                    }
                }
                else if (y == 1 || y == 7)
                {
                    if (x == 1 || x == 7)
                    {
                        map[startX + x, startY + y * j].type =
                            MapTileType.Mountain;
                        map[startX + x, startY + y * j].tileSprites =
                            mountainTiles;
                        tileMap[startX + x, startY + y * j] = mountainTiles[1];
                    }
                    else if (x > 1 && x < 7)
                    {
                        map[startX + x, startY + y * j].type =
                            MapTileType.Stone;
                        map[startX + x, startY + y * j].tileSprites =
                            stoneTiles;
                        tileMap[startX + x, startY + y * j] =
                            stoneTiles[Random.Range(0, stoneTiles.Length)];
                    }
                }
                else
                {
                    if (x == 0 || x == 8)
                    {
                        map[startX + x, startY + y * j].type =
                            MapTileType.Mountain;
                        map[startX + x, startY + y * j].tileSprites =
                            mountainTiles;
                        tileMap[startX + x, startY + y * j] = mountainTiles[1];
                    }
                    else
                    {
                        map[startX + x, startY + y * j].type =
                            MapTileType.Stone;
                        map[startX + x, startY + y * j].tileSprites =
                            stoneTiles;
                        tileMap[startX + x, startY + y * j] =
                            stoneTiles[Random.Range(0, stoneTiles.Length)];
                    }
                }
            }
        }
        map[startX + 4, startY].type = MapTileType.Stone;
        map[startX + 4, startY].tileSprites = stoneTiles;
        tileMap[startX + 4, startY] =
            stoneTiles[Random.Range(0, stoneTiles.Length)];
        ChangeWaterSpriteIfNeeded (map, startX, startY);
        InitializeFreeTitles (map);
    }

    private void ChangeWaterSpriteIfNeeded(
        TerrainType[,] map,
        int startX,
        int startY
    )
    {
        if (dungeonTempleEntry.y < playerStartPosition.y)
        {
            startY -= 8;
        }
        int endY = (startY + 8 == mapHeight - 1 ? startY + 8 : startY + 9);
        startY = (startY == 0 ? startY : startY - 1);
        for (int x = startX; x < startX + 9; ++x)
        {
            for (int y = startY; y <= endY; ++y)
            {
                if (map[x, y].type == MapTileType.Water)
                {
                    if (y + 1 >= mapHeight)
                    {
                        tileMap[x, y] = waterEdgeTile;
                    }
                    else if (map[x, y + 1].type != MapTileType.Water)
                    {
                        tileMap[x, y] = waterEdgeTile;
                    }
                }
            }
        }
    }

    private void InitializeFreeTitles(TerrainType[,] map)
    {
        freeTiles.Clear();
        List<MapTileType> freeTilesTypes =
            new List<MapTileType> { MapTileType.Sand, MapTileType.Grass };
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                if (freeTilesTypes.Contains(map[x, y].type))
                {
                    freeTiles.Add(new Vector3(x, y, 0f));
                }
            }
        }
        freeTiles.Remove(getPlayerStartPostition());
        freeTiles.Remove (dungeonTempleEntry);
    }

    private void generatePlayerStartPoint(TerrainType[,] map, int seed)
    {
        Vector3Int psp = new Vector3Int(seed % mapWidth, seed % mapHeight, 0);
        int count = 0;
        while (map[psp.x, psp.y].type != MapTileType.Sand &&
            map[psp.x, psp.y].type != MapTileType.Grass
        )
        {
            if (count % 2 == 0)
            {
                psp.x += 1;
                if (psp.x >= mapWidth)
                {
                    psp.x = 0;
                }
            }
            else
            {
                psp.y += 1;
                if (psp.y >= mapHeight)
                {
                    psp.y = 0;
                }
            }
            count++;
            if (count >= 1000)
            {
                playerStartPosition = TakeFirstMatchingTile(map);
                break;
            }
        }
        playerStartPosition = psp;
        freeTiles.Remove (playerStartPosition);
    }

    private Vector3 TakeFirstMatchingTile(TerrainType[,] map)
    {
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                if (
                    map[x, y].type == MapTileType.Sand ||
                    map[x, y].type == MapTileType.Grass
                )
                {
                    return new Vector3(x, y, 0f);
                }
            }
        }
        return new Vector3(-1, -1, 0f);
    }

    public void drawMap()
    {
        mapHolder = new GameObject("Map").transform;
        int width = tileMap.GetLength(0);
        int heigth = tileMap.GetLength(1);

        for (int x = -10; x < width + 10; ++x)
        {
            for (int y = -8; y < heigth + 8; ++y)
            {
                if (x < 0 || y < 0 || x >= width || y >= heigth)
                {
                    GameObject instance =
                        Instantiate(gameBarrierTile,
                        new Vector3(x, y, 0f),
                        Quaternion.identity) as
                        GameObject;
                    instance.transform.SetParent (mapHolder);
                }
                else
                {
                    GameObject instance =
                        Instantiate(tileMap[x, y],
                        new Vector3(x, y, 0f),
                        Quaternion.identity) as
                        GameObject;
                    instance.transform.SetParent (mapHolder);
                }
            }
        }
    }

    public Vector3 getPlayerStartPostition()
    {
        return playerStartPosition;
    }

    public void SetupScene()
    {
        GenerateMap();
        int y = playerStartPosition.y != mapHeight - 1 ? 1 : -1;
        InstantiateDungeonTempleObjects();
        InstantiatePositiveObjects();
        InstantiateNeutralObjects();
        InstantiateEnemies();
    }

    private void InstantiatePositiveObjects()
    {
        int numberOfPositiveObjects = positiveObjectsRange.Random;
        for (int i = 0; i < numberOfPositiveObjects; ++i)
        {
            Vector3 randomPosition =
                freeTiles[Random.Range(0, freeTiles.Count)];
            Instantiate(positiveObjects[Random
                .Range(0, positiveObjects.Length)],
            randomPosition,
            Quaternion.identity);
            freeTiles.Remove (randomPosition);
        }
    }

    private void InstantiateNeutralObjects()
    {
        int numberOfNeutralObjects = neutralObjectsRange.Random;
        for (int i = 0; i < numberOfNeutralObjects; ++i)
        {
            Vector3 randomPosition =
                freeTiles[Random.Range(0, freeTiles.Count)];
            Instantiate(neutralObjects[Random.Range(0, neutralObjects.Length)],
            randomPosition,
            Quaternion.identity);
            freeTiles.Remove (randomPosition);
        }
    }

    private void InstantiateDungeonTempleObjects()
    {
        Instantiate(dungeonEnter,
        new Vector3(dungeonTempleEntry.x,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 5 : -5),
            0f),
        Quaternion.identity);
        InstantiateDungeonColumns();
        InstantiateDungeonChests();
    }

    private void InstantiateDungeonColumns()
    {
        Instantiate(dungeonColumn,
        new Vector3(dungeonTempleEntry.x - 1,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 4 : -4),
            0f),
        Quaternion.identity);
        Instantiate(dungeonColumn,
        new Vector3(dungeonTempleEntry.x - 1,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 6 : -6),
            0f),
        Quaternion.identity);
        Instantiate(dungeonColumn,
        new Vector3(dungeonTempleEntry.x + 1,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 4 : -4),
            0f),
        Quaternion.identity);
        Instantiate(dungeonColumn,
        new Vector3(dungeonTempleEntry.x + 1,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 6 : -6),
            0f),
        Quaternion.identity);
    }

    private void InstantiateDungeonChests()
    {
        Instantiate(dungeonChest,
        new Vector3(dungeonTempleEntry.x,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 8 : -8),
            0f),
        Quaternion.identity);
        Instantiate(dungeonChest,
        new Vector3(dungeonTempleEntry.x - 3,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 5 : -5),
            0f),
        Quaternion.identity);
        Instantiate(dungeonChest,
        new Vector3(dungeonTempleEntry.x + 3,
            dungeonTempleEntry.y +
            (dungeonTempleEntry.y > playerStartPosition.y ? 5 : -5),
            0f),
        Quaternion.identity);
    }

    private void InstantiateEnemies()
    {
        int numberOfEnemies = enemiesRange.Random;
        for (int i = 0; i < numberOfEnemies; ++i)
        {
            Vector3 randomPosition =
                freeTiles[Random.Range(0, freeTiles.Count)];
            Instantiate(GameManager
                .instance
                .enemiesList[Random
                    .Range(0, GameManager.instance.enemiesList.Length)],
            randomPosition,
            Quaternion.identity);
            freeTiles.Remove (randomPosition);
        }
        int j = dungeonTempleEntry.y > playerStartPosition.y ? 1 : -1;
        Instantiate(GameManager.instance.guardian,
        new Vector3(dungeonTempleEntry.x, dungeonTempleEntry.y + j, 0f),
        Quaternion.identity);
    }

    void OnValidate()
    {
        if (mapWidth < 50)
        {
            mapWidth = 50;
        }
        if (mapHeight < 50)
        {
            mapHeight = 50;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[Serializable]
public struct TerrainType
{
    public MapTileType type;

    public float height;

    public GameObject[] tileSprites;
}

public enum MapTileType
{
    Water,
    Sand,
    Grass,
    Tree,
    Mountain,
    Stone
}
