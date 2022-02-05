using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    public int dungeonWidth = 50;

    public int dungeonHeight = 50;

    public Quantity roomsNumber = new Quantity(10, 20);

    public Quantity roomWidth = new Quantity(3, 10);

    public Quantity roomHeight = new Quantity(3, 10);

    public Quantity corridorLength = new Quantity(5, 15);

    public Quantity positiveObjectsRange;

    public Quantity neutralObjectsRange;

    public Quantity dungeonEnemiesRange;

    public GameObject[] floorTiles;

    public GameObject[] wallTiles;

    public GameObject[] positiveObjects;

    public GameObject[] neutralObjects;

    public GameObject finalObject;

    private List<Vector3> freeTiles = new List<Vector3>();

    private Transform dungeonHolder;

    private Vector3 playerStartPosition;

    private Vector3 finalObjectPosition;

    private DungeonTileType[,] tiles;

    private Room[] rooms;

    private Corridor[] corridors;

    // Start is called before the first frame update
    void InitialiseGrid()
    {
        freeTiles.Clear();

        for (int i = 0; i < dungeonWidth - 1; ++i)
        {
            for (int j = 0; j < dungeonHeight - 1; ++j)
            {
                if (tiles[i, j] == DungeonTileType.Floor)
                {
                    freeTiles.Add(new Vector3(i, j, 0f));
                }
            }
        }
        freeTiles.Remove (playerStartPosition);
        freeTiles.Remove (finalObjectPosition);
    }

    private void GenerateDungeon()
    {
        dungeonHolder = new GameObject("DungeonHolder").transform;
        InitialiseTilesArray();
        CalculateRoomsAndCorridorsPlacement();
        SetTilesForRoomsAndCorridors();
        DrawDungeon();
    }

    private void InitialiseTilesArray()
    {
        tiles = new DungeonTileType[dungeonWidth, dungeonHeight];
        for (int i = 0; i < dungeonWidth; ++i)
        {
            for (int j = 0; j < dungeonHeight; ++j)
            {
                tiles[i, j] = DungeonTileType.Wall;
            }
        }
    }

    private void CalculateRoomsAndCorridorsPlacement()
    {
        rooms = new Room[roomsNumber.Random];

        corridors = new Corridor[rooms.Length - 1];

        rooms[0] = new Room();
        corridors[0] = new Corridor();

        rooms[0]
            .GenerateFirstRoom(roomWidth,
            roomHeight,
            dungeonWidth,
            dungeonHeight);
        playerStartPosition =
            new Vector3(rooms[0].x + rooms[0].width / 2,
                rooms[0].y + rooms[0].height / 2,
                0f);
        corridors[0]
            .GenerateCorridor(rooms[0],
            corridorLength,
            roomWidth,
            roomHeight,
            dungeonWidth,
            dungeonHeight,
            true);
        for (int i = 1; i < rooms.Length; ++i)
        {
            rooms[i] = new Room();

            rooms[i]
                .GenerateRoomBasedOnCorridor(roomWidth,
                roomHeight,
                dungeonWidth,
                dungeonHeight,
                corridors[i - 1]);

            //Debug.Log("Room x: " + rooms[i].x + ", y: " + rooms[i].y);
            if (i < corridors.Length)
            {
                corridors[i] = new Corridor();
                corridors[i]
                    .GenerateCorridor(rooms[i],
                    corridorLength,
                    roomWidth,
                    roomHeight,
                    dungeonWidth,
                    dungeonHeight,
                    false);
                // Debug
                //     .Log("Corridor x: " +
                //     corridors[i].x +
                //     ", y: " +
                //     corridors[i].y +
                //     ", length: " +
                //     corridors[i].length+", direction: "+corridors[i].direction);
            }
        }
    }

    private void SetTilesForRoomsAndCorridors()
    {
        SetTilesForRooms();
        SetTilesForCorridors();
    }

    private void SetTilesForRooms()
    {
        for (int i = 0; i < rooms.Length; ++i)
        {
            Room room = rooms[i];
            for (int j = 0; j < room.width; ++j)
            {
                int x = room.x + j;
                for (int k = 0; k < room.height; ++k)
                {
                    int y = room.y + k;
                    tiles[x, y] = DungeonTileType.Floor;
                }
            }
        }
    }

    private void SetTilesForCorridors()
    {
        for (int i = 0; i < corridors.Length; ++i)
        {
            Corridor corridor = corridors[i];
            for (int j = 0; j < corridor.length; ++j)
            {
                int x = corridor.x;
                int y = corridor.y;
                if (
                    corridor.direction == Direction.North ||
                    corridor.direction == Direction.South
                )
                {
                    y += (corridor.direction == Direction.North ? j : -1 * j);
                }
                else
                {
                    x += (corridor.direction == Direction.East ? j : -1 * j);
                }
                tiles[x, y] = DungeonTileType.Floor;
            }
        }
    }

    private void DrawDungeon()
    {
        for (int x = -10; x < dungeonWidth + 10; ++x)
        {
            for (int y = -10; y < dungeonHeight + 10; ++y)
            {
                if (
                    x < 0 ||
                    y < 0 ||
                    x >= dungeonWidth ||
                    y >= dungeonHeight ||
                    tiles[x, y] == DungeonTileType.Wall
                )
                {
                    GameObject instance =
                        Instantiate(wallTiles[Random
                            .Range(0, wallTiles.Length)],
                        new Vector3(x, y, 0f),
                        Quaternion.identity) as
                        GameObject;
                    instance.transform.SetParent (dungeonHolder);
                }
                else
                {
                    GameObject instance =
                        Instantiate(floorTiles[Random
                            .Range(0, floorTiles.Length)],
                        new Vector3(x, y, 0f),
                        Quaternion.identity) as
                        GameObject;
                    instance.transform.SetParent (dungeonHolder);
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
        GenerateDungeon();
        GenerateFinalObject();
        InitialiseGrid();
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

    private void InstantiateEnemies()
    {
        int numberOfEnemies = dungeonEnemiesRange.Random;
        for (int i = 0; i < numberOfEnemies; ++i)
        {
            Vector3 randomPosition =
                freeTiles[Random.Range(0, freeTiles.Count)];
            Instantiate(GameManager
                .instance
                .dungeonEnemiesList[Random
                    .Range(0, GameManager.instance.dungeonEnemiesList.Length)],
            randomPosition,
            Quaternion.identity);
            freeTiles.Remove (randomPosition);
        }
    }

    private void GenerateFinalObject()
    {
        float maxDistance =
            Vector3
                .Distance(playerStartPosition,
                new Vector3(rooms[0].x, rooms[0].y, 0f));
        int foundIndex = 0;
        for (int i = 1; i < rooms.Length; ++i)
        {
            float tempDistance =
                Vector3
                    .Distance(playerStartPosition,
                    new Vector3(rooms[i].x, rooms[i].y, 0f));
            if (tempDistance > maxDistance)
            {
                maxDistance = tempDistance;
                foundIndex = i;
            }
        }
        finalObjectPosition =
            new Vector3((
                int
                )(rooms[foundIndex].x + rooms[foundIndex].width / 2),
                (int)(rooms[foundIndex].y + rooms[foundIndex].height / 2),
                0f);
        Instantiate(finalObject, finalObjectPosition, Quaternion.identity);
    }
}

public enum DungeonTileType
{
    Wall,
    Floor
}
