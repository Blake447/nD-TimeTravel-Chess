using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetupMenu : MonoBehaviour
{
	public static GameSetupMenu instance;
	public GameObject MainMenuObjects;
	
	public UniversalLoader loader;
	public Board boardTemplate;
	public string fileName;
	public GameObject GameRoot;
	public RoomList roomList;
	public HistoryLibrarian librarian;

	public bool isBoardSpawned = false;

	public GameDescriptor[] presets;

	public TMPro.TMP_Dropdown boardDropDown;
	public MVNode[] boards;

	public TMPro.TMP_Dropdown layoutDropDown;
	public BoardLayout[] layouts;
	public List<BoardLayout> loadedLayouts;
	public string[] filenames;
	
	public int[][][] exclusions;

	public int selectedBoard;
	public MVNode selectedNode;
	public BoardLayout selectedLayout;

	public GameSettings settings;

	public void Awake()
	{
		settings = new GameSettings();
		instance = this;
		RefreshLayouts();
		UpdateBoards();
	}
	public void RefreshLayouts()
	{
		Debug.Log("Refreshing Layouts");
		layouts = (BoardLayout[])BoardLoader.GetSavedLayouts().Clone();
	}
	public void SetTimeTravel(bool _timeTravel)
	{
		settings.isTimeTravel = _timeTravel;
	}
	public MVNode GetNode(int index)
	{
		if (index >= 0 && index < boards.Length)
		{
			return boards[index];
		}
		return null;
	}
	public void UpdateBoards()
	{
		if (boards != null)
		{
			boardDropDown.ClearOptions();
			List<TMPro.TMP_Dropdown.OptionData> list = new List<TMPro.TMP_Dropdown.OptionData>();
			foreach (MVNode node in boards)
			{
				TMPro.TMP_Dropdown.OptionData optionData = new TMPro.TMP_Dropdown.OptionData();
				optionData.text = node.displayName;
				list.Add(optionData);
			}
			boardDropDown.AddOptions(list);

			if (boards.Length >0)
			{
				UpdateSelectedBoard(0);
			}
		}
	}

	public void UpdateSelectedBoard(int _index)
	{
		layoutDropDown.ClearOptions();
		List<TMPro.TMP_Dropdown.OptionData> list = new List<TMPro.TMP_Dropdown.OptionData>();

		Debug.Log("Board index selected: " + _index);
		Debug.Log("Board selected: " + boards[_index].displayName);
		selectedNode = boards[_index];
		selectedBoard = _index;

		loadedLayouts = new List<BoardLayout>();
		if (layouts != null)
		{
			for (int i = 0; i < layouts.Length; i++)
			{
				if (layouts[i].boardName == boards[_index].displayName)
				{
					TMPro.TMP_Dropdown.OptionData optionData = new TMPro.TMP_Dropdown.OptionData();
					optionData.text = layouts[i].displayName;
					list.Add(optionData);
					loadedLayouts.Add(layouts[i]);
				}
			}
			layoutDropDown.AddOptions(list);
		}
		else
		{
			Debug.LogError("Failed to load layouts from file");
		}
		UpdateSelectedLayout(0);
	}

	public void UpdateSelectedLayout(int _index)
	{
		if (loadedLayouts != null && loadedLayouts.Count > 0 && _index < loadedLayouts.Count)
		{
			selectedLayout = loadedLayouts[_index];
		}
		else
		{
			Debug.LogWarning("No layouts found for board, or invalid layout selected " + boards[_index].displayName);
		}
	}

	public void CreateBoard()
	{
		int boardIndex = selectedBoard;
		BoardLayout layout = selectedLayout;
		GameSettings settings = this.settings;

		CreateBoardLocally(boardIndex, layout, settings);
		if (PhotonNetwork.IsConnected)
		{
			librarian.SendGameBoard(boardIndex, layout, settings);
		}
		
	}
	public void CreateBoardLocally(int boardIndex, BoardLayout layout, GameSettings settings)
	{
		Debug.Log("CreateBoardLocally(" + boardIndex + ", " + layout + ", " + settings + ")");
		isBoardSpawned = true;
		roomList.DisableMenu();
		MainMenuObjects.SetActive(false);
		loader.LoadGame(boardIndex, layout, settings);
	}
	public void CleanupBoard()
	{
		isBoardSpawned = false;
		MainMenuObjects.SetActive(true);
		for (int i = 0; i < GameRoot.transform.childCount; i++)
		{
			Destroy(GameRoot.transform.GetChild(i).gameObject);
		}
	}


}
