using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;

public class AutoChessController : MonoBehaviour
{
    enum TileType
    {
        none,
        wall,
        unit,
    }
    //variables
    public static int width = 15;      //tile map width
    public static int height = 12;     //tile map height
    public static float scale = 64f; //32
    Sprite tilePrefab;
    string message = "";

    public static float hexScale = scale + 4f; //addtional 4f makes tiles seperated
    float xScale = hexScale/2;
    float yScale = hexScale/2;

    List<int> passableValues;
    public static List<Unit> allUnits;
    public GameObject allMapTiles;     //the map and tiles
    public GameObject unitPrefab;

    private int turn = 0;
    private int team = 0;
    public static Dictionary<Vector2Int, int> map;

    public Vector2Int nullVector;

    //List<Vector2Int> path;


    // Start is called before the first frame update
    void Start()
    {
        StopAllCoroutines();

        // Camerea
        Camera.main.orthographicSize = scale * 5;
        Camera.main.gameObject.transform.position = new Vector3(width * scale / 2, height * scale / 4, -10);
        //tilePrefab = Resources.Load<Sprite>("Sprites/hex-sliced_113");
        // Ground texture
        Texture2D texture = Resources.Load<Texture2D>("Sprites/hex-sliced_113");
        tilePrefab = Sprite.Create(texture, new Rect(0, 0, scale, scale), new Vector2(0.5f, 0.5f), 1f);

        // add which tiles are passable
        passableValues = new List<int>();
        passableValues.Add((int)TileType.none);

        // Init map
        map = mapToDict6(generateMapArray(width, height));
        renderMap(map);

        allUnits = new List<Unit>();

        /*placeUnit(new Unit(new Vector2Int(2,3), 1));
        placeUnit(new Unit(new Vector2Int(1,1), 1));
        placeUnit(new Unit(new Vector2Int(2,1)));
        placeUnit(new Unit(new Vector2Int(3,0), 0));
        placeUnit(new Unit(new Vector2Int(0,3), 2));
        */

    }

    public void createUnit(Unit unit)
    {
        //GameObject unitGO = Instantiate(unitPrefab) as GameObject;
        GameObject unitGO = new GameObject("unit" + allUnits.Count);

        unitGO.AddComponent<SpriteRenderer>();
        unitGO.GetComponent<SpriteRenderer>().sprite = tilePrefab; //TODO get sprite
        unitGO.GetComponent<SpriteRenderer>().sortingOrder = 2;
        switch (unit.getAlliance())
        {
            case 0:
                unitGO.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case 1:
                unitGO.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case 2:
                unitGO.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                unitGO.GetComponent<SpriteRenderer>().color = Color.black;
                break;

        }
        unit.setGameObject(unitGO);
        allUnits.Add(unit);
    }

    public void placeUnit(Unit unit, Vector2Int pos)
    {
        //pos = pos == nullVector ? unit.getPos() : pos;
        if (!allUnits.Contains(unit))
        {
            createUnit(unit);
        }
        //map[pos] = (int)TileType.unit;
        unit.setPos(pos);
        setTransformPosition(unit, pos);
    }

    public void placeUnit(Unit unit)
    {
        Vector2Int pos = unit.getPos();
        if (!allUnits.Contains(unit))
        {
            createUnit(unit);
        }
        //map[pos] = (int)TileType.unit;
        unit.setPos(pos);
        setTransformPosition(unit, pos);
    }

