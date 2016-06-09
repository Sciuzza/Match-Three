using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartGame_S : MonoBehaviour {

	public void StartGame()
    {
        SceneManager.LoadScene("Cri_Scene");
    }
	
}