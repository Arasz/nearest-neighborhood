using NearestNeighborhood.Data;
using System.Collections.Generic;
using System.Linq;

namespace NearestNeighborhood.Algorithm
{
    public class NearestNeighborhoodAlgorithm
    {
        private readonly int _k;
        private readonly UsersListenHistory _usersListenHistory;
        private SimilarUsers _similarUsers;

        public NearestNeighborhoodAlgorithm(UsersListenHistory usersListenHistory, int k = 100)
        {
            _usersListenHistory = usersListenHistory;
            _k = k;
        }

        public SimilarUsers FindNearest(int similarUsersCount)
        {
            _similarUsers = new SimilarUsers(_k);

            var coreUseres = GetCoreUseres(similarUsersCount);

            foreach (var coreUser in coreUseres)
            {
                var coreUserSongs = UserSongs(coreUser);

                foreach (var similarUser in _usersListenHistory.Users)
                {
                    var similarUserSongs = UserSongs(similarUser);

                    var similarity = MeasureSimilarity(coreUserSongs, similarUserSongs);

                    AddSimilarUser(coreUser, similarity, similarUser);
                }
            }

            return _similarUsers;
        }

        private void AddSimilarUser(string userId, double similarity, string similarUserId) => _similarUsers
            .AddUserSimilarUser(userId, SimilarUsers.CreateSimilarUser(similarUserId, similarity));

        private string[] GetCoreUseres(int similarUsersCount) => similarUsersCount > 0 ?
            _usersListenHistory.Users.Take(similarUsersCount).ToArray() :
            _usersListenHistory.Users.ToArray();

        private double MeasureSimilarity(ICollection<string> firstUserSongs, ICollection<string> secondUserSongs)
        {
            var intersectedCount = firstUserSongs.Intersect(secondUserSongs).Count();

            if (intersectedCount == 0)
                return intersectedCount;

            var uninonCount = (double)(firstUserSongs.Count + secondUserSongs.Count - intersectedCount);

            return intersectedCount / uninonCount;
        }

        private ICollection<string> UserSongs(string userId) => _usersListenHistory.GetSongsForUser(userId);
    }
}