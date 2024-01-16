using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEditor : MonoBehaviour
{
    public int selectedPiece = 0;

    [SerializeField] GameInstance game;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] GameObject cursorGizmo;
    [SerializeField] GameObject snapCursorGizmo;
    [SerializeField] GameObject buffCursorGizmo;


    public PlayerSlot[] playerSlots;
    public ButtonRelay[] buttonRelays;

    int[] coordBuffer = null;

    public PalleteButton defaultButton;
    public TMPro.TMP_InputField inputField;
    public TMPro.TMP_InputField fenField;

    public void SubmitTurn()
    {
        game.SubmitTurn();
    }
    public void MirrorY(bool isNegative)
    {
        MirrorState(1, isNegative);
    }
    public void MirrorW(bool isNegative)
    {
        MirrorStateDouble(isNegative);
    }
    public void MirrorState(int dimension_index, bool isNegativeToPositive)
    {
        Board board = game.GetMultiverse().GetRootBoard();
        board.MirrorState(dimension_index, isNegativeToPositive);
    }
    public void MirrorStateDouble(bool isNegativeToPositive)
    {
        Board board = game.GetMultiverse().GetRootBoard();
        board.MirrorStateDouble(3, isNegativeToPositive);
    }
    public void LoadFen()
    {

    }


    public void SaveGameState()
    {
        string name = inputField.text;
        int[] dimensions = (int[])game.GetRuleSet().local_dimensions.Clone();
        int[] boardState = (int[])game.GetMultiverse().GetRootBoard().RequestState().Clone();
        BoardState state = new BoardState(name, dimensions, boardState, null);
        BoardLoader.SaveCustomBoardState(state);
    }
    public void LoadGameState()
    {
        string name = inputField.text;
        BoardState state = BoardLoader.LoadCustomBoardState(name);
        if (state != null)
        {
            Board board = game.GetMultiverse().GetRootBoard();
            int length = state.board_state.Length;
            int[] changes = new int[length];
            bool[] enpassant = new bool[length];
            board.TransferState(state.board_state, changes, enpassant);
        }
    }

    // Actions are controlling the game. Focus Camera, Clicking on coordinates, etc
    #region Game Control
    void FocusCamera(Vector3 position)
    {
        Multiverse multiverse = game.GetMultiverse();
        Board board = multiverse.GetNearestBoard(position);
        if (board != null)
        {
            Vector3 camPos = board.SnapCamera(position);
            cameraRig.SetTarget(camPos);
        }
    }
    public void UndoMove()
    {
        Historian historian = game.GetHistorian();
        historian.UndoMove(game);
    }
    void ClickOnCoordinate(int[] coordinate, int mouseButton)
    {
        Multiverse multiverse = game.GetMultiverse();
        Board board = multiverse.GetBoardFromCoordinate(coordinate);
        multiverse.SetPieceAt(selectedPiece + (mouseButton == 1 ? 32 : 0), coordinate);
    }
    void ClickWheelOnCoordinate(int[] coordinate)
    {
        int piece = game.GetMultiverse().GetPieceAt(coordinate);
        if (piece >= 0)
            game.HighlightMoves(coordinate);
        else
            game.HighlightClear();
    }
    void ClickOffBoard()
    {
        if (Input.GetMouseButtonDown(0))
            ClearBuffer();
    }
    void ClearBuffer()
    {

    }
    #endregion
    // Raycasting interactions through the mouse
    #region Raycasting
    void HandleCasts()
    {
        bool hasHit = false;
        hasHit = hasHit || CastBoard();
        if (!hasHit)
            cameraRig.SetRaycastMissed();
        hasHit = hasHit || CastGizmos();
        if (!hasHit)
            ClickOffBoard();
    }
    bool CastGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float cast_dist = 10000.0f;
        int layerMask = 1 << 16;
        bool raycastHit = Physics.Raycast(ray, out hit, cast_dist, layerMask);
        return raycastHit;
    }
    bool CastBoard()
    {
        Vector3 rawCast = Vector3.zero;
        Vector3 snpCast = Vector3.zero;
        bool didSnap = false;
        int[] coordinate = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float cast_dist = 10000.0f;
        int layerMask = 1 << 15;
        bool raycastHit = Physics.Raycast(ray, out hit, cast_dist, layerMask);
        if (raycastHit)
        {
            rawCast = hit.point;
            snpCast = hit.point;

            Multiverse multiverse = game.GetMultiverse();
            coordinate = multiverse.PositionToCoordinate(rawCast);

            if (coordinate != null)
            {
                snpCast = multiverse.CoordinateToPosition(coordinate);
                didSnap = true;
            }

        }
        bool castCursor = raycastHit && cursorGizmo != null;
        if (castCursor)
            cursorGizmo.transform.position = rawCast;
        cursorGizmo.SetActive(castCursor);
        bool snapCursor = didSnap && snapCursorGizmo != null;
        if (snapCursor)
            snapCursorGizmo.transform.position = snpCast;
        snapCursorGizmo.SetActive(snapCursor);
        bool snapInput = didSnap && coordinate != null && (Input.GetMouseButtonDown(0));
        int mouseIndex = (Input.GetKey(KeyCode.LeftShift) ? 1 : 0);
        if (snapInput)
            ClickOnCoordinate(coordinate, mouseIndex);
        bool snapWheel = didSnap && coordinate != null && (Input.GetMouseButtonDown(1));
        if (snapWheel)
            ClickWheelOnCoordinate(coordinate);
        bool snapCamera = raycastHit && Input.GetKeyDown(KeyCode.F);
        if (snapCamera)
            FocusCamera(rawCast);
        return raycastHit;
    }



    #endregion
    // methods like Awake, Update, Start
    #region Standard Methods
    //private void Awake()
    //{
    //    InitializeClient();
    //}
    void Update()
    {
        if (game != null)
            HandleCasts();
    }
    #endregion
    // Initialization and Reset
    #region Setup
    public void InitializeClient(GameInstance gameInstance)
    {
        game = gameInstance;
        PiecePallete pallete = gameInstance.GetRuleSet().GetPallete();
        for (int i = 0; i < pallete.GetPieceCount(); i++)
        {
            nChessPiece piece = pallete.GetPiece(i);
            if (piece != null)
            {
                PalleteButton button = Instantiate(defaultButton);
                button.transform.SetParent(defaultButton.transform.parent);
                button.transform.localScale = Vector3.one;
                button.SetChessPiece(piece);
            }
       
            

        }
        //if (game != null)
        //{
        //    for (int i = 0; i < playerSlots.Length; i++)
        //        playerSlots[i].BindToGame(game, i);
        //    foreach (ButtonRelay br in buttonRelays)
        //        br.BindButtonToGameStatus(game);
        //}
        ClearBuffer();
        



    }
    #endregion
}
