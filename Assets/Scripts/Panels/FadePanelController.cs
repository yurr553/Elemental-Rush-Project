using System.Collections;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{

    public Animator panelAnim;
    public Animator gameInfoAnim;


    public void Ok()
    {
        if (panelAnim != null && gameInfoAnim != null)
        {
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(GameStartCo());
        }
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);

        Board board = FindFirstObjectByType<Board>();
        if (board != null)
        {
            board.currentState = GameState.move;
            Debug.Log("OK pressed → Game unpaused");
        }

        // Hide the panel after fade
        gameObject.SetActive(false);
    }


    public void GameOver()
    {
        if (panelAnim != null)
        {
            panelAnim.SetBool("Out", false);
            panelAnim.SetBool("Game Over", true);
        }
    }
}
