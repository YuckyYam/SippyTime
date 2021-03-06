using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

/* 
 * This class defines a random map generation script. This method starts off with some "snakes" starting at the middle
 * of the map, then makes those snakes move around in random directions. 
 */
public class MapGeneration : MonoBehaviour
{
    // Arrays of room prefabs to make the map with
    public GameObject[] deadEndRooms;
    public GameObject[] elbowRooms;
    public GameObject[] hallwayRooms;
    public GameObject[] tripleRooms;
    public GameObject[] quadRooms;
    public GameObject[] bossRooms;
    public GameObject chestPrefab;
    public GameObject[] enemyPrefabs;
    
    public int height = 11;
    public int width = 11;
    public int chestNumber;

    // The size of each chunk, in meters
    public float roomSize = 50;

    public int snakeNum = 3;
    public int snakeMoves = 30;
    public TurnTypeEnum turnType = TurnTypeEnum.Equal;
    public float turnChance = 0.5f;

    Chunk[,] board;
    
    // Determines whether a room is a dead end, elbow, hallway, triple-, or quadruple-way
    enum RoomType
    {
        DeadEnd, Elbow, Hallway, Triple, Quad, BossRoom
    }

    // Displays the map to the debug console
    void DisplayDebugMap()
    {
        string output = "";
        for (int i = 0; i < height; i++)
        {
            output += i + ": ";
            for (int j = 0; j < width; j++)
            {
                if (board[i, j].IsSet())
                    output += "+";
                else
                    output += "  ";
                output += " ";
            }
            
            output += "\n";
            Debug.Log(output);
            output = "";
        }
    }

    // Creates the map for the player
    void Start()
    {
        // keep generating boards until one is valid
        while ((board = GenerateBoard()) == null)
        {
        }

        //DisplayDebugMap();
        
        // Goes through every chunk in the board and instantiates it in the engine
        InstantiateRooms(board);
    }

    Chunk[,] GenerateBoard()
    {
        // Creates the board and sets the center of the map to be 1
        board = new Chunk[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                board[i, j] = new Chunk();
            }
        }

        int[] origin = {(int) Mathf.Floor(height / 2), (int) Mathf.Floor(width / 2)};
        board[origin[0], origin[1]].Set();

        // Instantiate "snakes" that move around the board, creating rooms
        int[,] snakes = new int[snakeNum, 3];
        for (int i = 0; i < snakeNum; i++)
        {
            // the three elements of the array are {x position, y position, direction}
            snakes[i, 0] = origin[0];
            snakes[i, 1] = origin[1];
            snakes[i, 2] = (int) Mathf.Floor(Random.Range(0, 3.999f));
        }
        
        List<Chunk> chunkList = new List<Chunk>();

