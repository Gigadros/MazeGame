using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MapMaker : MonoBehaviour {

	public int tileWidth = 4;
	public int tileHeight = 4;
	public float xOffset = 0;
	public float zOffset = 0;
    public GameObject level;
    public GameObject[] prefabs;
	public string characters;
	public TextAsset[] asciiMap;

	private int mapWidth;    
	private string map;
    private NavMeshSurface surface;
	
	void Start() {
        GenerateMap();
    }

    private void GenerateMap() {
        level = Instantiate(level);
        level.name = "Level";
        for (int i = 0; i < asciiMap.Length; i++) LoadMap(asciiMap[i], i);
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