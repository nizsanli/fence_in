using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FenceListener : MonoBehaviour {

	public Transform mouse;
	public Transform field;
	public Transform arrow;
	public Transform wallPrefab;
	public Transform wallContainer;
	
	public BoxCollider2D wallColliderPrefab;
	public MovingObj movingObj;

	public int areaToWin = 17;
	public float fenceGrowRate = 0.5f;
	public Color electricWallCol = new Color(255f/255f, 88f/255f, 88f/255f, 1f);
	public Color regularWallCol = Color.white;
	public float cellSize = 1f;

	public Text scoreText;
	public CanvasRenderer winPanel;

	private float gridLength;

	private bool build;

	private Vector3 mouseNormal;
	private Vector3 buildPos;

	private float minDist;
	private float sectionMult;

	private int section;
	private Vector3[] buildDirs;

	private byte[] grid;
	private Transform[] wallsInGrid;
	private byte[] fillGrid;

	private Transform currWallPart;
	private List<WallScript> electricWalls;

	private bool win;
	private int area;

	private float timeDead;

	// Use this for initialization
	void Start () {
		arrow.GetComponent<SpriteRenderer>().enabled = false;
		mouse.GetComponent<SpriteRenderer>().enabled = false;

		electricWalls = new List<WallScript>((int)gridLength);

		build = false;
		sectionMult = 1f / 90f;
		
		buildDirs = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, -1f, 0f)
		};

		timeDead = 0f;
		minDist = 1f;

		Camera.main.orthographicSize = 15f;
		gridLength = 2f * Camera.main.orthographicSize;

		win = false;

		field.transform.localScale = new Vector3(cellSize*gridLength, cellSize*gridLength, 0f);

		grid = new byte[(int)(gridLength * gridLength)];
		wallsInGrid = new Transform[(int)(gridLength * gridLength)];

		// make walls around border
		buildPos = new Vector3(-field.transform.localScale.x*0.5f + 0.5f, -field.transform.localScale.y*0.5f + 0.5f + 1f, 0f);
		for (int x = 0; x < 2; x++)
		{
			for (int y = 1; y < gridLength-1; y++)
			{
				Transform wallPart = (Transform) Instantiate(wallPrefab, buildPos, Quaternion.identity);
				wallPart.transform.localScale = new Vector3(1f, 1f, 1f);

				BoxCollider2D wallCollider = (BoxCollider2D) Instantiate(wallColliderPrefab, wallPart.position, Quaternion.identity);
				wallCollider.transform.parent = wallPart.transform;
				
				buildPos.y += 1f;
				
				grid[(int)(y * gridLength + x * (gridLength-1))] = (byte) 1;
				wallsInGrid[(int)(y * gridLength + x * (gridLength-1))] = wallPart;
			}

			buildPos.x = field.transform.localScale.x*0.5f - 0.5f;
			buildPos.y = -field.transform.localScale.y*0.5f + 0.5f + 1f;
		}

		buildPos = new Vector3(-field.transform.localScale.x*0.5f + 0.5f, -field.transform.localScale.y*0.5f + 0.5f, 0f);
		for (int y = 0; y < 2; y++)
		{
			for (int x = 0; x < gridLength; x++)
			{
				Transform wallPart = (Transform) Instantiate(wallPrefab, buildPos, Quaternion.identity);
				wallPart.transform.localScale = new Vector3(1f, 1f, 1f);

				BoxCollider2D wallCollider = (BoxCollider2D) Instantiate(wallColliderPrefab, wallPart.position, Quaternion.identity);
				wallCollider.transform.parent = wallPart;

				buildPos.x += 1f;
				
				grid[(int)((y * (gridLength-1))*gridLength + x)] = (byte) 1;
				wallsInGrid[(int)((y * (gridLength-1))*gridLength + x)] = wallPart;
			}

			buildPos.x = -field.transform.localScale.x*0.5f + 0.5f;
			buildPos.y = field.transform.localScale.y*0.5f - 0.5f;
		}
	}

	private int logArea()
	{
		int xIndex = Mathf.FloorToInt(movingObj.transform.position.x - (-gridLength*0.5f) - 0.5f);
		int yIndex = Mathf.FloorToInt(movingObj.transform.position.y - (-gridLength*0.5f) - 0.5f);

		fillGrid = new byte[(int)(gridLength * gridLength)];

		int area = floodFill(xIndex, yIndex);
		//Debug.Log(area);

		return area;
	}

	private int floodFill(int x, int y)
	{
		if (grid[(int)(y * gridLength + x)] == 1)
		{
			return 0;
		}

		if (fillGrid[(int)(y * gridLength + x)] == 1)
		{
			return 0;
		}

		fillGrid[(int)(y * gridLength + x)] = 1;
		int result = 1;

		// left
		result += floodFill(x-1, y);
		// up
		result += floodFill(x, y+1);
		// right
		result += floodFill(x+1, y);
		// down
		result += floodFill(x, y-1);

		return result;
	}

	// Update is called once per frame
	void Update () {
		Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");

		if (win == true)
		{
			scoreText.text = area.ToString();
			winPanel.gameObject.SetActive(true);
		}

		if (movingObj.isDead())
		{
			build = false;

			if ((1f - currWallPart.localScale.x) > 0.001f)
			{
				float adder = (1f - currWallPart.localScale.x) * 0.8f;
				currWallPart.localScale = new Vector3(currWallPart.localScale.x + adder, currWallPart.localScale.y + adder, 1f);
			}
			else
			{
				currWallPart.localScale = new Vector3(1f, 1f, 1f);
			}

			if (timeDead > 1.5f)
			{
				timeDead = 0f;
				reset();
			}

			timeDead += Time.deltaTime;
		}

		if (build == true && !movingObj.isDead())
		{
			if ((1f - currWallPart.localScale.x) > 0.001f)
			{
				float speed = 90f;
				float adder = (1f - currWallPart.localScale.x) * fenceGrowRate * Time.deltaTime * speed;
				currWallPart.localScale = new Vector3(currWallPart.localScale.x + adder, currWallPart.localScale.y + adder, 1f);
			}
			else
			{
				currWallPart.localScale = new Vector3(1f, 1f, 1f);
				currWallPart.GetComponent<WallScript>().emitParticles();

				BoxCollider2D wallCollider = (BoxCollider2D) Instantiate(wallColliderPrefab, currWallPart.position, Quaternion.identity);
				wallCollider.transform.parent = currWallPart;

				float yIndex = ((buildPos.y - 0.5f) / cellSize) + gridLength*0.5f;
				float xIndex = ((buildPos.x - 0.5f) / cellSize) + gridLength*0.5f;
				
				if (grid[(int)(yIndex * gridLength + xIndex)] != 1)
				{
					Vector3 dir = buildDirs[section];
					
					currWallPart = (Transform) Instantiate(wallPrefab, buildPos, Quaternion.identity);
					currWallPart.GetComponent<SpriteRenderer>().enabled = true;

					currWallPart.parent = wallContainer;
					
					buildPos += dir;
					
					grid[(int)(yIndex * gridLength + xIndex)] = (byte) 1;
					wallsInGrid[(int)(yIndex * gridLength + xIndex)] = currWallPart;

					electricWalls.Add(currWallPart.GetComponent<WallScript>());
					currWallPart.GetComponent<WallScript>().setElectric(true);

					currWallPart.GetComponent<SpriteRenderer>().color = electricWallCol;
				}
				else
				{
					build = false;

					foreach (WallScript wall in electricWalls)
					{
						wall.setElectric(false);
						wall.gameObject.GetComponent<SpriteRenderer>().color = regularWallCol;
						
						wall.GetComponent<WallScript>().emitParticles();
					}

					electricWalls.Clear();

					area = logArea();
					if (area < areaToWin)
					{
						win = true;
					}
				}
			}
		}

		if (Input.GetMouseButtonDown(0) && build == false && !movingObj.isDead() && win == false)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0f;

			mouse.GetComponent<SpriteRenderer>().enabled = true;

			mouseNormal = new Vector3(Mathf.Floor(mousePos.x) + cellSize*0.5f, Mathf.Floor(mousePos.y) + cellSize*0.5f, 0f);
			mouse.transform.position = mouseNormal;
		}

		if (Input.GetMouseButton(0) && build == false && !movingObj.isDead() && win == false)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0f;

			float dist = Vector3.Distance(mousePos, mouseNormal);

			if (dist > minDist)
			{
				Vector3 mouseVec = (mousePos - mouseNormal).normalized;

				float angle = Mathf.Acos(Vector3.Dot(mouseVec, Vector3.up)) * 180f / Mathf.PI;
				Vector3 cross = Vector3.Cross(mouseVec, Vector3.up);

				float fixedAngle = 0f;
				if (cross.z >= 0)
				{
					fixedAngle = 90f - angle;
				}
				else 
				{
					fixedAngle = 90f + angle;
				}

				if (fixedAngle < 0)
				{
					fixedAngle += 360f;
				}

				section = Mathf.FloorToInt((fixedAngle + 45f) * sectionMult);

				if (section > 3)
				{
					section = 0;
				}

				arrow.localRotation = Quaternion.Euler(0f, 0f, section * 90f);

				arrow.transform.position = mouseNormal;
				arrow.transform.Translate(new Vector3(mouse.localScale.x + mouse.localScale.y * 0.25f, 0f, 0f), Space.Self);

				arrow.GetComponent<SpriteRenderer>().enabled = true;
			}
			else
			{
				arrow.GetComponent<SpriteRenderer>().enabled = false;
			}
		}
		else
		{
			mouse.GetComponent<SpriteRenderer>().enabled = false;

			if (arrow.GetComponent<SpriteRenderer>().enabled)
			{
				build = true;
				arrow.GetComponent<SpriteRenderer>().enabled = false;

				buildPos = mouseNormal;

				float yIndex = ((buildPos.y - 0.5f) / cellSize) + gridLength*0.5f;
				float xIndex = ((buildPos.x - 0.5f) / cellSize) + gridLength*0.5f;
				
				if (grid[(int)(yIndex * gridLength + xIndex)] != 1)
				{	
					currWallPart = (Transform) Instantiate(wallPrefab, buildPos, Quaternion.identity);
					currWallPart.GetComponent<SpriteRenderer>().enabled = true;
					currWallPart.transform.localScale = new Vector3(1f, 1f, 1f);

					Vector3 dir = buildDirs[section];
					buildPos += dir;
					
					grid[(int)(yIndex * gridLength + xIndex)] = (byte) 1;
					wallsInGrid[(int)(yIndex * gridLength + xIndex)] = currWallPart;

					electricWalls.Add(currWallPart.GetComponent<WallScript>());
					currWallPart.GetComponent<WallScript>().setElectric(true);

					currWallPart.GetComponent<SpriteRenderer>().color = electricWallCol;

					currWallPart.parent = wallContainer;
				}
				else
				{
					Vector3 dir = buildDirs[section];
					buildPos += dir;

					currWallPart = wallsInGrid[(int)(yIndex * gridLength + xIndex)];
				}
			}
		}
	}

	public void reset()
	{
		winPanel.gameObject.SetActive(false);

		movingObj.reset();

		for (int i = 0; i < wallContainer.childCount; i++)
		{
			Destroy(wallContainer.GetChild(i).gameObject);
		}

		win = false;

		// clear made walls
		for (int y = 1; y < gridLength-1; y++)
		{
			for (int x = 1; x < gridLength-1; x++)
			{
				grid[(int)(y*gridLength+x)] = (byte) 0;
			}

		}

		electricWalls.Clear();
	}

	public byte gridVal(int x, int y)
	{
		return grid[(int)(y * gridLength + x)];
	}

	public float GridLength
	{
		get {return gridLength;}
	}
}
