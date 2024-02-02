using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Western : PiecePallete
{
    
    public override void DefinePieces(int[] dimensions, PalleteOptions options, int[] forwards=null, int[] laterals=null)
    {
        bool useForwardLateralExclusion = options.useForwardLateral;
        bool isMultiverseTimeTravel = options.isMultiverseTimeTravel;
        bool allowPromotions = options.allowPromotion;

        int coordinate_length = dimensions.Length;
        int[][] forward_lateral_rule = new int[2][];
        forward_lateral_rule[0] = new int[coordinate_length];
        forward_lateral_rule[1] = new int[coordinate_length];

        for (int i = 0; i < coordinate_length - (options.isMultiverseTimeTravel ? 2 : 0); i++)
        {
            forward_lateral_rule[0][i] = i & 1;
            forward_lateral_rule[1][i] = (i + 1) & 1;
        }

        int[] color_modifier = new int[coordinate_length];
        int[] color_exclusion = new int[coordinate_length];
        int[] direct_exclusion = new int[coordinate_length];
        int[] promotion_distance = new int[coordinate_length];
        for (int i = 0; i < coordinate_length; i++)
        {
            color_modifier[i] = i & 1;
            color_exclusion[i] = -(i & 1);
            direct_exclusion[i] = (i + 1) & 1;
            promotion_distance[i] = -((i + 1) & 1);
        }
        if (forwards != null)
        {
            for (int i = 0; i < forwards.Length; i++)
            {
                forward_lateral_rule[0][i] = forwards[i];
                color_modifier[i] = forwards[i];
                color_exclusion[i] = -forwards[i];
                promotion_distance[i] = -(1 - forwards[i]);
            }
        }
        if (laterals != null)
        {
            for (int i = 0; i < laterals.Length; i++)
            {
                forward_lateral_rule[1][i] = laterals[i];
                direct_exclusion[i] = laterals[i];
            }
        }

        if (isMultiverseTimeTravel)
        {
            color_modifier[coordinate_length - 2] = 1;
            color_modifier[coordinate_length - 1] = 0;

            color_exclusion[coordinate_length - 2] = -1;
            color_exclusion[coordinate_length - 1] = 0;

            direct_exclusion[coordinate_length - 2] = 0;
            direct_exclusion[coordinate_length - 1] = 1;

            promotion_distance[coordinate_length - 1] = 0;
            promotion_distance[coordinate_length - 1] = -1;
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
        Pawn.AddColorModifier(color_modifier);
        Pawn.AddColorExclusion(color_exclusion);
        Pawn.AddDirectExclusion(direct_exclusion);
        Pawn.SetConditions(new int[1] { nChessPiece.CONDITION_NONCAPTURE });
        if (allowPromotions)
            Pawn.AddPromotion(promotion_distance, 2);

        nChessPiece PawnNonStart = new nChessPiece("Pawn NonStart", 15, dimensions.Length);
        PawnNonStart.SetAgonals(new int[1] { 1 });
        PawnNonStart.SetRange(1);
        PawnNonStart.AddColorModifier(color_modifier);
        PawnNonStart.AddColorExclusion(color_exclusion);
        PawnNonStart.AddDirectExclusion(direct_exclusion);
        PawnNonStart.SetConditions(new int[1] { nChessPiece.CONDITION_NONCAPTURE });
        if (allowPromotions)
            PawnNonStart.AddPromotion(promotion_distance, 2);

        nChessPiece PawnStarting = new nChessPiece("Pawn Starting", 26, dimensions.Length);
        PawnStarting.SetAgonals(new int[1] { 1 });
        PawnStarting.SetRange(2);
        PawnStarting.AddColorModifier(color_modifier);
        PawnStarting.AddColorExclusion(color_exclusion);
        PawnStarting.AddDirectExclusion(direct_exclusion);
        PawnStarting.SetConditions(new int[3] { nChessPiece.CONDITION_STARTING, nChessPiece.CONDITION_NONCAPTURE, nChessPiece.CONDITION_NONOCCLUDED });
        PawnStarting.SetMutualExclusions(forward_lateral_rule);

        nChessPiece PawnAttacking = new nChessPiece("Pawn Attacking", 27, dimensions.Length);
        PawnAttacking.SetAgonals(new int[1] { 2 });
        PawnAttacking.SetRange(1);
        PawnAttacking.AddColorModifier(color_modifier);
        PawnAttacking.AddColorExclusion(color_exclusion);
        //PawnAttacking.AddDirectExclusion(direct_exclusion);
        PawnAttacking.SetConditions(new int[1] { nChessPiece.CONDITION_ATTACKING });
        PawnAttacking.SetMutualExclusions(forward_lateral_rule);

        Pawn.subpieces = new nChessPiece[2] { PawnStarting, PawnAttacking };
        PawnNonStart.subpieces = new nChessPiece[1] { PawnAttacking };

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
        //QueenCapture.SetMutualExclusions(forward_lateral_rule);

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
        rPawn.AddColorModifier(color_modifier);
        rPawn.AddColorExclusion(color_modifier);
        rPawn.AddDirectExclusion(direct_exclusion);
        rPawn.SetConditions(new int[1] { nChessPiece.CONDITION_NONCAPTURE });
        if (useForwardLateralExclusion)
            rPawn.SetMutualExclusions(forward_lateral_rule);
        if (allowPromotions)
            rPawn.AddPromotion(promotion_distance, 2);

        nChessPiece Devil = new nChessPiece("Devil", 14, dimensions.Length);
        Devil.SetAgonals(new int[1] { 5 });
        Devil.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        Devil.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        //Dragon.SetMutualExclusions(forward_lateral_rule);


        nChessPiece rPawnStarting = new nChessPiece("Reverse Pawn Starting", 28, dimensions.Length);
        rPawnStarting.SetAgonals(new int[1] { 1 });
        rPawnStarting.SetRange(2);
        rPawnStarting.AddColorModifier(color_modifier);
        rPawnStarting.AddColorExclusion(color_modifier);
        rPawnStarting.AddDirectExclusion(direct_exclusion);
        rPawnStarting.SetConditions(new int[3] { nChessPiece.CONDITION_STARTING, nChessPiece.CONDITION_NONCAPTURE, nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            rPawnStarting.SetMutualExclusions(forward_lateral_rule);

        nChessPiece rPawnAttacking = new nChessPiece("Reverse Pawn Attacking", 29, dimensions.Length);
        rPawnAttacking.SetAgonals(new int[1] { 2 });
        rPawnAttacking.SetRange(1);
        rPawnAttacking.AddColorModifier(color_modifier);
        rPawnAttacking.AddColorExclusion(color_modifier);
        //rPawnAttacking.AddDirectExclusion(direct_exclusion);
        rPawnAttacking.SetMutualExclusions(forward_lateral_rule);
        rPawnAttacking.SetConditions(new int[1] { nChessPiece.CONDITION_ATTACKING });
        if (useForwardLateralExclusion)
            rPawnAttacking.SetMutualExclusions(forward_lateral_rule);

        rPawn.subpieces = new nChessPiece[2] { rPawnStarting, rPawnAttacking };


        nChessPiece GhostPawn = new nChessPiece("Ghost Pawn", 30, dimensions.Length);



        nChessPiece sKing = new nChessPiece("Shogi King", 32, dimensions.Length);
        sKing.SetAgonals(all);
        sKing.SetRange(1);
        sKing.isRoyalty = true;
        if (useForwardLateralExclusion)
            sKing.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Cross = new nChessPiece("Cross", 33, dimensions.Length);
        Cross.SetAgonals(new int[1] { 1 });
        Cross.SetRange(1);
        if (useForwardLateralExclusion)
            Cross.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Ex = new nChessPiece("Ex", 34, dimensions.Length);
        Ex.SetAgonals(new int[1] { 2 });
        Ex.SetRange(1);
        if (useForwardLateralExclusion)
            Ex.SetMutualExclusions(forward_lateral_rule);

        nChessPiece sRook = new nChessPiece("sRook", 35, dimensions.Length);
        sRook.SetAgonals(new int[1] { 1 });
        sRook.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });

        nChessPiece sBishop = new nChessPiece("sBishop", 36, dimensions.Length);
        sBishop.SetAgonals(new int[1] { 2 });
        sBishop.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });
        if (useForwardLateralExclusion)
            sBishop.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Gold = new nChessPiece("Gold", 37, dimensions.Length);
        Gold.SetAgonals(new int[1] { 2 });
        Gold.SetRange(1);
        Gold.AddColorModifier(new int[4] { 0, 1, 0, 1 });
        Gold.AddColorExclusion(new int[4] { 0, -1, 0, -1 });
        Gold.subpieces = new nChessPiece[1] { Cross };
        if (useForwardLateralExclusion)
            Gold.SetMutualExclusions(forward_lateral_rule);

        nChessPiece Silver = new nChessPiece("Silver", 38, dimensions.Length);
        Silver.SetAgonals(new int[1] { 1 });
        Silver.SetRange(1);
        Silver.AddDirectExclusion(new int[4] { 1, 0, 1, 0 });
        Silver.AddColorModifier(new int[4] { 0, 1, 0, 1 });
        Silver.AddColorExclusion(new int[4] { 0, -1, 0, -1 });
        Silver.subpieces = new nChessPiece[1] { Ex };
        if (useForwardLateralExclusion)
            Silver.SetMutualExclusions(forward_lateral_rule);

        nChessPiece sKnight = new nChessPiece("sKnight", 39, dimensions.Length);

        nChessPiece Lance = new nChessPiece("sLance", 40, dimensions.Length);
        Lance.SetAgonals(new int[1] { 1 });
        Lance.AddColorModifier(new int[4] { 0, 1, 0, 1 });
        Lance.AddColorExclusion(new int[4] { 0, -1, 0, -1 });
        Lance.AddDirectExclusion(new int[4] { 1, 0, 1, 0 });
        Lance.SetConditions(new int[1] { nChessPiece.CONDITION_NONOCCLUDED });

        nChessPiece sPawn = new nChessPiece("sPawn", 41, dimensions.Length);
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

        AssignPiece(Devil);

        AssignPiece(PawnStarting);
        AssignPiece(PawnAttacking);
        AssignPiece(rPawnStarting);
        AssignPiece(rPawnAttacking);

        AssignPiece(PawnNonStart);
        AssignPiece(GhostPawn);

        AssignPiece(sKing);
        AssignPiece(Cross);
        AssignPiece(Ex);
        AssignPiece(sRook);
        AssignPiece(sBishop);
        AssignPiece(Gold);
        AssignPiece(Silver);
        AssignPiece(sKnight);
        AssignPiece(Lance);
        AssignPiece(sPawn);


        base.GenerateMoves(dimensions);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
