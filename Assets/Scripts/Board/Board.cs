using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    BlankTile,
    NormalTile,
    Breakable,
    FireTile,
    AcidTile,
    MagmaTile,
    RustTile,
    IceTile,
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    [Header("Scriptable Object Stuff")]
    public World world;
    public int level;

    [Header("Board Dimensions")]
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    [Header("Runtime")]
    private bool[,] blankSpaces;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;

    public TileType[] boardLayout;

    private ScoreManager scoreManager;
    private SoundManager soundManager;
    public int basePieceValue = 25;
    private int streakValue = 1;

    public int[] scoreGoals;
    private GoalManager goalManager;

    public float refillDelay = 0.5f;


    [Header("Tiles")]
    public GameObject fireTilePrefab;
    public BackgroundTile[,] fireTiles;
    public GameObject acidTilePrefab;
    public BackgroundTile[,] acidTiles;
    public GameObject iceTilePrefab;
    public BackgroundTile[,] iceTiles;
    public GameObject magmaTilePrefab;
    public BackgroundTile[,] magmaTiles;
    public GameObject rustTilePrefab;
    public BackgroundTile[,] rustTiles;

    private PowerInventory powerInv;
    private static readonly Vector2Int[] neighbourOffsets =
{
    new Vector2Int( 1, 0),  
    new Vector2Int(-1, 0),  
    new Vector2Int( 0, 1),  
    new Vector2Int( 0,-1),  
};
    private void Awake()
    {
        currentState = GameState.pause;
        Debug.Log("Current Level Loaded = " + level);
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length && world.levels[level] != null)
            {
                width = world.levels[level].width;
                height = world.levels[level].height;
                dots = world.levels[level].dots;
                scoreGoals = world.levels[level].scoreGoals;
                boardLayout = world.levels[level].boardLayout;
            }
        }

    }

    void Start()
    {
        fireTiles = new BackgroundTile[width, height];
        acidTiles = new BackgroundTile[width, height];
        iceTiles = new BackgroundTile[width, height];
        magmaTiles = new BackgroundTile[width, height];
        rustTiles = new BackgroundTile[width, height];

        goalManager = FindFirstObjectByType<GoalManager>();
        findMatches = FindFirstObjectByType<FindMatches>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
        soundManager = FindFirstObjectByType<SoundManager>();
        powerInv = FindFirstObjectByType<PowerInventory>();

        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];

        SetUp();
        currentState = GameState.pause;

        StartCoroutine(EnsurePlayableStart());
    }



    public void GenerateBlanksSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.BlankTile)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    private void GenerateFireTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.FireTile)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(fireTilePrefab, tempPosition, Quaternion.identity);
                fireTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void GenerateAcidTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.AcidTile)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(acidTilePrefab, tempPosition, Quaternion.identity);
                acidTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void GenerateIceTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.IceTile)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(iceTilePrefab, tempPosition, Quaternion.identity);
                iceTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void GenerateMagmaTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.MagmaTile)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(magmaTilePrefab, tempPosition, Quaternion.identity);
                magmaTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void GenerateRustTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.RustTile)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(rustTilePrefab, tempPosition, Quaternion.identity);
                rustTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void SetUp()
    {
        GenerateBlanksSpaces();
        GenerateFireTiles();
        GenerateAcidTiles();
        GenerateRustTiles();
        GenerateIceTiles();
        GenerateMagmaTiles();


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null && fireTiles[i, j] == null && acidTiles[i, j] == null && rustTiles[i, j] == null && iceTiles[i, j] == null && magmaTiles[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";

                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;

                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }



    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {

            if (findMatches != null)
            {
                findMatches.currentMatches.Remove(allDots[column, row]);
            }

            if (goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            if (soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise();
            }

            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);

            Destroy(allDots[column, row]);

            if (scoreManager != null)
            {
                scoreManager.IncreaseScore(basePieceValue * streakValue);
            }

            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        AwardPowersForCurrentMatches();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo2());
    }


    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null && fireTiles[i, j] == null && acidTiles[i, j] == null && rustTiles[i, j] == null && iceTiles[i, j] == null && magmaTiles[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allDots[i, k] != null)
                        {
                            Dot dotToMove = allDots[i, k].GetComponent<Dot>();
                            dotToMove.row = j;
                            allDots[i, j] = allDots[i, k];
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && fireTiles[i, j] == null && acidTiles[i, j] == null && rustTiles[i, j] == null && iceTiles[i, j] == null && magmaTiles[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        if (findMatches != null) findMatches.FindAllMatches();
        yield return null;

        // If there are chain matches, resolve them and exit this pass.
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break; // we'll come back here via the coroutine chain
        }

        currentDot = null;

        // Deadlock? Shuffle (ShuffleBoardCo already restores state when done unless paused)
        if (IsDeadLocked())
        {
            StartCoroutine(ShuffleBoardCo());
            yield break; // let the shuffle coroutine handle state
        }

        // Board is stable and playable again
        yield return new WaitForSeconds(refillDelay);
        streakValue = 1;

        // ✅ IMPORTANT: if not paused, return control to the player
        if (currentState != GameState.pause)
            currentState = GameState.move;
    }



    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 2)
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                                allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag &&
                                allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right)) return false;
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up)) return false;
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoardCo()
    {

        bool wasPaused = (currentState == GameState.pause);

        if (!wasPaused)
            currentState = GameState.wait;

        yield return new WaitForSeconds(refillDelay * 2);


        List<GameObject> piecesToShuffle = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    piecesToShuffle.Add(allDots[i, j]);

                    allDots[i, j] = null;
                }
            }
        }


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                if (!blankSpaces[i, j] && fireTiles[i, j] == null && acidTiles[i, j] == null && rustTiles[i, j] == null && iceTiles[i, j] == null && magmaTiles[i, j] == null)
                {

                    if (piecesToShuffle.Count == 0) break;


                    int pieceToUseIndex = Random.Range(0, piecesToShuffle.Count);
                    GameObject selectedPiece = piecesToShuffle[pieceToUseIndex];


                    int maxIterations = 0;
                    while (MatchesAt(i, j, selectedPiece) && maxIterations < 100)
                    {
                        pieceToUseIndex = Random.Range(0, piecesToShuffle.Count);
                        selectedPiece = piecesToShuffle[pieceToUseIndex];
                        maxIterations++;
                    }


                    Dot dotComponent = selectedPiece.GetComponent<Dot>();
                    dotComponent.column = i;
                    dotComponent.row = j;


                    selectedPiece.transform.position = new Vector2(i, j);


                    allDots[i, j] = selectedPiece;


                    piecesToShuffle.RemoveAt(pieceToUseIndex);
                }
            }
        }


        if (IsDeadLocked())
        {
            Debug.Log("Board is still deadlocked after shuffle. Shuffling again...");
            yield return StartCoroutine(ShuffleBoardCo());
        }
        else
        {
            Debug.Log("Board shuffled and is now playable.");

            if (findMatches != null)
            {
                findMatches.FindAllMatches();
                DestroyMatches();
            }


            if (!wasPaused)
                currentState = GameState.move;
        }
    }


    private void AwardPowersForCurrentMatches()
    {
        if (powerInv == null) return;

        
        bool[,] visitedCell = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileGO = allDots[x, y];
                if (tileGO == null) continue;

                Dot dotComp = tileGO.GetComponent<Dot>();
                if (dotComp == null || !dotComp.isMatched) continue;   
                if (visitedCell[x, y]) continue;                       

                string groupTag = tileGO.tag;                          

               
                int groupSize = 0;
                var floodStack = new System.Collections.Generic.Stack<Vector2Int>();
                floodStack.Push(new Vector2Int(x, y));
                visitedCell[x, y] = true;

                while (floodStack.Count > 0)
                {
                    Vector2Int cell = floodStack.Pop();
                    groupSize++;

                    foreach (Vector2Int offset in neighbourOffsets)
                    {
                        int nx = cell.x + offset.x;
                        int ny = cell.y + offset.y;

                       
                        if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                        if (visitedCell[nx, ny]) continue;

                        GameObject neighborGO = allDots[nx, ny];
                        if (neighborGO == null) continue;

                        Dot neighborDot = neighborGO.GetComponent<Dot>();
                        if (neighborDot == null || !neighborDot.isMatched) continue; 
                        if (neighborGO.tag != groupTag) continue;                    

                        visitedCell[nx, ny] = true;
                        floodStack.Push(new Vector2Int(nx, ny));
                    }
                }
                if (groupSize >= 3)
                {
                    powerInv.AddIcons(groupTag, 1);
                }
            }
        }
    }

    private IEnumerator EnsurePlayableStart()
    {
        
        yield return null;

        
        if (findMatches != null) findMatches.FindAllMatches();

        
        while (IsDeadLocked())
        {
            yield return StartCoroutine(ShuffleBoardCo());
            yield return null;
            if (findMatches != null) findMatches.FindAllMatches();
        }
    }
}
