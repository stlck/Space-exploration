using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIntoScene : MonoBehaviour {

    public UnityEngine.UI.Image Image;
    public UnityEngine.UI.RawImage RawImage;
    public float TimeToFade = .2f;
    public AnimationCurve CurveToFollow;
    
    float timer = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if(timer < 1f)
        {
            timer += Time.deltaTime * TimeToFade;

            Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, CurveToFollow.Evaluate(timer));

            if(timer >= 1f)
            {
                gameObject.SetActive(false);
            }
        }
	}
}
