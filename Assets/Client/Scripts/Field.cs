using UnityEditor;
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

    [SerializeField][HideInInspector] private Cell[,] cells;
    [SerializeField][HideInInspector] private Candy[,] candies;

    public Color[] color;

    [ContextMenu ("Activate")]
    private void Awake()
    {
        ClearField();

        if (cells == null) 
           CreateField();
        if (candies == null)
           SpawnCandys();

        SetDirty();
    }

    [ContextMenu("Reset")]
    private void ResetCandy()
    {
        if (cells == null || candies == null)
            return;

        for(int x = 0; x < FieldSizeX; x++)
        {
            for (int y = 0; y < FieldSizeY; y++)
            {
                if (candies[x, y])
                {
                    candies[x, y].SetValue(x, y, color[Random.Range(0, color.Length)]);
                }
            }
        }

        SetDirty();
    }

    [ContextMenu("Clear Field")]
    private void ClearField()
    {
        if (cells == null || candies == null)
        {
            var childrens = transform.GetComponentsInChildren<Transform>();

            foreach (var item in childrens)
            {
                if (item != transform)
                    DestroyImmediate(item.gameObject);
            }

            return;
        }

        for (int x = 0; x < FieldSizeX; x++)
        {
            for (int y = 0; y < FieldSizeY; y++)
            {
                if (cells[x, y])
                    DestroyImmediate(cells[x, y].gameObject);

                if (candies[x, y])
                    DestroyImmediate(candies[x,y].gameObject);
            }
        }

        cells = null;
        candies = null;
        SetDirty();
    }

    private void CreateField()
    {
        cells = new Cell[FieldSizeX,FieldSizeY];

        for (int x = 0;x < FieldSizeX;x++)
        {
            for (int y = 0;y < FieldSizeY; y++)
            {
                var cell = Instantiate(cellPref, transform, false);           
                cell.transform.localPosition = Position(x, y);
                cells[x, y] = cell;
                cell.SetValue(x, y);

            }
        }
    }

    private void SpawnCandys()
    {
        candies = new Candy[FieldSizeX, FieldSizeY];

        for (int x = 0; x < FieldSizeX; x++)
        {
            for (int y = 0; y < FieldSizeY; y++)
            {
                var candy = Instantiate(candyPref, transform, false);
                candy.transform.localPosition = Position(x, y);
                candies[x, y] = candy;
                candy.SetValue(x, y, color[Random.Range(0, color.Length)]);
            }
        }
    }

    private Vector2 Position(int x, int y)
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

    private void SetDirty()
    {
        EditorUtility.SetDirty(this);
    }
}
