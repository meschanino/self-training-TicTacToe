using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TicTacToe
{
    public class GamesDataBase
    {
        private List<GameRecord> GamesRecords { get; set; }

        public int Length()
        {
            return GamesRecords.Count;
        }

        public GamesDataBase()
        {
            GamesRecords = new List<GameRecord>();
        }

        public void AddIfNew(GameRecord gameRecord)
        {
            var gameRecordLength = gameRecord.GetLength();
            var recordContainedInBase = false;
            foreach (var record in GamesRecords)
            {
                var recordIsEqual = true;
                for (int i = 0; i < gameRecordLength; i++)
                {
                    recordIsEqual = recordIsEqual&&(gameRecord.MovesList[i] == record.MovesList[i]);
                }
                if (!recordIsEqual) continue;
                recordContainedInBase = true;
                break;
            }
            if (!recordContainedInBase)
                GamesRecords.Add(gameRecord);
        }

        public void AddRotationAndReflectedIfNew(GameRecord gameRecord)
        {
            for (int i = 0; i < 4; i++)
            {
                var rotatedGameRecord = gameRecord.RotateCw90(i);
                AddIfNew(rotatedGameRecord);
                AddIfNew(rotatedGameRecord.SimmetricReflect());
            }
        }

        public void Remove(GameRecord gameRecord)
        {
            GamesRecords.Remove(gameRecord);
        }

        public List<GameRecord> ContainsDebut(GameRecord partOfGameRecord)
        {
            var recordsContainsPart= new List<GameRecord>();
            foreach (var record in GamesRecords)
            {
                var partIsIn = true;
                for (var i = 0; i < partOfGameRecord.GetLength(); i++)
                    partIsIn = partIsIn && (partOfGameRecord.MovesList[i] == record.MovesList[i]);
                if (partIsIn)
                    recordsContainsPart.Add(record);
            }
            return recordsContainsPart;
        }
    }
}