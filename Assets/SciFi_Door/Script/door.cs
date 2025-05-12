using UnityEngine;
using System.Collections;

public class door : MonoBehaviour {
	GameObject thedoor;

void OnTriggerEnter ( Collider obj  ){
	thedoor= GameObject.FindWithTag("LastDoors");
	thedoor.GetComponent<Animation>().Play("open");
}


    void OnTriggerExit ( Collider obj  ){
    	thedoor= GameObject.FindWithTag("LastDoors");
    	thedoor.GetComponent<Animation>().Play("close");
    }
}