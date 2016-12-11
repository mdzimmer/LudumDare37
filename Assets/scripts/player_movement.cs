using UnityEngine;
using System.Collections;

public class player_movement : MonoBehaviour {
	float jumpImpulse = 50f;
	float moveForce = 200f;
	float linearDrag = 20f;
//	float punchDecayTimeMax = .5f;
//	float punchDecayTime = 0f;
	float punchRadius = 0.5f;
	float punchDistance = .725f;
	float gravityForce = 100f;
//	float noGravityTimeMax = 0.5f;
//	float noGravityTime = 0f;
	float comboTime = 0f;
	float comboTimeMax = 0.5f;
	float downForce = 100f;
	float groundTestLength = 0.6f;
	float attackReturnTimeMax = 0.5f;
	float attackReturnTime = 0f;
//	float jumpMaxCooldown = .25f;
//	float jumpCooldown = 0f;
//	int punchCounter = 0;
	bool doubleJumpReady = true;
	bool onGround = true;
	bool doingCombo = false;
	Vector3 startScale;
	Rigidbody2D rb;
	level_rotate levelRotator;
	rotate_indicator rotateIndicator;
	debugCircle recentPunch;
	State state = State.OTHER;
	Animator animator;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		levelRotator = GameObject.FindObjectOfType<level_rotate> ();
//		StartCoroutine (manageJumpCooldown());
//		StartCoroutine (managePunchCounter());
//		StartCoroutine (manageNoGravity ());
		StartCoroutine (manageAnim ());
		StartCoroutine (manageState ());
		StartCoroutine (manageCombo ());
		recentPunch = new debugCircle (Vector2.zero, 0f);
		startScale = transform.localScale;
		animator = GetComponent<Animator> ();
		rotateIndicator = GameObject.FindObjectOfType<rotate_indicator> ();
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
			if (state == State.OTHER) {
				face (true);
			}
			move (Direction.RIGHT);
		} else if (leftInput && !rightInput) {
			if (state == State.OTHER) {
				face (false);
			}
			move (Direction.LEFT);
		}
		if (Input.GetKey (KeyCode.S)) {
			rb.AddForce (new Vector2 (0f, -downForce));
		}
		bool leftClick = Input.GetMouseButtonDown (0);
		bool rightClick = Input.GetMouseButtonDown (1);
		if (rightClick && !leftClick) {
			punch (true);
		} else if (leftClick && !rightClick) {
			punch (false);
		}
		drag ();
		gravity ();
//		print (punchCounter);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawSphere (recentPunch.position, recentPunch.radius);
	}

	void face(bool right) {
		Vector3 scale = transform.localScale;
		if (right) {
			scale.x = -startScale.x;
		} else {
			scale.x = startScale.x;
		}
		transform.localScale = scale;
	}

	void testOnGround() {
		onGround = Physics2D.Raycast (transform.position, -Vector2.up, groundTestLength, 1 << 8);
		Debug.DrawLine (transform.position, transform.position - (Vector3)Vector2.up * groundTestLength);
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
		face (right);
//		punchCounter++;
		rotateIndicator.incrementIndicator(right);
//		punchDecayTime = punchDecayTimeMax;
		Vector2 punchPoint = Vector2.zero;
		if (right) {
			punchPoint = (Vector2)transform.position + Vector2.right * punchDistance;
		} else {
			punchPoint = (Vector2)transform.position + Vector2.left * punchDistance;
		}
		Collider2D[] hits = Physics2D.OverlapCircleAll (punchPoint, punchRadius, 1 << 9);
		recentPunch = new debugCircle (punchPoint, punchRadius);
		foreach (Collider2D hit in hits) {
			enemy_movement em = hit.GetComponent<enemy_movement> ();
			if (em == null) {
				continue;
			}
//			print (em.gameObject.nam);
			em.takePunch(right, rb.velocity.y);
		}
//		print (hits.Length);
		if (hits.Length > 0) {
			doingCombo = true;
			rb.velocity = Vector2.zero;
			doubleJumpReady = true;
			comboTime = comboTimeMax;
//			StartCoroutine (manageCombo ());
		}
		setState (progressAttack ());
		attackReturnTime = attackReturnTimeMax;
//		if (punchCounter >= 4) {
//			if (right) {
//				levelRotator.rotateLevel (true);
//			} else {
//				levelRotator.rotateLevel (false);
//			}
//			punchCounter = 0;
//		}
	}

	void gravity() {
//		print (noGravityTime);
		if (comboTime <= 0f) {
			rb.AddForce (new Vector2 (0f, -gravityForce));
		}
	}

	void drag() {
		Vector2 appliedDrag = rb.velocity * -linearDrag;
		appliedDrag.y = 0f;
		rb.AddForce (appliedDrag);
	}

	void setState(State _state) {
		state = _state;
	}

	State progressAttack() {
		switch (state) {
		case State.ATTACKONE:
			return State.ATTACKTWO;
		case State.ATTACKTWO:
			return State.ATTACKTHREE;
		case State.ATTACKTHREE:
			return State.ATTACKONE;
		case State.OTHER:
			return State.ATTACKONE;
		}
		return State.OTHER;
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

//	IEnumerator managePunchCounter() {
//		while (true) {
//			punchDecayTime -= Time.deltaTime;
//			if (punchDecayTime <= 0f && punchCounter > 0) {
//				punchCounter--;
//				punchDecayTime = punchDecayTimeMax;
//			}
//			yield return new WaitForEndOfFrame ();
//		}
//	}

//	IEnumerator manageNoGravity() {
//		while (true) {
//			if (noGravityTime > 0f) {
//				noGravityTime -= Time.deltaTime;
//			}
//			yield return new WaitForEndOfFrame ();
//		}
//	}

	IEnumerator manageAnim() {
		attackReturnTime = attackReturnTimeMax;
		while (true) {
			if (state != State.OTHER) {
				attackReturnTime -= Time.deltaTime;
				if (attackReturnTime <= 0f) {
					setState (State.OTHER);
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageState() {
		State previousState = state;
		while (true) {
//			print (_state);
			if (state != previousState) {
//				print (state);
				switch (state) {
				case State.ATTACKONE:
					animator.Play ("attack");
					break;
				case State.ATTACKTWO:
					animator.Play ("attack2");
					break;
				case State.ATTACKTHREE:
					animator.Play ("attack3");
					break;
				case State.OTHER:
					animator.Play ("idle");
					break;
				}
				previousState = state;
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageCombo() {
//		noGravityTime = noGravityTimeMax;
//		for (float comboTime = comboTimeMax; comboTime > 0f; comboTime -= Time.deltaTime) {
//			yield return new WaitForEndOfFrame ();
//		}
		while (comboTime > 0f) {
//			doingCombo = true;
			comboTime -= Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
//		doingCombo = false;
	}

	enum Direction {
		RIGHT,
		LEFT
	}

	enum State {
		ATTACKONE,
		ATTACKTWO,
		ATTACKTHREE,
		OTHER
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
