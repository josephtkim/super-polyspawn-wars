using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSpawnerExplosion : MonoBehaviour {
	void Start () {
        Destroy(this.gameObject, 2f);	
	}		
}
