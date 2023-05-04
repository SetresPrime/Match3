using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System;

public class Field : MonoBehaviour
{
    private const float CANDY_SPAWN_ANIMATION_DELAY = 0.2f;

    public int FieldSizeY;
    public int FieldSizeX;

    [SerializeField]
    private float CellSize;
    [SerializeField]
    private float Spacing;
    [SerializeField]
    private float DELETE_WAIT_TIME;

    [SerializeField]
    private Cell cellPref;
    [SerializeField]
    private Candy candyPref;
    [SerializeField]
    private RectTransform rt;

    [HideInInspector] public Cell[,] cells;
    [HideInInspector] public Candy[,] candies;

    public Color[] color;

    private Candy selectCandy;
    private Candy lastSwipeCandy;

    private bool IsStartRound;
    private bool IsWork;
    private bool WasWorkedInRound;
    private bool InAction;

    private void Start()
    {
        IsStartRound = true;
        CandyAnimation.OnEndAnim += LinesCheck;
        if (cells == null || candies == null)
            CreatField();
    }
    private void ClearField()
    {
        var cells = transform.GetComponentsInChildren<Cell>();
        var candies = transform.GetComponentsInChildren<Candy>();

        for (int i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            Destroy(cell.gameObject);
        }

        for (int i = 0; i < candies.Length; i++)
        {
            var candy = candies[i];
            Destroy(candy.gameObject);
        }
    }
    [ContextMenu("Activate")]
    private void CreatField()
    {
        cells = new Cell[FieldSizeX, FieldSizeY];
        candies = new Candy[FieldSizeX, FieldSizeY];

        ClearField();
        Position(0, 0, true);

        SpawnCells(FieldSizeX, FieldSizeY);
        StartCoroutine(SpawnCandiesRoutine(FieldSizeX, FieldSizeY));
    }
    private void SpawnCells(int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cell = Instantiate(cellPref, transform, false);
                cell.transform.localPosition = Position(x, y, false);
                cells[x, y] = cell;
                cell.SetValue(x, y);
            }
        }
    }
    private IEnumerator SpawnCandiesRoutine(int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            yield return new WaitForSeconds(CANDY_SPAWN_ANIMATION_DELAY);

            for (int x = 0; x < width; x++)
            {
                if (y == height - 1 && x == width - 1)
                    SpawnCandy(x, y, 2, true, false);  
                else
                    SpawnCandy(x, y, 2, false, false);
            }
         
        }
        yield return new WaitForSeconds(CANDY_SPAWN_ANIMATION_DELAY);
    }
    private Candy SpawnCandy(int x, int y, int i, bool IsLastCandy, bool IsXline)
    {
        var candy = Instantiate(candyPref, transform, false);

        candy.transform.localPosition = Position(x, IsXline ? FieldSizeY : FieldSizeY + i, false);
        candies[x, y] = candy;

        candy.SetValue(x, y, Random.Range(1, color.Length));
        candy.SetColor(color[candy.color]);
        candy.Fall(transform.TransformPoint(Position(x, y, false)), IsLastCandy);
        candy.OnSelected += OnSelectedCandy;

        return candy;
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
    private void OnSelectedCandy(Candy candy)
    {
        if (!selectCandy && !InAction)
            selectCandy = candy;

        if (selectCandy && Math.Abs(selectCandy.X - candy.X) == 1 && Math.Abs(selectCandy.Y - candy.Y) == 0)
        {
            swipe(candy, false);
            lastSwipeCandy = candy;
        }
        else if (selectCandy && Math.Abs(selectCandy.Y - candy.Y) == 1 && Math.Abs(selectCandy.X - candy.X) == 0)
        {
            swipe(candy, false);
            lastSwipeCandy = candy;
        }
        else if (selectCandy != candy)
            selectCandy = null;
    }
    private void swipe(Candy candy ,bool IsBack)
    {
        InAction = true;
        var mediatorX = candy.X;
        var mediatorY = candy.Y;

        candy.SetValue(selectCandy.X, selectCandy.Y, candy.color);
        selectCandy.SetValue(mediatorX, mediatorY, selectCandy.color);

        candies[candy.X, candy.Y] = candy;
        candies[selectCandy.X, selectCandy.Y] = selectCandy;

        CandyAnimation.SwipeAnim(selectCandy, candy, IsBack);
    }
    private void LinesCheck()
    {
        Invoke("XLinesCheck", DELETE_WAIT_TIME);
    }
    private void XLinesCheck()
    {
        IsWork = false;
        int i = 0;
        for (int y = 0; y < FieldSizeY; y++)
        {
            int currentcolor = 0;
            int pastcolor;
            for (int x = 0; x < FieldSizeX; x++)
            {
                pastcolor = currentcolor;
                currentcolor = candies[x, y].color;

                i = pastcolor == currentcolor ? i + 1 : i;

                if (i >= 2 && currentcolor != pastcolor )
                {
                    DeleteXLine(candies[x - 1, y], i);
                    i = 0;
                }
                else if (i >= 2 && x == FieldSizeX - 1)
                {
                    DeleteXLine(candies[FieldSizeX - 1, y], i);
                    i = 0;
                }
                else if (pastcolor != currentcolor)
                    i = 0;
            }
            i = 0;
            if (IsWork)
                break;
        }
        if (!IsWork)
            YLinesCheck();       
    }
    private void YLinesCheck()
    {
        int i = 0;
        for (int x = 0; x < FieldSizeX; x++)
        {
            int currentcolor = 0;
            int pastcolor;
            for (int y = 0; y < FieldSizeY; y++)
            {
                pastcolor = currentcolor;
                currentcolor = candies[x, y].color;

                i = pastcolor == currentcolor ? i + 1 : i;
                if (i >= 2 && pastcolor != currentcolor)
                {
                    DeleteYLine(candies[x, y - 1], i);
                    i = 0;
                }
                else if (i >= 2 && y == FieldSizeY - 1)
                {
                    DeleteYLine(candies[x, FieldSizeY - 1], i);
                    i = 0;
                }
                else if (pastcolor != currentcolor)
                    i = 0;
            }
            i = 0;
            
        }
        if (!IsWork)
            WasWorkedCheck();       
    }
    private void DeleteXLine(Candy candy, int NumOfCandy)
    {
        WasWorkedInRound = true;
        IsWork = true;
        for (int i = 0; i <= NumOfCandy; i++)
        {
            DeleteCandy(candies[candy.X - i, candy.Y]);
            for (int y = candy.Y; y < FieldSizeY - 1; y++)
            {
                var currentCandy = candies[candy.X - i, y];
                var candyOverCurrent = candies[candy.X - i, y + 1];

                candies[candy.X - i, y] = candyOverCurrent;
                candyOverCurrent.SetValue(candy.X - i, y, candyOverCurrent.color);
                candyOverCurrent.Fall(currentCandy.transform.position, false);
            }
            SpawnCandy(candy.X - i, FieldSizeY - 1, i, i == NumOfCandy, true);       
        }
    }
    private void DeleteYLine(Candy candy, int NumOfCandy)
    {
        WasWorkedInRound = true;
        IsWork = true;
        for (int i = 0; i <= NumOfCandy; i++)
        {
            DeleteCandy(candies[candy.X, candy.Y - NumOfCandy + i]);
            if (candy.Y + 1 + i < FieldSizeY)
            {
                var currentCandy = candies[candy.X, candy.Y + 1 + i];
                var candyUnderCurrent = candies[candy.X, candy.Y - NumOfCandy + i];

                candies[candy.X, candy.Y - NumOfCandy + i] = currentCandy;
                currentCandy.SetValue(candyUnderCurrent.X, candyUnderCurrent.Y, currentCandy.color);
                currentCandy.Fall(candyUnderCurrent.transform.position, false);
            }
        }
        for (int i = 0; i <= NumOfCandy; i++)
            SpawnCandy(candy.X, FieldSizeY - 1 - NumOfCandy + i, i, i == NumOfCandy, false);
    }
    private void DeleteCandy(Candy candy)
    {
        candy.OnSelected -= OnSelectedCandy;
        Destroy(candy.gameObject);
    }
    private void WasWorkedCheck()
    {
        if (!WasWorkedInRound && !IsStartRound)
            swipe(lastSwipeCandy, true);

        WasWorkedInRound = false;
        InAction = false;
        if (selectCandy)
            selectCandy = null;
        if (lastSwipeCandy)
            lastSwipeCandy = null;

        if (IsStartRound)
            IsStartRound = false;
    }
}