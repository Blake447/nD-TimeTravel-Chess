using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blake447
{
public class GameClient : MonoBehaviour
{
        [SerializeField] GameInstance game;
        [SerializeField] CameraRig cameraRig;
        [SerializeField] GameObject cursorGizmo;
        [SerializeField] GameObject snapCursorGizmo;
        [SerializeField] GameObject buffCursorGizmo;

        public PlayerSlot[] playerSlots;
        public ButtonRelay[] buttonRelays;
    
        int[] coordBuffer = null;

        public void SubmitTurn()
        {
            game.SubmitTurn();
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
        void ClickOnCoordinate(int[] coordinate)
        {
            Multiverse multiverse = game.GetMultiverse();
            Board board = multiverse.GetBoardFromCoordinate(coordinate);

            // If we dont have anything selected, try to select it
            if (coordBuffer == null && multiverse != null && board != null && coordinate != null)
            {
            
                int piece_code = multiverse.GetPieceAt(coordinate);
                if (piece_code > 0)
                {
                    coordBuffer = (int[])coordinate.Clone();
                    Board boardBuffer = multiverse.GetBoardFromCoordinate(coordBuffer);
                    game.HighlightMoves(coordinate);
                    if (buffCursorGizmo != null)
                    {
                        buffCursorGizmo.transform.position = boardBuffer.CoordinateToPosition(coordBuffer);
                        buffCursorGizmo.SetActive(true);
                    }
                }
            }
            else
            {
                if (game != null && game.IsGameInProgress())
                {
                    bool bufferFilled = (coordBuffer != null);
                    if (bufferFilled)
                        game.ProcessClick(coordBuffer, coordinate);
                    else
                        Debug.LogWarning("buffer is not properly filled");
                }
                coordBuffer = null;
                game.HighlightClear();
                if (buffCursorGizmo != null)
                    buffCursorGizmo.SetActive(false);
            }
        }
        void ClickOffBoard()
        {
            if (Input.GetMouseButtonDown(0))
                ClearBuffer();
        }
        void ClearBuffer()
        {
            coordBuffer = null;
            if (game != null)
                game.HighlightClear();
            if (buffCursorGizmo != null)
                buffCursorGizmo.SetActive(false);
        }
        #endregion
        // Raycasting interactions through the mouse
        #region Raycasting
        void HandleCasts()
        {
            if (CastUI())
            {
                cameraRig.Lock();
                //Debug.Log("Locking Camera");
            }
            else
            {
                cameraRig.Unlock();
                //Debug.Log("Unlocking Camera");
            }



            bool hasHit = false;
            hasHit = hasHit || CastBoard();
            if (!hasHit)
                cameraRig.SetRaycastMissed();
            hasHit = hasHit || CastGizmos();
            if (!hasHit)
                ClickOffBoard();
        }
        bool CastUI()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            float cast_dist = 10000.0f;
            int layerMask = 1 << 5;
            bool raycastHit = Physics.Raycast(ray, out hit, cast_dist, layerMask);
            return raycastHit;
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
            bool snapInput = didSnap && coordinate != null && Input.GetMouseButtonDown(0);
            if (snapInput)
                ClickOnCoordinate(coordinate);
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
            if (game != null)
            {
                for (int i = 0; i < playerSlots.Length; i++)
                    playerSlots[i].BindToGame(game, i);
                foreach (ButtonRelay br in buttonRelays)
                    br.BindButtonToGameStatus(game);
            }
            ClearBuffer();
        }
        #endregion
    }
}
