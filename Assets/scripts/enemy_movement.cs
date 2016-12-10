using UnityEngine;
using System.Collections;

public class enemy_movement : MonoBehaviour {
	public wall.Type type;

	float gravityForce = 100f;
	float moveForce = 15f;
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
	bool moveRight = true;
	bool onGround = false;
	bool recentPunchRight = true;
	bool tumbling = false;
	bool dead = false;
	int punchesTaken = 0;
//	Vector2 upVector;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		moveRight = (Random.Range (0f, 1f) < 0.5f) ? true : false;
		startRotation = transform.rotation.eulerAngles.z;
//		upVector = transform.up;
		StartCoroutine (manageJumpDecision());
		StartCoroutine (manageDirectionDecision ());
		StartCoroutine (manageTakingPunches ());
	}
	
	// Update is called once per frame
	void Update () {
		applyGravity ();
		onGround = Physics2D.Raycast (transform.position, -transform.up, 0.4f, 1 << 8);
		if (punchesTaken == 0 && !tumbling && !dead) {
			drag ();
			move ();
		}
//		rb.add
	}

	public void takePunch(bool isRight, float upForce = -1f) {
		rb.velocity = Vector2.zero;
		punchesTaken++;
		breakLockTime = breakLockMaxTime;
		recentPunchRight = isRight;
		if (upForce > minimumComboUpForce) {
			comboUpForce = upForce;
		}
	}

	void applyGravity() {
//		print (transform.up);
//		rb.AddForce (transform.up * -gravityForce);
		if (punchesTaken <= 0 && !tumbling && !dead) {
			rb.AddRelativeForce (new Vector2 (0f, -gravityForce));
//			rb.AddForce(new Vector2(0f, -upVector.y * gravityForce));
//			print (Time.frameCount);
		} else if (tumbling && !dead) {
			rb.AddForce(new Vector2(0f, -gravityForce));
		}
	}

	void jump() {
//		rb.AddForce (transform.up * Random.Range(minJumpForce, maxJumpForce), ForceMode2D.Impulse);
		rb.AddRelativeForce(new Vector2(0f, Random.Range(minJumpForce, maxJumpForce)), ForceMode2D.Impulse);
	}

	void drag() {
//		print (transform.InverseTransformDirection(rb.velocity).x);
		rb.AddRelativeForce (new Vector2(-transform.InverseTransformDirection(rb.velocity).x * linearDrag, 0f));
	}

	void move() {
		rb.AddRelativeForce (new Vector2 (moveForce * (moveRight ? 1f : -1f), 0f));
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
		if (_wall != null && _wall.type != type) {
			die ();
		} else {
			tumbling = false;
		}
	}

	void die() {
		dead = true;
		StopCoroutine(manageTumbling());
		rb.velocity = Vector2.zero;
		Destroy (rb);
		Destroy (gameObject, deathDelay);
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
			if (punchesTaken > 0) {
				breakLockTime -= Time.deltaTime;
				if (breakLockTime <= 0f) {
//					print (gameObject.name + " : " + breakForce * punchesTaken * (recentPunchRight ? 1f : -1f));
					rb.AddForce (new Vector2 (breakForce * punchesTaken * (recentPunchRight ? 1f : -1f),
						comboUpForce * punchesTaken * comboUpEffectRate),
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
}
