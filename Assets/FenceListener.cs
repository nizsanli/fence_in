using UnityEngine;
using System.Collections;

public class FenceListener : MonoBehaviour {

	float cellSize;

	public Transform mouse;
	public Transform field;
	public Transform arrow;
	public Transform wallPrefab;

	float gridLength;

	bool isDown;
	Vector3 mouseNormal;

	float minDist = 1f;

	float sectionMult;

	bool build;

	Vector3 buildPos;

	int section;

	Vector3[] buildDirs;

	byte[] grid;

	Transform currWallPart;

	// Use this for initialization
	void Start () {

		cellSize = 1f;
		gridLength = Camera.main.orthographicSize * 2f;

		field.transform.localScale = new Vector3(cellSize*gridLength, cellSize*gridLength, 0f);

		grid = new byte[(int)(gridLength * gridLength)];

		buildPos = new Vector3(-field.transform.localScale.x*0.5f + 0.5f, -field.transform.localScale.y*0.5f + 0.5f + 1f, 0f);
		// make wall around border
		for (int x = 0; x < 2; x++)
		{
			for (int y = 1; y < gridLength-1; y++)
			{
				Transform wallPart = (Transform) Instantiate(wallPrefab, buildPos, Quaternion.identity);
				wallPart.transform.localScale = new Vector3(1f, 1f, 1f);
				
				buildPos.y += 1f;
				
				grid[(int)(y * gridLength + x * (gridLength-1))] = (byte) 1;
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

				buildPos.x += 1f;
				
				grid[(int)((y * (gridLength-1))*gridLength + x)] = (byte) 1;
			}

			buildPos.x = -field.transform.localScale.x*0.5f + 0.5f;
			buildPos.y = field.transform.localScale.y*0.5f - 0.5f;
		}

		arrow.GetComponent<SpriteRenderer>().enabled = false;
		mouse.GetComponent<SpriteRenderer>().enabled = false;

		isDown = false;
		build = false;

		sectionMult = 1f / 90f;

		buildDirs = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, -1f, 0f)
		};
	}
	
	// Update is called once per frame
	void Update () {
		Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");

		if (build == true)
		{
			if ((1f - currWallPart.localScale.x) > 0.01f)
			{
				float adder = (1f - currWallPart.localScale.x) * 0.8f;
				currWallPart.localScale = new Vector3(currWallPart.localScale.x + adder, currWallPart.localScale.y + adder, 1f);
			}
			else
			{
				currWallPart.localScale = new Vector3(1f, 1f, 1f);

				float yIndex = ((buildPos.y - 0.5f) / cellSize) + gridLength*0.5f;
				float xIndex = ((buildPos.x - 0.5f) / cellSize) + gridLength*0.5f;
				
				if (grid[(int)(yIndex * gridLength + xIndex)] != 1)
				{
					Vector3 dir = buildDirs[section];
					
					currWallPart = (Transform) Instantiate(wallPrefab, buildPos, Quaternion.identity);
					currWallPart.GetComponent<SpriteRenderer>().enabled = true;
					
					buildPos += dir;
					
					grid[(int)(yIndex * gridLength + xIndex)] = 1;
				}
				else
				{
					build = false;
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0f;

			mouse.GetComponent<SpriteRenderer>().enabled = true;

			mouseNormal = new Vector3(Mathf.Floor(mousePos.x) + cellSize*0.5f, Mathf.Floor(mousePos.y) + cellSize*0.5f, 0f);
			mouse.transform.position = mouseNormal;
		}

		if (Input.GetMouseButton(0))
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

					Vector3 dir = buildDirs[section];
					buildPos += dir;
					
					grid[(int)(yIndex * gridLength + xIndex)] = 1;
				}
				else
				{
					Vector3 dir = buildDirs[section];
					buildPos += dir;
				}
			}
		}
	}
}
