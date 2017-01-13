using System.Collections.Generic;
using System.Data;
using System.Linq;
using NearestNeighborhood.Data;

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
            _similarUsers = new SimilarUsers(similarUsersCount);

            var coreUseres = GetCoreUseres(similarUsersCount);

            foreach (var coreUser in coreUseres)
            {
                var coreUserSongs = UserSongs(coreUser);

                foreach (var similarUser in _usersListenHistory.GetUseresWithoutSelf(coreUser))
                {
                    var similarUserSongs = UserSongs(similarUser);

                    var similarity = MeasuereSimiliarity(coreUserSongs, similarUserSongs);

                    AddSimilarUser(coreUser, similarity, similarUser);
                }
            }

            return _similarUsers;
        }

        private void AddSimilarUser(string userId, double similarity, string similarUserId) => _similarUsers
            .AddUserSimilarUser(userId, SimilarUsers.CreateSimilarUser(similarUserId, similarity));

        private string[] GetCoreUseres(int similarUsersCount) => similarUsersCount > 0 ?
            _usersListenHistory.Users.Take(_k).ToArray() :
            _usersListenHistory.Users.ToArray();

        private double MeasuereSimiliarity(ICollection<string> firstUserSongs, ICollection<string> secondUserSongs)
        {
            var intersectedCount = (double)firstUserSongs.Intersect(secondUserSongs).Count();
            var uninonCount = (double)firstUserSongs.Union(secondUserSongs).Count();

            return intersectedCount / uninonCount;
        }

        private ICollection<string> UserSongs(string userId) => _usersListenHistory.GetSongsForUser(userId);
    }
}