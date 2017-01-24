using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TicTacToe
{
    public class GameRecord
    {
        private const int MaxMovesNumber = 9;
        private readonly int[] _rotationCw90Matrix = {  2, 5, 8,
                                                        1, 4, 7,
                                                        0, 3, 6};
        private readonly int[] _simmetricReflectionMatrix = {   0, 3, 6,
                                                                1, 4, 7,
                                                                2, 5, 8};

        //public int Id { get; private set; }
        public GameResult Result { get; private set; }
        public List<int> MovesList { get; set; }
        
        public GameRecord()
        {
            Result = GameResult.PlayerLose;
            MovesList = new List<int>();
        }

        public GameRecord(GameResult result, List<int> moveList)
        {
            Result = result;
            if (moveList.Count<= MaxMovesNumber)
                MovesList = moveList;
            else throw new Exception("Too many moves");
        }

        public void AddMove(int cellNumber)
        {
            MovesList.Add(cellNumber);
        }

        public void AddResult(GameResult result)
        {
            Result = result;
        }

        public int GetLength()
        {
            return MovesList.Count;
        }

        private GameRecord TransformBy(int[] transformationMatrix)
        {
            var thisMovesListCount = this.MovesList.Count;
            var transformatedRecord = new GameRecord
            {
                Result = this.Result,
                MovesList = new int[thisMovesListCount].ToList()
            };
            for (int i = 0; i < thisMovesListCount; i++)
                transformatedRecord.MovesList[i] = transformationMatrix[this.MovesList[i]];
            return transformatedRecord;
        }

        public GameRecord RotateCw90()
        {
            return TransformBy(_rotationCw90Matrix);
        }

        public GameRecord RotateCw90(int quarterNumber)
        {
            var rotatedRecord=this;
            for (int i = 0; i < quarterNumber; i++)
                rotatedRecord = rotatedRecord.RotateCw90();
            return rotatedRecord;
        }

        public GameRecord SimmetricReflect()
        {
            return TransformBy(_simmetricReflectionMatrix);
        }
    }
}