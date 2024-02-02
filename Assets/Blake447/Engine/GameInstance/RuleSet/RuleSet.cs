using UnityEngine;
using System.Collections.Generic;


public class RuleSet : MonoBehaviour
{
    //PieceDefiner pieceDefiner;
    nChessPiece[] pieces;
    public int[] local_dimensions;
    public bool FriendlyFire = false;
    public bool EnforceTurns = true;
    public bool EnforceMoves = true;
    public bool AutoSubmit = true;
    public bool AllowEnPassant = true;
    public bool useForwardLateralRule = true;
    bool allowDrops;
    public int time_index = 5;
    public int drops_index;

    PiecePallete pallete;

    Command GhostPawnRemovalBuffer = null;

    //[SerializeField]
    //int[] local_dimensions;
    //[SerializeField]
    //GameDescriptor gameDescriptor;


    // Methods that handle the rules of the game
    #region Rule Methods

    public PiecePallete GetPallete()
    {
        return pallete;
    }


    public int[][] GetCheckingCoordinates(Multiverse multiverse, int[] king_coordinate)
    {
        List<int[]> coordinates = new List<int[]>();
        //Multiverse multiverse = game.GetMultiverse();
        if (king_coordinate != null)
        {
            int[][] valid_moves = GeneratePossibleChecking(multiverse, king_coordinate);
            if (valid_moves != null)
            {
                for (int i = 0; i < valid_moves.Length; i++)
                {
                    bool inBounds = multiverse.IsInBounds(valid_moves[i]);
                    bool isActive = multiverse.IsBoardActive(valid_moves[i]);

                    if (inBounds && isActive)
                    {
                        int piece = multiverse.GetPieceAt(valid_moves[i]);
                        if (piece >= 0)
                        {
                            

                            if (IsCoordinateInTargetCoords(multiverse, valid_moves[i], king_coordinate))
                            {
                                coordinates.Add(valid_moves[i]);
                            }
                        }
                    }
                }
                multiverse.HightlightMoves((int[][])valid_moves.Clone()); ;
            }
        }
        if (coordinates.Count > 0)
        {
            return coordinates.ToArray();
        }
        return null;
    }

