using UnityEngine;
using System.Collections;

public class particleColor : MonoBehaviour {
	ParticleSystem ps;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setTint (Tint tint) {
		ps = GetComponent<ParticleSystem> ();
		Color newColor = Color.green;
		switch (tint) {
		case Tint.GREEN:
			break;
		case Tint.BLACK:
			newColor = Color.black;
			break;
		case Tint.YELLOW:
			newColor = Color.yellow;
			break;
		case Tint.WHITE:
			newColor = Color.white;
			break;
		case Tint.RED:
			newColor = Color.red;
			break;
		}
//		print (newColor);
		ps.startColor = newColor;
		ps.Play ();
	}

	public enum Tint {
		GREEN,
		BLACK,
		YELLOW,
		WHITE,
		RED
	}
}
