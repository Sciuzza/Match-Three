using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MatchThree
{
    public class UI_S : MonoBehaviour {

        General_functions Scene_m_linking;

        void Awake()
        {
            Scene_m_linking = GameObject.Find("Scene Manager").GetComponent<General_functions>();
        }

        // Scene selector
        public void StartGame()
        {
            Scene_m_linking.Loading_gameplay();
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