    public Move CanMakeMove(Multiverse multiverse, int current_players_turn, Click click)
    {
        Board board = multiverse.GetRootBoard();
        int player_turn = click.player_turn;
        int piece_from = click.piece_from;
        int piece_to = click.piece_to;
        int[] coord_from = (int[])click.coord_from.Clone();
        int[] coord_to = (int[])click.coord_to.Clone();
        
        //Debug.Log(Coordinates.CoordinateToString(coord_from) + " -> " + Coordinates.CoordinateToString(coord_to));
        board.ReprocessCoordinates(coord_from, coord_to);
        //Debug.Log(Coordinates.CoordinateToString(coord_from) + " -> " + Coordinates.CoordinateToString(coord_to));


        //Multiverse multiverse = game.GetMultiverse();

        if (!IsCorrectBoardTurn(multiverse, coord_from, piece_from))
            return null;
        if (!IsCorrectBoardTurn(multiverse, coord_to, piece_from))
            return null;
        if (!IsBoardActive(multiverse, coord_from))
            return null;
        if (!IsSquareAvailable(piece_from, piece_to))
            return null;
        if (EnforceTurns && !IsCorrectTurn(current_players_turn, piece_from))
            return null;

        //Debug.Log("Here");

        if (EnforceMoves && !IsCoordinateInTargetCoords(multiverse, coord_from, coord_to))
            return null;
        //Debug.Log("CanMakeMove()");
        Command command = new Command(piece_from, piece_to, coord_from, coord_to);
        Move move = new Move(command);

        Command enPassantCommand = GenerateEnPassantCommand(multiverse, piece_from, piece_to, coord_from, coord_to);
        if (enPassantCommand != null)
        {
            move.Add(enPassantCommand);
        }
        Command ghostPawnCommand = GenerateGhostPawnCommand(multiverse, piece_from, coord_from, coord_to);
        if (ghostPawnCommand != null)
        {
            move.Add(ghostPawnCommand);
        }
        Command captureCommand = GenerateCaptureCommand(multiverse, piece_from, piece_to, coord_from);
        if (captureCommand != null)
        {
            move.Add(captureCommand);
        }


        Command promoteCommand = GeneratePromotionCommand(multiverse, piece_from,  coord_from, coord_to);
        if (promoteCommand != null)
        {
            move.Add(promoteCommand);
        }

        return move;
    }
    public bool IsDrop(int[] coordinate_from, int[] coordinate_to, bool allowDrops)
    {
        if (!allowDrops)
            return false;
        if ((coordinate_from[drops_index] != 1) || (coordinate_to[drops_index] != 1))
            return true;
        return false;
    }
    Command GenerateEnPassantCommand(Multiverse multiverse, int piece_from, int piece_to, int[] coordinate_from, int[] coordinate_to)
    {

        if ((piece_to % Overseer.PIECE_COUNT) != 30)
            return null;
        if ((piece_from % Overseer.PIECE_COUNT) != 6)
            return null;

        Debug.Log("GenerateEnPassantCommand(" + piece_from + ", " + piece_to + ", " + Coordinates.CoordinateToString(coordinate_from) + ", " + Coordinates.CoordinateToString(coordinate_to) + ")");
        int direction = -1 + 2 * (piece_from / Overseer.PIECE_COUNT);
        Debug.Log("Direction: " + direction);
        int[] forwards = (int[])pallete.GetForwards().Clone();
        Debug.Log("forwards: " + Coordinates.CoordinateToString(forwards));
        for (int i = 0; i < coordinate_to.Length; i++)
        {
            int[] offset = new int[coordinate_to.Length];
            offset[i] = forwards[i] * direction;
            Debug.Log("Offset: " + Coordinates.CoordinateToString(offset));
            int[] target = new int[offset.Length];
            for (int j = 0; j < target.Length; j++)
                target[j] = coordinate_to[j] + offset[j];
            Debug.Log("Target: " + Coordinates.CoordinateToString(target));
            if (multiverse.IsInBounds(target))
            {
                int piece_at = multiverse.GetPieceAt(target);
                if (piece_at % Overseer.PIECE_COUNT == 6 && ((piece_at / Overseer.PIECE_COUNT) != (piece_from / Overseer.PIECE_COUNT)))
                { 
                    Debug.Log("Detected En Passant");
                    int color = piece_at / Overseer.PIECE_COUNT;
                    Command command = new Command(0, 6 + color * Overseer.PIECE_COUNT, target, target);
                    return command;
                }
            }
        }
        return null;
    }
    Command GenerateCaptureCommand(Multiverse multiverse, int piece_from, int piece_to, int[] coord_from)
    {
        if (piece_to != 0 && allowDrops)
        {
            int[] coordinate = new int[coord_from.Length];
            int piece_color = piece_to / Overseer.PIECE_COUNT;
            int new_piece_color = (piece_to + Overseer.PIECE_COUNT) % (2 * Overseer.PIECE_COUNT);
            coordinate[drops_index] = 2 * piece_color;
            Command command = new Command(new_piece_color, 0, coordinate, coordinate);
            return command;
            
        }
        return null;
    }
    Command GeneratePromotionCommand(Multiverse multiverse, int piece_index, int[] coordinate_from, int[] coordinate_to)
    {

        //Multiverse multiverse = game.GetMultiverse();
        int[] board_size = (int[])multiverse.GetBoardSize().Clone();
        nChessPiece piece = GetPiece(piece_index % Overseer.PIECE_COUNT);
        int color = piece_index / Overseer.PIECE_COUNT;
        int promotion_from = piece.PassPromotion(board_size, coordinate_from, color);
        int promotion_to = piece.PassPromotion(board_size, coordinate_to, color);
        int promotion = Mathf.Max(promotion_from, promotion_to);
        Command command = new Command(promotion + color * Overseer.PIECE_COUNT, piece_index, coordinate_to, coordinate_to);
        if (promotion != 0)
            return command;

        return null;
    }
    Command GenerateGhostPawnCommand(Multiverse multiverse, int piece_index, int[] coordinate_from, int[] coordinate_to)
    {
        //Debug.Log("Generate Ghost Pawn Command");
        if ((piece_index % Overseer.PIECE_COUNT) != 6)
            return null;
        //Debug.Log("Pawn: Passed");
        int max_offset = 0;
        for (int i = 0; i < coordinate_to.Length; i++)
            max_offset = Mathf.Max(max_offset, Mathf.Abs(coordinate_to[i] - coordinate_from[i]));
        if (max_offset != 2)
            return null;
        //Debug.Log("Offset: Passed");
        int[] average = new int[coordinate_to.Length];
        int color = piece_index / Overseer.PIECE_COUNT;
        for (int i = 0; i < average.Length; i++)
            average[i] = (coordinate_to[i] + coordinate_from[i]) / 2;

        Command command = new Command(30 + color * Overseer.PIECE_COUNT, 0, average, average);
        return command;
    }

