using UnityEngine;
using System.Collections;

public class sword : MonoBehaviour {
//	public bool bringUp {
//		set {
//			_bringUp = value;
//			if (value) {
//
//			} else {
//
//			}
//		}
//		get {
//			return _bringUp;
//		}
//	}
	public bool bringUp;
//	bool _bringUp;
	SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (bringUp) {
			sr.sortingOrder = 4;
		} else {
			sr.sortingOrder = -4;
		}
	}
}
