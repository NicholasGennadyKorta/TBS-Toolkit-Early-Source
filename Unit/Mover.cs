using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour 
{
	private Seeker seeker;
    public Path path;
    Vector3 dir;
	public float speed = 20;
	private float nextWaypointDistance = 0.5f;
	private int currentWaypoint;
    Vector2 targetGridPosition;

    public Vector2 gridPosition;


	void Start() 
	{
		seeker = GetComponent<Seeker> ();
	}

	void Update () 
	{
		if (path == null)
			return;

		if (currentWaypoint >= path.pathNodes.Count)
			return;

		dir = (path.pathNodes [currentWaypoint].position - transform.position).normalized;
		dir *= speed * Time.deltaTime;

		if (Vector3.Distance (transform.position, path.pathNodes [currentWaypoint].position) <= nextWaypointDistance)
		{
			currentWaypoint++;
			if (currentWaypoint >= path.pathNodes.Count)
				OnMovementComplete();
		}

		transform.position += dir;
		transform.forward = dir;
	}

	public void SetPath(Vector2 endGridPosition)
	{

        path = seeker.CalculatePath(gridPosition, endGridPosition);
        targetGridPosition = endGridPosition;
        GetComponent<Animator>().SetBool("running", true);
        path.pathNodes.RemoveAt(0);

        if (gridPosition ==  targetGridPosition)
            OnMovementComplete();
	}

	public void OnMovementComplete()
	{
		path = null;
        currentWaypoint = 0;
        GetComponent<Unit>().GetComponent<Mover>().gridPosition = targetGridPosition;
        GameLoop.phase = "choose";
        GetComponent<Animator>().SetBool("running", false);
	}

	public void OnMovementCanceled()
	{
		path = null;
		currentWaypoint = 0;
        GameLoop.phase = "move";
        GetComponent<Animator>().SetBool("running", false);
	}
}