    public bool IsCoordinateInTargetCoords(Multiverse multiverse, int[] coordinate_from, int[] coordinate_to)
    {

        //Multiverse multiverse = game.GetMultiverse();
        if (coordinate_to == null)
            return false;
        if (coordinate_from == null)
            return false;
        int[][] tempCoords = CreateTargetCoords(multiverse, coordinate_from);
        if (tempCoords == null)
            return false;
        int[][] TargetCoords = (int[][])tempCoords.Clone();
        if (TargetCoords == null)
            return false;
        for (int i = 0; i < TargetCoords.Length; i++)
        {
            if (TargetCoords[i] != null)
                if (multiverse.IsInBounds(TargetCoords[i]))
                    if (AreCoordinatesEqual(multiverse, TargetCoords[i], coordinate_to))
                        return true;
        }
        return false;
    }
    public int[][] GeneratePossibleChecking(Multiverse multiverse, int[] coord)
    {
        //Multiverse multiverse = game.GetMultiverse();
        if (multiverse == null)
            return null;
        int CHECKING_PIECE = 12;
        nChessPiece piece = GetPiece(CHECKING_PIECE);
        if (piece == null)
            return null;
        nChessPiece[] subpieces = piece.EnumerateSubPieces();
        if (subpieces == null)
            return null;
        int total_moves = CountMoves(subpieces);
        if (total_moves == 0)
            return null;

        int move_count = 0;
        int[][] moves = new int[total_moves][];
        for (int subpiece = 0; subpiece < subpieces.Length; subpiece++)
        {
            nChessPiece check_piece = subpieces[subpiece];
            bool valid = check_piece != null && check_piece.moves != null;
            for (int move = 0; valid && move < subpieces[subpiece].moves.Length; move++)
            {
                if (subpieces[subpiece].moves[move] != null)
                {
                    int[] coord_from = coord;
                    int[] offset = subpieces[subpiece].moves[move];
                    int[] coord_to = (int[])AddCoordinate(coord_from, offset).Clone();

                    if (IsMoveValid(multiverse, subpieces[subpiece], coord_from, coord_to))
                    {
                        moves[move_count] = coord_to;
                        move_count++;
                    }
                }
            }
        }
        if (move_count == 0)
            return null;
        int[][] moves_out = new int[move_count][];
        for (int i = 0; i < move_count; i++)
            moves_out[i] = (int[])moves[i].Clone();
        return moves_out;

    }
    public int[][] CreateTargetCoords(Multiverse multiverse, int[] coord)
    {
        //Multiverse multiverse = game.GetMultiverse();
        if (multiverse == null)
            return null;
        Board board = multiverse.GetBoardFromCoordinate(coord);
        if (board == null || coord == null)
            return null;
        int piece_index = multiverse.GetPieceAt(coord);
        if (piece_index <= 0)
            return null;
        nChessPiece piece = GetPiece(piece_index % Overseer.PIECE_COUNT);
        if (piece == null)
            return null;
        nChessPiece[] subpieces = piece.EnumerateSubPieces();
        if (subpieces == null)
            return null;


        int total_moves = CountMoves(subpieces);
        if (total_moves == 0)
            return null;

        int move_count = 0;
        int[][] moves = new int[total_moves][];
        for (int subpiece = 0; subpiece < subpieces.Length; subpiece++)
        {
            for (int move = 0; subpieces[subpiece].moves != null && move < subpieces[subpiece].moves.Length; move++)
            {
                if (subpieces[subpiece].moves[move] != null)
                {
                    int[] coord_from = coord;
                    int[] offset = subpieces[subpiece].moves[move];
                    int[] coord_to = (int[])AddCoordinate(coord_from, offset).Clone();

                    if (IsMoveValid(multiverse, subpieces[subpiece], coord_from, coord_to))
                    {
                        moves[move_count] = coord_to;
                        move_count++;
                    }
                }
            }
        }
        if (move_count == 0)
            return null;
        int[][] moves_out = new int[move_count][];
        for (int i = 0; i < move_count; i++)
            moves_out[i] = (int[])moves[i].Clone();
        return moves_out;
    }
    bool IsMoveValid(Multiverse multiverse, nChessPiece subpiece, int[] coord_from, int[] coord_to)
    {
        bool isDrop = IsDrop(coord_from, coord_to, allowDrops);
        if (isDrop)
        {
            Debug.Log("Drop coordinates detected");
            return false;
        }
        else
        {
            if (coord_from == null || coord_to == null)
                return false;
            //Multiverse multiverse = game.GetMultiverse();
            if (multiverse == null)
                return false;
            Board board_from = multiverse.GetBoardFromCoordinate(coord_from);
            Board board_to = multiverse.GetBoardFromCoordinate(coord_to);
            if (board_from == null || board_to == null)
                return false;
            int piece_from = multiverse.GetPieceAt(coord_from);
            int piece_to = multiverse.GetPieceAt(coord_to);
            int color_from = piece_from / Overseer.PIECE_COUNT;
            int color_to = piece_to / Overseer.PIECE_COUNT;
            bool ghostPawn = (piece_to % Overseer.PIECE_COUNT) == 30;
            //if (ghostPawn)
            //    Debug.Log("Occluded by ghost pawn");
            if ( (piece_to > 0 && color_from == color_to) && !ghostPawn)
                return false;
            Command command = new Command(piece_from, piece_to, coord_from, coord_to);
            Move move = new Move();
            move.Add(command);
            bool hasPassed = subpiece.CheckConditions(multiverse, move, time_index);
            return hasPassed;
        }
    }

