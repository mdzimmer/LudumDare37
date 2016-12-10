using UnityEngine;
using System.Collections;

public class player_movement : MonoBehaviour {
	float jumpImpulse = 50f;
	float moveForce = 200f;
	float linearDrag = 20f;
	float punchDecayTimeMax = .5f;
	float punchDecayTime = 0f;
	float punchRadius = 0.5f;
	float punchDistance = .725f;
//	float jumpMaxCooldown = .25f;
//	float jumpCooldown = 0f;
	int punchCounter = 0;
	bool doubleJumpReady = true;
	bool onGround = true;
	Rigidbody2D rb;
	level_rotate levelRotator;
	debugCircle recentPunch;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		levelRotator = GameObject.FindObjectOfType<level_rotate> ();
//		StartCoroutine (manageJumpCooldown());
		StartCoroutine (managePunchCounter());
		recentPunch = new debugCircle (Vector2.zero, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		testOnGround ();
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
		bool leftClick = Input.GetMouseButtonDown (0);
		bool rightClick = Input.GetMouseButtonDown (1);
		if (rightClick && !leftClick) {
			punch (true);
		} else if (leftClick && !rightClick) {
			punch (false);
		}
		drag ();
//		print (punchCounter);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawSphere (recentPunch.position, recentPunch.radius);
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

	void punch(bool right) {
		punchCounter++;
		punchDecayTime = punchDecayTimeMax;
		Vector2 punchPoint = Vector2.zero;
		if (right) {
			punchPoint = (Vector2)transform.position + Vector2.right * punchDistance;
		} else {
			punchPoint = (Vector2)transform.position + Vector2.left * punchDistance;
		}
		Physics2D.OverlapCircleAll (punchPoint, punchRadius);
		recentPunch = new debugCircle (punchPoint, punchRadius);
		if (punchCounter >= 4) {
			if (right) {
				levelRotator.rotateLevel (true);
			} else {
				levelRotator.rotateLevel (false);
			}
			punchCounter = 0;
		}
	}

	void drag() {
		Vector2 appliedDrag = rb.velocity * -linearDrag;
		appliedDrag.y = 0f;
		rb.AddForce (appliedDrag);
	}

//	void drawDebugCircle() {
//
//	}

//	IEnumerator manageJumpCooldown() {
//		while (true) {
//			if (jumpCooldown > 0f) {
//				jumpCooldown -= Time.deltaTime;
//			}
//			yield return new WaitForEndOfFrame ();
//		}
//	}

	IEnumerator managePunchCounter() {
//		float decay = punchDecayTime;
		while (true) {
			punchDecayTime -= Time.deltaTime;
			if (punchDecayTime <= 0f && punchCounter > 0) {
				punchCounter--;
				punchDecayTime = punchDecayTimeMax;
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	enum Direction {
		RIGHT,
		LEFT
	}

	struct debugCircle {
		public Vector2 position;
		public float radius;

		public debugCircle(Vector2 _position, float _radius) {
			position = _position;
			radius = _radius;
		}
	}
}
