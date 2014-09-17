using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    public Transform target;
    public float speed = 5;
    private Quaternion targetRotation;
    public float originalPositionY;

	// Use this for initialization
	void Start () {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x, target.transform.position.y + 5, target.transform.position.z - 35), Time.smoothDeltaTime * speed);
        transform.eulerAngles = new Vector3(50, 0, 0);

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 25, target.transform.position.z - 7);
        originalPositionY = transform.position.y;
    }
	
	// Update is called once per frame
	void Update () {

       
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x, originalPositionY, target.transform.position.z - 35), Time.smoothDeltaTime * speed);
        transform.eulerAngles = new Vector3(50, 0, 0);
        

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            var originalRotion = transform.rotation;
            transform.eulerAngles = new Vector3(0, 0, 0);
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition.x * 0.1f, 0, -touchDeltaPosition.y * 0.1f);
            transform.rotation = originalRotion;
        }
    }
}
