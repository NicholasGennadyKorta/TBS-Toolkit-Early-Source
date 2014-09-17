using UnityEngine;
using System.Collections;

public class NoneTouchCursor : MonoBehaviour {

    public Vector2 gridPosition;
    public double moveTimer;
    public double moveInterval = 0.1f;
    bool canMove;
    AudioClip selectorMoveAudio = new AudioClip();
    bool audioPlayed = false;

	// Use this for initialization
	void Start () 
    {
        gridPosition =  ObjectPool.units[0].GetComponent<Mover>().gridPosition;
        transform.position = GridGraph.instance.pathNodes[(int)gridPosition.x, (int)gridPosition.y].position;

       
        transform.localScale = new Vector3(GridGraph.instance.nodeSize * 0.1f, 1, GridGraph.instance.nodeSize * 0.1f);

        renderer.material.shader = Shader.Find("Unlit/Transparent");
        renderer.material.color = new Color(1, 1, 1, 1);

        if (GridGraph.instance.gridType == 0)
            renderer.material.SetTexture(0, Resources.Load<Texture>("TRPG/Selector/Texture_SquareSelector"));
        else
            renderer.material.SetTexture(0, Resources.Load<Texture>("TRPG/Selector/Texture_HexSelector"));

        selectorMoveAudio = ObjectPool.audioDatabase.GetSoundEffect("SelectorMove");
        gameObject.AddComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        if (GameLoop.phase != "choose" && GameLoop.phase != "moving" && GameLoop.phase != "items")
            this.renderer.enabled = true;
        else
            this.renderer.enabled = false;

        if (GameLoop.phase != "choose" && GameLoop.phase != "moving" && GameLoop.phase != "items")
        {
            moveTimer += Time.smoothDeltaTime;
            if (moveTimer >= moveInterval)
            {
                moveTimer = 0;
                canMove = true;
            }

            if (Input.GetAxisRaw("Vertical") > 0.3f && canMove)
            {
                gridPosition.y += 1;
                moveTimer = 0;
                if (!audioPlayed)
                {
                    audio.PlayOneShot(selectorMoveAudio);
                    audioPlayed = true;
                }
            }
            else if (Input.GetAxisRaw("Vertical") < -0.5f && canMove)
            {
                gridPosition.y -= 1;
                moveTimer = 0;
                if (!audioPlayed)
                {
                    audio.PlayOneShot(selectorMoveAudio);
                    audioPlayed = true;
                };
            }

            if (Input.GetAxisRaw("Horizontal") > 0.3f && canMove)
            {
                gridPosition.x += 1;
                moveTimer = 0;
                if (!audioPlayed)
                {
                    audio.PlayOneShot(selectorMoveAudio);
                    audioPlayed = true;
                }
            }
            else if (Input.GetAxisRaw("Horizontal") < -0.3f && canMove)
            {
                gridPosition.x -= 1;
                moveTimer = 0;
                if (!audioPlayed)
                {
                    audio.PlayOneShot(selectorMoveAudio);
                    audioPlayed = true;
                }
            }

            gridPosition.x = Mathf.Clamp(gridPosition.x, 0, GridGraph.instance.width - 1);
            gridPosition.y = Mathf.Clamp(gridPosition.y, 0, GridGraph.instance.depth - 1);
            transform.position = new Vector3( GridGraph.instance.pathNodes[(int)gridPosition.x, (int)gridPosition.y].position.x,0.1f, GridGraph.instance.pathNodes[(int)gridPosition.x, (int)gridPosition.y].position.z);
            transform.position += new Vector3(0, 0.25f, 0);

            GetComponent<MeshFilter>().mesh.vertices = GridGraph.instance.GetCellVertices(new Vector2(gridPosition.x, gridPosition.y));
            GetComponent<MeshFilter>().mesh.RecalculateBounds();
            
            canMove = false;
            audioPlayed = false;
        }
	}
}
