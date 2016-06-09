using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MatchThree
{
    public class UI_S : MonoBehaviour {

        // Scene selector
        public void StartGame()
        {
            SceneManager.LoadScene("Cri_Scene");
        }

        public void Credits()
        {
            SceneManager.LoadScene("Credits");
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        // Quit game script
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}