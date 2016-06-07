using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MatchThree
{
    public class Menu_S : MonoBehaviour
    {

        public SpriteRenderer sr;

        void Awake() //Vengono chiamati prima degli start, da inserire per inizializzare le classi. E' simile ad un costruttore.
        {
            sr = GetComponent<SpriteRenderer>();
        }

        void OnMouseUP()
        {
            Debug.Log("Relase");
            Scene gameScene = SceneManager.GetSceneByName("Gameplay");
            SceneManager.SetActiveScene(gameScene);
            sr.color = new Color(1, 0, 0, 1);
        }

        /*void OnMouseOver()
        {
            Debug.Log("Test");
            sr.color = new Color(1, 0, 0, 1);
        }*/

    }
}
