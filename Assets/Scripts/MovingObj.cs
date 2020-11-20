using UnityEngine;
using System.Collections;

public class MovingObj : MonoBehaviour {

	float speed;
	Vector3 moveDir;

	public FenceListener main;

	bool dead;

	// Use this for initialization
	void Start () {
		System.Random rand = new System.Random();

		int turn = rand.Next(0, 360);
		float turnRad = turn * Mathf.PI / 180f;

		moveDir = new Vector3(Mathf.Cos(turnRad), Mathf.Sin(turnRad), 0f);

		speed = 8f;

		dead = false;
	}

	private void checkDeath()
	{
		int x = Mathf.FloorToInt((transform.position.x - (-main.GridLength*0.5f)) - 0.5f);
		int y = Mathf.FloorToInt((transform.position.y - (-main.GridLength*0.5f)) - 0.5f);

		int xRight = Mathf.CeilToInt((transform.position.x - (-main.GridLength*0.5f)) + 0.5f);
		int yRight = Mathf.CeilToInt((transform.position.y - (-main.GridLength*0.5f)) + 0.5f);

		for (; y < yRight; y++)
		{
			for (; x < xRight; x++)
			{
				if (main.gridVal(x, y) == 1)
				{
					dead = true;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		checkDeath();

		if (dead == false)
		{
			Vector2 origin;
			Collider2D collider = null;
			
			if (moveDir.x >= 0)
			{
				// check right
				origin = new Vector2(transform.position.x + transform.localScale.x*0.51f, transform.position.y);
				RaycastHit2D hitRight = Physics2D.Raycast(origin, Vector2.right, speed*Time.deltaTime);
				
				collider = hitRight.collider;
				
				if (collider != null)
				{
					moveDir.x *= -1f;
					
					WallScript wall = (WallScript) collider.transform.parent.gameObject.GetComponent<WallScript>();
					if (wall != null)
					{
						if (wall.isElectric())
						{
							dead = true;
						}
					}
				}
			}
			else
			{
				// check left
				origin = new Vector2(transform.position.x - transform.localScale.x*0.51f, transform.position.y);
				RaycastHit2D hitLeft = Physics2D.Raycast(origin, Vector2.right * -1f, speed*Time.deltaTime);
				
				collider = hitLeft.collider;
				
				if (collider != null)
				{
					moveDir.x *= -1f;
					
					WallScript wall = (WallScript) collider.transform.parent.gameObject.GetComponent<WallScript>();
					if (wall != null)
					{
						if (wall.isElectric())
						{
							dead = true;
						}
					}
				}
			}
			
			if (moveDir.y > 0)
			{
				// check up 
				origin = new Vector2(transform.position.x, transform.position.y + transform.localScale.x*0.51f);
				RaycastHit2D hitUp = Physics2D.Raycast(origin, Vector2.up, speed*Time.deltaTime);
				
				collider = hitUp.collider;
				
				if (collider != null)
				{
					moveDir.y *= -1f;
					
					WallScript wall = (WallScript) collider.transform.parent.gameObject.GetComponent<WallScript>();
					if (wall != null)
					{
						if (wall.isElectric())
						{
							dead = true;
						}
					}
				}
			}
			else
			{
				// check down
				origin = new Vector2(transform.position.x, transform.position.y - transform.localScale.x*0.51f);
				RaycastHit2D hitDown = Physics2D.Raycast(origin, Vector2.up * -1f, speed*Time.deltaTime);
				
				collider = hitDown.collider;
				
				if (collider != null)
				{
					moveDir.y *= -1f;
					
					WallScript wall = (WallScript) collider.transform.parent.gameObject.GetComponent<WallScript>();
					if (wall != null)
					{
						if (wall.isElectric())
						{
							dead = true;
						}
					}
				}
			}
			
			// move
			transform.Translate(moveDir * speed * Time.deltaTime);
		}
		else
		{	
			if (gameObject.GetComponent<SpriteRenderer>().color.a < 0.001)
			{
				gameObject.SetActive(false);
			}
			else
			{
				transform.localScale *= 1.2f;
				Color col = gameObject.GetComponent<SpriteRenderer>().color;
				gameObject.GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, col.a * 0.9f);
			}
		}
	}

	public void reset()
	{
		transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<SpriteRenderer>().color = new Color(32f/255f, 202f/255f, 0f, 1f);
		gameObject.SetActive(true);

		System.Random rand = new System.Random();
		
		int turn = rand.Next(0, 360);
		float turnRad = turn * Mathf.PI / 180f;
		
		moveDir = new Vector3(Mathf.Cos(turn), Mathf.Sin(turn), 0f);
		
		speed = 8f;

		dead = false;

		transform.position = new Vector3(0f, 0f, 0f);
	}


	public bool isDead()
	{
		return dead;
	}

}