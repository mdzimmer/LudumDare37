using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class level_rotate : MonoBehaviour {
	public State state = State.BLACK;
	public GameObject[] blackFloor;
	public GameObject[] yellowFloor;
	public GameObject[] whiteFloor;
	public GameObject[] redFloor;
	public GameObject blackBackground;
	public GameObject yellowBackground;
	public GameObject whiteBackground;
	public GameObject redBackground;
	public GameObject blackIndicator;
	public GameObject yellowIndicator;
	public GameObject whiteIndicator;
	public GameObject redIndicator;
	public Color blackFloorColor;
	public Color yellowFloorColor;
	public Color whiteFloorColor;
	public Color redFloorColor;
	public Color blackBackgroundColor;
	public Color yellowBackgroundColor;
	public Color whiteBackgroundColor;
	public Color redBackgroundColor;
	public Color clearFloorColor;
	public Color clearBackgroundColor;

	public float goal = 40;
	public float blackProgress = 0;
	public float yellowProgress = 0;
	public float whiteProgress = 0;
	public float redProgress = 0;

	bool rotating = false;
//	bool rotateClockwise = true;
	float rotateRate = 100f;
//	float timeToEnd = 2f;
	float loseTime = 4f;
	float winTime = 10f;

	// Use this for initialization
	void Awake () {
		foreach (GameObject go in blackFloor) {
			doColor (go, blackFloorColor);
		}
		foreach (GameObject go in yellowFloor) {
			doColor (go, yellowFloorColor);
		}
		foreach (GameObject go in whiteFloor) {
			doColor (go, whiteFloorColor);
		}
		foreach (GameObject go in redFloor) {
			doColor (go, redFloorColor);
		}
		doColor (blackBackground, blackBackgroundColor);
		doColor (yellowBackground, yellowBackgroundColor);
		doColor (whiteBackground, whiteBackgroundColor);
		doColor (redBackground, redBackgroundColor);
		doColor (blackIndicator, blackFloorColor);
		doColor (yellowIndicator, yellowFloorColor);
		doColor (whiteIndicator, whiteFloorColor);
		doColor (redIndicator, redFloorColor);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.K)) {
			rotateLevel (false);
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			rotateLevel (true);
		}
	}

	public void rotateLevel(bool clockwise) {
		if (!rotating) {
			StartCoroutine (manageRotateLevel(clockwise));
			changeState (clockwise);
		}
	}

	public void endLevel() {
		StartCoroutine (manageEndGame (loseTime));
	}

	public void recordProgress(wall.Type kind) {
		switch (kind) {
		case wall.Type.BLACK:
			blackProgress++;
			doProgress (blackBackground, blackProgress / goal, blackBackgroundColor, clearBackgroundColor);
			doProgress (blackIndicator, blackProgress / goal, blackFloorColor, clearFloorColor);
			if (blackProgress >= goal) {
				foreach (GameObject go in blackFloor) {
					doProgress (go, 1f, blackFloorColor, clearFloorColor);
				}
				destroyByType (wall.Type.BLACK);
			}
			break;
		case wall.Type.YELLOW:
			yellowProgress++;
			doProgress (yellowBackground, yellowProgress / goal, yellowBackgroundColor, clearBackgroundColor);
			doProgress (yellowIndicator, yellowProgress / goal, yellowFloorColor, clearFloorColor);
			if (yellowProgress >= goal) {
				foreach (GameObject go in yellowFloor) {
					doProgress (go, 1f, yellowFloorColor, clearFloorColor);
				}
				destroyByType (wall.Type.YELLOW);
			}
			break;
		case wall.Type.WHITE:
			whiteProgress++;
			doProgress (whiteBackground, whiteProgress / goal, whiteBackgroundColor, clearBackgroundColor);
			doProgress (whiteIndicator, whiteProgress / goal, whiteFloorColor, clearFloorColor);
			if (whiteProgress >= goal) {
				foreach (GameObject go in whiteFloor) {
					doProgress (go, 1f, whiteFloorColor, clearFloorColor);
				}
				destroyByType (wall.Type.WHITE);
			}
			break;
		case wall.Type.RED:
			redProgress++;
			doProgress (redBackground, redProgress / goal, redBackgroundColor, clearBackgroundColor);
			doProgress (redIndicator, redProgress / goal, redFloorColor, clearFloorColor);
			if (redProgress >= goal) {
				foreach (GameObject go in redFloor) {
					doProgress (go, 1f, redFloorColor, clearFloorColor);
				}
				destroyByType (wall.Type.RED);
			}
			break;
		}
		if (blackProgress >= goal && yellowProgress >= goal && whiteProgress >= goal && redProgress >= goal) {
			winGame ();
		}
	}

	void destroyByType(wall.Type type) {
		enemy_movement[] ofType = GameObject.FindObjectsOfType<enemy_movement> ();
		foreach (enemy_movement em in ofType) {
			if (em.type == type) {
				em.victoryDie ();
			}
		}
//		switch (type) {
//		case wall.Type.BLACK:
//			foreach (enemy_movement em in ofType) {
//				if (em.type == type) {
//					em.victoryDie ();
//				}
//			}
//			break;
//		case wall.Type.YELLOW:
//			break;
//		case wall.Type.WHITE:
//			break;
//		case wall.Type.RED:
//			break;
//		}
	}

	void doColor(GameObject go, Color color) {
		SpriteRenderer sr = go.GetComponent<SpriteRenderer> ();
		sr.color = color;
	}

