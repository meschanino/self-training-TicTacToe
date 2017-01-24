using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TicTacToe
{
    partial class CrossesHolesForm : Form
    {
        const int CellWidth = 80;
        const string Cross = "☃"; // "✕" "☃" snowman
        const string Zero = "❄"; //"○"  "❄" snowflake

        int crossWinCounter = 0;
        int zeroWinCounter = 0;
        int drawnCounter = 0;

        public CrossesHolesForm(BattleField battleField, GamesDataBase gamesDataBase)
        {
            Text = "Self-training TicTacToe";
            Size = new Size(CellWidth*5, CellWidth*5);
            MinimumSize = Size;

            var gameRecord = new GameRecord();
            

            Label notifyLabel = new Label()
            {
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width, ClientSize.Height/5),
                ForeColor = Color.OrangeRed,
                Font = new Font(Font.Name, CellWidth/2),
                Text = "",
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(notifyLabel);

            Button[] battleFieldButtons = new Button[battleField.Length];
            int buttonsXLocation = (Size.Width - CellWidth*4)/2;
            int buttonsYLocation = notifyLabel.Bottom;
            for (int i = 0; i < battleFieldButtons.Length; i++)
            {
                battleFieldButtons[i] = new Button()
                {
                    Width = CellWidth,
                    Height = CellWidth,
                    Location = new Point(buttonsXLocation + i%3*CellWidth,
                        buttonsYLocation + i/3*CellWidth),
                    Font = new Font(Font.Name, CellWidth/2),
                    ForeColor = Color.Blue,
                    Text = SignCell(battleField.Cells[i])
                };
                Controls.Add(battleFieldButtons[i]);
            }

            Label dataBaseCounterLabel = new Label()
            {
                Location = new Point(0, battleFieldButtons[battleField.Length-1].Bottom+1),
                Size = new Size(ClientSize.Width, CellWidth/2),
                Font = new Font(Font.Name, CellWidth / 7),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(dataBaseCounterLabel);

            Button autoMoveButton = new Button()
            {
                Width= CellWidth,
                Height = CellWidth * 3,
                Location = new Point(buttonsXLocation+CellWidth*3,buttonsYLocation),
                Font = new Font(Font.Name,CellWidth/3),
                Text = String.Format("{0}\nmove",Cross)
            };
            Controls.Add(autoMoveButton);

            autoMoveButton.Click += (sender, args) =>
            {
                if (notifyLabel.Text == "")
                {
                    var numberOfCellToXMove = CalculateSignMove(CellValue.X, battleField,
                        gameRecord, gamesDataBase);
                    battleFieldButtons[numberOfCellToXMove].PerformClick();
                }
                else battleFieldButtons[0].PerformClick();
            };

            foreach (Button cellButton in battleFieldButtons)
            {
                cellButton.Click += (sender, args) =>
                {
                    if (notifyLabel.Text != "")
                    {
                        gamesDataBase.AddRotationAndReflectedIfNew(gameRecord);
                        gameRecord = new GameRecord(GameResult.PlayerLose, new List<int>());
                        battleField = new BattleField();
                        dataBaseCounterLabel.Text = String.Format
                        (" {0} wins {1}   {2} wins {3}    drawn {4}\nThere are {5} varianrts in DataBase",
                            Cross, crossWinCounter, Zero, zeroWinCounter, drawnCounter,
                            gamesDataBase.Length());
                        notifyLabel.Text = "";
                        foreach (Button cell in battleFieldButtons)
                        {
                            cell.Text = "";
                            cell.ForeColor = Color.Blue;
                        }
                    }
                    else if (cellButton.Text == "")
                    {
                        cellButton.Text = SignCell(CellValue.X);
                        for (int i = 0; i < battleField.Length; i++)
                        {
                            if (SignCell(battleField.Cells[i]) != battleFieldButtons[i].Text)
                            {
                                MoveAndCheckWinner(i, CellValue.X, battleField, battleFieldButtons,
                                    notifyLabel, gameRecord);
                            }
                            if (battleField.EmptyCells.Count == 0 && notifyLabel.Text == "")
                            {
                                notifyLabel.ForeColor = Color.Blue;
                                notifyLabel.Text = "drawn";
                                drawnCounter++;
                                gameRecord.AddResult(GameResult.Drawn);
                            }
                        }
                        if (notifyLabel.Text == "" && battleField.EmptyCells.Count > 0)
                        {
                            int oMoveCellNumber = CalculateSignMove(CellValue.O, battleField,
                                gameRecord, gamesDataBase);
                            MoveAndCheckWinner(oMoveCellNumber, CellValue.O, battleField,
                                battleFieldButtons, notifyLabel, gameRecord);
                        }
                    }
                };
            }
        }

        public void MoveAndCheckWinner(int cellNumber, CellValue sign, BattleField battleField,
            Button[] battleFieldButtons, Label notifyLabel, GameRecord gameRecord)
            {
                battleField.MakeMoveIn(cellNumber, sign, gameRecord);
                battleFieldButtons[cellNumber].Text = SignCell(sign);
                int[] winnerLine = battleField.CheckWinnerLine(cellNumber, sign);
                if (winnerLine != null)
                {
                    notifyLabel.ForeColor=Color.OrangeRed;
                    notifyLabel.Text = String.Format("{0} wins!", SignCell(sign));
                    if (sign == CellValue.X)
                        {crossWinCounter++;} 
                    else if (sign == CellValue.O)
                        {zeroWinCounter++;}
                    gameRecord.AddResult(sign==CellValue.X ? 
                        GameResult.PlayerWin:GameResult.PlayerLose);
                    foreach (int cellNum in winnerLine)
                        battleFieldButtons[cellNum].ForeColor = Color.OrangeRed;
                }
            }

        int CalculateSignMove (CellValue sign, BattleField battleField)
            {
                BattleField cloneBattleField = new BattleField();
                for (int i = 0; i < battleField.Length; i++)
                {
                    cloneBattleField.Cells[i] = battleField.Cells[i];
                }

                int thirdOtest = CloneWinnerLineTest(cloneBattleField, sign);
                if (thirdOtest < cloneBattleField.Length)
                    return thirdOtest;
                
                CellValue invertedSign = InvertSign(sign);
                int thirdXtest = CloneWinnerLineTest(cloneBattleField, invertedSign);
                if (thirdXtest < cloneBattleField.Length)
                    return thirdXtest;
                
                if (battleField.Cells[4]==CellValue.Empty) return 4;

                Random random= new Random();
                int rndEmptyCellNumber = random.Next(battleField.EmptyCells.Count);
                var cellToMoveNumber = battleField.EmptyCells[rndEmptyCellNumber];
                return cellToMoveNumber;
            }

        int CalculateSignMove(CellValue sign, BattleField battleField, 
                            GameRecord gameRecord, GamesDataBase gamesDataBase)
        {
            BattleField cloneBattleField = new BattleField();
            for (int i = 0; i < battleField.Length; i++)
                cloneBattleField.Cells[i] = battleField.Cells[i];
            //выиграть в 1 ход: если есть два О в линии и третья пустая ячейка,
            //ставить О в пустую ячейку
            int thirdOtest = CloneWinnerLineTest(cloneBattleField, sign);
            if (thirdOtest < cloneBattleField.Length)
                return thirdOtest;

            //помешать выиграть сопернику в 1 ход: если есть два Х в линии и третья пустая ячейка,
            // ставить О в пустую ячейку
            CellValue invertedSign = InvertSign(sign);
            int thirdXtest = CloneWinnerLineTest(cloneBattleField, invertedSign);
            if (thirdXtest < cloneBattleField.Length)
                return thirdXtest;
            /*ходить в центр*/
            if (battleField.Cells[4]==CellValue.Empty)
                return 4;
            
            /*выбрать из базы прошлые проигрыши и ходить не так*/
            var previouslyNotLosingMove = DontLoseAsPreviously (battleField,gameRecord,gamesDataBase);
            if (previouslyNotLosingMove >= 0)
                return previouslyNotLosingMove;

            /*выбрать из базы прошлые НЕпроигрышные варианты и ходить так же */
            var previouslyWinnerMove = WinAsPreviously(battleField, gameRecord, gamesDataBase);
            if (previouslyWinnerMove >= 0)
                return previouslyWinnerMove;
            
            /*ходить в случайную пустую ячейку*/
            var random = new Random();
            int randomEmptyCellNumber = random.Next(battleField.EmptyCells.Count);
            var randomCellToMoveNumber = battleField.EmptyCells[randomEmptyCellNumber];
            return randomCellToMoveNumber;
        }

        /*выбрать из базы прошлые НЕпроигрышные для компьютера варианты и ходить так же
                , если НЕпроигрышных >= чем проигрышных  */
        private int WinAsPreviously(BattleField battleField, GameRecord gameRecord,
            GamesDataBase gamesDataBase)
        {
            List<GameRecord> possibleVariants = gamesDataBase.ContainsDebut(gameRecord);
            var computerNonLoseVariants = possibleVariants
                                            .Where(v => v.Result != GameResult.PlayerWin)
                                            .ToList();
            if (computerNonLoseVariants.Count >= possibleVariants.Count/2 &&
                computerNonLoseVariants.Count > 0 &&
                battleField.EmptyCells.Count > 0)
            {
                var random = new Random();
                var rndIndex = random.Next(computerNonLoseVariants.Count);
                int currentMoveNumber = battleField.Length - battleField.EmptyCells.Count;
                return computerNonLoseVariants[rndIndex].MovesList[currentMoveNumber];
            }
            return -1;
        }

        /*выбрать из базы прошлые проигрыши и ходить не так*/
        private int DontLoseAsPreviously(BattleField battleField, GameRecord gameRecord,
                                        GamesDataBase gamesDataBase)
        {
            int currentMoveNumber = battleField.Length - battleField.EmptyCells.Count;
            var possibleVariants = gamesDataBase.ContainsDebut(gameRecord);

            var playerWinVariants = possibleVariants
                                    .Where(v => v.Result == GameResult.PlayerWin).ToList();
            List<int> playerWinCells = new List<int>();
            foreach (var record in playerWinVariants)
            {
                var cellNumber = record.MovesList[currentMoveNumber];
                if (playerWinCells.Contains(cellNumber)) continue;
                playerWinCells.Add(cellNumber);
            }
            var cellsToMove = battleField.EmptyCells.Except(playerWinCells).ToList();
            var cellsToMoveCount = cellsToMove.Count;
            var random = new Random();
            if (cellsToMoveCount > 0)
                return cellsToMove[random.Next(cellsToMoveCount)];
            return -1;
        }

        int CloneWinnerLineTest(BattleField cloneBattleField,CellValue sign)
            {
                for (int i = 0; i < cloneBattleField.Length; i++)
                {
                    if (cloneBattleField.Cells[i] != CellValue.Empty) continue;
                    cloneBattleField.Cells[i] = sign;
                    int[] testWinnerLine = cloneBattleField.CheckWinnerLine(i, sign);
                    if (testWinnerLine != null)
                        return i;
                    cloneBattleField.Cells[i] = CellValue.Empty;
                }
                return 100500;
            }

            CellValue InvertSign(CellValue sign)
            {
                switch (sign)
                {
                  case CellValue.O: return CellValue.X;
                  case CellValue.X:return CellValue.O;
                }
                return CellValue.Empty;
            }
            
            string SignCell(CellValue sign)
            {
                switch (sign)
                {
                    case CellValue.O:
                        return Zero;// "○"
                    case CellValue.X:
                        return Cross;//"✕" 
                    case CellValue.Empty:
                        return "";
                }
                return null;
            }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Form1";
        }
        #endregion
    }
}


