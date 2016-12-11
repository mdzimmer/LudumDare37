using UnityEngine;
using System.Collections;

public class level_rotate : MonoBehaviour {
	public State state = State.BLACK;

	bool rotating = false;
//	bool rotateClockwise = true;
	float rotateRate = 100f;

	// Use this for initialization
	void Start () {
	
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

	public enum State {
		BLACK,
		YELLOW,
		WHITE,
		RED
	}
}
