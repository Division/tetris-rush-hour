using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {

	public TetrisController tetrisController;
	public MainMenu mainMenu;
	public PauseMenu pauseMenu;
	public static MenuHandler Instance { get { return instance; } }
	
	public void ShowMainMenu()
	{
		if (currentMenu)
		{
			currentMenu.gameObject.SetActive(false);
		}
		mainMenu.gameObject.SetActive(true);
		currentMenu = mainMenu;
	}
	
	//--------------------------------------------------------------------------
	
	public void ShowNewGameTetrisController()
	{
		Time.timeScale = 1;
		tetrisController.gameObject.SetActive(true);
		tetrisController.Restart();
		mainMenu.gameObject.SetActive(false);
		SetMenuCameraDisplay(false);
		currentMenu = null;
	}
	
	//--------------------------------------------------------------------------
	
	public void ShowPause()
	{
		Time.timeScale = 0;
		pauseMenu.gameObject.SetActive(true);
		tetrisController.gameObject.SetActive(false);
		SetMenuCameraDisplay(true);
		currentMenu = pauseMenu;
	}
	
	//--------------------------------------------------------------------------
	
	public void ShowResumeTetrisController()
	{
		Time.timeScale = 1;
		tetrisController.gameObject.SetActive(true);
		SetMenuCameraDisplay(false);
	}
	
	//**************************************************************************
	//
	// MonoBehaviour
	//
	//**************************************************************************
	
	void Awake()
	{
		instance = this;
		tetrisController.gameObject.SetActive(false);
		mainMenu.gameObject.SetActive(false);
		SetMenuCameraDisplay(true);
		
		ShowMainMenu();
	}
	
	//--------------------------------------------------------------------------
	
	void Start()
	{
		
	}
	
	//**************************************************************************
	//
	// Private
	//
	//**************************************************************************
	
	private static MenuHandler instance;
	private BaseMenu currentMenu;
	
	private void SetMenuCameraDisplay(bool useMenuCamera)
	{
		GameObject.FindWithTag(TetrisConst.TAG_GAME_CAMERA).camera.enabled = !useMenuCamera;
		GameObject.FindWithTag(TetrisConst.TAG_MENU_CAMERA).camera.enabled = useMenuCamera;
	}
}