    bool AreCoordinatesEqual(int[] a, int[] b)
    {
        if (a == null && b == null)
            return true;
        if (a == null || b == null)
            return false;
        for (int i = 0; i < Mathf.Min(a.Length, b.Length); i++)
        {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }
    bool AreCoordinatesEqual(Multiverse multiverse, int[] a, int[] b)
    {
        multiverse.GetRootBoard().ReprocessCoordinates(a, b);
        if (a == null && b == null)
            return true;
        if (a == null || b == null)
            return false;
        for (int i = 0; i < Mathf.Min(a.Length, b.Length); i++)
        {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }
    bool IsCorrectTurn(int current_players_turn, int piece_index)
    {
        //int current_players_turn = game.GetPlayersTurn();
        int piece_color = piece_index / Overseer.PIECE_COUNT;
        return piece_color == current_players_turn;
    }
    bool IsCorrectBoardTurn(Multiverse multiverse, int[] coordinate, int piece_index)
    {
        //Multiverse multiverse = game.GetMultiverse();
        int player = piece_index / Overseer.PIECE_COUNT;
        return multiverse.IsPlayersBoard(coordinate, player);
    }
    bool IsBoardActive(Multiverse multiverse, int[] coordinate)
    {
        //Multiverse multiverse = game.GetMultiverse();
        return multiverse.IsBoardActive(coordinate);
    }


    bool IsSquareAvailable(int piece_from, int piece_to)
    {
        int color_from = piece_from / Overseer.PIECE_COUNT;
        int color_to = piece_to / Overseer.PIECE_COUNT;
        return (piece_to == 0 || color_from != color_to);
    }

    int CountMoves(nChessPiece[] pieces)
    {
        int moves = 0;
        for (int i = 0; i < pieces.Length; i++)
            if (pallete.GetPiece(pieces[i].ID) != null)
                moves += pallete.GetPiece(pieces[i].ID).GetMoveCount();
        return moves;
    }

    #endregion
    // Simple getter methods
    #region Getter Methods
    public nChessPiece GetPiece(int index)
    {
        nChessPiece piece = pallete.GetPiece(index);
        return piece;
    }
    #endregion
    // Methods that do math with arbitrary array sizes
    // TODO: Make this a static helper
    #region Array Math
    int[] AddCoordinate(int[] c1, int[] c2)
    {
        if (c1.Length > c2.Length)
        {
            int[] co = (int[])c1.Clone();
            for (int i = 0; i < Mathf.Min(c1.Length, c2.Length); i++)
            {
                co[i] = co[i] + c2[i];
                if (i == time_index)
                    co[i] += c2[i];
            }
            return (int[])co.Clone();
        }
        else
        {
            int[] co = (int[])c2.Clone();
            for (int i = 0; i < Mathf.Min(c1.Length, c2.Length); i++)
            {
                co[i] = co[i] + c1[i];
                if (i == time_index)
                    co[i] += c2[i];
            }
            return (int[])co.Clone();
        }
    }
    #endregion
    // Initialization and Reset
    #region Startup

    public void InitializeRuleSet(PiecePallete piecePallete)
    {
        Debug.Log("Initializing piece array");


        this.pallete = piecePallete;
        bool isMultiverseTimeTravel = false;
        bool useForwardLateralRule = true;
        bool allowPromotion = true;
        bool queenQuadragonals = true;
        GameDescriptor gameDescriptor = FindObjectOfType<GameDescriptor>();
        if (gameDescriptor != null)
        {
            drops_index = gameDescriptor.dropsIndex; 
            useForwardLateralRule = gameDescriptor.useForwardLateral;
            allowPromotion = gameDescriptor.allowPromotions;
            isMultiverseTimeTravel = gameDescriptor.isTimeTravel;
            allowDrops = gameDescriptor.allowDrops;
        }
        else
        {
            Debug.Log("Warning, failed to find game descriptor");
        }

        pieces = (nChessPiece[])piecePallete.InitializePiecePallete(local_dimensions, isMultiverseTimeTravel, useForwardLateralRule, allowPromotion, queenQuadragonals, gameDescriptor.forwards, gameDescriptor.laterals).Clone();
        //pieces = (nChessPiece[])piecePallete.GetPieceArray().Clone();
    }
    #endregion
}
