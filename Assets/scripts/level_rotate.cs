using UnityEngine;
using System.Collections;

public class level_rotate : MonoBehaviour {
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

	void rotateLevel(bool clockwise) {
		if (!rotating) {
			StartCoroutine (manageRotateLevel(clockwise));
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
}
