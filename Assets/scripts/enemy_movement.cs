using UnityEngine;
using System.Collections;

public class enemy_movement : MonoBehaviour {
	public wall.Type type;

	float gravityForce = 100f;
	float moveForce = 5f;
	float maxTimeToJump = 1f;
	float minJumpForce = 30f;
	float maxJumpForce = 60f;
	float maxChangeDirectionTime = 8f;
	float minChangeDirectionTime = 2f;
	float breakLockMaxTime = .5f;
	float breakLockTime = 0f;
	float breakForce = 10f;
	float comboUpForce = 0f;
	float minimumComboUpForce = 20f;
	float tumblingRotateRate = 1000f;
	float startRotation = 0f;
	float deathDelay = .2f;
	float comboUpEffectRate = .5f;
	float checkAheadLength = 1.5f;
	float linearDrag = 5f;
	float timeToAttack = 0f;
	float minTimeToAttack = 2f;
	float maxTimeToAttack = 6f;
	float attackLength = 1f;
	float attackRadius = 0.5f;
	float groundDetectionLength = 0.6f;
	float minVictoryTime = 0.25f;
	float maxVictoryTime = 2f;
	float entranceTurnRate = 75f;
	bool moveRight = true;
	bool onGround = false;
	bool recentPunchRight = true;
	bool tumbling = false;
	bool dead = false;
	bool entered = false;
	int punchesTaken = 0;
	Vector2 comboPosition;
	Vector3 startScale;
//	Vector2 upVector;
	Rigidbody2D rb;
	Animator animator;
	GameObject particleSystemPrefab;
	soundManager sm;
	level_rotate lr;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		moveRight = (Random.Range (0f, 1f) < 0.5f) ? true : false;
		startRotation = transform.rotation.eulerAngles.z;
		startScale = transform.localScale;
		animator = GetComponent<Animator> ();
		GameObject particleSystemPrefab = (GameObject)Resources.Load ("prefabs/HitParticles");
		sm = GameObject.FindObjectOfType<soundManager> ();
		lr = GameObject.FindObjectOfType<level_rotate> ();
//		print (particleSystemPrefab);
//		upVector = transform.up;
//		Vector3 newRotation = transform.localRotation.eulerAngles;
//		newRotation.y = 90f;
//		transform.localRotation = Quaternion.Euler (newRotation);
		StartCoroutine (manageEntrance());
//		StartCoroutine (manageTakingPunches ());
//		StartCoroutine (manageDirectionDecision ());
//		StartCoroutine (manageJumpDecision());
//		StartCoroutine (manageAttackDecision ());
	}
	
	// Update is called once per frame
	void Update () {
		applyGravity ();
		onGround = Physics2D.Raycast (transform.position, -transform.up, groundDetectionLength, 1 << 8);
		Debug.DrawLine (transform.position, transform.position + -transform.up * groundDetectionLength);
		if (punchesTaken == 0 && !tumbling && !dead && entered) {
			drag ();
			move ();
			face ();
		}
//		rb.add

//		if (type == wall.Type.BLACK || type == wall.Type.WHITE) {
//			Vector3 newRotation = transform.localRotation.eulerAngles;
//			newRotation.y += Time.deltaTime * 50f;
//			transform.localRotation = Quaternion.Euler (newRotation);
//		} else {
//			Vector3 newRotation = transform.localRotation.eulerAngles;
//			newRotation.x += Time.deltaTime * 50f;
//			transform.localRotation = Quaternion.Euler (newRotation);
//		}
	}

	public void takePunch(bool isRight, float upForce = -1f) {
		if (!dead && entered) {
			rb.velocity = Vector2.zero;
			punchesTaken++;
			breakLockTime = breakLockMaxTime;
			recentPunchRight = isRight;
			comboPosition = transform.position;
			if (upForce > minimumComboUpForce) {
				comboUpForce = upForce;
			}
		}
	}

	public void attackTrigger() {
		Vector2 targetDir = Vector2.zero;
		if (moveRight) {
			targetDir = transform.right;
		} else {
			targetDir = -transform.right;
		}
		Collider2D[] hits = Physics2D.OverlapCircleAll ((Vector2)transform.position + targetDir * attackLength, attackRadius);
		foreach (Collider2D hit in hits) {
			player_movement playerMovement = hit.GetComponent<player_movement> ();
			if (playerMovement) {
				playerMovement.takeHit (chooseTint());
			}
		}
	}

	public void victoryDie() {
		StartCoroutine (manageVictoryDie ());
	}

	particleColor.Tint chooseTint() {
		switch (type) {
		case wall.Type.BLACK:
			return particleColor.Tint.BLACK;
		case wall.Type.YELLOW:
			return particleColor.Tint.YELLOW;
		case wall.Type.WHITE:
			return particleColor.Tint.WHITE;
		case wall.Type.RED:
			return particleColor.Tint.RED;
		}
		return particleColor.Tint.BLACK;
	}

	void applyGravity() {
//		print (punchesTaken + " : " + dead);
//		print (transform.up);
//		rb.AddForce (transform.up * -gravityForce);
		if (punchesTaken <= 0 && !dead && !tumbling) {
//			print (Time.frameCount);
			rb.AddRelativeForce (new Vector2 (0f, -gravityForce));
//			rb.AddForce(new Vector2(0f, -upVector.y * gravityForce));
//			print (Time.frameCount);
		} else if (tumbling && !dead) {
			rb.AddForce(-Vector2.up * gravityForce);
		}
	}

	void jump() {
//		rb.AddForce (transform.up * Random.Range(minJumpForce, maxJumpForce), ForceMode2D.Impulse);
		rb.AddRelativeForce(new Vector2(0f, Random.Range(minJumpForce, maxJumpForce)), ForceMode2D.Impulse);
//		sm.playEnemyJump ();
	}

	void drag() {
//		print (transform.InverseTransformDirection(rb.velocity).x);
		rb.AddRelativeForce (new Vector2(
			-transform.InverseTransformDirection(rb.velocity).x * linearDrag * (moveRight ? 1f : -1f), 0f));
	}

	void move() {
		rb.AddRelativeForce (new Vector2 (moveForce * (moveRight ? 1f : -1f), 0f));
//		rb.AddRelativeForce (new Vector2 (moveForce, 0f));
	}

	void face() {
		Vector3 scale = startScale;
		if (moveRight) {
			scale.x = scale.x;
		} else {
			scale.x = -scale.x;
		}
		transform.localScale = scale;
	}

	void OnCollisionEnter2D(Collision2D col) {
//		print (col);
//		if (tumbling) {
//			wall _wall = col.collider.GetComponent<wall> ();
//			if (_wall != null && _wall.type != type) {
//				die ();
//			} else {
//				tumbling = false;
//			}
//		}
		wall _wall = col.collider.GetComponent<wall> ();
		if (_wall != null && _wall.type != type && !dead) {
			die ();
		} else {
			tumbling = false;
			animator.Play ("walk");
		}
	}

	void die() {
		dead = true;
		StopCoroutine(manageTumbling());
		rb.velocity = Vector2.zero;
		Vector3 particlePosition = transform.position;
		particlePosition.z = -1f;
//		print (particleSystemPrefab);
		if (particleSystemPrefab == null) {
			particleSystemPrefab = (GameObject)Resources.Load ("prefabs/HitParticles");
		}
		GameObject psgo = (GameObject)Instantiate(particleSystemPrefab, particlePosition, Quaternion.identity);
		psgo.GetComponent<particleColor> ().setTint (chooseTint());
		sm.playEnemyDie ();
//		Destroy (rb);
		lr.recordProgress(type);
		Destroy (gameObject, deathDelay);
	}

