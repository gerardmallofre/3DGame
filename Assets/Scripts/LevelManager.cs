using UnityEngine;

public class LevelManager : MonoBehaviour
{
	private int currentRoom = 1;

	public void CompleteRoom()
	{
		currentRoom++;
		HUDManager.Instance?.SetRoom(currentRoom);
	}
}