using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, 1f);
	}	
}
