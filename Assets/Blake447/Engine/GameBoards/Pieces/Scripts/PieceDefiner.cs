using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class PieceDefiner
{
    public MeshRenderer[] meshes = new MeshRenderer[16];

    int ROOK_ID = 1;
    int BISHOP_ID = 2;

    public int line_move_count = 0;
    public int[][] moves_line;

    public int permutables_move_count = 0;
    public int[][] moves_permutables;

    int[][][] moves_array = new int[Overseer.PIECE_COUNT][][];
    int[] moves_array_lengths = new int[Overseer.PIECE_COUNT];

    int BUFFER_INDEX = 31;

    nChessPiece[] chessPieces = new nChessPiece[Overseer.PIECE_COUNT];

    public void GeneratePieceMoves(nChessPiece[] pieces, int[] dimensions)
    {
        ClearMoveArray();
        chessPieces = (nChessPiece[])pieces.Clone();
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] != null)
            {
                GeneratePieceMoves(pieces[i], dimensions);
            }
        }
        LockPieceMoves(pieces);
    }

    void LockPieceMoves(nChessPiece[] pieces)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] != null && moves_array[i] != null)
            {
                pieces[i].SetMoves((int[][])moves_array[i].Clone());
            }
        }
    }
    public int GetPieceMoveCount(int piece_index, bool isRecursive)
    {
        int piece_id = piece_index & 31;
        int total = 0;
        if (chessPieces[piece_id] != null)
        {
            nChessPiece piece = chessPieces[piece_id];
            total += moves_array_lengths[piece.ID];
            if (isRecursive && piece.subpieces != null)
            {
                for (int i = 0; i < piece.subpieces.Length; i++)
                {
                    total += GetPieceMoveCount(piece.subpieces[i].ID);
                }
            }
        }
        return total;
    }

    public int GetDetectorIndex()
    {
        return 12;
    }

    public void ClearMoveArray()
    {
        for (int i = 0; i < moves_array.Length; i++)
        {
            moves_array[i] = null;
        }
    }


    public int GetPieceMoveCount(int piece_index)
    {
        int piece_id = piece_index & 31;

        int total = 0;
        if (chessPieces[piece_id] != null)
        {
            nChessPiece piece = chessPieces[piece_id];
            total += moves_array_lengths[piece.ID];
        }
        return total;
    }
  
    public bool IsExcluded(int[] move, nChessPiece piece)
    {
        if (piece.mutualExclusions != null)
        {
            for (int exclusion = 0; exclusion < piece.mutualExclusions.Length; exclusion++)
            {
                int check_length = Mathf.Min(move.Length, piece.mutualExclusions[exclusion].Length);

                int exclusion_counter = 0;
                for (int component = 0; component < check_length; component++)
                {
                    if (move[component]*piece.mutualExclusions[exclusion][component] != 0)
                    {
                        exclusion_counter++;
                    }
                }
                if (exclusion_counter > 1)
                {
                    return true;
                }
            }
        }
        if (piece.directExclusions != null)
        {
            for (int exclusion = 0; exclusion < piece.directExclusions.Length; exclusion++)
            {

                int excluded_elements = 0;
                for (int i = 0; i < piece.directExclusions[exclusion].Length; i++)
                {
                    if (piece.directExclusions[exclusion][i] != 0)
                    {
                        excluded_elements++;
                    }
                }

                int exclusion_hits = 0;
                int check_length = Mathf.Min(move.Length, piece.directExclusions[exclusion].Length);
                for (int component = 0; component < check_length; component++)
                {
                    if (move[component] * piece.directExclusions[exclusion][component] != 0)
                    {
                        
                        // Uncomment to enable strict exclusion
                        return true;
                        
                        exclusion_hits++;
                    }
                }
                if (exclusion_hits == excluded_elements && excluded_elements > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int CalculatePieceMoveCount(int piece_index, int[] dimensions, nChessPiece piece)
    {
        int max_distance = 0;
        for (int i = 0; i < dimensions.Length; i++)
        {
            max_distance = Mathf.Max(max_distance, dimensions[i] - 1);
        }

        if (piece.range != piece.GetRangeUnlimitedCode())
        {
            max_distance = piece.range;
        }

        int move_count = 0;
        if (piece.nagonals != null)
        {
            for (int i = 0; i < piece.nagonals.Length; i++)
            {
                move_count += CalculateLineNumber(dimensions.Length, piece.nagonals[i], max_distance);
            }
        }

        if (piece.permutables != null)
        {
            for (int i = 0; i < piece.permutables.Length; i++)
            {
                int[] permutable = (int[])piece.permutables[i].Clone();
                move_count += CalculatePermutationNumber(permutable, dimensions.Length);
            }
        }

        return move_count;
    }
    public int CalculatePieceMoveCount(int piece_index, int[] dimensions, nChessPiece piece, bool isRecursive)
    {
        int max_distance = 0;
        for (int i = 0; i < dimensions.Length; i++)
        {
            max_distance = Mathf.Max(max_distance, dimensions[i] - 1);
        }

        if (piece.range != piece.GetRangeUnlimitedCode())
        {
            max_distance = piece.range;
        }

        int move_count = 0;
        if (piece.nagonals != null)
        {
            for (int i = 0; i < piece.nagonals.Length; i++)
            {
                move_count += CalculateLineNumber(dimensions.Length, piece.nagonals[i], max_distance);
            }
        }

        if (piece.permutables != null)
        {
            for (int i = 0; i < piece.permutables.Length; i++)
            {
                int[] permutable = (int[])piece.permutables[i].Clone();
                move_count += CalculatePermutationNumber(permutable, dimensions.Length);
            }
        }

        if (isRecursive)
        {
            if (piece.subpieces != null)
            {
                for (int i = 0; i < piece.subpieces.Length; i++)
                {
                    int sub_count = CalculatePieceMoveCount(BUFFER_INDEX, dimensions, piece.subpieces[i]);
                    move_count += sub_count;
                }
            }
        }

        return move_count;
    }
    public void GeneratePieceMoves(nChessPiece piece, int[] dimensions)
    {
        int piece_index = piece.ID;
        if (moves_array[piece_index] == null)
        {
            int current_move = 0;
            chessPieces[piece_index] = piece;

            int max_distance = 0;
            for (int i = 0; i < dimensions.Length; i++)
            {
                max_distance = Mathf.Max(max_distance, dimensions[i] - 1);
            }

            if (piece.range != piece.GetRangeUnlimitedCode())
            {
                max_distance = piece.range;
            }

            int move_count = CalculatePieceMoveCount(piece_index, dimensions, piece);
            moves_array[piece_index] = new int[move_count][];

            if (piece.nagonals != null)
            {
                for (int agonal = 0; agonal < piece.nagonals.Length; agonal++)
                {
                    GenerateLineAndStoreInBuffer(dimensions, piece.nagonals[agonal], max_distance);
                    for (int j = 0; j < line_move_count; j++)
                    {
                        int[] move = moves_line[j];
                        if (!IsExcluded(move, piece))
                        {
                            moves_array[piece_index][current_move] = (int[])moves_line[j].Clone();
                            current_move++;
                        }
                    }
                }
            }

            if (piece.permutables != null)
            {
                for (int permutable = 0; permutable < piece.permutables.Length; permutable++)
                {
                    GeneratePermutationsAndStoreInBuffer(piece.permutables[permutable], dimensions.Length);
                    for (int j = 0; j < permutables_move_count; j++)
                    {
                        int[] move = (int[])moves_permutables[j].Clone();
                        if (!IsExcluded(move, piece))
                        {
                            moves_array[piece_index][current_move] = (int[])moves_permutables[j].Clone();
                            current_move++;
                        }
                    }
                }
            }

            if (piece.subpieces != null)
            {
                for (int i = 0; i < piece.subpieces.Length; i++)
                {
                    GeneratePieceMoves(piece.subpieces[i], dimensions);
                }
            }

            moves_array_lengths[piece_index] = current_move;
        }
    }

    public int CalculateLineNumber(int dimensions_count, int nonzero_count, int max_distance)
    {
        if (nonzero_count > dimensions_count)
        {
            return 0;
        }
        if (max_distance == 0)
        {
            return 0;
        }
        if (nonzero_count == 0)
        {
            return 0;
        }



        int power_of_two = 1 << nonzero_count;
        int combinations = Factorial(dimensions_count);
        combinations /= Factorial(nonzero_count);
        combinations /= Factorial(dimensions_count - nonzero_count);
        return combinations * power_of_two * max_distance;
    }

    public void GenerateLineAndStoreInBuffer(int[] dimensions, int nonzero_count, int max_distance)
    {
        line_move_count = 0;
        int memory_allocation = CalculateLineNumber(dimensions.Length, nonzero_count, max_distance);
        moves_line = new int[memory_allocation][];

        for (int i = 0; i < moves_line.Length; i++)
        {
            moves_line[i] = new int[dimensions.Length];
        }

        int[] combination = new int[nonzero_count];
        int[] bounds = new int[nonzero_count];
        int[] vec = new int[dimensions.Length];

        for (int i = 0; i < nonzero_count; i++)
        {
            bounds[i] = MaxValue(dimensions.Length, nonzero_count, i);
        }

        if (nonzero_count <= dimensions.Length && nonzero_count >= 1 && max_distance > 0)
        {
            SequenceArray(combination, 0, 0);
            int sentinel = combination.Length - 1;
            while (combination[0] <= bounds[0])
            {
                if (combination[sentinel] > bounds[sentinel])
                {
                    while (combination[sentinel] > bounds[sentinel])
                    {
                        sentinel--;
                    }
                    SequenceArray(combination, sentinel, combination[sentinel] + 1);
                    sentinel = nonzero_count - 1;
                }
                else
                {
                    for (int i = 0; i < vec.Length; i++)
                    {
                        vec[i] = 0;
                    }
                    for (int permutation = 0; permutation < (1 << nonzero_count); permutation++)
                    {
                        int[] parities = new int[nonzero_count];
                        for (int i = 0; i < parities.Length; i++)
                        {
                            parities[i] = (permutation >> i) & 1;
                        }
                        for (int component_maginitude = 1; component_maginitude <= max_distance; component_maginitude++)
                        {
                            for (int i = 0; i < nonzero_count; i++)
                            {
                                vec[combination[i]] = component_maginitude * (1 - 2*parities[i]);
                            }
                            moves_line[line_move_count] = (int[])vec.Clone();
                            line_move_count++;
                        }
                    }
                    combination[sentinel]++;
                }
            }
        }
    }

    public void SequenceArray(int[] array, int start, int value)
    {
        for (int i = 0; start + i < array.Length; i++)
        {
            array[start + i] = value + i;
        }
    }
    public int MaxValue(int n, int choose, int index)
    {
        return n - 1 - (choose - index - 1);
    }

    public int CalculatePermutationNumber(int[] seed, int length)
    {
        if (seed.Length <= length)
        {
            int[] sorted = new int[length];
            for (int i = 0; i < Mathf.Min(seed.Length, length); i++)
            {
                sorted[i] = seed[i];
            }
            System.Array.Sort(sorted);
            
            int unique_elements = 1;
            for (int i = 1; i < length; i++)
            {
                if (sorted[i] != sorted[i - 1])
                {
                    unique_elements++;
                }
            }

            int[] freq = new int[unique_elements];

            int j = 0;
            int k = 0;
            while (j < unique_elements && k < sorted.Length)
            {
                int value = sorted[k];
                freq[j] = 0;
                while (k < sorted.Length && sorted[k] == value)
                {
                    freq[j]++;
                    k++;
                }
                j++;
            }

            int nonzero_components = 0;
            for (int i = 0; i < sorted.Length; i++)
            {
                if (sorted[i] != 0)
                {
                    nonzero_components++;
                }
            }


            int power_of_two = 1 << nonzero_components;
            int permutation_count = Factorial(length);
            for (int i = 0; i < freq.Length; i++)
            {
                permutation_count /= Factorial(freq[i]);
            }

            return permutation_count * power_of_two;
        }
        return 0;
    }

    public int Factorial(int n)
    {
        int product = 1;
        for (int i = n; i > 0; i--)
        {
            product *= i;
        }
        return product;
    }


    public int CountNonzeroComponents(int[] array)
    {
        int non_zero_components = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != 0)
            {
                non_zero_components++;
            }
        }
        return non_zero_components;
    }
    public int[] ListNonzeroIndices(int[] array)
    {
        int non_zero_components = CountNonzeroComponents(array);
        int[] redirects = new int[non_zero_components];

        int current_index = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != 0)
            {
                redirects[current_index] = i;
                current_index++;
            }
        }

        return redirects;
    }

    public void GeneratePermutationsAndStoreInBuffer(int[] seed, int length)
    {
        int memory_allocation = CalculatePermutationNumber(seed, length);

        permutables_move_count = 0;
        moves_permutables = new int[memory_allocation][];

        if (seed.Length <= length)
        {
            for (int i = 0; i < moves_permutables.Length; i++)
            {
                moves_permutables[i] = new int[length];
            }

            int[] sorted = new int[length];
            for (int i = 0; i < Mathf.Min(seed.Length, length); i++)
            {
                sorted[i] = seed[i];
            }
            System.Array.Sort(sorted);
            int unique_elements = 1;
            for (int i = 1; i < length; i++)
            {
                if (sorted[i] != sorted[i - 1])
                {
                    unique_elements++;
                }
            }
            int[] val = new int[unique_elements];
            int[] freq = new int[unique_elements];

            int j = 0;
            int k = 0;
            while (j < unique_elements && k < sorted.Length)
            {
                int value = sorted[k];
                val[j] = value;
                freq[j] = 0;
                while (k < sorted.Length && sorted[k] == value)
                {
                    freq[j]++;
                    k++;
                }
                j++;
            }

            int[] permutation = new int[length];
            int[] pool = (int[])freq.Clone();

            int index = 0;
            while (index >= 0)
            {
                while (index < length)
                {
                    int first_index = FirstIndex(pool);
                    permutation[index] = first_index;
                    pool[first_index]--;
                    index++;
                }

                int[] vec = new int[permutation.Length];
                for (int i = 0; i < vec.Length; i++)
                {
                    vec[i] = val[permutation[i]];
                }

                int nonzero_components = CountNonzeroComponents(vec);
                //Debug.Log("there are " + nonzero_components + " non-zero componenets in " + CoordinateToString(vec));
                int[] redirects = ListNonzeroIndices(vec);
                int[] signed_vec = new int[vec.Length];

                for (int parity_permutation = 0; parity_permutation < (1 << nonzero_components); parity_permutation++)
                {
                    for (int i = 0; i < vec.Length; i++)
                    {
                        signed_vec[i] = 0;
                    }
                    int[] parities = new int[nonzero_components];
                    for (int i = 0; i < parities.Length; i++)
                    {
                        parities[i] = (parity_permutation >> i) & 1;
                    }
                    for (int i = 0; i < nonzero_components; i++)
                    {
                        signed_vec[redirects[i]] =  vec[redirects[i]] * (1 - 2 * parities[i]);
                    }
                    moves_permutables[permutables_move_count] = (int[])signed_vec.Clone();
                    permutables_move_count++;
                }

                index = length - 1;
                int current_element = permutation[index];
                int incremented_element = NextIndex(pool, permutation[index]);
                while (incremented_element >= pool.Length)
                { 
                    while (index >= 0 && permutation[index] == current_element)
                    {
                        pool[current_element]++;
                        permutation[index] = -1;
                        index--;
                        if (index < 0)
                        {
                            break;
                        }
                    }
                    if (index < 0)
                    {
                        break;
                    }
                    current_element = permutation[index];
                    incremented_element = NextIndex(pool, permutation[index]);
                }
                if (index >= 0)
                {
                    permutation[index] = incremented_element;
                    pool[incremented_element]--;
                    pool[current_element]++;
                    index++;
                }
            }
        }
    }

    public int FirstIndex(int[] array)
    {
        int i = 0;
        while (i < array.Length && array[i] == 0)
        {
            i++;
        }
        return i;
    }
    public int NextIndex(int[] array, int index)
    {
        int i = index + 1;
        while (i < array.Length && array[i] == 0)
        {
            i++;
        }
        return i;
    }


    string CoordinateToString(int[] coordinate)
    {
        string coordinate_str = "(";
        for (int i = 0; i < coordinate.Length; i++)
        {
            coordinate_str += coordinate[i].ToString();
            if (i < coordinate.Length - 1)
            {
                coordinate_str += ", ";
            }
        }
        coordinate_str += ")";
        return coordinate_str;
    }

}
