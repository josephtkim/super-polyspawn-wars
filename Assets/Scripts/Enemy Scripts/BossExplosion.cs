using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExplosion : MonoBehaviour {
	void Start () {
        Destroy(this.gameObject, 3f);
	}	
}
