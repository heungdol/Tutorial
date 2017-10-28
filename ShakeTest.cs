using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTest : MonoBehaviour {
	// 말 그대로 테스트하는 코드이다.
	public Properties testProperties;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			FindObjectOfType<CameraShake>().StartShake (testProperties);
		}
	}

}