using UnityEngine.AI;
using UnityEngine;

public class MonsterController : MonoBehaviour {

public GameObject target;

public NavMeshAgent agent;
	
	// Update is called once per frame
	void Update () {
		agent.SetDestination(target.transform.position);
	}
}
