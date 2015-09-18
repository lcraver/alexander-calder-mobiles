using UnityEngine;
using System.Collections;

public class AddForce : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space))
        {
            this.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 10, 0));
        }
	}
}
