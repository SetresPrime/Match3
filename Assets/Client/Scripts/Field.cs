using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
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
    private Cell cellPref;
    [SerializeField]
    private Candy candyPref;
    [SerializeField]
    private RectTransform rt;


    [HideInInspector] public Cell[,] cells;
    [HideInInspector] public Candy[,] candies;

    public Color[] color;

    private List<int> _fallingColumns = new List<int>();

    private void Start()
    {
        if (cells == null || candies == null)
        {
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
                var candy = candies[x, y];
                if (candy)
                {
                    candy.SetValue(x, y);
                    candy.SetColor(color[Random.Range(1, color.Length)]);
                    candy.OnClick += OnClickCandy;
                }
            }
        }
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
                SpawnCell(x, y);
            }
        }
        _fallingColumns.Clear();
    }

    private void SpawnCell(int x, int y)
    {
         var cell = Instantiate(cellPref, transform, false);
         cell.transform.localPosition = Position(x, y, false);
         cells[x, y] = cell;
         cell.SetValue(x, y);
    }

    private IEnumerator SpawnCandiesRoutine(int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            yield return new WaitForSeconds(CANDY_SPAWN_ANIMATION_DELAY);

            for (int x = 0; x < width; x++)
            {
                SpawnCandy(x, y);
            }
        }
        yield return new WaitForSeconds(CANDY_SPAWN_ANIMATION_DELAY);
        _fallingColumns.Clear();
    }

    private IEnumerator SpawnCandyRoutine(int x, int y, bool isUseDelay = true, Action<Candy> callback = null)
    {
        if (isUseDelay)
            yield return new WaitForSeconds(CANDY_SPAWN_ANIMATION_DELAY);

        var candy = SpawnCandy(x, y);

        yield return new WaitForSeconds(CANDY_SPAWN_ANIMATION_DELAY);
        callback?.Invoke(candy);
    }

    private Candy SpawnCandy(int x, int y)
    {
        var candy = Instantiate(candyPref, transform, false);

        candy.transform.localPosition = Position(x, y, false);
        candies[x, y] = candy;

        candy.SetValue(x, y);
        candy.SetColor(color[Random.Range(1, color.Length)]);
        candy.SpawnAnim(this);
        candy.OnClick += OnClickCandy;

        if (y == 0)
            _fallingColumns.Add(x);

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

    private void OnClickCandy(Candy candy)
    {
        if (candy == null || _fallingColumns.Contains(candy.X)) return;

        _fallingColumns.Add(candy.X);

        for (int y = candy.Y; y < FieldSizeY - 1; y++)
        {
            var currentCandy = candies[candy.X, y];
            var candyOverCurrent = candies[candy.X, y + 1];

            candies[candy.X, y] = candyOverCurrent;
            candyOverCurrent.SetValue(candy.X, y);
            candyOverCurrent.Fall(currentCandy.transform.position);

            Debug.Log($"FALL   X: {candyOverCurrent.X}, Y: {candyOverCurrent.Y}");
        }

        Debug.Log($"SPAWN   X: {candy.X}, Y: {FieldSizeY - 1}");
        StartCoroutine(SpawnCandyRoutine(candy.X,FieldSizeY - 1, callback: (spawnedCandy) => _fallingColumns.Remove(spawnedCandy.X)));

        candy.OnClick -= OnClickCandy;
        Destroy(candy.gameObject);
    }
}
