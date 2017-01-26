using System;
using System.Collections.Generic;
using System.Data;
using Accord.IO;

namespace NearestNeighborhood.Data
{
    public class DataLoader
    {
        private readonly string[] _columnNames;
        private readonly long _limit;
        private readonly long _pageSize;
        private readonly string _path;
        private string[] _buffer;

        public event Action<long> PageLoaded;

        public UsersListenHistory Data => LoadData();

        public DataLoader(string path, int pageSize, long limit = long.MaxValue)
        {
            _path = path;
            _pageSize = pageSize;
            _limit = limit;
            _buffer = new string[3];
        }

        protected virtual void OnPageLoaded(long pageSize) => PageLoaded?.Invoke(pageSize);

        private void LoadAllDataFromFile(CsvReader csvReader, long loadedLines, UsersListenHistory userToSongMap)
        {
            while (!csvReader.EndOfStream && loadedLines < _limit)
            {
                LoadPage(userToSongMap, csvReader);
                loadedLines += _pageSize;
                OnPageLoaded(loadedLines);
            }
        }

        private UsersListenHistory LoadData()
        {
            var userToSongMap = new UsersListenHistory();
            long loadedLines = 0;

            using (var csvReader = new CsvReader(_path, true))
            {
                LoadAllDataFromFile(csvReader, loadedLines, userToSongMap);
            }

            return userToSongMap;
        }

        private void LoadPage(UsersListenHistory usersListenHistory, CsvReader reader)
        {
            var readCount = 0;
            while (reader.ReadNextRecord() && readCount <= _pageSize)
            {
                reader.CopyCurrentRecordTo(_buffer);
                usersListenHistory.AddListen(_buffer[1], _buffer[0]);
                readCount++;
            }
        }
    }
}