        // Create the map layout by moving the "snakes"
        for (int m = 0; m < snakeMoves; m++)
        {
            for (int i = 0; i < snakeNum; i++)
            {
                int dir = snakes[i, 2];
                int dx = 0, dy = 0;

                switch (dir)
                {
                    case 0:
                        dy = -1;
                        break;
                    case 1:
                        dy = 1;
                        break;
                    case 2:
                        dx = -1;
                        break;
                    case 3:
                        dx = 1;
                        break;
                    default:
                        Debug.Log("invalid direction: " + dir);
                        break;
                }

                // Moves the snake vertically if it is not going out of bounds
                if ((0 <= snakes[i, 0] + dy) && (snakes[i, 0] + dy < height))
                {
                    board[snakes[i, 0], snakes[i, 1]].Connect(dy, 0);
                    snakes[i, 0] += dy;
                    board[snakes[i, 0], snakes[i, 1]].Connect(-dy, 0);
                }

                // Moves the snake horizontally if it is not going out of bounds
                if ((0 <= snakes[i, 1] + dx) && (snakes[i, 1] + dx < width))
                {
                    board[snakes[i, 0], snakes[i, 1]].Connect(0, dx);
                    snakes[i, 1] += dx;
                    board[snakes[i, 0], snakes[i, 1]].Connect(0, -dx);
                }

                // turns chunk into a room
                if (board[snakes[i, 0], snakes[i, 1]].Set())
                {
                    chunkList.Add(board[snakes[i, 0], snakes[i, 1]]);
                }

                // Turn snake after it has moved depending on the turn type
                switch (turnType)
                {
                    case TurnTypeEnum.Equal:
                        snakes[i, 2] = (int) Mathf.Floor(Random.Range(0, 3.999f));
                        break;
                    case TurnTypeEnum.Straight:
                        // it looks like there are some magic numbers here, but here is the explanation
                        // The 3f in the following line is there because there is a 3/N chance of turning,
                        // where N is 4 in Equal turn mode
                        int r = (int) Mathf.Floor(Random.Range(0, (3f / turnChance - .0001f)));
                        if (r >= 4)
                            break;
                        snakes[i, 2] = r;
                        break;
                    case TurnTypeEnum.ScrambleToStraight:
                        if (m < snakeMoves / 2)
                        {
                            snakes[i, 2] = (int) Mathf.Floor(Random.Range(0, (2.999f)));
                            break;
                        }
                        else
                        {
                            int randDir = (int) Mathf.Floor(Random.Range(0, (3f / turnChance - .0001f)));
                            if (randDir >= 4)
                                break;
                            snakes[i, 2] = randDir;
                            break;
                        }
                }
            }
        }
        
        // convert list of chunks to array
        Chunk[] chunks = ChunkListToArray(chunkList);
        chunkList.Clear();

        // Randomly determine which rooms chests should go in
        var hasBossRoom = SetChests(chunks);

        // if AddChests did not return true, search the rooms for a dead end
        if (!hasBossRoom)
        {
            foreach (var c in chunks)
            {
                if (c.GetRoomType() != RoomType.DeadEnd) continue;
                c.SetBoss();
                hasBossRoom = true;
                break;
            }
        }
        
        // if there is not a dead end for the boss room, create a new map
        if (!hasBossRoom) 
            return null;
        // makes it so the player cannot spawn in a dead end or a boss room
        if ((board[origin[0], origin[1]].GetRoomType() == RoomType.DeadEnd) ||
            (board[origin[0], origin[1]].GetRoomType() == RoomType.BossRoom))
            return null;
        
