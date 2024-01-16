using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePallete : MonoBehaviour
{
    int[] forwards;
    bool useForwardLateralExclusion;
    bool[] pieceExceptions;
    bool isBlackList;



    const int PIECE_COUNT = 32;
    nChessPiece[] chessPieces;

    [SerializeField]
    Mesh[] meshes;

    public struct PalleteOptions
    {
        public bool isMultiverseTimeTravel;
        public bool useForwardLateral;
        public bool allowPromotion;
        public bool queenQuadragonals;
    };

    public int[] GetForwards()
    {
        // TODO: STOP HARDCODING THIS
        return new int[] { 0, 1, 0, 1, 1, 0 };
    }
    public nChessPiece GetPiece(int index)
    {
        return chessPieces[index];
    }
    public int GetPieceCount()
    {
        return chessPieces == null ? 0 : chessPieces.Length;
    }
    public nChessPiece[] InitializePiecePallete(int[] dimensions, bool isMultiverseTimeTravel, bool userForwardLateral, bool allowPromotion, bool queenQuadragonals, int[] forwards = null, int[] laterals = null)
    {
        PalleteOptions options = new PalleteOptions();
        options.isMultiverseTimeTravel = isMultiverseTimeTravel;
        options.useForwardLateral = userForwardLateral;
        options.allowPromotion = allowPromotion;
        options.queenQuadragonals = queenQuadragonals;
        chessPieces = new nChessPiece[PIECE_COUNT];
        DefinePieces(dimensions, options, forwards, laterals);
        return chessPieces;
    }

    public nChessPiece[] GetPieceArray()
    {
        return chessPieces;
    }
    public Mesh GetMeshIndex(int index)
    {
        return meshes[index];
    }

    protected int[] AllAgonals(int dimension_count)
    {
        int[] all = new int[dimension_count];
        for (int i = 0; i < dimension_count; i++)
        {
            all[i] = i + 1;
        }
        return all;
    }

    protected void AssignPiece(nChessPiece piece)
    {
        chessPieces[piece.ID] = piece;
    }
    public virtual void DefinePieces(int[] dimensions, PalleteOptions options, int[] forwards = null, int[] laterals = null)
    {
        this.forwards = new int[] { 0, 1, 0, 1, 1, 0 };
        bool useForwardLateralExclusion = true;
        int coordinate_length = dimensions.Length;
        int[][] forward_lateral_rule = new int[2][];
        forward_lateral_rule[0] = new int[coordinate_length];
        forward_lateral_rule[1] = new int[coordinate_length];

        for (int i = 0; i < Mathf.Min(coordinate_length, 4); i++)
        {
            forward_lateral_rule[0][i] = i & 1;
            forward_lateral_rule[1][i] = (i + 1) & 1;
        }

        int[] all = AllAgonals(dimensions.Length);

        nChessPiece King = new nChessPiece("King", 1, dimensions.Length);
        King.SetAgonals(all);
        King.SetRange(1);
        King.isRoyalty = true;
        if (useForwardLateralExclusion)
            King.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Queen = new nChessPiece("Queen", 2, dimensions.Length);
        Queen.SetAgonals(all);
        Queen.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            Queen.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Bishop = new nChessPiece("Bishop", 3, dimensions.Length);
        Bishop.SetAgonals(new int[1] { 2 });
        Bishop.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            Bishop.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Knight = new nChessPiece("Knight", 4, dimensions.Length);
        Knight.AddPermutable(new int[2] { 1, 2 });
        if (useForwardLateralExclusion)
            Knight.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Rook = new nChessPiece("Rook", 5, dimensions.Length);
        Rook.SetAgonals(new int[1] { 1 });
        Rook.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            Rook.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Pawn = new nChessPiece("Pawn", 6, dimensions.Length);
        Pawn.SetAgonals(new int[1] { 1 });
        Pawn.SetRange(1);
        Pawn.AddColorModifier(new int[] { 0, 1, 0, 1, 1, 0 });
        Pawn.AddColorExclusion(new int[] { 0, -1, 0, -1, -1, 0 });
        Pawn.AddDirectExclusion(new int[] { 1, 0, 1, 0, 0, 1, 0, 0 });
        Pawn.SetConditions(new int[1] { nChessPiece.CONDITION_NONCAPTURE });
        if (useForwardLateralExclusion)
            Pawn.SetMutualExclusions(forward_lateral_rule);
        //Pawn.AddPromotion(new int[] { -1, 0, -1, 0, -1, -1, -1, -1 }, 2)

        nChessPiece PawnStarting = new nChessPiece("Pawn Starting", 26, dimensions.Length);
        PawnStarting.SetAgonals(new int[1] { 1 });
        PawnStarting.SetRange(2);
        PawnStarting.AddColorModifier(new int[] { 0, 1, 0, 1, 1, 0 });
        PawnStarting.AddColorExclusion(new int[] { 0, -1, 0, -1, -1, 0 });
        PawnStarting.AddDirectExclusion(new int[] { 1, 0, 1, 0, 0, 1, 0, 0 });
        PawnStarting.SetConditions(new int[3] { nChessPiece.CONDITION_STARTING, nChessPiece.CONDITION_NONCAPTURE, nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            PawnStarting.SetMutualExclusions(forward_lateral_rule);

        nChessPiece PawnAttacking = new nChessPiece("Pawn Attacking", 27, dimensions.Length);
        PawnAttacking.SetAgonals(new int[1] { 2 });
        PawnAttacking.SetRange(1);
        PawnAttacking.AddColorModifier(new int[] { 0, 1, 0, 1, 1, 0 });
        PawnAttacking.AddColorExclusion(new int[] { 0, -1, 0, -1, -1, 0 });
        PawnAttacking.AddDirectExclusion(new int[] { 0, 0, 0, 0, 0, 1, 0, 0 });
        PawnAttacking.SetMutualExclusions(forward_lateral_rule);
        PawnAttacking.SetConditions(new int[1] { nChessPiece.CONDITION_ATTACKING });
        if (useForwardLateralExclusion)
            PawnAttacking.SetMutualExclusions(forward_lateral_rule);
        Pawn.subpieces = new nChessPiece[2] { PawnStarting, PawnAttacking };

        nChessPiece Unicorn = new nChessPiece("Unicorn", 7, dimensions.Length);
        Unicorn.SetAgonals(new int[1] { 3 });
        Unicorn.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        //Unicorn.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Dragon = new nChessPiece("Dragon", 8, dimensions.Length);
        Dragon.SetAgonals(new int[1] { 4 });
        Dragon.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        Dragon.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        //Dragon.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Princess = new nChessPiece("Princess", 9, dimensions.Length);
        Princess.subpieces = new nChessPiece[2] { Rook, Bishop };
        Princess.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });

        nChessPiece QueenCapture = new nChessPiece("Queen Capture", 10, dimensions.Length);
        QueenCapture.SetAgonals(all);
        QueenCapture.SetConditions(new int[2] { nChessPiece.CONDITION_NONOCCLUDED, nChessPiece.CONDITION_ATTACKING });
        if (useForwardLateralExclusion)
            QueenCapture.SetMutualExclusions(forward_lateral_rule);

        nChessPiece KnightCapture = new nChessPiece("Knight Capture", 11, dimensions.Length);
        KnightCapture.AddPermutable(new int[2] { 1, 2 });
        KnightCapture.SetConditions(new int[2] { nChessPiece.CONDITION_NONOCCLUDED, nChessPiece.CONDITION_ATTACKING });
        if (useForwardLateralExclusion)
            KnightCapture.SetMutualExclusions(forward_lateral_rule);

        nChessPiece CheckDetector = new nChessPiece("Check Detector", 12, dimensions.Length);
        CheckDetector.subpieces = new nChessPiece[2] { QueenCapture, KnightCapture };

        nChessPiece rPawn = new nChessPiece("Reverse Pawn", 13, dimensions.Length);
        rPawn.SetAgonals(new int[1] { 1 });
        rPawn.SetRange(1);
        rPawn.AddColorModifier(new int[] { 0, 1, 0, 1, 1, 0 });
        rPawn.AddColorExclusion(new int[] { 0, 1, 0, 1, 1, 0 });
        rPawn.AddDirectExclusion(new int[] { 1, 0, 1, 0, 0, 1, 0, 0 });
        rPawn.SetConditions(new int[1] { nChessPiece.CONDITION_NONCAPTURE });
        if (useForwardLateralExclusion)
            rPawn.SetMutualExclusions(forward_lateral_rule);
        //rPawn.AddPromotion(new int[] { -1, 0, -1, 0, -1, -1, -1, -1 }, 2);

        nChessPiece rPawnStarting = new nChessPiece("Reverse Pawn Starting", 28, dimensions.Length);
        rPawnStarting.SetAgonals(new int[1] { 1 });
        rPawnStarting.SetRange(2);
        rPawnStarting.AddColorModifier(new int[] { 0, 1, 0, 1, 1, 0 });
        rPawnStarting.AddColorExclusion(new int[] { 0, 1, 0, 1, 1, 0 });
        rPawnStarting.AddDirectExclusion(new int[] { 1, 0, 1, 0, 0, 1, 0, 0 });
        rPawnStarting.SetConditions(new int[3] { nChessPiece.CONDITION_STARTING, nChessPiece.CONDITION_NONCAPTURE, nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            rPawnStarting.SetMutualExclusions(forward_lateral_rule);

        nChessPiece rPawnAttacking = new nChessPiece("Reverse Pawn Attacking", 29, dimensions.Length);
        rPawnAttacking.SetAgonals(new int[1] { 2 });
        rPawnAttacking.SetRange(1);
        rPawnAttacking.AddColorModifier(new int[] { 0, 1, 0, 1, 1, 0 });
        rPawnAttacking.AddColorExclusion(new int[] { 0, 1, 0, 1, 1, 0 });
        rPawnAttacking.AddDirectExclusion(new int[] { 0, 0, 0, 0, 0, 1, 0, 0 });
        rPawnAttacking.SetMutualExclusions(forward_lateral_rule);
        rPawnAttacking.SetConditions(new int[1] { nChessPiece.CONDITION_ATTACKING });
        if (useForwardLateralExclusion)
            rPawnAttacking.SetMutualExclusions(forward_lateral_rule);

        rPawn.subpieces = new nChessPiece[2] { rPawnStarting, rPawnAttacking };


        //nChessPiece


        nChessPiece Cross = new nChessPiece("Cross", 24, dimensions.Length);
        Cross.SetAgonals(new int[1] { 1 });
        Cross.SetRange(1);
        if (useForwardLateralExclusion)
            Cross.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Ex = new nChessPiece("Ex", 25, dimensions.Length);
        Ex.SetAgonals(new int[1] { 2 });
        Ex.SetRange(1);
        if (useForwardLateralExclusion)
            Ex.SetMutualExclusions(forward_lateral_rule);

        nChessPiece sKing = new nChessPiece("sKing", 16, dimensions.Length);
        sKing.SetAgonals(all);
        sKing.SetRange(1);
        if (useForwardLateralExclusion)
            sKing.SetMutualExclusions(forward_lateral_rule);

        nChessPiece sRook = new nChessPiece("sRook", 17, dimensions.Length);
        sRook.SetAgonals(new int[1] { 1 });
        sRook.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });

        nChessPiece sBishop = new nChessPiece("sBishop", 18, dimensions.Length);
        sBishop.SetAgonals(new int[1] { 2 });
        sBishop.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            sBishop.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Gold = new nChessPiece("Gold", 19, dimensions.Length);
        Gold.SetAgonals(new int[1] { 2 });
        Gold.SetRange(1);
        Gold.AddColorModifier(new int[4] { 0, 1, 0, 1 });
        Gold.AddColorExclusion(new int[4] { 0, -1, 0, -1 });
        Gold.subpieces = new nChessPiece[1] { Cross };
        if (useForwardLateralExclusion)
            Gold.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Silver = new nChessPiece("Silver", 20, dimensions.Length);
        Silver.SetAgonals(new int[1] { 1 });
        Silver.SetRange(1);
        Silver.AddDirectExclusion(new int[4] { 1, 0, 1, 0 });
        Silver.AddColorModifier(new int[4] { 0, 1, 0, 1 });
        Silver.AddColorExclusion(new int[4] { 0, -1, 0, -1 });
        Silver.subpieces = new nChessPiece[1] { Ex };
        if (useForwardLateralExclusion)
            Silver.SetMutualExclusions(forward_lateral_rule);

        nChessPiece sKnight = new nChessPiece("sKnight", 21, dimensions.Length);

        nChessPiece Lance = new nChessPiece("sLance", 22, dimensions.Length);
        Lance.SetAgonals(new int[1] { 1 });
        Lance.AddColorModifier(new int[4] { 0, 1, 0, 1 });
        Lance.AddColorExclusion(new int[4] { 0, -1, 0, -1 });
        Lance.AddDirectExclusion(new int[4] { 1, 0, 1, 0 });
        Lance.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });

        nChessPiece sPawn = new nChessPiece("sPawn", 23, dimensions.Length);
        sPawn.SetAgonals(new int[1] { 1 });
        sPawn.SetRange(1);
        sPawn.AddColorModifier(new int[4] { 0, 1, 0, 1 });
        sPawn.AddColorExclusion(new int[4] { 0, -1, 0, -1 });
        sPawn.AddDirectExclusion(new int[4] { 1, 0, 1, 0 });

        AssignPiece(King);
        AssignPiece(Queen);
        AssignPiece(Bishop);
        AssignPiece(Knight);
        AssignPiece(Rook);
        AssignPiece(Pawn);
        AssignPiece(rPawn);

        AssignPiece(Unicorn);
        AssignPiece(Dragon);
        AssignPiece(Princess);

        AssignPiece(QueenCapture);
        AssignPiece(KnightCapture);


        AssignPiece(CheckDetector);

        AssignPiece(Lance);
        AssignPiece(Cross);
        AssignPiece(Ex);

        AssignPiece(sKing);
        AssignPiece(sRook);
        AssignPiece(sBishop);
        AssignPiece(Gold);
        AssignPiece(Silver);
        //GeneratePieceMoves(sKnight, dimensions);

        AssignPiece(Lance);
        AssignPiece(sPawn);

        AssignPiece(PawnStarting);
        AssignPiece(PawnAttacking);
        AssignPiece(rPawnStarting);
        AssignPiece(rPawnAttacking);
        GenerateMoves(dimensions);
    }

    protected void GenerateMoves(int[] dimensions)
    {
        PieceDefiner pieceDefiner = new PieceDefiner();
        pieceDefiner.GeneratePieceMoves(chessPieces, dimensions);
    }



}
