using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class CreateLevel : MonoBehaviour
{
    public GameObject player;                   // Reference to the player object.
    private bool falls;
    private bool falling;
    [SerializeField] float fallstart = 5f;
    [SerializeField] float fallinterval = 2f;
    [SerializeField] float shakelength = 1f;
    private SceneController sc = new SceneController();
    private bool rowFallen = false;
    private int enemies = 0;
    private GameObject door;
    private float timepassed = 0.0f;
    private int fallingrow = 0;
                                                // We need to position it according to the level.
    [SerializeField] public GameObject ground, wall, box, coin, slime, exitdoor, goomba, spikeTrap, slimetile;  // References to objects we need to instantiate to
                                                                        // build the level.

    private int level = -1;
    private string[] levels = new string[10];

    // Start is called before the first frame update
    void Start()
    {
        levels[0] = "/Maps/map.txt";
        levels[1] = "/Maps/map2.txt";
        // dataPath is the directory path to the Assets in the project
        // We want to load file map.txt inside directory Maps.
        advanceLevel();
    }

    public void advanceLevel()
    {
        level += 1;
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
            // Read map file line by line tokenizing them into the numbers we need.
            // Specifically, width, height and the tile ids for all positions in the map.

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
            for (int y = height - 1; y >= 0; y--)
            {
                line = reader.ReadLine();
                tokens = line.Split(" ");
                for (int x = 0; x < width; x++)
                {
                    if (x == width/2 && y == height - 1)
                    {
                        // All tiles will have a ground instance under them. We instantiate it here.
                        GameObject obj = Instantiate(ground, new Vector3(x, 0.0f, y), transform.rotation);
                        // All instances created by this script end as children of the object that contains the script.
                        obj.transform.parent = transform;
                        obj = Instantiate(exitdoor, new Vector3(x, 0.0f, y), transform.rotation);
                        obj.transform.parent = transform;
                        obj.GetComponent<DoorHandler>().setLevelCreator(this.transform.gameObject);
                        door = obj;
                    }
                    else
                    {
                        int tile = int.Parse(tokens[x]);

                        // All tiles will have a ground instance under them. We instantiate it here.
                        GameObject obj = Instantiate(ground, new Vector3(x, 0.0f, y), transform.rotation);
                        // All instances created by this script end as children of the object that contains the script.
                        obj.transform.parent = transform;

                        // Now, for objects other than the player we spawn an instance.
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
                                obj = Instantiate(goomba, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                obj.GetComponent<SlimeHandler>().setLevelCreator(this.transform.gameObject);
                                ++enemies;
                                break;
                            case 5:
                                obj = Instantiate(spikeTrap, new Vector3(x, 0.0f, y), transform.rotation);
                                obj.transform.parent = transform;
                                break;
                            case 6:
                                // For the player, we position it at the location of the tile with the player tile id.
                                player.transform.localPosition=new Vector3(x, 0.0f, y);
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            // Hopefully this should not happen. But just in case ...
            Debug.Log("Map file could not be found!!!");
        }
    }

    public void restart()
    {
        level = -1;
        advanceLevel();
    }

    void Update()
    {
        timepassed += Time.deltaTime;
        if (Input.GetKey(KeyCode.P)) sc.changeScene("Prova");
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
                        if (child.gameObject.tag=="Ground" && child.localPosition.z == fallingrow)
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
        if (enemies == 0)
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
        GameObject obj=Instantiate(slimetile, new Vector3(pos.x, 0.1f, pos.z), new Quaternion(0f, 0f, 0f, 0f));
        obj.transform.parent = transform;
    }
}