    void OnGUI()
    {
        // END TURN
        if (GUI.Button(new Rect(10, 50, 150, 30), "End turn"))
        {
            message = $"Turn {turn}";
            /*foreach (Unit unit in allUnits)
            {
                Unit enemy = allUnits.Where(x => x.getAlliance() != unit.getAlliance()).First();
                Debug.Log(unit.getPos() +" vs "+ enemy.getPos());
                findPath(unit, enemy);
            }*/
            Unit ally = allUnits.Where(x => x.getAlliance() == 0).First();
            Unit enemy = allUnits.Where(x => x.getAlliance() == 1).First();
            Debug.Log(ally.getPos() +" vs "+ enemy.getPos());
            findPath(ally, enemy);


            turn += 1;
        }

        //SUMMON UNIT
        if (GUI.Button(new Rect(10, 10, 150, 30), "Summon unit"))
        {
            team = team == 1 ? 0 : 1;
            //while(true)

            Vector2Int random = new Vector2Int((int)Random.Range(0, width), (int)Random.Range(0, height));

            if (allUnits.Count == 0)
            {
                placeUnit(new Unit(random, team), random);
            }

            else if(allUnits.All(x => x.getPos() != random))
            {
                Debug.Log("summon " + random);
                placeUnit(new Unit(random, team), random);
                //break;
            }
            else
            {
                Debug.Log(random + " Occupied");
            }

            /*Debug.Log(map.Count + "  mapcount ");
            foreach (var item in map)
            {
                GUI.Label(new Rect(item.Key.x, item.Key.y, 150, 30), item.Key.ToString());
            }*/

        }

        if (GUI.Button(new Rect(10, 90, 150, 30), "Attack"))
        {
            foreach (Unit unit in allUnits)
            {
                Unit enemy = allUnits.Where(x => x.getAlliance() != unit.getAlliance()).First();
                attack(unit, enemy);

            }

        }

        /*if (GUI.Button(new Rect(10, 130, 150, 30), "See coordinates"))
        {
            foreach (var key in map.Keys) {
                GUI.Label(new Rect(key.x, key.y, 150, 30), "hmm");
            }
        }
        */

        GUI.Label(new Rect(180, 20, 300, 30), message);


    }

    public void findPath(Unit active, Unit target)
    {

        //StopAllCoroutines();

        var activePos = active.getPos();
        var targetPos = target.getPos();

        //find
        List<Vector2Int> path;
        path = PathFinding2D.find6X(activePos, targetPos, map, passableValues);
        Debug.Log("--"+activePos + targetPos + map.Count + " "+ path.Count);

        if (path.Count == 0)
        {
            message = "Can find target";
        }
        else
        {
            StartCoroutine(moveUnit(active, path));
        }
    }

    IEnumerator moveUnit(Unit unit, List<Vector2Int> path)
    {
        if(path.Count > 1)
        {
            path.RemoveAt(0);
            setTransformPosition(unit, path[0]);
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

    void setTransformPosition(Unit unit, Vector2Int pos)
    {
        Transform trans = unit.getGameObject().transform;
        if (pos.y % 2 == 0)
        {
            trans.position = new Vector2(pos.x * xScale * 2, pos.y * yScale);
        }
        else
        {
            trans.position = new Vector2((2 * pos.x + 1) * xScale, pos.y * yScale);
        }
        unit.setPos(pos);
    }

    void renderMap(Dictionary<Vector2Int, int> map)
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

    int[,] generateMapArray(int pwidth, int pheight)
    {
        var mapArray = new int[pwidth, pheight];
        for (int x = 0; x < pwidth; x++)
        {
            for (int y = 0; y < pheight; y++)
            {
                mapArray[x, y] = (int)TileType.none;
                //mapArray[x, y] = Random.Range(0, 100) < obstacleFillPercent ? (int)TileType.wall : (int)TileType.none;
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

    void attack(Unit active, Unit target)
    {
        string a = active.getAlliance() + "[" + active.getHealth() + "]:" + active.getPos();
        string b = target.getAlliance() + "[" + active.getHealth() + "]:" + target.getPos();
        Debug.Log(a + " attacked " + b + " for " + target.getAttack() + " dmg");
        target.setHealth(target.getHealth() - active.getAttack());

        a = active.getAlliance() + "[" + active.getHealth() + "]:" + active.getPos();
        b = target.getAlliance() + "[" + active.getHealth() + "]:" + target.getPos();
        if (target.getHealth() > 0 )
        {
            Debug.Log("> " + b);
        }
        else
        {
            Debug.Log("> " + b + " died");
            allUnits.Remove(target);
        }
    }



}
