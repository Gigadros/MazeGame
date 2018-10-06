using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MapMaker : MonoBehaviour {

	public int tileWidth = 4;
	public int tileHeight = 4;
	public float xOffset = 0;
	public float zOffset = 0;
    public int mapSize = 50;
    public bool loadFromTxt;
    public GameObject level;
    public GameObject[] prefabs;
	public string characters;
	public TextAsset[] asciiMap;


    private int mapWidth;    
	private string map;
    private NavMeshSurface surface;
    private int[,] mapGrid;
    private int xPos, yPos, dir;
    private int steps;
    private string proceduralMap = "";
    private bool hasSpawn = false;
    // private float wave;
	
	void Start() {
        GenerateMap(loadFromTxt);
    }

    private void GenerateMap(bool loadFromTxt) {
        level = Instantiate(level);
        level.name = "Level";
        if (loadFromTxt) {
            for (int i = 0; i < asciiMap.Length; i++) LoadMap(asciiMap[i], i);
        } else {
            // procedurally generate map
            mapGrid = new int[mapSize, mapSize];
            // set map digger start position to middle of map
            xPos = mapSize / 2;
            yPos = mapSize / 2;
            steps = mapSize * mapSize / 4;
            for (int i = 0; i < steps; i++) {
                //move digger
                //wave = Mathf.Sin(steps/mapSize) * 0.5f + 1f;
                if (Random.Range(0f, 1f) > 0.75f) dir = Random.Range(0, 4);
                switch (dir) {
                    case 0:
                        xPos++;
                        break;
                    case 1:
                        yPos++;
                        break;
                    case 2:
                        xPos--;
                        break;
                    case 3:
                        yPos--;
                        break;
                }
                // prevent digger from out of bounds
                xPos = Mathf.Clamp(xPos, 1, mapSize - 2);
                yPos = Mathf.Clamp(yPos, 1, mapSize - 2);
                mapGrid[xPos, yPos] = 1; // set current position to floor
            }
            for (int y = 1; y < mapSize-1; y++) {
                for (int x = 1; x < mapSize-1; x++) {
                    if(mapGrid[x,y] == 1) {
                        if (mapGrid[x + 1, y] == 0) mapGrid[x + 1, y] = 2;
                        if (mapGrid[x - 1, y] == 0) mapGrid[x - 1, y] = 2;
                        if (mapGrid[x, y + 1] == 0) mapGrid[x, y + 1] = 2;
                        if (mapGrid[x, y - 1] == 0) mapGrid[x, y - 1] = 2;
                    }
                }
            }
                    for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    switch (mapGrid[x, y]) {
                        case 0:
                            proceduralMap += "H";
                            break;
                        case 1:
                            if (!hasSpawn) {
                                proceduralMap += "C";
                                hasSpawn = true;
                            } else {
                                proceduralMap += "O";
                            }
                            break;
                        case 2:
                            proceduralMap += "X";
                            break;
                    }
                }
                proceduralMap += "\n";
            }
            mapWidth = mapSize + 1;
            ParseMap(proceduralMap, 1);
        }
        surface = level.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

	private IEnumerator WaitForSecondsThenLoadMap(int wait, int floor) {
		yield return new WaitForSeconds(wait);
		//DestroyMap(0);
		LoadMap(asciiMap[floor], floor);
	}
	
	public void LoadMap(TextAsset asciiMap, int floor) {
		mapWidth = asciiMap.text.IndexOf("\n") + 1;
		ParseMap(asciiMap.text, floor);
	}

	private void ParseMap(string map, int floor) {
		float x = 0 , y = floor * tileHeight, z = 0;       
		GameObject prefab;
        GameObject currentObject;
        
		for (int i = 0; i < map.Length ; i++) {
			prefab = getPrefabForCharacter(map[i]);
			if (prefab != null) {
				currentObject = Instantiate(prefab, new Vector3((x * tileWidth) + xOffset, y, (z * -tileWidth) + zOffset), Quaternion.identity);
                currentObject.transform.parent = level.transform;
			}
			x++;
			if (x == mapWidth) {
				x = 0; z++;
			}
		}
	}
	
	public void DestroyMap(float delay) {
		GameObject.Destroy(level, delay);
	}
	
	public string getMappingsAsString() {
		string result = "";
		for (int i = 0; i < characters.Length || i < prefabs.Length; i++) {
			result += characters[i] + " = " + prefabs[i].name;
			if (i != prefabs.Length - 1) {
				result += ", ";
			} else {
				result += ".";
			}
		}
		if (characters.Length > prefabs.Length) {
			result += " There are " + (characters.Length - prefabs.Length) + " unmapped characters.";
		} else if (characters.Length < prefabs.Length) {
			result += " There are " + (prefabs.Length - characters.Length) + " unmapped prefabs.";
		}
		return result;
	}
	
	private GameObject getPrefabForCharacter(char c) {
		for (int i = 0; i < characters.Length || i < prefabs.Length; i++) {
			if (characters[i] == c) {
				return (prefabs[i]);
			}
		}
		return null;
	}
}