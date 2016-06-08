using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MatchThree
{
    public class Menu_S : MonoBehaviour
    {

        public TextMesh tm; //Attenzione all'elemento da caricare, in questo caso è una test mesh

        void Awake() //Vengono chiamati prima degli start, da inserire per inizializzare le classi. E' simile ad un costruttore.
        {
            tm = GetComponent<TextMesh>();
        }

        void OnMouseUp()
        {
            Debug.Log("Relase");
            SceneManager.LoadScene("Cri_Scene");
        }

        void OnMouseOver()
        {
            Debug.Log("Test");
            tm.color = new Color(1, 0, 0, 1);
        }

    }
}
