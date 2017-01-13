using System.Collections.Generic;
using System.Data;
using System.Linq;
using NearestNeighborhood.Data;

namespace NearestNeighborhood.Algorithm
{
    public class NearestNeighborhoodAlgorithm
    {
        private readonly int _k;
        private readonly UserToSongMap _userToSongMap;
        private Dictionary<string, SortedSet<SimilarUser>> _similarityMap;

        public NearestNeighborhoodAlgorithm(UserToSongMap userToSongMap, int k = 100)
        {
            _userToSongMap = userToSongMap;
            _k = k;
            _similarityMap = new Dictionary<string, SortedSet<SimilarUser>>();
        }

        public Dictionary<string, SortedSet<SimilarUser>> Find(int forUsers = -1)
        {
            string[] users;

            if (forUsers > 0)
                users = _userToSongMap.Users.Take(_k).ToArray();
            else
                users = _userToSongMap.Users.ToArray();

            foreach (var user in users)
            {
                var userSongs = _userToSongMap.GetSongsForUser(user);

                foreach (var similarUser in _userToSongMap.Users)
                {
                    var similarUserSongs = _userToSongMap.GetSongsForUser(similarUser);

                    var similarity = MeasuereSimiliarity(userSongs, similarUserSongs);

                    AddSimilarUser(user, similarity, similarUser);
                }
            }

            return _similarityMap;
        }

        private static void AddSimilarUser(double similarity, string similarUserId, SortedSet<SimilarUser> similarUsers)
        {
            similarUsers.Add(new SimilarUser(similarUserId, similarity));
        }

        private void AddSimilarUser(string userId, double similarity, string similarUserId)
        {
            if (_similarityMap.ContainsKey(userId))
            {
                InserSimilarUser(userId, similarity, similarUserId);
            }
            else
            {
                _similarityMap[userId] = new SortedSet<SimilarUser>(SimilarUser.SimilarUserComparer);
            }
        }

        private void InserSimilarUser(string userId, double similarity, string similarUserId)
        {
            var similarUsers = _similarityMap[userId];
            if (similarUsers.Count == _k)
            {
                ReplaceWithLoweSimilarity(similarity, similarUserId, similarUsers);
            }
            else
            {
                AddSimilarUser(similarity, similarUserId, similarUsers);
            }
        }

        private double MeasuereSimiliarity(ICollection<string> firstUserSongs, ICollection<string> secondUserSongs)
        {
            var intersectedCount = (double)firstUserSongs.Intersect(secondUserSongs).Count();
            var uninonCount = (double)firstUserSongs.Union(secondUserSongs).Count();

            return intersectedCount / uninonCount;
        }

        private void ReplaceWithLoweSimilarity(double similarity, string similarUserId, SortedSet<SimilarUser> similarUsers)
        {
            if (!(similarity > similarUsers.Min.Similarity))
                return;
            similarUsers.Remove(similarUsers.Min);
            AddSimilarUser(similarity, similarUserId, similarUsers);
        }
    }
}