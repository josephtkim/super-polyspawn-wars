using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour {	
	void Start () {
        Destroy(this.gameObject, 1f);	
	}		
}
