using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class describes variables for each room that can be used in other scripts
public class RoomProperties : MonoBehaviour
{
    // Used when the room in the editor does not line up with the x-axis
    public int Rotation = 0;
    // used when the room goes into the floor and would collide with the room underneath it
    public bool HasDepth = false;
    // used to dictate the min and max number of enemies that spawn in this room
    public int MinEnemies = 0;
    public int MaxEnemies = 0;
}
