using UnityEngine;
using System.Collections;

public class LivesCount : MonoBehaviour {
	public GameObject zeroPrefab;
	public GameObject onePrefab;
	public GameObject twoPrefab;
	public GameObject threePrefab;

	float decayTime = 4f;
	GameObject curDisplayed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown (KeyCode.Alpha8)) {
//			showNumber (0);
//		}
	}

	public void showNumber(int number) {
		GameObject go = null;
		switch (number) {
		case 0:
			go = (GameObject)Instantiate (zeroPrefab, transform.position, Quaternion.identity);
			break;
		case 1:
			go = (GameObject)Instantiate (onePrefab, transform.position, Quaternion.identity);
			break;
		case 2:
			go = (GameObject)Instantiate (twoPrefab, transform.position, Quaternion.identity);
			break;
		case 3:
			go = (GameObject)Instantiate (threePrefab, transform.position, Quaternion.identity);
			break;
		}
		StartCoroutine (decayNumber (go));
	}

	IEnumerator decayNumber(GameObject number) {
		if (curDisplayed) {
			Destroy (curDisplayed);
		}
		curDisplayed = number;
		SpriteRenderer sr = number.GetComponent<SpriteRenderer> ();
		for (float decay = decayTime; decay > 0f; decay -= Time.deltaTime) {
			if (number == null) {
				break;
			}
			Color c = sr.color;
			c.a = decay / decayTime;
			sr.color = c;
			yield return new WaitForEndOfFrame ();
		}
		Destroy (number);
	}
}
