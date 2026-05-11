using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}