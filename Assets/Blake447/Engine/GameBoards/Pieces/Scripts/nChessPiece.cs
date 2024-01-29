using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nChessPiece
{
    public int ID;
    public string name;

    public int dimensions;
    public int[] nagonals;
    public int range = -1;
    public int[][] permutables;
    public int[][] explicit_moves;

    public int[][] mutualExclusions;
    public int[][] directExclusions;
    public int[][] colorModifiers;
    public int[][] colorExclusions;

    public int[]   neighborsRequired;
    public int[][] neighborsPosition;
    public int[] neighborBuffer;

    public int[]   promotionPiece;
    public int[][] promotionDistance;


    public bool isAttacking;
    public bool isRoyalty;
    public bool flagEnPassant;
    public bool checkEnPassant;

    const int RANGE_UNLIMITED = -1;
    const int NOT_COLOR_DEPENDENT = -1;

    public nChessPiece[] subpieces;

    public int[] conditions;

    public const int CONDITION_STARTING = 1;
    public const int CONDITION_ATTACKING = 2;
    public const int CONDITION_NONCAPTURE = 3;
    public const int CONDITION_NONOCCLUDED = 4;

    public int[][] moves;

    public int CountMoves(int color)
    {
        int counter = 0;
        for (int i = 0; i < moves.Length; i++)
        {
            if (IsMoveValid(moves[i], color))
                counter++;
        }
        if (subpieces != null)
        {
            for (int i = 0; i < subpieces.Length; i++)
            {
                counter += subpieces[i].CountMoves(color);
            }
        }

        return counter;
    }
    //public int[][] GenerateMoveOffsets(int[] coordinate, int piece_id)
    //{
    //    int color = piece_id / 32;
    //    int counter = CountMoves(color);
    //    if (counter == 0)
    //        return null;
    //    int[][] move_coordinates = new int[counter][];
    //    counter = 0;
    //    for (int i = 0; i < moves.Length; i++)
    //    {
    //        if (IsMoveValid(moves[i], color))
    //        {
    //            move_coordinates[counter] = AddCoordinate(coordinate, moves[i]);
    //            counter++;
    //        }
    //    }
    //    if (subpieces != null)
    //    {
    //        for (int subpiece = 0; subpiece < subpieces.Length; subpiece++)
    //        {
    //            for (int i = 0; i < subpieces[subpiece].moves.Length; i++)
    //            {
    //                if (IsMoveValid(subpieces[subpiece].moves[i], color))
    //                {
    //                    move_coordinates[counter] = AddCoordinate(coordinate, subpieces[subpiece].moves[i]);
    //                    counter++;
    //                }
    //            }
    //        }
    //    }
    //    return (int[][])move_coordinates.Clone();
    //}
    bool IsMoveValid(int[] offset, int color)
    {
        if (offset == null)
        {
            return false;
        }
        if (colorExclusions != null && colorModifiers != null)
        {
            for (int exclusion = 0; exclusion < colorExclusions.Length; exclusion++)
            {
                for (int modifier = 0; modifier < colorModifiers.Length; modifier++)
                {
                    for (int i = 0; i < Mathf.Min(offset.Length, colorExclusions[exclusion].Length, colorModifiers[modifier].Length); i++)
                    {

                        int color_flip = colorModifiers[modifier][i] == color ? -1 : 1;
                        if (colorExclusions[exclusion][i] * offset[i] * color_flip > 0)
                            return false;
                    }
                }
            }
        }
        return true;
    }
    public bool CheckConditions(Multiverse multiverse, Move move, int time_index)
    {
        //Multiverse multiverse = game.GetMultiverse();
        // TODO: Refactor later
        
        Command command = move.tail;
        int[] coord_from = (int[])command.from.Clone();
        int[] coord_to = (int[])command.to.Clone();

        bool isStarting = multiverse.IsStarting(coord_from);
        int piece_from = command.pfrom;
        int piece_to = command.pto;

        int color_from = piece_from / 32;
        int color_to = piece_to / 32;

        int offset_length = Mathf.Min(coord_from.Length, coord_to.Length);
        int[] offset = new int[offset_length];
        for (int i = 0; i < offset.Length; i++)
            offset[i] = coord_to[i] - coord_from[i];

        bool hasFailed = false;
        hasFailed = hasFailed || !PassColorExclusions(offset, color_from);
        hasFailed = hasFailed || !PassDirectExclustion(offset);
        hasFailed = hasFailed || !PassGeneralExclusions(piece_from, piece_to, color_from, color_to, isStarting);
        hasFailed = hasFailed || !PassOcclusion(multiverse, color_from, coord_from, coord_to, time_index);
        return !hasFailed;
    }
    public nChessPiece[] EnumerateSubPieces()
    {
        nChessPiece[] enumerated = new nChessPiece[1] { this };
        if (subpieces != null)
        {
            for (int i = 0; i < subpieces.Length; i++)
            {
                nChessPiece[] recursive_subpiece = subpieces[i].EnumerateSubPieces();
                nChessPiece[] enumerated_new = new nChessPiece[enumerated.Length + recursive_subpiece.Length];
                System.Array.Copy(enumerated, 0, enumerated_new, 0, enumerated.Length);
                System.Array.Copy(recursive_subpiece, 0, enumerated_new, enumerated.Length, recursive_subpiece.Length);
                enumerated = (nChessPiece[])enumerated_new.Clone();
            }
        }
        return enumerated;
    }
    public int[] ConstructExclusion(int component, int sign, bool isColorDependent, int color)
    {
        int[] exclusion = new int[3];
        exclusion[0] = component;
        exclusion[1] = sign;
        exclusion[2] = isColorDependent ? color : NOT_COLOR_DEPENDENT;
        return exclusion;
    }

    public int PassPromotion(int[] board_size, int[] coordinate, int color)
    {
        if (promotionDistance != null)
        {
            if (color == 0)
            {
                int[] minDistance = new int[coordinate.Length];
                for (int i = 0; i < minDistance.Length; i++)
                    minDistance[i] = -1;
                for (int i = 0; i < Mathf.Min(promotionDistance[0].Length, coordinate.Length, board_size.Length); i++)
                {
                    minDistance[i] = promotionDistance[0][i] == -1 ? -1 : board_size[i] - 1 - promotionDistance[0][i];
                }
                for (int i = 0; i < minDistance.Length; i++)
                {
                    if (coordinate[i] < minDistance[i])
                        return 0;
                }
                return promotionPiece[0];
            }
            else
            {
                int[] maxDistance = new int[coordinate.Length];
                for (int i = 0; i < maxDistance.Length; i++)
                    maxDistance[i] = int.MaxValue;
                for (int i = 0; i < Mathf.Min(promotionDistance[0].Length, coordinate.Length, board_size.Length); i++)
                {
                    maxDistance[i] = promotionDistance[0][i] == -1 ? board_size[i] : promotionDistance[0][i];
                }
                for (int i = 0; i < maxDistance.Length; i++)
                {
                    if (coordinate[i] > maxDistance[i])
                        return 0;
                }
                return promotionPiece[0];
            }
        }
        return 0;
    }
    bool PassOcclusion(Multiverse multiverse, int color_from, int[] coord_from, int[] coord_to, int time_index)
    {
        bool occludable = false;
        if (conditions == null)
            return true;
        for (int i = 0; i < conditions.Length; i++)
                occludable = occludable || (conditions[i] == CONDITION_NONOCCLUDED);
        if (!occludable)
            return true;

        int[] strip_march = (int[])multiverse.GetStrip(coord_from, coord_to, time_index).Clone();
        bool isClear = true;
        if (strip_march != null)
        {
            int ghostPawn = 30;
            isClear = true;
            for (int i = 0; i < strip_march.Length - 1; i++)
                isClear = isClear && ((strip_march[i] == 0) || ((strip_march[i] % 32) == 30)) && (strip_march[i] != -1);
            int last_piece = strip_march[strip_march.Length - 1];
            int last_color = last_piece / 32;
            if ( (last_piece) != 0 && (last_color == color_from) )
                isClear = false;
        }
        return isClear;
    }
    bool PassGeneralExclusions(int piece_from, int piece_to, int color_from, int color_to, bool isStarting)
    {
        bool hasFailed = false;
        if (conditions != null)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                switch (conditions[i])
                {
                    case CONDITION_STARTING:
                        hasFailed = hasFailed || !isStarting;
                        break;
                    case CONDITION_ATTACKING:
                        hasFailed = hasFailed || (piece_to == 0);
                        hasFailed = hasFailed || !(color_from != color_to);
                        break;
                    case CONDITION_NONCAPTURE:
                        hasFailed = hasFailed || !(piece_to == 0);
                        break;
                    default:
                        break;
                }
            }
        }
        return !hasFailed;
    }
    bool PassDirectExclustion(int[] offset)
    {
        if (directExclusions != null)
        {
            foreach(int[] exclusion in directExclusions)
            {
                if (exclusion != null)
                {
                    int checkLength = Mathf.Min(offset.Length, exclusion.Length);
                    for (int i = 0; i < checkLength; i++)
                    {
                        if (Mathf.Abs(offset[i]) * exclusion[i] > 0)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
    bool PassColorExclusions(int[] offset, int piece_color)
    {

        if (colorExclusions != null && colorModifiers != null)
        {
            foreach (int[] exlusion in colorExclusions)
            {
                if (exlusion != null)
                {
                    foreach (int[] modifier in colorModifiers)
                    {
                        if (modifier != null)
                        {
                            int checkLength = Mathf.Min(offset.Length, exlusion.Length, modifier.Length);
                            for (int i = 0; i < checkLength; i++)
                            {
                                int sign = 1;
                                if (piece_color != 0)
                                    sign = modifier[i] == 0 ? 1 : -1;
                                if (offset[i] * sign * exlusion[i] > 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }
        return true;
    }
    
    public int GetRangeUnlimitedCode()
    {
        return RANGE_UNLIMITED;
    }
    public int GetMoveCount()
    {
        nChessPiece[] subpieces = EnumerateSubPieces();
        int sum = 0;
        for (int i = 0; i < subpieces.Length; i++)
        {
            sum += subpieces[i].moves.Length;
        }
        return sum;
    }

    int[] AddCoordinate(int[] c1, int[] c2)
    {
        if (c1.Length > c2.Length)
        {
            int[] co = (int[])c1.Clone();
            for (int i = 0; i < Mathf.Min(c1.Length, c2.Length); i++)
            {
                co[i] = co[i] + c2[i];
            }
            return (int[])co.Clone();
        }
        else
        {
            int[] co = (int[])c2.Clone();
            for (int i = 0; i < Mathf.Min(c1.Length, c2.Length); i++)
            {
                co[i] = co[i] + c1[i];
            }
            return (int[])co.Clone();
        }
    }
    public void AddPermutable(int[] permutable)
    {
        int[][] buffer = null;
        if (permutables != null)
        {
            buffer = new int[permutables.Length + 1][];
            for (int i = 0; i < permutables.Length; i++)
            {
                buffer[i] = (int[])permutables[i].Clone();
                buffer[permutables.Length] = (int[])permutable.Clone();
            }
        }
        else
        {
            buffer = new int[1][];
            buffer[0] = (int[])permutable.Clone();
        }
        permutables = buffer;
    }
    public void AddColorExclusion(int[] exclusion)
    {
        int[][] buffer = null;
        if (colorExclusions != null)
        {
            buffer = new int[colorExclusions.Length + 1][];
            for (int i = 0; i < colorExclusions.Length; i++)
            {
                buffer[i] = (int[])colorExclusions[i].Clone();
                buffer[colorExclusions.Length] = (int[])exclusion.Clone();
            }
        }
        else
        {
            buffer = new int[1][];
            buffer[0] = (int[])exclusion.Clone();
        }
        colorExclusions = (int[][])buffer.Clone();
    }
    public void AddColorModifier(int[] modifier)
    {
        int[][] buffer = null;
        if (colorModifiers != null)
        {
            buffer = new int[colorModifiers.Length + 1][];
            for (int i = 0; i < colorModifiers.Length; i++)
            {
                buffer[i] = (int[])colorModifiers[i].Clone();
                buffer[colorModifiers.Length] = (int[])modifier.Clone();
            }
        }
        else
        {
            buffer = new int[1][];
            buffer[0] = (int[])modifier.Clone();
        }
        colorModifiers = (int[][])buffer.Clone();
    }
    public void AddDirectExclusion(int[] exclusion)
    {
        int[][] buffer = null;
        if (directExclusions != null)
        {
            buffer = new int[directExclusions.Length + 1][];
            for (int i = 0; i < directExclusions.Length; i++)
            {
                buffer[i] = (int[])directExclusions[i].Clone();
                buffer[directExclusions.Length] = (int[])exclusion.Clone();
            }
        }
        else
        {
            buffer = new int[1][];
            buffer[0] = (int[])exclusion.Clone();
        }
        directExclusions = buffer;
    }
    public void AddPromotion(int[] promotionDistance, int promotionPiece)
    {
        int[][] buffer = null;
        if (this.promotionDistance != null)
        {
            buffer = new int[this.promotionDistance.Length + 1][];
            for (int i = 0; i < this.promotionDistance.Length; i++)
            {
                buffer[i] = (int[])this.promotionDistance[i].Clone();
                buffer[this.promotionDistance.Length] = (int[])promotionDistance.Clone();
            }
        }
        else
        {
            buffer = new int[1][];
            buffer[0] = (int[])promotionDistance.Clone();
        }
        this.promotionDistance = buffer;

        if (this.promotionPiece != null)
        {
            int[] pieceBuffer = new int[this.promotionPiece.Length + 1];
            System.Array.Copy(this.promotionPiece, 0, pieceBuffer, 0, this.promotionPiece.Length);
            pieceBuffer[this.promotionPiece.Length] = promotionPiece;
            this.promotionPiece = (int[])pieceBuffer.Clone();
        }
        else
        {
            int[] pieceBuffer = new int[1];
            pieceBuffer[0] = promotionPiece;
            this.promotionPiece = (int[])pieceBuffer.Clone();
        }
    }


    public void SetEnPassant(bool set_flag, bool check_flag)
    {
        checkEnPassant = check_flag;
        flagEnPassant = set_flag;
    }
    public void SetConditions(int[] conditions)
    {
        this.conditions = (int[])conditions.Clone();
    }
    public void SetMoves(int[][] moves)
    {
        this.moves = new int[moves.Length][];
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] != null)
            {
                //Debug.Log(Coordinates.CoordinateToString(moves[i]));
                this.moves[i] = (int[])moves[i].Clone();
            }
        }
    }
    public void SetPromotions(int[][] promotionDistances, int[] promotionPieces)
    {
        this.promotionDistance = (int[][])promotionDistances.Clone();
        this.promotionPiece = (int[])promotionPieces.Clone();
        
    }
    public void SetNeighbors(int[][] neighborsPositions, int[] neighborsRequired)
    {
        this.neighborsPosition = (int[][])neighborsPositions.Clone();
        this.neighborsRequired = (int[])neighborsRequired.Clone();
    }
    public void SetDirectExclusions(int[][] directExclusions, bool isWhitelist)
    {
        this.directExclusions = new int[directExclusions.Length][];
        for (int i = 0; i < directExclusions.Length; i++)
        {
            this.directExclusions[i] = (int[])directExclusions[i].Clone();
        }
    }
    public void SetPermutables(int[][] permutables)
    {
        this.permutables = new int[permutables.Length][];
        for (int i = 0; i < this.permutables.Length; i++)
        {
            this.permutables[i] = (int[])permutables[i].Clone();
        }
    }
    public void SetMutualExclusions(int[][] mutualExclusions)
    {
        this.mutualExclusions = new int[mutualExclusions.Length][];
        for (int i = 0; i < this.mutualExclusions.Length; i++)
        {
            this.mutualExclusions[i] = new int[dimensions];
            for (int j = 0; j < Mathf.Min(dimensions, mutualExclusions[i].Length); j++)
            {
                this.mutualExclusions[i][j] = mutualExclusions[i][j];
            }
        }
    }
    public void SetAgonals(int[] nagonals)
    {
        this.nagonals = (int[])nagonals.Clone();
    }
    public void SetRange(int range)
    {
        this.range = range;
    }

    public nChessPiece(string name, int ID, int dimensions)
    {
        this.name = name;
        this.ID = ID;
        this.nagonals = null;
        this.dimensions = dimensions;
        this.range = RANGE_UNLIMITED;
    }
}