//	void doAlpha(GameObject go, float alpha) {
//		SpriteRenderer sr = go.GetComponent<SpriteRenderer> ();
//		Color color = sr.color;
//		color.a = alpha;
//		sr.color = color;
//	}

	void doProgress(GameObject go, float progress, Color a, Color b) {
		SpriteRenderer sr = go.GetComponent<SpriteRenderer> ();
//		Color a = sr.color;
		Color newColor = Color.Lerp (a, b, progress);
//		print (a + " : " + newColor + " : " + b);
//		print (progress);
		sr.color = newColor;
	}

	void winGame() {
//		print ("win");
		GameObject victoryPrefab = (GameObject)Resources.Load ("prefabs/victory");
		Instantiate (victoryPrefab, transform.position, Quaternion.identity);
		enemy_movement[] remaining = GameObject.FindObjectsOfType<enemy_movement> ();
		foreach (enemy_movement em in remaining) {
			em.victoryDie ();
		}
		GameObject.FindObjectOfType<player_movement> ().invincible = true;
		StartCoroutine (manageEndGame (winTime));
	}

	void changeState (bool clockwise) {
		switch (state) {
		case State.BLACK:
			if (clockwise) {
				state = State.YELLOW;
			} else {
				state = State.RED;
			}
			break;
		case State.YELLOW:
			if (clockwise) {
				state = State.WHITE;
			} else {
				state = State.BLACK;
			}
			break;
		case State.WHITE:
			if (clockwise) {
				state = State.RED;
			} else {
				state = State.YELLOW;
			}
			break;
		case State.RED:
			if (clockwise) {
				state = State.BLACK;
			} else {
				state = State.WHITE;
			}
			break;
		}
	}

	IEnumerator manageRotateLevel(bool clockwise) {
		rotating = true;
		for (float totalRotation = 0f; totalRotation < 90f;) {
			float deltaRotation = Time.deltaTime * rotateRate;
			if (totalRotation + deltaRotation > 90f) {
				deltaRotation = 90f - totalRotation;
			}
			transform.Rotate(new Vector3(0f, 0f, clockwise ? -deltaRotation : deltaRotation));
			totalRotation += deltaRotation;
			yield return new WaitForEndOfFrame ();
		}
		rotating = false;
	}

	IEnumerator manageEndGame(float time) {
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene ("scene");
	}

	public enum State {
		BLACK,
		YELLOW,
		WHITE,
		RED
	}
}
