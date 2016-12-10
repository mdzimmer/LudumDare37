using UnityEngine;
using System.Collections;

public class spawn_enemies : MonoBehaviour {
	Transform one_two;
	Transform two_three;
	Transform three_four;
	Transform four_one;
	GameObject redPrefab;
	GameObject whitePrefab;
	GameObject blackPrefab;
	GameObject yellowPrefab;
	float enemyHeight = 0.35f;
	float enemyWidth = 0.35f;
	float spawnMargin = 1f;

	// Use this for initialization
	void Start () {
		one_two = GameObject.Find ("one_two").transform;
		two_three = GameObject.Find ("two_three").transform;
		three_four = GameObject.Find ("three_four").transform;
		four_one = GameObject.Find ("four_one").transform;
		redPrefab = (GameObject)Resources.Load ("prefabs/redEnemy");
		whitePrefab = (GameObject)Resources.Load ("prefabs/whiteEnemy");
		blackPrefab = (GameObject)Resources.Load ("prefabs/blackEnemy");
		yellowPrefab = (GameObject)Resources.Load ("prefabs/yellowEnemy");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			spawn (one_two, four_one, whitePrefab, 180f);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			spawn (two_three, one_two, yellowPrefab, 90f);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			spawn (three_four, two_three, blackPrefab, 0f);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			spawn (four_one, three_four, redPrefab, 270f);
		}
	}

	void spawn (Transform left, Transform right, GameObject prefab, float rotation) {
		Vector2 parallelVector = right.position - left.position;
		parallelVector.Normalize ();
//		print (parallelVector);
		Vector2 leftExtreme = (Vector2)left.position + parallelVector * spawnMargin;
		Vector2 rightExtreme = (Vector2)right.position - parallelVector * spawnMargin;
		Vector2 spawnPoint = Vector2.Lerp (leftExtreme, rightExtreme, Random.Range (0f, 1f));
		Vector2 perpendicularVector = new Vector2 (-parallelVector.y, parallelVector.x);
//		print (perpendicularVector);
		spawnPoint += perpendicularVector * enemyHeight / 2f;
//		Vector3 rotation = new Vector3 (0f, 0f, );
		GameObject enemyGO = (GameObject)Instantiate(prefab, spawnPoint, Quaternion.identity);
		enemyGO.transform.parent = transform;
		enemyGO.transform.localRotation = Quaternion.Euler (new Vector3(0f, 0f, rotation));
	}
}
