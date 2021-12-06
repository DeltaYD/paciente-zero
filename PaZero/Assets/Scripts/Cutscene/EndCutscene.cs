using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCutscene : MonoBehaviour
{
    public PlayerManager _pm;

    public void SelectCutscene()
    {
        if (_pm._radioProgress < 26)
        {
            // ENDING A: PARASITE WINS
            SceneManager.LoadScene("A-ENDING");
        }
        else
        {
            // ENDING B: PLAYER MUST END HIMSELF SOMEWHERE ELSE
            SceneManager.LoadScene("N-ENDING");
        }
    }
}
