using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour {

    static RangeIndicator instance;
    public static RangeIndicator Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
        TurnOff();
    }

    public ParticleSystem particles;
    

	public void TurnOn(Vector3 position, float scale)
    {
        transform.position = position + Vector3.up * .2f;
        transform.localScale = Vector3.one * scale * 1.5f;
        //gameObject.SetActive(true);
        particles.Emit(1);
    }

    public void TurnOff()
    {
        //gameObject.SetActive(false);
    }

    void Update()
    {

    }
}
