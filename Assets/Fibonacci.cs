using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]

public class Fibonacci : MonoBehaviour {

	public int resolution = 1;
	public float buildTime;
	public int cellCount = 6;
	public FibonacciCell cellObj;
	public GameObject trail;

	private List<FibonacciCell> _cells = new List<FibonacciCell>();
	private LineRenderer _line;
	private int _currentCell;
	private float _curveT;
	private bool _complete;
	private FibonacciCell _selectedCell;

	float n1 = 0,n2 = 1,n3;

	void Start () 
	{
		_line = GetComponent<LineRenderer> ();
		_line.SetVertexCount(cellCount * resolution + 1);
		StartCoroutine(SetUpSpiral ());
	}

	void Update () 
	{
		if(!_complete)
			return;

		Vector3 start = Vector3.zero;
		Vector3 end = Vector3.zero;
		Vector3 middle = Vector3.zero;

		if(_curveT >= 1.0f)
		{
			if(_currentCell < _cells.Count)
				_currentCell ++;

			if(_currentCell >= _cells.Count)
				return;	
			_curveT = 0;
		}

		_selectedCell = _cells [_currentCell];
		SetInterpPoints (out start, out end, out middle);
		trail.transform.position = CurveVelocity(_curveT,start,middle, end);
		_curveT += 0.025f;
	}

	float CalculateFibonacciNumber()
	{
		n3 = n1 + n2;

		n1 = n2;
		n2 = n3;

		return n3;
	}

	IEnumerator SetUpSpiral()
	{
		FibonacciCell firstCell = Instantiate (cellObj) as FibonacciCell;
		float initSize = CalculateFibonacciNumber ();
		firstCell.cellDirection = CellDirection.up;

		StartCoroutine(firstCell.SetUp(0,0,initSize,-initSize));
		_line.SetPosition (0, new Vector3 (initSize, -initSize));
		_cells.Add (firstCell);

		yield return new WaitForSeconds (buildTime);
		
		for (int i = 0; i < cellCount;i++)
		{
			int modulos = i % 4;
			FibonacciCell cell = null;
			float size = CalculateFibonacciNumber();
			FibonacciCell lastCell = null;

			float top;
			float left;
			float right;
			float bottom;

			if(_cells.Count > 0)
				lastCell = _cells[i];

			//x = left, y == top
			switch(modulos)
			{
			case 0:
				//left
				cell = Instantiate(cellObj) as FibonacciCell;
				cell.cellDirection = CellDirection.left;

				top = lastCell.top;
				left = lastCell.left - size;
				right = lastCell.left;
				bottom = lastCell.top - size;

				_line.SetPosition(i + 1,new Vector3(right,top));

				StartCoroutine(cell.SetUp(top,left,right,bottom));
				break;
			case 1:
				//down
				cell = Instantiate(cellObj) as FibonacciCell;
				cell.cellDirection = CellDirection.down;

				top = lastCell.bottom;
				left = lastCell.left;
				right = lastCell.left + size;
				bottom = top - size;

				_line.SetPosition(i + 1,new Vector3(left,top));

				StartCoroutine(cell.SetUp(top,left,right,bottom));
				break;
			case 2:
				//right
				cell = Instantiate(cellObj) as FibonacciCell;
				cell.cellDirection = CellDirection.right;

				bottom = lastCell.bottom;
				left = lastCell.right;
				right = left + size;
				top = bottom + size;

				_line.SetPosition(i + 1,new Vector3(left,bottom));

				StartCoroutine(cell.SetUp(top,left,right,bottom));
				break;

			case 3:
				//up
				cell = Instantiate(cellObj) as FibonacciCell;
				cell.cellDirection = CellDirection.up;

				top = lastCell.top + size;
				left = lastCell.right - size;
				right = lastCell.right;
				bottom = lastCell.top;

				_line.SetPosition(i + 1,new Vector3(right,bottom));

				StartCoroutine(cell.SetUp(top,left,right,bottom));
				break;
			}
			if(cell)
				_cells.Add(cell);
			yield return new WaitForSeconds(buildTime);
		}

		_complete = true;
	}

	void SetInterpPoints (out Vector3 start, out Vector3 end, out Vector3 middle)
	{
		switch (_selectedCell.cellDirection) {
		case CellDirection.left:
			start = new Vector3 (_selectedCell.right, _selectedCell.top);
			end = new Vector3 (_selectedCell.left, _selectedCell.bottom);
			middle = new Vector3 (_selectedCell.left, _selectedCell.top);
			break;
		case CellDirection.down:
			start = new Vector3 (_selectedCell.left, _selectedCell.top);
			end = new Vector3 (_selectedCell.right, _selectedCell.bottom);
			middle = new Vector3 (_selectedCell.left, _selectedCell.bottom);
			break;
		case CellDirection.right:
			start = new Vector3 (_selectedCell.left, _selectedCell.bottom);
			end = new Vector3 (_selectedCell.right, _selectedCell.top);
			middle = new Vector3 (_selectedCell.right, _selectedCell.bottom);
			break;
		case CellDirection.up:
			start = new Vector3 (_selectedCell.right, _selectedCell.bottom);
			end = new Vector3 (_selectedCell.left, _selectedCell.top);
			middle = new Vector3 (_selectedCell.right, _selectedCell.top);
			break;
		default:
			start = Vector3.zero;
			end = Vector3.zero;
			middle = Vector3.zero;
			break;
		}
	}

	Vector3 CurveVelocity(float t,Vector3 p0, Vector3 p1, Vector3 p2)
	{
		return (1.0f - t) * (1.0f - t) * p0 
			+ 2.0f * (1.0f - t) * t * p1
				+ t * t * p2;
	}
}