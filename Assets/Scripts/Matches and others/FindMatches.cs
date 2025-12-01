using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindFirstObjectByType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        ClearAllMatchFlags();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot == null) continue;

                // Horizontal
                if (i > 0 && i < board.width - 1)
                {
                    GameObject leftDot = board.allDots[i - 1, j];
                    GameObject rightDot = board.allDots[i + 1, j];
                    if (leftDot && rightDot &&
                        leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                    {
                        if (!currentMatches.Contains(leftDot)) currentMatches.Add(leftDot);
                        if (!currentMatches.Contains(rightDot)) currentMatches.Add(rightDot);
                        if (!currentMatches.Contains(currentDot)) currentMatches.Add(currentDot);

                        leftDot.GetComponent<Dot>().isMatched = true;
                        rightDot.GetComponent<Dot>().isMatched = true;
                        currentDot.GetComponent<Dot>().isMatched = true;
                    }
                }

                // Vertical
                if (j > 0 && j < board.height - 1)
                {
                    GameObject upDot = board.allDots[i, j + 1];
                    GameObject downDot = board.allDots[i, j - 1];
                    if (upDot && downDot &&
                        upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                    {
                        if (!currentMatches.Contains(upDot)) currentMatches.Add(upDot);
                        if (!currentMatches.Contains(downDot)) currentMatches.Add(downDot);
                        if (!currentMatches.Contains(currentDot)) currentMatches.Add(currentDot);

                        upDot.GetComponent<Dot>().isMatched = true;
                        downDot.GetComponent<Dot>().isMatched = true;
                        currentDot.GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
        yield break;
    }







    private void ClearAllMatchFlags()
    {
        currentMatches.Clear();
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                var go = board.allDots[x, y];
                if (go != null)
                {
                    var d = go.GetComponent<Dot>();
                    if (d != null) d.isMatched = false;
                }
            }
        }
    }
}
