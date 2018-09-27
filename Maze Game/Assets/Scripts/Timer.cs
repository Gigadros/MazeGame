using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public Text timerUI;
	private float secondCount;
	private int minuteCount;
	private string seconds, minutes;

	void Start () {
		secondCount = 0f;
		minuteCount = 0;
	}

	void Update () {
		secondCount += Time.deltaTime;
		if (secondCount >= 60f) {
			minuteCount++;
			secondCount = 0f;
		}
		seconds = (int)secondCount < 10 ? "0" + ((int)secondCount).ToString() : ((int)secondCount).ToString();
		minutes = minuteCount < 10 ? "0" + minuteCount.ToString() : minuteCount.ToString();
		timerUI.text = "Time: " + minutes + ":" + seconds;
	}
}
