using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class spawn_enemies : MonoBehaviour {
	public bool spawnEnabled = false;

	Transform one_two;
	Transform two_three;
	Transform three_four;
	Transform four_one;
	GameObject redPrefab;
	GameObject whitePrefab;
	GameObject blackPrefab;
	GameObject yellowPrefab;
	level_rotate lr;
	float enemyHeight = 0.35f;
	float enemyWidth = 0.35f;
	float spawnMargin = 1f;
	float currentSpawnInterval = 0.5f;
	float minSpawnInterval = 0.1f;
	float multiplierInterval = 5f;
	float initialWait = 3f;
	int maxEnemies = 40;

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
		lr = GameObject.FindObjectOfType<level_rotate> ();
		StartCoroutine (manageSpawning ());
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

	int determineType() {
		List<int> available = new List<int> ();
		if (lr.blackProgress < lr.goal) {
			available.Add (2);
		}
		if (lr.yellowProgress < lr.goal) {
			available.Add (1);
		}
		if (lr.whiteProgress < lr.goal) {
			available.Add (0);
		}
		if (lr.redProgress < lr.goal) {
			available.Add (3);
		}
		if (available.Count == 0) {
			return -1;
		}
		return available [Random.Range (0, available.Count)];
	}

	IEnumerator manageSpawning() {
		yield return new WaitForSeconds (initialWait);
		float spawnTimeRemaining = currentSpawnInterval;
		float multiplierTime = 0f;
		while (true) {
			if (!spawnEnabled) {
				yield return new WaitForEndOfFrame ();
				continue;
			}
			multiplierTime += Time.deltaTime;
			if (multiplierTime >= multiplierInterval) {
				currentSpawnInterval /= 2f;
				currentSpawnInterval = Mathf.Max (currentSpawnInterval, minSpawnInterval);
				multiplierTime = 0f;
			}
			spawnTimeRemaining -= Time.deltaTime;
			if (spawnTimeRemaining <= 0f) {
				enemy_movement[] enemyList = GameObject.FindObjectsOfType<enemy_movement> ();
				if (enemyList.Length >= maxEnemies) {
					spawnTimeRemaining = currentSpawnInterval;
					continue;
				}
//				int enemyType = Random.Range (0, 4);
				int enemyType = determineType();
				if (enemyType == -1) {
					spawnEnabled = false;
					break;
				}
				switch (enemyType) {
				case 0:
					spawn (one_two, four_one, whitePrefab, 180f);
					break;
				case 1:
					spawn (two_three, one_two, yellowPrefab, 90f);
					break;
				case 2:
					spawn (three_four, two_three, blackPrefab, 0f);
					break;
				case 3:
					spawn (four_one, three_four, redPrefab, 270f);
					break;
				}
				spawnTimeRemaining = currentSpawnInterval;
			}
			yield return new WaitForEndOfFrame ();
		}
	}
}
