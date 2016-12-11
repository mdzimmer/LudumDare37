using UnityEngine;
using System.Collections;

public class rotate_indicator : MonoBehaviour {
	public GameObject black;
	public GameObject yellow;
	public GameObject white;
	public GameObject red;

	float decayTime = 0f;
	float decayTimeMax = 1f;
	bool clockwise = true;
	State state = State.ZERO;
	level_rotate levelRotate;

	// Use this for initialization
	void Start () {
		black.SetActive (false);
		yellow.SetActive (false);
		white.SetActive (false);
		red.SetActive (false);
		levelRotate = GameObject.FindObjectOfType<level_rotate> ();
		StartCoroutine (manageDecayIndicators ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void incrementIndicator(bool _clockwise) {
//		return;
		decayTime = decayTimeMax;
		switch (state) {
		case State.ZERO:
			clockwise = _clockwise;
			setState (State.ONE);
			break;
		case State.ONE:
			if (_clockwise == clockwise) {
				setState (State.TWO);
			} else {
				setState (State.ZERO);
			}
			break;
		case State.TWO:
			if (_clockwise == clockwise) {
				setState (State.THREE);
			} else {
				setState (State.ONE);
			}
			break;
		case State.THREE:
			if (_clockwise == clockwise) {
				setState (State.FOUR);
			} else {
				setState (State.TWO);
			}
			break;
		case State.FOUR:
			break;
		}
	}

	void setState(State _state) {
//		print (_state + " : " + clockwise);
		switch (_state) {
		case State.ZERO:
			disable (black);
			disable (yellow);
			disable (white);
			disable (red);
			break;
		case State.ONE:
			switch (levelRotate.state) {
			case level_rotate.State.BLACK:
				enable (white);
				disable (black);
				disable (yellow);
//				disable (white);
				disable (red);
				break;
			case level_rotate.State.YELLOW:
				enable (red);
				disable (black);
				disable (yellow);
				disable (white);
//				disable (red);
				break;
			case level_rotate.State.WHITE:
				enable (black);
//				disable (black);
				disable (yellow);
				disable (white);
				disable (red);
				break;
			case level_rotate.State.RED:
				enable (yellow);
				disable (black);
//				disable (yellow);
				disable (white);
				disable (red);
				break;
			}
			break;
		case State.TWO:
			switch (levelRotate.state) {
			case level_rotate.State.BLACK:
				if (clockwise) {
					enable (white);
					enable (yellow);
					disable (black);
//					disable (yellow);
//					disable (white);
					disable (red);
				} else {
					enable (white);
					enable (red);
					disable (black);
					disable (yellow);
//					disable (white);
//					disable (red);
				}
				break;
			case level_rotate.State.YELLOW:
				if (clockwise) {
					enable (red);
					enable (white);
					disable (black);
					disable (yellow);
//					disable (white);
//					disable (red);
				} else {
					enable (red);
					enable (black);
//					disable (black);
					disable (yellow);
					disable (white);
//					disable (red);
				}
				break;
			case level_rotate.State.WHITE:
				if (clockwise) {
					enable (black);
					enable (red);
//					disable (black);
					disable (yellow);
					disable (white);
//					disable (red);
				} else {
					enable (black);
					enable (yellow);
//					disable (black);
//					disable (yellow);
					disable (white);
					disable (red);
				}
				break;
			case level_rotate.State.RED:
				if (clockwise) {
					enable (yellow);
					enable (black);
//					disable (black);
//					disable (yellow);
					disable (white);
					disable (red);
				} else {
					enable (yellow);
					enable (white);
					disable (black);
//					disable (yellow);
//					disable (white);
					disable (red);
				}
				break;
			}
			break;
		case State.THREE:
			switch (levelRotate.state) {
			case level_rotate.State.BLACK:
				if (clockwise) {
					enable (white);
					enable (yellow);
					enable (black);
					disable (red);
				} else {
					enable (white);
					enable (red);
					enable (black);
					disable (yellow);
				}
				break;
			case level_rotate.State.YELLOW:
				if (clockwise) {
					enable (red);
					enable (white);
					enable (yellow);
					disable (black);
				} else {
					enable (red);
					enable (black);
					enable (yellow);
					disable (white);
				}
				break;
			case level_rotate.State.WHITE:
				if (clockwise) {
					enable (black);
					enable (red);
					enable (white);
					disable (yellow);
				} else {
					enable (black);
					enable (yellow);
					enable (white);
					disable (red);
				}
				break;
			case level_rotate.State.RED:
				if (clockwise) {
					enable (yellow);
					enable (black);
					enable (red);
					disable (white);
				} else {
					enable (yellow);
					enable (white);
					enable (red);
					disable (black);
				}
				break;
			}
			break;
		case State.FOUR:
			enable (black);
			enable (yellow);
			enable (white);
			enable (red);
			levelRotate.rotateLevel (clockwise);
			StartCoroutine (manageReturnToZero ());
			break;
		}
		state = _state;
	}

	void enable(GameObject go) {
		setAlpha (go, 1f);
		go.SetActive (true);
	}

	void disable(GameObject go) {
		setAlpha (go, 0f);
		go.SetActive (false);
	}

	void decrementState() {
		switch (state) {
		case State.ZERO:
			break;
		case State.ONE:
			setState (State.ZERO);
			break;
		case State.TWO:
			setState (State.ONE);
			break;
		case State.THREE:
			setState (State.TWO);
			break;
		case State.FOUR:
			setState (State.THREE);
			break;
		}
	}

	GameObject getCurrentIndicator() {
		switch (state) {
		case State.ZERO:
			break;
		case State.ONE:
			switch (levelRotate.state) {
			case level_rotate.State.BLACK:
				return white;
			case level_rotate.State.YELLOW:
				return red;
			case level_rotate.State.WHITE:
				return black;
			case level_rotate.State.RED:
				return yellow;
			}
			break;
		case State.TWO:
			switch (levelRotate.state) {
			case level_rotate.State.BLACK:
				if (clockwise) {
					return yellow;
				} else {
					return red;
				}
				break;
			case level_rotate.State.YELLOW:
				if (clockwise) {
					return white;
				} else {
					return black;
				}
				break;
			case level_rotate.State.WHITE:
				if (clockwise) {
					return red;
				} else {
					return yellow;
				}
				break;
			case level_rotate.State.RED:
				if (clockwise) {
					return black;
				} else {
					return white;
				}
				break;
			}
			break;
		case State.THREE:
			switch (levelRotate.state) {
			case level_rotate.State.BLACK:
				return black;
			case level_rotate.State.YELLOW:
				return yellow;
			case level_rotate.State.WHITE:
				return white;
			case level_rotate.State.RED:
				return red;
			}
			break;
		case State.FOUR:
			switch (levelRotate.state) {
			case level_rotate.State.BLACK:
				if (clockwise) {
					return yellow;
				} else {
					return red;
				}
				break;
			case level_rotate.State.YELLOW:
				if (clockwise) {
					return white;
				} else {
					return red;
				}
				break;
			case level_rotate.State.WHITE:
				if (clockwise) {
					return red;
				} else {
					return yellow;
				}
				break;
			case level_rotate.State.RED:
				if (clockwise) {
					return black;
				} else {
					return white;
				}
				break;
			}
			break;
		}
		return null;
	}

	void setAlpha (GameObject go, float alpha) {
		SpriteRenderer sr = go.GetComponent<SpriteRenderer> ();
		Color color = sr.color;
		color.a = alpha;
		sr.color = color;
	}

	IEnumerator manageDecayIndicators() {
		while (true) {
			if (state != State.ZERO && state != State.FOUR) {
				decayTime -= Time.deltaTime;
				GameObject curIndicator = getCurrentIndicator ();
				setAlpha (curIndicator, decayTime / decayTimeMax);
//				SpriteRenderer sr = curIndicator.GetComponent<SpriteRenderer> ();
//				Color color = sr.color;
//				color.a = decayTime / decayTimeMax;
//				sr.color = color;
				if (decayTime <= 0f) {
					decrementState ();
					decayTime = decayTimeMax;
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator manageReturnToZero() {
		for (float _decayTime = decayTimeMax; _decayTime > 0f; _decayTime -= Time.deltaTime) {
			setAlpha (black, _decayTime / decayTimeMax);
			setAlpha (yellow, _decayTime / decayTimeMax);
			setAlpha (white, _decayTime / decayTimeMax);
			setAlpha (red, _decayTime / decayTimeMax);
			yield return new WaitForEndOfFrame ();
		}
		setState (State.ZERO);
	}

	enum State {
		ZERO,
		ONE,
		TWO,
		THREE,
		FOUR
	}
}
