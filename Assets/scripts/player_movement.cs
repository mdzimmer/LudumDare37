using UnityEngine;
using System.Collections;

public class player_movement : MonoBehaviour {
	float jumpImpulse = 30f;
	float moveForce = 200f;
	float linearDrag = 20f;
//	float jumpMaxCooldown = .25f;
//	float jumpCooldown = 0f;
	bool doubleJumpReady = true;
	bool onGround = true;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
//		StartCoroutine (manageJumpCooldown());
	}
	
	// Update is called once per frame
	void Update () {
		testOnGround ();
		//input
		if (Input.GetKeyDown (KeyCode.W)) {
			jump ();
		}
		bool rightInput = Input.GetKey (KeyCode.D);
		bool leftInput = Input.GetKey (KeyCode.A);
		if (rightInput && !leftInput) {
			move (Direction.RIGHT);
		} else if (leftInput && !rightInput) {
			move (Direction.LEFT);
		}
		drag ();
	}

	void testOnGround() {
		onGround = Physics2D.Raycast (transform.position, -Vector2.up, 0.4f, 1 << 8);
//		print (onGround);
		if (onGround) {
			doubleJumpReady = true;
		}
	}	

	void jump() {
		if (onGround || doubleJumpReady) {
			rb.velocity = new Vector2 (rb.velocity.x, 0f);
			rb.AddForce (new Vector2 (0f, jumpImpulse), ForceMode2D.Impulse);
//			jumpCooldown = jumpMaxCooldown;
			if (!onGround) {
				doubleJumpReady = false;
			}
		}
	}

	void move(Direction dir) {
		Vector2 appliedForce = Vector2.zero;
		switch (dir) {
		case Direction.RIGHT:
			appliedForce = new Vector2 (moveForce, 0f);
			break;
		case Direction.LEFT:
			appliedForce = new Vector2 (-moveForce, 0f);
			break;
		}
		rb.AddForce (appliedForce);
	}

	void drag() {
		Vector2 appliedDrag = rb.velocity * -linearDrag;
		appliedDrag.y = 0f;
		rb.AddForce (appliedDrag);
	}

//	IEnumerator manageJumpCooldown() {
//		while (true) {
//			if (jumpCooldown > 0f) {
//				jumpCooldown -= Time.deltaTime;
//			}
//			yield return new WaitForEndOfFrame ();
//		}
//	}

	enum Direction {
		RIGHT,
		LEFT
	}
}
