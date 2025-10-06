using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UniversalLoader : MonoBehaviour
{
    public static UniversalLoader instance;

    // Start is called before the first frame update
    public GameLoader mvtime;
    public GameLoader singularity;
    public GameLoader editor;

    public GameObject GameRoot;

	private void Awake()
	{
        instance = this;
        //LoadGame();
	}
	public void LoadGame(int boardIndex, BoardLayout layout, GameSettings settings)
    {
        Debug.Log("LoadGame(" + boardIndex + ", " + layout + ", " + settings + ")");
        GameDescriptor gameDescriptor = GetComponentInChildren<GameDescriptor>();
        if (gameDescriptor == null || gameDescriptor.gameObject == this.gameObject)
        {
            if (gameDescriptor == null)
            {
                gameDescriptor = this.AddComponent<GameDescriptor>();
            }

            gameDescriptor.board = GameSetupMenu.instance.GetNode(boardIndex);
            gameDescriptor.isTimeTravel = settings.isTimeTravel;
            gameDescriptor.useForwardLateral = false;
            gameDescriptor.allowPromotions = true;
            gameDescriptor.friendlyVisualizer = true;
            gameDescriptor.enemyVisualizer = true;
            gameDescriptor.allowDrops = false;
            gameDescriptor.dropsIndex = 0;
            gameDescriptor.multiverse_offset = gameDescriptor.board.board.GetMVOffsets().x;
		    gameDescriptor.timetravel_offset = gameDescriptor.board.board.GetMVOffsets().y;
            gameDescriptor.timeIndex = gameDescriptor.board.board.GetBoardSize().Length + 1;
            int[] dimensions = gameDescriptor.board.board.GetBoardSize();
            if (gameDescriptor.isTimeTravel)
            {
			    int[] newDimensions = new int[dimensions.Length + 2];
                System.Array.Copy(dimensions, newDimensions, dimensions.Length);
                newDimensions[dimensions.Length] = 32;
                newDimensions[dimensions.Length + 1] = 32;
                dimensions = newDimensions;
            }
            gameDescriptor.dimensions = (int[])dimensions.Clone();
            Debug.Log(Coordinates.CoordinateToString(dimensions));

            gameDescriptor.board_state = (int[])layout.state.Clone();
            int length = dimensions.Length + (gameDescriptor.isTimeTravel ? 2 : 0);
            int[] forwards = new int[dimensions.Length];
            int[] laterals = new int[dimensions.Length];
            for (int i = 0; i < Mathf.Min(forwards.Length, layout.forwards.Length); i++)
            {
                forwards[i] = layout.forwards[i] ? 1 : 0;
                //Debug.Log(menu.selectedLayout.forwards[i]);
            }
		    for (int i = 0; i < Mathf.Min(laterals.Length, layout.laterals.Length); i++)
		    {
			    laterals[i] = layout.laterals[i] ? 1 : 0;
		    }
            gameDescriptor.forwards = (int[])forwards.Clone();
            gameDescriptor.laterals = (int[])laterals.Clone();
        }
        else
        {
            BoardState boardState = BoardLoader.LoadCustomBoardState(gameDescriptor.filename);
            gameDescriptor.board_state = (int[])boardState.board_state.Clone();
        }

        GameLoader loader = null;
		if (RoomList.instance != null && RoomList.instance.editor)
        {
            GameDescriptor descriptor = gameDescriptor;
            {
                loader = Instantiate(editor);
			}
        }
        else
        {
            GameDescriptor descriptor = gameDescriptor;
            if (descriptor != null)
            {
                if (descriptor.isTimeTravel)
                {
                    loader = Instantiate(mvtime);
                }
                else
                {
                    loader = Instantiate(singularity);
                }
            }
        }
        if (loader != null)
        {
            loader.transform.parent = GameRoot.transform;
            Debug.Log("loader.initialise(" + gameDescriptor + ")");
            loader.Initialize(gameDescriptor);
		}
    }
    public void LoadEditor()
    {
    }
}
