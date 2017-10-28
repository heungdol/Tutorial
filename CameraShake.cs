using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*#1
	 * 카메라 진동을 계산 및 결정하도록 하는 구성변수들이다.*/

[System.Serializable]
public class Properties {
	public float angle;		// 초기 각 변수
	public float strength;	// 진동 비율 정도, 0이면 진동하지 않는다.
	public float speed;		// n과 n+1 사이 이동속도
	public float duration;	// 전체 진행 정도
	[Range(0, 1)]
	public float noisePercent;		// 각 변수 노이즈
	[Range(0, 1)]
	public float dampingPercent;	// 반경 변수 댐핑 정도
	[Range(0, 1)]
	public float rotationPercent;	// 방향을 진동하는 데 쓰이는 변수

	public Properties (float angle, float strength, float speed, float duration, float noisePercent, float dampingPercent, float rotationPercent)
	{
		this.angle = angle;
		this.strength = strength;
		this.speed = speed;
		this.duration = duration;
		this.noisePercent = noisePercent;
		this.dampingPercent = dampingPercent;
		this.rotationPercent = rotationPercent;
	}
}

public class CameraShake : MonoBehaviour {

	/*#2
	 * 카메라 진동 중 새로운 진동 명령이 씹히지 않도록 하는 장치이다.*/

	IEnumerator currentShakeCoroutine;

	public void StartShake (Properties properties) {
		if (currentShakeCoroutine != null) {	// 만약 코루틴이 진행 중이면 정지시킨다.
			StopCoroutine (currentShakeCoroutine);
		}

		currentShakeCoroutine = Shake (properties);
		StartCoroutine (currentShakeCoroutine);
	}
		
	/*#3
	 * 실제 카메라가 진동하도록 하는 장치이며 코루틴으로 구성되어있다.*/

	const float maxAngle = 10;	// 위치가 아닌 방향을 진동할 때 사용되는 함수이다.

	IEnumerator Shake (Properties properties) {
		// 초기 설정
		float completionPercent = 0;
		float movePercent = 0;
		float angleRadians = properties.angle * Mathf.Deg2Rad - Mathf.PI;	// do while문을 고려해 먼저 PI를 빼준다.
		float moveDistance = 0;

		Vector3 previousWaypoint = Vector3.zero;
		Vector3 currentWaypoint = Vector3.zero;

		Quaternion targetRotation = Quaternion.identity;
		Quaternion previousRotation = Quaternion.identity;

		// 반복
		do {	// moveDistance 초기 값이 0인 것을 고려해 do while문 사용한다.
			if (movePercent >= 1 || completionPercent == 0) {
				float dampingFactor = DampingCurve (completionPercent, properties.dampingPercent);	// 적절한 반경 값을 얻는다.
				float noiseAngle = (Random.value - 0.5f) * Mathf.PI;	

				angleRadians += Mathf.PI + noiseAngle * properties.noisePercent;	// 각 180도 회전 뒤 적절하게 노이즈를 준다.
				previousWaypoint = transform.localPosition;
				currentWaypoint = new Vector3 (Mathf.Cos (angleRadians), Mathf.Sin (angleRadians)) * properties.strength * dampingFactor;	// 타깃 위치 변수를 새로 갱신한다.

				moveDistance = Vector3.Distance (previousWaypoint, currentWaypoint);
				movePercent = 0;

				targetRotation = Quaternion.Euler (new Vector3 (currentWaypoint.x, currentWaypoint.y).normalized * properties.rotationPercent * dampingFactor * maxAngle);	// 타깃 방향 변수
				previousRotation = transform.localRotation;

			}

			completionPercent += Time.deltaTime / properties.duration;	// 지정한 값만큼 시간이 흐르면 1 이상의 값을 얻는다. 반경 값을 구할 때 필요하다.
			movePercent += Time.deltaTime / moveDistance * properties.speed;	// 지정한 값 단위로 1이며 중간 값을 얻기 위한 Lerp/Slerp함수에서 필요하다.
			transform.localPosition = Vector3.Lerp (previousWaypoint, currentWaypoint, movePercent);	// 매 프레임마다 위치를 갱신한다.
			transform.localRotation = Quaternion.Lerp (previousRotation, targetRotation, movePercent);	// 매 프레임마다 방향을 갱신한다.
			yield return null;

		}  while (moveDistance > 0);	// moveDistance가 0인 경우는 타깃위치와 중심위치가 같을 때이다.

	}

	/*#4
	 * 전체 진행 정도에 따라 적절한 반경을 계산해주는 함수이다.
	 * y = (1 - x^a)^3 뿐만 아니라 다른 함수를 이용해도 괜찮을 것 같다. */

	float DampingCurve (float x, float dampingPercent) {
		x = Mathf.Clamp01 (x);
		float a = Mathf.Lerp (2, 0.25f, dampingPercent);	// dampintPercent는 사용자가 직접 줄 수 있는 값이다.
		float b = 1 - Mathf.Pow (x, a);
		return b * b * b;
	}


}