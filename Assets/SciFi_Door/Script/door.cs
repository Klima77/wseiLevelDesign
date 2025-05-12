using UnityEngine;
using System.Collections;

public class door : MonoBehaviour {
	GameObject thedoor;

void OnTriggerEnter ( Collider obj  ){
	thedoor= GameObject.FindWithTag("SF_Door");
	thedoor.GetComponent<Animation>().Play("close");
}

//void OnTriggerExit ( Collider obj  ){
//	thedoor= GameObject.FindWithTag("SF_Door");
//	thedoor.GetComponent<Animation>().Play("open");
//}
}