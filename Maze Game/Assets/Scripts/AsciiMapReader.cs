using UnityEngine;
using System.Collections;
using System.IO;

public class AsciiMapReader : MonoBehaviour {

	public int tileWidth = 4;
	public int tileHeight = 4;
	public float xOffset = 0;
	public float zOffset = 0;
	public GameObject[] prefabs;
	public string characters;
	public TextAsset[] asciiMap;
	
	private ArrayList mapObjects;
	private int mapWidth;    
	private string map;
	
	void Start() {
		for (int i = 0; i < asciiMap.Length; i++) LoadMap(asciiMap[i], i);
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
		Object currentObject;  
		mapObjects = new ArrayList();          
		for (int i = 0; i < map.Length ; i++) {
			prefab = getPrefabForCharacter(map[i]);
			if (prefab != null) {
				currentObject = GameObject.Instantiate(prefab, new Vector3((x * tileWidth) + xOffset, y, (z * -tileWidth) + zOffset), Quaternion.identity);
				mapObjects.Add(currentObject);
			}
			x++;
			if (x == mapWidth) {
				x = 0; z++;
			}
		}
	}
	
	public void DestroyMap(float delay) {
		if (mapObjects != null) {
			foreach (GameObject gameObject in mapObjects) {
				GameObject.Destroy(gameObject, delay);
			}
		}
		mapObjects = null;
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