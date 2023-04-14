using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Threading;
using System.Collections;
using UnityEditor.PackageManager;

public class Field : MonoBehaviour
{
    public static Field Instance;

    private bool isStart;

    public int FieldSizeY;
    public int FieldSizeX;

    [SerializeField]
    private float CellSize;
    [SerializeField]
    private float Spacing;

    [SerializeField]
    private Cell cellPref;
    [SerializeField]
    private Candy candyPref;
    [SerializeField]
    private RectTransform rt;


    [HideInInspector] public Cell[,] cells;
    [HideInInspector] public Candy[,] candies;

    public Color[] color;


    private void Start()
    {
        if (Instance == null)
            Instance = this;

        if (cells == null || candies == null)
        {
            isStart = true;
            CreatField();
        }        
    }
    [ContextMenu("Reset")]
    private void ResetCandies()
    {
        if (cells == null || candies == null)
            return;

        for (int y = 0; y < FieldSizeY; y++)
        {
            for(int x = 0; x < FieldSizeX; x++)
            {
            
                if (candies[x, y])
                {
                    candies[x, y].SetValue(x, y, Random.Range(1, color.Length));
                }
            }
        }
    }
    private void ClearField()
    {
        var childrens = transform.GetComponentsInChildren<Transform>();
        foreach (var item in childrens)
        {
            if (item != transform)
            Destroy(item.gameObject);
        }
    }
    private bool FindField(int x = 0, int y = 0)
    {
        var oldCandy = transform.GetComponentsInChildren<Candy>();
        foreach (var item in oldCandy)
        {
            candies[x, y] = item;
            x = x != FieldSizeX - 1 ? x + 1 : 0;
            y = x == 0 ? y + 1 : y;
        }
        var oldCell = transform.GetComponentsInChildren<Cell>();
        x= 0; y= 0;
        foreach (var item in oldCell)
        {
            cells[x, y] = item;
            x = x != FieldSizeX - 1 ? x + 1 : 0;
            y = x == 0 ? y + 1 : y;
        }
        return cells[0,0] != transform && candies[0,0] != transform;
    }

    [ContextMenu("Activate")]
    private void CreatField()
    {
        cells = new Cell[FieldSizeX, FieldSizeY];
        candies = new Candy[FieldSizeX, FieldSizeY];

        ClearField();

        //if (FindField())
            //return;
        Position(0, 0, true);
        for (int y = 0; y < FieldSizeY; y++)
        {
            for (int x = 0; x < FieldSizeX; x++)
            {
                SpawnCell(x, y);
                StartCoroutine(SpawnCandy(x, y));
            }
        }
    }
    private void SpawnCell(int x, int y)
    {
         var cell = Instantiate(cellPref, transform, false);
         cell.transform.localPosition = Position(x, y, false);
         cells[x, y] = cell;
         cell.SetValue(x, y);
    }
    private IEnumerator SpawnCandy(int x, int y)
    {
        var candy = Instantiate(candyPref, transform, false);
        candy.transform.localPosition = Position(x, y, false);
        candies[x, y] = candy;
        candy.SetValue(x, y, Random.Range(1, color.Length));
        y  = isStart ? y : 0;
        yield return new WaitForSeconds(y / 2f);
        if (isStart)
           candy.SpawnAnim();
        else 
            candy.image.color = color[candy.ColorNum];
    }
    public Vector2 Position(int x, int y, bool UpdateField)
    {
        float fieldWidth = FieldSizeX * (CellSize + Spacing) + Spacing;
        float fieldHight = FieldSizeY * (CellSize + Spacing) + Spacing;

        float startX = -(fieldWidth / 2) + (CellSize / 2) + Spacing;
        float startY = -(fieldHight / 2) + (CellSize / 2) + Spacing;

        var position = new Vector2(startX + (x * (CellSize + Spacing)), startY + (y * (CellSize + Spacing)));
        if (UpdateField)
            rt.sizeDelta = new Vector2(fieldWidth, fieldHight);

        return position;
    }
}
