using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MatchThree {

    public class General_functions : MonoBehaviour
    {

        void Awake()
        {
            DontDestroyOnLoad(this.transform.gameObject);
        }

        public void restart_scene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Loading_gameplay()
        {
            SceneManager.LoadScene("GamePlay");
        }

        public void Application_quit()
        {
            Application.Quit();
        }
    }
}