        return board;
    }

    // convert list of chunks to array
    Chunk[] ChunkListToArray(List<Chunk> chunkList)
    {
        var index = 0;
        Chunk[] chunks = new Chunk[chunkList.Count];
        foreach (var chunk in chunkList)
        {
            chunks[index] = chunk;
            index++;
        }

        return chunks;
    }

    // adds chests to chestNumber amount of rooms and turns the first dead end into a boss room. returns true if it makes a boss room
    bool SetChests(Chunk[] chunks)
    {
        var hasBossRoom = false;
        for (int i = 0; i < chunks.Length; i++)
        {
            var randomIndex = Random.Range(i, chunks.Length);
            var temp = chunks[i];
            chunks[i] = chunks[randomIndex];
            chunks[randomIndex] = temp;
        }
        
        var extraChests = 0;
        for (int i = 0; i < chestNumber + extraChests; i++)
        {
            if (i >= chunks.Length)
                break;
            if ((chunks[i].GetRoomType() == RoomType.DeadEnd) && (!hasBossRoom))
            {
                chunks[i].SetBoss();
                hasBossRoom = true;
                extraChests++;
                continue;
            }
            
            chunks[i].AddChest();
        }

        return hasBossRoom;
    }
    
    // instantiates the rooms with enemies and chests
    void InstantiateRooms(Chunk[,] board)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (board[i, j].IsSet())
                {
                    Chunk currentChunk = board[i, j];
                    RoomType roomType = currentChunk.GetRoomType();
                    GameObject currentRoomObject;
                    switch (roomType)
                    {
                        case RoomType.DeadEnd:
                            currentRoomObject = InstantiateDeadEnd(board, i, j);
                            break;
                        
                        case RoomType.Elbow:
                            currentRoomObject = InstantiateElbow(board, i, j);
                            break;
                        
                        case RoomType.Hallway:
                            currentRoomObject = InstantiateHallway(board, i, j);
                            break;
                        
                        case RoomType.Triple:
                            currentRoomObject = InstantiateTriple(board, i, j);
                            break;
                        
                        case RoomType.Quad:
                            currentRoomObject = InstantiateQuad(board, i, j);
                            break;
                        
                        case RoomType.BossRoom:
                            currentRoomObject = InstantiateBoss(board, i, j);
                            break;
                        default:
                            Debug.Log("invalid room type: " + roomType);
                            currentRoomObject = null;
                            break;
                    }
                    
                    // don't spawn enemies or chests in the start room
                    if ((i != (int) Mathf.Floor(height / 2)) || (j != (int) Mathf.Floor(width / 2)))
                        PopulateRoom(currentChunk, currentRoomObject);
                }
            }
        }
    }

    // creates a dead end room prefab with its own rotation requirements
    GameObject InstantiateDeadEnd(Chunk[,] board, int i, int j)
    {
        Chunk currentChunk = board[i, j];
        RoomType roomType = currentChunk.GetRoomType();
        Quaternion q = Quaternion.identity;
        
        // the dead end needs to be rotated depending on the rooms next to it
        if (currentChunk.HasUp())
            q = Quaternion.Euler(0, 180, 0);
        else if (currentChunk.HasDown())
            q = Quaternion.Euler(0, 0, 0);
        else if (currentChunk.HasLeft())
            q = Quaternion.Euler(0, 90, 0);
        else //(currentChunk.HasRight())
            q = Quaternion.Euler(0, -90, 0);

        // randomly picks a room from one in the list given in the engine
        int r = (int) Mathf.Floor(Random.Range(0, deadEndRooms.Length - .001f));
                            
        // rotates the room depending on its orientation in the editor
        q *= Quaternion.Euler(0, deadEndRooms[r].GetComponent<RoomProperties>().Rotation, 0);
        return Instantiate(deadEndRooms[r], new Vector3(roomSize*(i-width/2), 0, roomSize*(j-height/2)), q);
    }

    // creates a elbow room prefab with its own rotation requirements
    GameObject InstantiateElbow(Chunk[,] board, int i, int j)
    {
        Chunk currentChunk = board[i, j];
        RoomType roomType = currentChunk.GetRoomType();
        Quaternion q = Quaternion.identity;
        
        if (currentChunk.HasUp() && currentChunk.HasRight())
            q = Quaternion.Euler(0, -90, 0);
        else if (currentChunk.HasDown() && currentChunk.HasLeft())
            q = Quaternion.Euler(0, 90, 0);
        else if (currentChunk.HasLeft() && currentChunk.HasUp())
            q = Quaternion.Euler(0, 180, 0);
        else //(currentChunk.HasRight())
            q = Quaternion.Euler(0, 0, 0);
        int r = (int) Mathf.Floor(Random.Range(0, elbowRooms.Length - .001f));
        q *= Quaternion.Euler(0, elbowRooms[r].GetComponent<RoomProperties>().Rotation, 0);
        return Instantiate(elbowRooms[r], new Vector3(roomSize*(i-width/2), 0, roomSize*(j-height/2)), q);
    }
    
    
    // creates a hallway room prefab with its own rotation requirements
    GameObject InstantiateHallway(Chunk[,] board, int i, int j)
    {
        Chunk currentChunk = board[i, j];
        RoomType roomType = currentChunk.GetRoomType();
        Quaternion q = Quaternion.identity;

        if (currentChunk.HasUp())
            q = Quaternion.Euler(0, 0, 0);
        else // (currentChunk.HasRight())
            q = Quaternion.Euler(0, 90, 0);
        int r = (int) Mathf.Floor(Random.Range(0, hallwayRooms.Length - .001f));
        q *= Quaternion.Euler(0, hallwayRooms[r].GetComponent<RoomProperties>().Rotation, 0);
        return Instantiate(hallwayRooms[r], new Vector3(roomSize*(i-width/2), 0, roomSize*(j-height/2)), q);
    }
    
    
    // creates a three-way intersection room prefab with its own rotation requirements
    GameObject InstantiateTriple(Chunk[,] board, int i, int j)
    {
        Chunk currentChunk = board[i, j];
        RoomType roomType = currentChunk.GetRoomType();
        Quaternion q = Quaternion.identity;

        if (!currentChunk.HasUp())
            q = Quaternion.Euler(0, 180, 0);
        else if (!currentChunk.HasDown())
            q = Quaternion.Euler(0, 0, 0);
        else if (!currentChunk.HasLeft())
            q = Quaternion.Euler(0, 90, 0);
        else //(!currentChunk.HasRight())
            q = Quaternion.Euler(0, -90, 0);
        int r = (int) Mathf.Floor(Random.Range(0, tripleRooms.Length - .001f));
        q *= Quaternion.Euler(0, tripleRooms[r].GetComponent<RoomProperties>().Rotation, 0);
        return Instantiate(tripleRooms[r], new Vector3(roomSize*(i-width/2), 0, roomSize*(j-height/2)), q);
    }
    
    // creates a quad room prefab without rotating it
    GameObject InstantiateQuad(Chunk[,] board, int i, int j)
    {
        Chunk currentChunk = board[i, j];
        RoomType roomType = currentChunk.GetRoomType();
        Quaternion q = Quaternion.identity;

        int r = (int) Mathf.Floor(Random.Range(0, quadRooms.Length - .001f));
        return Instantiate(quadRooms[r], new Vector3(roomSize*(i-width/2), 0, roomSize*(j-height/2)), Quaternion.identity);
    }
    
    // creates a dead end boss room prefab with its own rotation requirements
    GameObject InstantiateBoss(Chunk[,] board, int i, int j)
    {
        Chunk currentChunk = board[i, j];
        RoomType roomType = currentChunk.GetRoomType();
        Quaternion q = Quaternion.identity;

        // the dead end needs to be rotated depending on the rooms next to it
        if (currentChunk.HasUp())
            q = Quaternion.Euler(0, 180, 0);
        else if (currentChunk.HasDown())
            q = Quaternion.Euler(0, 0, 0);
        else if (currentChunk.HasLeft())
            q = Quaternion.Euler(0, 90, 0);
        else //(currentChunk.HasRight())
            q = Quaternion.Euler(0, -90, 0);

        // randomly picks a room from one in the list given in the engine
        int r = (int) Mathf.Floor(Random.Range(0, deadEndRooms.Length));
                            
        // rotates the room depending on its orientation in the editor
        q *= Quaternion.Euler(0, bossRooms[r].GetComponent<RoomProperties>().Rotation, 0);
        return Instantiate(bossRooms[r], new Vector3(roomSize*(i-width/2), 0, roomSize*(j-height/2)), q);
    }

    // Adds chests and enemies to the newly created room
    void PopulateRoom(Chunk currentChunk, GameObject currentRoomObject)
    {
        // adds chest to the room if it is supposed to be there
        if (currentChunk.HasChest())
        {
            List<Transform> chestPositions = new List<Transform>();
            
            if (currentRoomObject is { })
                foreach (Transform child in currentRoomObject.transform)
                {
                    if (child.CompareTag("ChestSpawn"))
                    {
                        chestPositions.Add(child);
                    }
                }

            // place chest if there is a spot for it
            if (chestPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, chestPositions.Count);
                // the chest must be rotated according to how it was made in the editor (-90, 180, 0) and depending on which way it faces in the room and which way the room is facing
                Instantiate(chestPrefab, chestPositions[randomIndex].position, Quaternion.Euler(-90+chestPositions[randomIndex].rotation.x, chestPositions[randomIndex].rotation.y, chestPositions[randomIndex].rotation.z));
            }
            else
            {
                Debug.Log("Room has nowhere to place chest");
            }
        }
        // place enemies
        List<Transform> enemyPositions = new List<Transform>();
        foreach (Transform child in currentRoomObject.transform)
        {
            if (child.CompareTag("EnemySpawn"))
            {
                enemyPositions.Add(child);
            }
        }

        int[] order = RandomOrder(currentRoomObject.GetComponent<RoomProperties>().MinEnemies,
            currentRoomObject.GetComponent<RoomProperties>().MaxEnemies);
        if (order == null)
            return;
        foreach (var i in order)
        {
            //instantiate random enemy from list at enemyPositions[i].position;
            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomEnemyIndex], enemyPositions[i].position + Vector3.up * 5,
                Quaternion.Euler(-90 + enemyPositions[i].rotation.x,
                    enemyPositions[i].rotation.y, enemyPositions[i].rotation.z));
        }
    }

    // creates a randomly ordered list of indeces to index another array
    int[] RandomOrder(int min, int max)
    {
        if (min > max)
        {
            Debug.Log("invalid min and max");
            return null;
        }
            // create list from 0 to max-1
        int[] nums = new int[max];
        for (int i = 0; i < nums.Length; i++)
            nums[i] = i;

        // randomize that list
        for (int i = 0; i < nums.Length; i++)
        {
            var randomIndex = Random.Range(i, nums.Length);
            var temp = nums[i];
            nums[i] = nums[randomIndex];
            nums[randomIndex] = temp;
        }
        
        // randomly select how many to choose from (min to max)
        var length = Random.Range(min, max);
        int[] output = new int[length];
        for (int i = 0; i < length; i++)
            output[i] = nums[i];
        return output;
    }

    // Update is called once per frame this is unneeded as the generation happens then stops immediately
    void Update()
    {
        
    }

    // This nested class is a box in a grid to make the rooms fit together better
    private class Chunk
    {
        private bool up;
        private bool down;
        private bool left;
        private bool right;

        private bool _set;

        private int order;
        private bool hasChest;
        private bool isBossRoom;

        public Chunk()
        {
            up = false;
            down = false;
            left = false;
            right = false;

            _set = false;

            order = 0;
            hasChest = false;
            isBossRoom = false;
        }

        public bool Set()
        {
            var alreadySet = _set;
            _set = true;
            return !alreadySet;
        }

        public void Connect(int dy, int dx)
        {
            if (dy == 1)
                down = true;
            else if (dy == -1)
                up = true;
            else if (dx == 1)
                right = true;
            else if (dx == -1)
                left = true;
        }

        public void AddChest()
        {
            hasChest = true;
        }

        public void SetBoss()
        {
            isBossRoom = true;
        }

        public RoomType GetRoomType()
        {
            if (isBossRoom)
                return RoomType.BossRoom;
            order = 0; // the number of entrances/exits into the room
            if (up)
                order++;
            if (down)
                order++;
            if (left)
                order++;
            if (right)
                order++;
            switch (order)
            {
                case 1: // if there is only one way into the room, it is a dead end
                    return RoomType.DeadEnd;
                case 2: // if the room has two entrances, it is either a hallway or an elbow
                    // hallways are either vertical (up == down == true) or horizontal (up == down == false)
                    return up == down ? RoomType.Hallway : RoomType.Elbow; 
                case 3:
                    return RoomType.Triple;
                case 4:
                    return RoomType.Quad;
                default: // return quad type in case something goes wrong so that the player will always be able to enter and exit in any orientation
                    return RoomType.Quad; 
            }
        }

        public bool HasUp()
        {
            return up;
        }

        public bool HasLeft()
        {
            return left;
        }

        public bool HasRight()
        {
            return right;
        }

        public bool HasDown()
        {
            return down;
        }
        
        public bool IsSet()
        {
            return _set;
        }

        public bool HasChest()
        {
            return hasChest;
        }
    }
}

/*
     *  The turnType determines how the maps are randomly generated
     *  Equal: equally likely to go in any direction
     *  Straight: more likely to go straight. Equally likely for all other directions
     *  ScramleToStraight: more likely to turn at the beginning half, then more likely to go straight afterwards
     */
public enum TurnTypeEnum
{
    Equal,
    Straight,
    ScrambleToStraight
}