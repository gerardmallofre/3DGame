using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Tilemaps;
using UnityEditorInternal;

public class CreateLevel : MonoBehaviour
{
    public GameObject player;                   // Reference to the player object.
    private int enemies = 0;
    private GameObject door;
    private float timepassed = 0.0f;
                                                // We need to position it according to the level.
    [SerializeField] public GameObject ground, wall, box, coin, slime, exitdoor;  // References to objects we need to instantiate to
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
                            case 5:
                                obj = Instantiate(box, new Vector3(x, 0.0f, y), transform.rotation);
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

    void Update()
    {
        timepassed += Time.deltaTime;
        if (timepassed > 6)
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Ground")
                {
                    child.GetComponent<GroundHandler>().setFallState(FallState.FALL);
                }
                timepassed = 10f;
            }
        }
        else if (timepassed > 5)
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Ground")
                {
                    child.GetComponent<GroundHandler>().setFallState(FallState.SHAKE);
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
}
