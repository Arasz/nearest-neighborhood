using System.Collections.Generic;
using System.Linq;

namespace NearestNeighborhood.Data
{
    public class UsersListenHistory
    {
        private Dictionary<string, HashSet<string>> _userSongs = new Dictionary<string, HashSet<string>>();

        public ICollection<string> Users => _userSongs.Keys;

        public void AddListen(string userId, string songId)
        {
            if (_userSongs.ContainsKey(userId))
            {
                var songsCollection = _userSongs[userId];
                songsCollection.Add(songId);
            }
            else
                _userSongs[userId] = new HashSet<string> { songId };
        }

        public ICollection<string> GetSongsForUser(string userId) => _userSongs[userId];

        public IEnumerable<string> GetUseresWithoutSelf(string userId) => _userSongs.Keys
            .Where(id => id != userId);
    }
}