using UnityEngine;
using System.Collections;

public class enemy_movement : MonoBehaviour {
	float gravityForce = 100f;
	float moveForce = 10f;
	float maxTimeToJump = 1f;
	float minJumpForce = 30f;
	float maxJumpForce = 60f;
	float maxChangeDirectionTime = 4f;
	float minChangeDirectionTime = 2f;
	bool moveRight = true;
	bool onGround = false;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		moveRight = (Random.Range (0f, 1f) < 0.5f) ? true : false;
		StartCoroutine (manageJumpDecision());
		StartCoroutine (manageDirectionDecision ());
	}
	
	// Update is called once per frame
	void Update () {
		applyGravity ();
		onGround = Physics2D.Raycast (transform.position, -transform.up, 0.4f, 1 << 8);
		move ();
//		rb.add
	}

	void applyGravity() {
//		print (transform.up);
//		rb.AddForce (transform.up * -gravityForce);
		rb.AddRelativeForce (new Vector2(0f, -gravityForce));
	}

	void jump() {
//		rb.AddForce (transform.up * Random.Range(minJumpForce, maxJumpForce), ForceMode2D.Impulse);
		rb.AddRelativeForce(new Vector2(0f, Random.Range(minJumpForce, maxJumpForce)), ForceMode2D.Impulse);
	}

	void move() {
		rb.AddRelativeForce (new Vector2 (moveForce * (moveRight ? 1f : -1f), 0f));
	}

	IEnumerator manageJumpDecision() {
		float timeToJump = Random.Range (0f, maxTimeToJump);
		bool inAir = false;
		while (true) { 
			if (onGround) {
				if (inAir) {
					timeToJump = Random.Range (0f, maxTimeToJump);
				}
				timeToJump -= Time.deltaTime;
				if (timeToJump <= 0f) {
					jump ();
					timeToJump = Random.Range (0f, maxTimeToJump);
				}
			} else {
				inAir = true;
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageDirectionDecision() {
		float changeDirectionTime = Random.Range (minChangeDirectionTime, maxChangeDirectionTime);
		while (true) {
			changeDirectionTime -= Time.deltaTime;
			if (changeDirectionTime <= 0f) {
				moveRight = !moveRight;
				changeDirectionTime = Random.Range (minChangeDirectionTime, maxChangeDirectionTime);
			}
			yield return new WaitForEndOfFrame ();
		}
	}
}
