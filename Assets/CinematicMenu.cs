using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicMenu : MonoBehaviour
{
    public GameObject[] cinematicObjects;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject cinematicObject in cinematicObjects)
        {
            cinematicObject.SetActive(false);
        }
    }

    public void ShowCinematic()
    {
        foreach (GameObject cinematicObject in cinematicObjects)
        {
            cinematicObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Scenes/Game");

    }

    public void ExplosionSound()
    {
        GetComponent<AudioSource>().Play();
    }
}
