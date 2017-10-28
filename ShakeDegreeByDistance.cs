using UnityEngine;

// 이 스크립트의 주체는 폭발물 따위이다.

public class ShakeDegreeByDistance : MonoBehaviour {

	private GameObject cameraGameObject;	// 곧 플레이어를 가리킨다.

	public Properties minProperties;
	public Properties maxProperties;

	public float maxDetectDistance;

	void Start () {cameraGameObject = Camera.main.gameObject;}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			float distanceRatio = Vector3.Distance (transform.position, cameraGameObject.transform.position);
			distanceRatio = Mathf.Clamp (distanceRatio, 0, maxDetectDistance);
			distanceRatio /= maxDetectDistance;

			Properties currentProperties = LerpProperties (minProperties, maxProperties, distanceRatio);

			cameraGameObject.GetComponent<CameraShake> ().StartShake (currentProperties);
		}
	}

	// 0과 1사이의 비율을 얻어 두개의 Properties를 섞는 함수이다.
	public Properties LerpProperties (Properties p1, Properties p2, float f) {
		Properties returnProperties = new Properties(
			p1.angle * f + p2.angle * (1 - f)
			, p1.strength * f + p2.strength * (1 - f)
			, p1.speed * f + p2.speed * (1 - f)
			, p1.duration * f + p2.duration * (1 - f)
			, p1.noisePercent * f + p2.noisePercent * (1 - f)
			, p1.dampingPercent * f + p2.dampingPercent * (1 - f)
			, p1.rotationPercent * f + p2.rotationPercent * (1 - f));

		return returnProperties;
	}
}
