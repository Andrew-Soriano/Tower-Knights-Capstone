using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTileTexture : MonoBehaviour
{
    [SerializeField] private Texture _summer;
    [SerializeField] private Texture _fall;
    [SerializeField] private Texture _winter;
    [SerializeField] private Texture _spring;

    private void Awake()
    {
        GetComponent<Renderer>().material.mainTexture = ApplyTextureForScene();
    }

    private Texture ApplyTextureForScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        return sceneName switch
        {
            "Level" => _summer,
            _ => null
        };
    }
}
