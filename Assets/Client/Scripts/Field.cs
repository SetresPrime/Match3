using UnityEngine;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{    
    
    public float CellSize;
    public float Spacing;
    public int FieldSizeX;
    public int FieldSizeY;

    [SerializeField]
    private Cell cellPref;
    [SerializeField]
    private Candy candyPref;
    [SerializeField]
    private RectTransform rt;

    private Cell[,] cells;
    private Candy[,] candies;
    public Color[] color;

    [ContextMenu ("Activate")]
    private void Awake()
    { 
        if (cells == null) 
           CreatField();
        if (candies == null)
            CreatCandy();
    }
    [ContextMenu("Reset")]
    private void ResetCandy()
    {
        for(int x = 0; x < FieldSizeX; x++)
            for(int y = 0; y < FieldSizeY; y++)
                candies[x, y].SetValue(x, y, color[Random.Range(0, color.Length)]);
    }
    private void CreatField()
    {
        cells = new Cell[FieldSizeX,FieldSizeY];
        for (int x = 0;x < FieldSizeX;x++)
            for (int y = 0;y < FieldSizeY; y++)
            {
                var cell = Instantiate(cellPref, transform, false);           
                cell.transform.localPosition = Position(x, y);
                cells[x, y] = cell;
                cell.SetValue(x, y);

            }
    }
    private void CreatCandy()
    {
        candies = new Candy[FieldSizeX, FieldSizeY];
        for (int x = 0; x < FieldSizeX; x++)
            for (int y = 0; y < FieldSizeY; y++)
            {
                var candy = Instantiate(candyPref, transform, false);
                candy.transform.localPosition = Position(x, y);
                candies[x, y] = candy;
                candy.SetValue(x, y, color[Random.Range(0, color.Length)]);
            }  
    }
    Vector2 Position(int x, int y)
    {
        float fieldWidth = FieldSizeX * (CellSize + Spacing) + Spacing;
        float fieldHight = FieldSizeY * (CellSize + Spacing) + Spacing;
        
        float startX = -(fieldWidth / 2) +(CellSize / 2) + Spacing;
        float startY = -(fieldHight / 2) +(CellSize / 2) + Spacing;

        var position = new Vector2(startX + (x * (CellSize + Spacing)), startY + (y * (CellSize + Spacing)));
        if (rt.sizeDelta.x < fieldWidth || rt.sizeDelta.y < fieldHight)
            rt.sizeDelta = new Vector2(fieldWidth, fieldHight);

        return position;        
    }
}
