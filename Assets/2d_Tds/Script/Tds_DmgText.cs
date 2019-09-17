using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Tds_DmgText : MonoBehaviour {

	public Vector2 vGoingTo = Vector2.zero;						//check where it will goes from the initiale position

	private Vector2 vOriginalPosition;
	private Vector2 vDestinationPosition;
	private Text vText;
	private RectTransform vRectTransform;

	public void StartMoving(string vMessage, Color vNewColor)
	{
		StartCoroutine (vMoveDMg (vMessage, vNewColor));
	}

	IEnumerator vMoveDMg(string vMessage, Color vNewColor)
	{
		//get text component
		vText = transform.GetComponent<Text> ();
		vText.color = vNewColor;
		vText.text = vMessage;
		vRectTransform = transform.GetComponent<RectTransform> ();

		//calculate starting position
		vOriginalPosition = vRectTransform.anchoredPosition;

		//then calculate destination position
		vDestinationPosition = vOriginalPosition + vGoingTo;

		float vcpt = 1f;
		float velapsetime = 0f;

		while (velapsetime <= vcpt) {
			//wait some time
			velapsetime += Time.deltaTime;
			yield return null;

			//calculate perc moved
			float vperc = (float)(velapsetime/vcpt);

			//then move a little bit
			vRectTransform.anchoredPosition = Vector2.Lerp(vOriginalPosition,vDestinationPosition,vperc);

			//change it's color
			vText.color = new Color(vText.color.r, vText.color.g, vText.color.b, 1f-(float)(velapsetime/vcpt));
		}

		//destroy itself after
		GameObject.Destroy (this.gameObject);
	}
}
