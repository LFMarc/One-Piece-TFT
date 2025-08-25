using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip musicForThisScene;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(musicForThisScene);
        }
    }
}
