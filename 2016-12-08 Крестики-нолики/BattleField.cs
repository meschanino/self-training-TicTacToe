using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{

    public class BattleField
    {
        private const int BattleFieldLenth = 9;

        internal CellValue[] Cells{get; set;}
        internal List<int> EmptyCells {get; set;}
        internal int Length { get; set; }
        
        public BattleField()
        {
            Cells= new CellValue[BattleFieldLenth];
            EmptyCells = new List<int>();
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = CellValue.Empty;
                EmptyCells.Add(i);
            }
            Length = Cells.Length;
        }

        public void MakeMoveIn(int cellNumber, CellValue sign, GameRecord gameRecord)
        {
            Cells[cellNumber] = sign;
            EmptyCells.Remove(cellNumber);
            gameRecord.AddMove(cellNumber);
        }

        internal int MovesToEnd()
        {
            return Cells.Count(cell => cell == CellValue.Empty);
        }

        public int[] CheckWinnerLine(int cellNumber, CellValue sign)
        {
            int firstInRow = cellNumber - cellNumber % 3;
            bool checkHoriz = (Cells[firstInRow + 0] == sign) &&
                             (Cells[firstInRow + 1] == sign) &&
                             (Cells[firstInRow + 2] == sign);
            if (checkHoriz) return new []{firstInRow + 0, firstInRow + 1,firstInRow + 2};

            int firstInColumn = cellNumber % 3;
            bool checkVert = Cells[firstInColumn + 0] == sign &&
                            Cells[firstInColumn + 3] == sign &&
                            Cells[firstInColumn + 6] == sign;
            if (checkVert) return new[] {firstInColumn, firstInColumn + 3, firstInColumn + 6};
            bool checkLeftDiag = cellNumber % 4 == 0 && 
                                Cells[0] == sign &&
                                Cells[4] == sign &&
                                Cells[8] == sign;
            if (checkLeftDiag) return new[] {0, 4, 8};
            bool checkRightDiag = cellNumber % 2 == 0 && 
                                 Cells[2] == sign &&
                                 Cells[4] == sign &&
                                 Cells[6] == sign;
            if (checkRightDiag) return new[] {2, 4, 6};
            return null;
        }
    }
}
