using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Corridor
{
    public int x;

    public int y;

    public int length;

    public Direction direction;

    public int endX
    {
        get
        {
            if (direction == Direction.North || direction == Direction.South)
            {
                return this.x;
            }
            else
            {
                return this.x +
                (
                direction == Direction.East ? (length - 1) : (length * -1 + 1)
                );
            }
        }
    }

    public int endY
    {
        get
        {
            if (direction == Direction.East || direction == Direction.West)
            {
                return this.y;
            }
            else
            {
                return this.y +
                (
                direction == Direction.North ? (length - 1) : (-1 * length + 1)
                );
            }
        }
    }

    public void GenerateCorridor(
        Room room,
        Quantity corridorLength,
        Quantity roomWidth,
        Quantity roomHeight,
        int dungeonWidth,
        int dungeonHeight,
        bool isFirstCorridor
    )
    {
        direction = (Direction) Random.Range(0, 4);

        if (!isFirstCorridor)
        {
            Direction oppositeDirection =
                (Direction)(((int) room.enterCorridorDirection + 2) % 4);
            if (oppositeDirection == direction)
            {
                direction =
                    (
                    Direction
                    )(((int) direction + (Random.Range(0, 2) == 0 ? 1 : 3)) %
                    4);
            }
        }
        this.length = corridorLength.Random + 1;

        int maxLength = corridorLength.max;

        switch (direction)
        {
            case Direction.North:
                this.x = Random.Range(room.x, room.x + room.width - 1);
                this.y = room.y + room.height;
                maxLength = dungeonHeight - y - roomHeight.min;
                break;
            case Direction.South:
                this.x = Random.Range(room.x, room.x + room.width-1);
                this.y = room.y;
                maxLength = y - roomHeight.min;
                break;
            case Direction.West:
                this.x = room.x;
                this.y = Random.Range(room.y, room.y + room.height-1);
                maxLength = x - roomWidth.min;
                break;
            case Direction.East:
                this.x = room.x + room.width;
                this.y = Random.Range(room.y, room.y + room.height - 1);
                maxLength = dungeonWidth - x - roomWidth.min;
                break;
            default:
                this.x = -230;
                this.y = -500;
                break;
        }
        length = Mathf.Clamp(length, 0, maxLength);
    }
}

public enum Direction
{
    West,
    North,
    East,
    South
}
