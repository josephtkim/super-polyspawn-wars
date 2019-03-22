using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour {	
	void Start () {
        Destroy(this.gameObject, 1f);
	}	
}
