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
    [SerializeField] GameObject transition;
    float transitionDuration;
    float transitionTime=-1f;
    bool pendingTransition = false;
    private bool rowFallen = false;
    private int enemies = 0;
    private GameObject door;
    private float timepassed = 0.0f;
    private int fallingrow = 0;
    // We need to position it according to the level.
    [SerializeField]
    public GameObject ground, coin, slime, exitdoor, giant,
        spikeTrap, slimetile, axeTrap, bomb, goblin, emptyWall, outerWall, arrowTrap,
        torch, chest, barrel, carpet, pileBones, rock,  hole;

    private int level = -1;
    private TextAsset[] levels = new TextAsset[10];
    [SerializeField] private TextAsset level1;
    [SerializeField] private TextAsset level2;
    [SerializeField] private TextAsset level3;
    [SerializeField] private TextAsset level4;
    [SerializeField] private TextAsset level5;
    [SerializeField] private TextAsset level6;
    [SerializeField] private TextAsset level7;
    [SerializeField] private TextAsset level8;
    [SerializeField] private TextAsset level9;
    [SerializeField] private TextAsset level10;

    // Start is called before the first frame update
    void Start()
    {
        levels[0] = level1;
        levels[1] = level2;
        levels[2] = level3;
        levels[3] = level4;
        levels[4] = level5;
        levels[5] = level6;
        levels[6] = level7;
        levels[7] = level8;
        levels[8] = level9;
        levels[9] = level10;
        advanceLevel();

        transitionDuration = transition.GetComponent<ScreenTransitionHandler>().getDuration();
    }

    public void enteredDoor()
    {
        transition.GetComponent<ScreenTransitionHandler>().transition();
        transitionTime = 0f;
        pendingTransition = true;
        player.GetComponent<PlayerHandler>().allowControl(false);

        falls = false;
        falling = false;
        rowFallen = false;
        timepassed = 0f;
        fallingrow = 0;
    }

    public void advanceLevel()
    {
        level += 1;

        if (level == 10)
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
        bool spikeMakesSound = true;

        player.GetComponent<MovePlayer>().stopMove();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        string text = levels[level].text;
        string[] lines = text.Split('\n');
        int currentLine = 0;
        string line = lines[currentLine].Trim(); ++currentLine;
        if (line == "FALLS")
        {
            falls = true;
        }
        line = lines[currentLine]; ++currentLine;
        string[] tokens = line.Split(' ');
        int width, height;
        width = int.Parse(tokens[0]);
        height = int.Parse(tokens[1]);
        for (int y = height; y >= -1; y--)
        {
            if (y != height && y != -1) {
                line = lines[currentLine].Trim(); ++currentLine;
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
                    string tile = tokens[x];
                    GameObject obj;
                    if (tile != "k")
                    {
                        obj = Instantiate(ground, new Vector3(x, 0.0f, y), transform.rotation);
                        obj.transform.parent = transform;
                    }

                    switch (tile)
                    {
                        case "1":
                            obj = Instantiate(coin, new Vector3(x, 1.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "2":
                            obj = Instantiate(rock, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "3":
                            obj = Instantiate(slime, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<SlimeHandler>().setLevelCreator(this.transform.gameObject);
                            ++enemies;
                            break;
                        case "4":
                            obj = Instantiate(giant, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<GiantHandler>().setLevelCreator(this.transform.gameObject);
                            obj.GetComponent<GiantHandler>().setPlayer(player);
                            ++enemies;
                            break;
                        case "5":
                            obj = Instantiate(spikeTrap, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            if (spikeMakesSound)
                            {
                                spikeMakesSound = false;
                                obj.GetComponent<SpikeTrapHandler>().enableSound();
                            }
                            break;
                        case "6":
                            player.transform.localPosition = new Vector3(x, 0.0f, y);
                            break;
                        case "7":
                            obj = Instantiate(axeTrap, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "8":
                            obj = Instantiate(axeTrap, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<AxeHandler>().invertDirection();
                            break;
                        case "9":
                            obj = Instantiate(bomb, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            BombHandler b = obj.GetComponent<BombHandler>();
                            b.setPlayer(player);
                            b.setLevelCreator(this.transform.gameObject);
                            ++enemies;
                            break;
                        case "a":
                            obj = Instantiate(goblin, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<GoblinHandler>().setLevelCreator(this.transform.gameObject);
                            obj.GetComponent<GoblinHandler>().setPlayer(player);
                            ++enemies;
                            break;
                        case "b":
                            obj = Instantiate(arrowTrap, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<ArrowTrapHandler>().setLevelCreator(this.transform.gameObject);
                            obj.GetComponent<ArrowTrapHandler>().setDirection(Direction.RIGHT);
                            break;
                        case "c":
                            obj = Instantiate(arrowTrap, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<ArrowTrapHandler>().setLevelCreator(this.transform.gameObject);
                            obj.GetComponent<ArrowTrapHandler>().setDirection(Direction.LEFT);
                            break;
                        case "d":
                            obj = Instantiate(arrowTrap, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<ArrowTrapHandler>().setLevelCreator(this.transform.gameObject);
                            obj.GetComponent<ArrowTrapHandler>().setDirection(Direction.UP);
                            break;
                        case "e":
                            obj = Instantiate(arrowTrap, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            obj.GetComponent<ArrowTrapHandler>().setLevelCreator(this.transform.gameObject);
                            obj.GetComponent<ArrowTrapHandler>().setDirection(Direction.DOWN);
                            break;
                        case "f":   // Torxa 
                            obj = Instantiate(torch, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "g":   // Cofre
                            obj = Instantiate(chest, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "h":   // Barril
                            obj = Instantiate(barrel, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "i":   // Catifa 
                            obj = Instantiate(carpet, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "j":   // Pila d'ossos
                            obj = Instantiate(pileBones, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case "k":   // Forat
                            obj = Instantiate(hole, new Vector3(x, 0.0f, y), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                    }
                }
            }
        }
    }

    public void restart()
    {
        level = -1;
        advanceLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) { level = -1; advanceLevel(); HUDManager.Instance?.SetRoom(1); }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) { level = 0; advanceLevel(); HUDManager.Instance?.SetRoom(2); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { level = 1; advanceLevel(); HUDManager.Instance?.SetRoom(3); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { level = 2; advanceLevel(); HUDManager.Instance?.SetRoom(4); }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { level = 3; advanceLevel(); HUDManager.Instance?.SetRoom(5); }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { level = 4; advanceLevel(); HUDManager.Instance?.SetRoom(6); }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { level = 5; advanceLevel(); HUDManager.Instance?.SetRoom(7); }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { level = 6; advanceLevel(); HUDManager.Instance?.SetRoom(8); }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) { level = 7; advanceLevel(); HUDManager.Instance?.SetRoom(9); }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) { level = 8; advanceLevel(); HUDManager.Instance?.SetRoom(10); }


        if (transitionTime > transitionDuration) transitionTime = -1;
        else if (transitionTime > transitionDuration / 2 && pendingTransition == true)
        {
            advanceLevel();
            pendingTransition = false;
            player.GetComponent<PlayerHandler>().allowControl(true);
        }
        if (transitionTime > -1) transitionTime += Time.deltaTime;

        timepassed += Time.deltaTime;
        if (Input.GetKey(KeyCode.M)) SceneController.ChangeToMenuScene();
        if (Input.GetKeyDown(KeyCode.G)) player.GetComponent<PlayerHandler>().toggleGodMode(); 
        if (falls)
        {
            if (!falling && timepassed > fallstart)
            {
                falling = true;
                timepassed = fallinterval;
                AudioManager.instance.PlayCrumble();
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
            if (child.gameObject.tag == "SlimeTile" && Mathf.Abs(child.localPosition.z - pos.z)<0.3 && Mathf.Abs(child.localPosition.x - pos.x)<0.3)
            {
                Destroy(child.gameObject);
                break;
            }
        }
        GameObject obj = Instantiate(slimetile, new Vector3(pos.x, 0.1f, pos.z), new Quaternion(0f, 0f, 0f, 0f));
        obj.transform.parent = transform;
    }
}