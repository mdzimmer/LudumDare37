using UnityEngine;
using System.Collections;

public class RatioEnforcer : MonoBehaviour {
	Camera cam;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
//		print (cam.aspect);
		cam.aspect = 1f;
//		print (cam.aspect);
		Screen.SetResolution(600, 600, false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
