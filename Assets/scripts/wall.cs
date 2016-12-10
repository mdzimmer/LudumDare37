using UnityEngine;
using System.Collections;

public class wall : MonoBehaviour {
	public Type type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public enum Type {
		RED,
		WHITE,
		BLACK,
		YELLOW
	}
}
