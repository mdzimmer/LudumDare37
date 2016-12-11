using UnityEngine;
using System.Collections;

public class soundManager : MonoBehaviour {
	public AudioSource playerDamage;
	public AudioSource playerAttackHit;
	public AudioSource enemyDie;
	public AudioSource jump;
	public AudioSource enemyJump;
	public AudioSource slash;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playPlayerDamage() {
		playerDamage.Play ();
	}

	public void playPlayerAttackHit() {
		playerAttackHit.Play ();
	}

	public void playEnemyDie() {
		enemyDie.Play ();
	}

	public void playJump() {
		jump.Play ();
	}

	public void playEnemyJump() {
		enemyJump.Play ();
	}

//	public static soundManager instance() {
//		GameObject instance = GameObject.FindObjectOfType<soundManager> ();
//		return instance.GetComponent<soundManager> ();
//	}
}
