using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public enum Dimension
    {
        Line,
        Colum,
        Block
    }
    public enum SolverType
    {
        SolveLogical,
        SolveGuessing,
        SolveEmpty
    }

    public static class Solver
    {
        public static int[] input = new int[]
{
            0,0,4,3,0,0,2,0,9,
            0,0,5,0,0,9,0,0,1,
            0,7,0,0,6,0,0,4,3,
            0,0,6,0,0,2,0,8,7,
            1,9,0,0,0,7,4,0,0,
            0,5,0,0,8,3,0,0,0,
            6,0,0,0,0,0,1,0,5,
            0,0,3,5,0,8,6,9,0,
            0,4,2,9,1,0,3,0,0
};

        public static int[] inputSolved = new int[]
{
            8,6,4,3,7,1,2,5,9,
            3,2,5,8,4,9,7,6,1,
            9,7,1,2,6,5,8,4,3,
            4,3,6,1,9,2,5,8,7,
            1,9,8,6,5,7,4,3,2,
            2,5,7,4,8,3,9,1,6,
            6,8,9,7,3,4,1,2,5,
            7,1,3,5,2,8,6,9,4,
            5,4,2,9,1,6,3,7,8
};

        public static int[] Hardest = new int[]
        {
            8,0,0,0,0,0,0,0,0,
            0,0,3,6,0,0,0,0,0,
            0,7,0,0,9,0,2,0,0,
            0,5,0,0,0,7,0,0,0,
            0,0,0,0,4,5,7,0,0,
            0,0,0,1,0,0,0,3,0,
            0,0,1,0,0,0,0,6,8,
            0,0,8,5,0,0,0,1,0,
            0,9,0,0,0,0,4,0,0
        };

        public static int[] hell = new int[]
{
            0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,3,0,8,5,
            0,0,1,0,2,0,0,0,0,
            0,0,0,5,0,7,0,0,0,
            0,0,4,0,0,0,1,0,0,
            0,9,0,0,0,0,0,0,0,
            5,0,0,0,0,0,0,7,3,
            0,0,2,0,1,0,0,0,0,
            0,0,0,0,4,0,0,0,9
};
        static SDataSet sData;

        public static bool showDebug = true;

        public static int emptyStartCount = 0;

        
        public static bool Solve()
        {
            Console.WriteLine("Calculating possible permutations...");

            sData.GetPermutations();

            Console.WriteLine("Permutations calculated.");
            Console.WriteLine("------------------------------");
          


            emptyStartCount = sData.CountEmptys();

            int removeCount = 0;

            for (int o = 0; o < 5; o++)
            {
                removeCount = 0;
                for (int i = 0; i < 81; i++)
                {
                    removeCount += sData.CheckISValid(i);
                }
                Console.WriteLine($"Removed ({removeCount}) this run");
                if (removeCount == 0)
                {
                    Console.WriteLine($"No more removed items found.");
                    break;
                }
            }

            for (int i = 0; i < 81; i++)
            {
                Console.WriteLine($"==============================================");
                List<FlattenValue> result = sData.Flat(i);
                int[] indexOfI = i.getIndexOf(Dimension.Line);
                Console.WriteLine($"Index [{i}] [{indexOfI[0]}][{indexOfI[1]}] == result count: {result.Count}");
                int totaalCount = result.Sum(x => x.count);
                double percentage = 0;
                for (int u = 0; u < result.Count; u++)
                {
                    percentage = (double)result[u].count / (double)totaalCount;
                    percentage *= 100;
                    Console.WriteLine($"== ({result[u].value}) Count: {result[u].count} == {percentage.ToString("0.0")} %");

                }
                Console.WriteLine($"==============================================");

                sData.Fill(i, result);
            }

            int empty = sData.CountEmptys();
            Console.WriteLine($"Starting empty count: {emptyStartCount}, new empty count: {empty}, filled totaal: {emptyStartCount - empty}");


            int totaalPosLeft = 0;
            int linetotaalPosLeft = 0;

            for (int y = 0; y < 9; y++)
            {
                linetotaalPosLeft = 0;
                for (int x = 0; x < 9; x++)
                {
                    Console.Write(sData.sdLines[y].data[x] + ",");
                    linetotaalPosLeft += sData.sdLines[y].possiblePermutations.Count();
                    linetotaalPosLeft += sData.sdCols[y].possiblePermutations.Count();
                    linetotaalPosLeft += sData.sdBlocks[y].possiblePermutations.Count();
                }
                Console.WriteLine($" == {linetotaalPosLeft} options left");
                totaalPosLeft += linetotaalPosLeft;
            }

            Console.WriteLine($"Totaal opions left: {totaalPosLeft/243}");

            if (empty > 0)
                return true;

            return false;
        }

        public static void Call()
        {
            input = hell;
           // input = Hardest;

            if (sData == null) sData = new SDataSet();
            sData.Reset();
            Random r = new Random();

            Console.WriteLine("resetting solver...");

            sData.Permutations = SolverHelper.Permute(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Console.WriteLine($"Possible line solutions: {sData.Permutations.Count}");
            Console.WriteLine($"Possible line length: {sData.Permutations[0].Count}");
            Console.WriteLine($"Check count at {1}: {sData.Permutations.Where(x => x[0] == 1).Count()}");

            Console.WriteLine("Reading sudoku input...");
            sData.ReadSudokuData(input, Dimension.Line);
            sData.ReadSudokuData(input, Dimension.Colum);
            sData.ReadSudokuData(input, Dimension.Block);

            Console.WriteLine("Converted to Sudoku data set model.");
            Console.WriteLine("------------------------------");

            if(Solve() == false)
                Console.WriteLine("------------------------------ NOT SOLVED ");

            Console.ReadLine();
        }
    }
}