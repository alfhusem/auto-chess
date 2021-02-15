using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Example : MonoBehaviour
{
    enum TileType
    {
        none,
        wall,
    }
    // permanent??
    public int width = 15;      //tile map width
    public int height = 12;     //tile map height
    public int obstacleFillPercent = 90;    //30 -tile map obstacle fill percent
    public float scale = 64f; //32

    Sprite tilePrefab;
    string message = "";

    List<int> passableValues;

    GameObject allMapTiles;     //the map and tiles
    GameObject player;          //the player
    GameObject goal;            //the goal

    Unit playerUnit;
    Unit goalUnit;

    private int turn = 0;
    Dictionary<Vector2Int, int> map;


    void Start()
    {
        Camera.main.orthographicSize = scale * 10;
        Camera.main.gameObject.transform.position = new Vector3(width * scale, height * scale/2, -10);
        //tilePrefab = Resources.Load<Sprite>("Sprites/hex-sliced_113");
        Texture2D texture = Resources.Load<Texture2D>("Sprites/hex-sliced_113");
        tilePrefab = Sprite.Create(texture, new Rect(0, 0, scale*2, scale*2), new Vector2(0.5f, 0.5f), 1f);
        //tilePrefab = Sprite.Create(new Texture2D((int)scale, (int)scale), new Rect(0, 0, scale, scale), new Vector2(0.5f, 0.5f), 1f);

        goal = new GameObject("goal");
        goal.AddComponent<SpriteRenderer>();
        goal.GetComponent<SpriteRenderer>().sprite = tilePrefab;
        goal.GetComponent<SpriteRenderer>().color = Color.yellow;
        goal.GetComponent<SpriteRenderer>().sortingOrder = 1;

        player = new GameObject("player");
        player.AddComponent<SpriteRenderer>();
        player.GetComponent<SpriteRenderer>().sprite = tilePrefab;
        player.GetComponent<SpriteRenderer>().color = Color.red;
        player.GetComponent<SpriteRenderer>().sortingOrder = 2;


        passableValues = new List<int>();
        passableValues.Add((int)TileType.none);

        initPathFinding6();
    }

    void OnGUI()
    {

        if (GUI.Button(new Rect(10, 50, 150, 30), "Start"))
        {
            message = $"Turn {turn}";
            simPathFinding6(playerUnit, goalUnit);
            //simPathFinding6(goalUnit, playerUnit);
            turn += 1;
        }

        GUI.Label(new Rect(180, 20, 300, 30), message);
    }

    public void initPathFinding6()
    {
        StopAllCoroutines();

        //init map
        map = mapToDict6(generateMapArray(width, height));
        var hexScale = scale*2 + 4f; //addtional 4f makes tiles seperated
        float xScale = hexScale / 2;
        float yScale = hexScale/2;
        renderMap(map, xScale, yScale);

        //init player and goal
        var mapPoses = map.Keys.ToList();
        mapPoses.Sort((a, b) => a.x + a.y - b.x - b.y);

        //var playerPos = mapPoses.First();
        var playerPos = new Vector2Int(1,3);
        playerUnit = new Unit(playerPos);
        map[playerPos] = (int)TileType.none;
        setTransformPosition(playerUnit, player.transform, playerPos, xScale, yScale);

        //var goalPos = mapPoses.Last();
        var goalPos = new Vector2Int(10, 15);
        goalUnit = new Unit(goalPos);
        map[goalPos] = (int)TileType.none;
        setTransformPosition(goalUnit, goal.transform, goalPos, xScale, yScale);

    }

    /**
     * simulate path finding in hexagonal grid tilemaps
     */
    public void simPathFinding6(Unit playerUnit, Unit goalUnit)
    {
        var hexScale = scale*2 + 4f; //addtional 4f makes tiles seperated
        float xScale = hexScale / 2;
        float yScale = hexScale/2;

        var playerPos = playerUnit.getPos();
        var goalPos = goalUnit.getPos();
        Debug.Log("pp " + playerPos);
        Debug.Log("gp " + goalPos);
        //find
        List<Vector2Int> path;

        path = PathFinding2D.find6X(playerPos, goalPos, map, passableValues);

        foreach (Vector2Int item in path) {
            Debug.Log("-->" + item);
        }

        if (path.Count == 0)
        {
            message = "oops! cant find goal";
        }
        else
        {
            StartCoroutine(movePlayerStep(path, xScale, yScale));
        }
    }


    void setTransformPosition(Unit unit, Transform trans, Vector2Int pos, float xScale, float yScale)
    {
        trans.position = new Vector2(pos.x * xScale, pos.y * yScale);
        unit.setPos(pos);
        Debug.Log("newPos  " + pos);
    }

    void renderMap(Dictionary<Vector2Int, int> map, float xScale, float yScale)
    {
        Destroy(allMapTiles);
        allMapTiles = new GameObject("allMapTiles");
        foreach (var item in map)
        {
            GameObject temp = new GameObject();
            temp.transform.position = new Vector2(item.Key.x * xScale, item.Key.y * yScale);
            SpriteRenderer spr = temp.AddComponent<SpriteRenderer>();
            spr.sprite = tilePrefab;
            switch (item.Value)
            {
                case (int)TileType.none:
                    spr.color = Color.white;
                    break;
                case (int)TileType.wall:
                    spr.color = Color.black;
                    break;
            }
            temp.transform.parent = allMapTiles.transform;
        }
    }

    IEnumerator movePlayer(List<Vector2Int> path, float xScale, float yScale, float interval = 0.1f)
    {
        foreach(var item in path)
        {
            setTransformPosition(playerUnit, player.transform, item, xScale, yScale);
            yield return new WaitForSeconds(interval);
        }

        message = "reach goal !";
    }

    IEnumerator movePlayerStep(List<Vector2Int> path, float xScale, float yScale)
    {
        if(path.Count > 0)
        {
            path.RemoveAt(0);
            var item = path[0];
            setTransformPosition(playerUnit, player.transform, item, xScale, yScale);
            yield return null;
        }
        else if (path.Count == 1) {
            message = "Enemy in range";
        }
        else
        {
            message = "Reach goal!";
        }
    }

    int[,] generateMapArray(int pwidth, int pheight)
    {
        var mapArray = new int[pwidth, pheight];
        for (int x = 0; x < pwidth; x++)
        {
            for (int y = 0; y < pheight; y++)
            {
                mapArray[x, y] = Random.Range(0, 100) < obstacleFillPercent-30 ? (int)TileType.wall : (int)TileType.none;
            }
        }
        return mapArray;
    }

    Dictionary<Vector2Int, int> mapToDict6(int[,] mapArray)
    {
        Dictionary<Vector2Int, int> mapDict = new Dictionary<Vector2Int, int>();
        for (int x = 0; x < mapArray.GetLength(0); x++)
        {
            for (int y = 0; y < mapArray.GetLength(1); y++)
            {

                if (y % 2 == 0)
                {
                    mapDict.Add(new Vector2Int(2 * x, y), mapArray[x, y]);
                }
                else
                {
                    mapDict.Add(new Vector2Int(2 * x + 1, y), mapArray[x, y]);
                }

            }
        }
        return mapDict;
    }
}
