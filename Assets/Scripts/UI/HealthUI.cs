using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour {

    public GenericComparitor<float> HealthComparitor;
    public FloatUpdate HealtEvent;

    // Use this for initialization
    void Start () {
        HealthComparitor = new GenericComparitor<float>();
        HealthComparitor.ComparisonAction = () => HealthComparitor.Compare(MyAvatar.Instance.MyStats.CurrentHealth / MyAvatar.Instance.MyStats.MaxHealth);
        HealthComparitor.UpdateAction = (f) => HealtEvent.Invoke(f);
    }
	
	// Update is called once per frame
	void Update () {
        HealthComparitor.ComparisonAction();
    }
}
