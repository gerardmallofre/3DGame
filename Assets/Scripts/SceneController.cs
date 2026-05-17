using UnityEngine.SceneManagement;

public static class SceneController
{
    private static readonly string menuScene = "MenuScene";
    private static readonly string creditsScene = "CreditsScene";
    private static readonly string gameScene = "SampleScene";

    private static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void ChangeToMenuScene()
    {
        ChangeScene(menuScene);
    }

    public static void ChangeToGameScene()
    {
        ChangeScene(gameScene);
    }

    public static void ChangeToCreditsScene()
    {
        ChangeScene(creditsScene);
    }

}