using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffectObject : MonoBehaviour {

    public float MaxLength = 10;
    public float MaxDeviation = .2f;
    public LineRenderer Line;
    public float LineDeviationSpeed = .25f;
    int linePoints;
    Vector3[] deviations;
    Vector3[] currentPoints;
    Material LaserMat;
    float offset = 0f;
    public Vector2 OffsetDirecetion = new Vector2(-1, 0);
    // Use this for initialization
    void Start () {
        linePoints = Line.positionCount;
        deviations = new Vector3[linePoints];
        Line.material = Line.material;

        currentPoints = new Vector3[linePoints];
        for (int i = 0; i < linePoints; i++)
        {
            //currentPoints[i] = Vector3.forward * i;
            deviations[i] = Random.insideUnitSphere * MaxDeviation;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //MiddlePosition.Invoke(Vector3.Lerp(transform.position, transform.position + transform.forward * MaxLength, .5f));
        //SizeUpdate.Invoke(Vector3.forward * MaxLength);
        //Line.SetPosition(1, Vector3.forward * MaxLength);

        for(int i = 1; i < linePoints; i++)
        {
            currentPoints[i] = Vector3.MoveTowards(currentPoints[i], deviations[i], LineDeviationSpeed * Time.deltaTime);
            if(Vector3.Distance( currentPoints[i], deviations[i]) < .001f)
                deviations[i] = Random.insideUnitSphere * MaxDeviation;

            Line.SetPosition(i, currentPoints[i] + Vector3.forward * i * MaxLength / linePoints);
        }

        offset += Time.deltaTime;
        offset = Mathf.Repeat(offset, 1f);
        Line.material.SetTextureOffset("_MainTex", offset * OffsetDirecetion);
    }

}
