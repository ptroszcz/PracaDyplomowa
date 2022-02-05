using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int x;

    public int y;

    public int width;

    public int height;

    public Direction enterCorridorDirection;

    public void GenerateFirstRoom(
        Quantity roomWidth,
        Quantity roomHeight,
        int dungeonWidth,
        int dungeonHeight
    )
    {
        this.width = roomWidth.Random;
        this.height = roomHeight.Random;

        x = Mathf.RoundToInt(dungeonWidth / 2f - this.width / 2f);
        y = Mathf.RoundToInt(dungeonHeight / 2f - this.height / 2f);
    }

    public void GenerateRoomBasedOnCorridor(
        Quantity roomWidth,
        Quantity roomHeight,
        int dungeonWidth,
        int dungeonHeight,
        Corridor corridor
    )
    {
        enterCorridorDirection = corridor.direction;

        this.width = roomWidth.Random;
        this.height = roomHeight.Random;

        switch (enterCorridorDirection)
        {
            case Direction.North:
                this.height =
                    Mathf.Clamp(this.height, 1, dungeonHeight - corridor.endY);
                y = corridor.endY;
                x = Random.Range(corridor.endX - this.width + 1, corridor.endX);
                x = Mathf.Clamp(x, 0, dungeonWidth - this.width);
                break;
            case Direction.South:
                this.height = Mathf.Clamp(this.height, 1, corridor.endY);
                y = corridor.endY - this.height;
                x = Random.Range(corridor.endX - this.width + 1, corridor.endX);
                x = Mathf.Clamp(x, 0, dungeonWidth - this.width);
                break;
            case Direction.East:
                this.width =
                    Mathf.Clamp(this.width, 1, dungeonWidth - corridor.endX);
                x = corridor.endX;
                y =
                    Random
                        .Range(corridor.endY - this.height + 1, corridor.endY);
                y = Mathf.Clamp(y, 0, dungeonHeight - this.height);
                break;
            case Direction.West:
                this.width = Mathf.Clamp(this.width, 1, corridor.endX);
                x = corridor.endX - this.width;
                y =
                    Random
                        .Range(corridor.endY - this.height + 1, corridor.endY);
                y = Mathf.Clamp(y, 0, dungeonHeight - this.height);
                break;
        }
    }
}
