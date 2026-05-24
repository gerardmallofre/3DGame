using System.IO;
using UnityEngine;

public class CreateLevel : MonoBehaviour
{
    public GameObject player;                   // Reference to the player object.
    private bool falls;
    private bool falling;
    [SerializeField] float fallstart = 5f;
    [SerializeField] float fallinterval = 2f;
    [SerializeField] float shakelength = 1f;
    private bool rowFallen = false;
    private int enemies = 0;
    private GameObject door;
    private float timepassed = 0.0f;
    private int fallingrow = 0;
    // We need to position it according to the level.
    [SerializeField] public GameObject ground, wall, box, coin, slime, exitdoor, giant, spikeTrap, slimetile, axeTrap, bomb, goblin, emptyWall, outerWall;

    private int level = -1;
    private string[] levels = new string[10];

    // Start is called before the first frame update
    void Start()
    {
        levels[0] = "/Maps/map.txt";
        levels[1] = "/Maps/map2.txt";
        levels[2] = "/Maps/map3.txt";
        levels[3] = "/Maps/map4.txt";
        levels[4] = "/Maps/map5.txt"; // El Boss final
        advanceLevel();
    }

    public void advanceLevel()
    {
        level += 1;

        if (level > 4)
        {
            SceneController.ChangeToCreditsScene();
            return; 
        }

        falls = false;
        falling = false;
        rowFallen = false;
        timepassed = 0f;
        fallingrow = 0;
        enemies = 0;

        string filename = Application.dataPath + levels[level];
        player.GetComponent<MovePlayer>().stopMove();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (File.Exists(filename))
        {
            TextReader reader = File.OpenText(filename);
            string line = reader.ReadLine();
            if (line == "FALLS")
            {
                falls = true;
            }
            line = reader.ReadLine();
            string[] tokens = line.Split(' ');
            int width, height;
            width = int.Parse(tokens[0]);
            height = int.Parse(tokens[1]);
            for (int y = height; y >= -1; y--)
            {
                if (y != height && y != -1) {
                    line = reader.ReadLine();
                    tokens = line.Split(" ");
                }
                for (int x = -1; x < width + 1; x++)
                {
                    if (y == height || y == -1 || x == -1 || x == width)
                    {
                        if (x == width / 2 && y == height)
                        {
                            GameObject obj = Instantiate(ground, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj = Instantiate(exitdoor, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<DoorHandler>().setLevelCreator(this.transform.gameObject);
                            door = obj;
                        }
                        else {
                            GameObject obj = Instantiate(emptyWall, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            if ((y == height && x != -1 && x != width) || (x == -1 && y != -1 && y!=height))
                            {
                                obj = Instantiate(outerWall, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                if (y != height)
                                {
                                    obj.GetComponent<OuterWallDecorator>().setLeftWall(true);
                                    obj.transform.Rotate(0, -90, 0);
                                }
                                obj.GetComponent<OuterWallDecorator>().decorate();
                            }
                        }
                    }
                    else {
                        GameObject obj = Instantiate(ground, new Vector3(x, 0.0f, y), transform.rotation);
                        obj.transform.parent = transform;
                        int tile = int.Parse(tokens[x]);

                        switch (tile)
                        {
                            case 1:
                                obj = Instantiate(coin, new Vector3(x, 1.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                break;
                            case 2:
                                obj = Instantiate(wall, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                break;
                            case 3:
                                obj = Instantiate(slime, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                obj.GetComponent<SlimeHandler>().setLevelCreator(this.transform.gameObject);
                                ++enemies;
                                break;
                            case 4:
                                obj = Instantiate(giant, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                obj.GetComponent<SlimeHandler>().setLevelCreator(this.transform.gameObject);
                                ++enemies;
                                break;
                            case 5:
                                obj = Instantiate(spikeTrap, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                break;
                            case 6:
                                player.transform.localPosition = new Vector3(x, 0.0f, y);
                                break;
                            case 7:
                                obj = Instantiate(axeTrap, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                break;
                            case 8:
                                obj = Instantiate(bomb, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                BombHandler b = obj.GetComponent<BombHandler>();
                                b.setPlayer(player);
                                b.setLevelCreator(this.transform.gameObject);
                                ++enemies;
                                break;
                            case 9:
                                obj = Instantiate(goblin, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                obj.GetComponent<SlimeHandler>().setLevelCreator(this.transform.gameObject);
                                ++enemies;
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Error: No es troba el mapa! Buscant a la ruta: " + filename);
        }
    }

    public void restart()
    {
        level = -1;
        advanceLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { level = -1; advanceLevel(); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { level = 0; advanceLevel(); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { level = 1; advanceLevel(); }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { level = 2; advanceLevel(); }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { level = 3; advanceLevel(); }

        timepassed += Time.deltaTime;
        if (Input.GetKey(KeyCode.M)) SceneController.ChangeToMenuScene();

        if (falls)
        {
            if (!falling && timepassed > fallstart)
            {
                falling = true;
                timepassed = fallinterval;
            }
            else if (falling)
            {
                if (timepassed > fallinterval)
                {
                    foreach (Transform child in transform)
                    {
                        if (child.gameObject.tag == "Ground" && child.localPosition.z == fallingrow)
                        {
                            child.GetComponent<GroundHandler>().setFallState(FallState.SHAKE);
                        }
                    }
                    timepassed = 0f;
                    rowFallen = false;
                }
                else if (!rowFallen && timepassed > shakelength)
                {
                    foreach (Transform child in transform)
                    {
                        if (child.gameObject.tag == "Ground" && child.localPosition.z == fallingrow)
                        {
                            child.GetComponent<GroundHandler>().setFallState(FallState.FALL);
                        }
                    }
                    fallingrow += 1;
                    rowFallen = true;
                }
            }
        }
    }

    public void enemyKilled()
    {
        --enemies;
        if (enemies <= 0)
        {
            door.GetComponent<DoorHandler>().open();
        }
    }

    public void slimeTile(Vector3 pos)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "SlimeTile" && child.localPosition.z == pos.z && child.localPosition.x == pos.x)
            {
                Destroy(child.gameObject);
                break;
            }
        }
        GameObject obj = Instantiate(slimetile, new Vector3(pos.x, 0.1f, pos.z), new Quaternion(0f, 0f, 0f, 0f));
        obj.transform.parent = transform;
    }
}