//	level_rotate.State chooseKind() {
//		switch (type) {
//
//		}
//	}

	void attack() {
		animator.Play ("attack");
	}

	IEnumerator manageJumpDecision() {
		float timeToJump = Random.Range (0f, maxTimeToJump);
		bool inAir = false;
		while (true) {
			if (punchesTaken == 0 && !tumbling && !dead) {
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
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageDirectionDecision() {
		float changeDirectionTime = Random.Range (minChangeDirectionTime, maxChangeDirectionTime);
		while (true) { 
			RaycastHit2D hit = Physics2D.Raycast (transform.position,
				                   (moveRight ? 1f : -1f) * transform.right,
									checkAheadLength, 1 << 8);
			Debug.DrawLine (transform.position, 
				transform.position + (moveRight ? 1f : -1f) * transform.right * checkAheadLength);
			if (hit) {
//				print ("turn");
				moveRight = !moveRight;
				changeDirectionTime = Random.Range (minChangeDirectionTime, maxChangeDirectionTime);
			} else {
				changeDirectionTime -= Time.deltaTime;
				if (changeDirectionTime <= 0f) {
					moveRight = !moveRight;
					changeDirectionTime = Random.Range (minChangeDirectionTime, maxChangeDirectionTime);
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageTakingPunches() {
		breakLockTime = breakLockMaxTime;
		while (true) {
			if (dead) {
				StopCoroutine (manageTakingPunches ());
			}
			if (punchesTaken > 0) {
				animator.Play ("hit");
				transform.position = comboPosition;
				breakLockTime -= Time.deltaTime;
//				if (breakLockTime <= 0f) {
				if (breakLockTime <= 0f || punchesTaken >= 4) {
////					print (gameObject.name + " : " + breakForce * punchesTaken * (recentPunchRight ? 1f : -1f));
					rb.AddForce (new Vector2 (breakForce * punchesTaken * (recentPunchRight ? 1f : -1f),
						Mathf.Max(comboUpForce * punchesTaken * comboUpEffectRate, minimumComboUpForce)),
						ForceMode2D.Impulse);
					punchesTaken = 0;
					tumbling = true;
					StartCoroutine (manageTumbling ());
				}
			} else {
				comboUpForce = minimumComboUpForce;
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageTumbling() {
		while (tumbling) {
			transform.Rotate (new Vector3 (0f, 0f, (recentPunchRight ? 1f : -1f) * Time.deltaTime * tumblingRotateRate));
			yield return new WaitForEndOfFrame ();
		}
		transform.rotation = Quaternion.Euler (new Vector3(0f, 0f, startRotation));
	}

	IEnumerator manageAttackDecision() {
		timeToAttack = Random.Range (minTimeToAttack, maxTimeToAttack);
		while (true) {
			if (!dead && !tumbling) {
				timeToAttack -= Time.deltaTime;
				if (timeToAttack <= 0f) {
					attack ();
					timeToAttack = Random.Range (minTimeToAttack, maxTimeToAttack);
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageVictoryDie() {
		float time = Random.Range (minVictoryTime, maxVictoryTime);
		yield return new WaitForSeconds (time);
		die ();
	}

	IEnumerator manageEntrance() {
		float angle = 90f;
//		Vector3 newRotation = transform.localRotation.eulerAngles;
//		if (type == wall.Type.BLACK || type == wall.Type.WHITE) {
//			newRotation.y = angle;
//		} else {
//			newRotation.x = angle;
//		}
//		transform.localRotation = Quaternion.Euler (newRotation);
		while (angle > 0f) {
			angle -= Time.deltaTime * entranceTurnRate;
			if (angle < 0f) {
				angle = 0f;
			}
			Vector3 newRotation = transform.localRotation.eulerAngles;
			if (type == wall.Type.BLACK || type == wall.Type.WHITE) {
				newRotation.y = angle;
			} else {
				newRotation.x = angle;
			}
			transform.localRotation = Quaternion.Euler (newRotation);
			yield return new WaitForEndOfFrame();
		}
		entered = true;
		StartCoroutine (manageTakingPunches ());
		StartCoroutine (manageDirectionDecision ());
		StartCoroutine (manageJumpDecision());
		StartCoroutine (manageAttackDecision ());
	}
}
