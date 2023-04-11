using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Threading;
using System.Collections;

public class Field : MonoBehaviour
{
    public static Field Instance;

    public float CellSize;
    public float Spacing;
    public int FieldSizeX;
    public int FieldSizeY;

    [SerializeField]
    private Cell cellPref;
    [SerializeField]
    private Candy candyPref;
    [SerializeField]
    private CandyAnimation animationPref;
    [SerializeField]
    private RectTransform rt;

    [HideInInspector] public Cell[,] cells;
    [HideInInspector] public Candy[,] candies;

    public Color[] color;

    [ContextMenu("Activate")]
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        DOTween.Init();

        ClearField();

        if (cells == null)
            CreateField();
        if (candies == null)
            SpawnCandys();
    }

    public void CandyFall(int x, int y)
    {
        Instantiate(animationPref, transform, false).Fall(candies[x, y + 1], candies[x, y]);
    }
    public void NewCandyAnim(Candy candy)
    {
        Instantiate(animationPref, transform, false).NewCandyAnim(candy);
    }

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
                if (candies[x, y])
                {
                    candies[x, y].SetValue(x, y, Random.Range(1, color.Length));
                }
                var candy = Instantiate(candyPref, transform, false);
                candy.transform.localPosition = Position(x, y);
                candies[x, y] = candy;
                candy.SetValue(x, y, color[Random.Range(0, color.Length)]);
            }
        }
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
                    DestroyImmediate(candies[x, y].gameObject);
            }
        }

        cells = null;
        candies = null;
    }

    private void CreateField()
    {
        cells = new Cell[FieldSizeX, FieldSizeY];

        for (int x = 0; x < FieldSizeX; x++)
        {
            for (int y = 0; y < FieldSizeY; y++)
            {
                var cell = Instantiate(cellPref, transform, false);
                cell.transform.localPosition = Position(x, y, true);
                cells[x, y] = cell;
                cell.SetValue(x, y);
            }
        }
    }
    private void SpawnCandys()
    {
        candies = new Candy[FieldSizeX, FieldSizeY];

        for (int y = 0; y < FieldSizeY; y++)
        {
            for (int x = 0; x < FieldSizeX; x++)
            {
                StartCoroutine(NewCandy(x, y));
            }
        }
    }
    private IEnumerator NewCandy(int x, int y)
    {
        var candy = Instantiate(candyPref, transform, false);
        candy.transform.localPosition = Position(x, y, false);
        candies[x, y] = candy;
        candy.SetValue(x, y, 0);
        yield return new WaitForSeconds(y / 2f);
        NewCandyAnim(candy);
    }
    public Vector2 Position(int x, int y, bool UpdateField)

    private Vector2 Position(int x, int y)
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

    private void SetDirty()
    {
        EditorUtility.SetDirty(this);
    }
}
