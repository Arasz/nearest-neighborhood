using C5;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NearestNeighborhood.Data
{
    [DataContract]
    public class SimilarUsers
    {
        private readonly int _similarUsersCount;
        private IntervalHeap<SimilarUser> _currentProcessedSimilarUsers;

        [DataMember]
        private Dictionary<string, IntervalHeap<SimilarUser>> _similarUsersForUser;

        public IReadOnlyDictionary<string, IntervalHeap<SimilarUser>> SimilarUsersForUser => _similarUsersForUser;

        private static IntervalHeap<SimilarUser> NewSimilarUsersCollection => new IntervalHeap<SimilarUser>(SimilarUser.SimilarUserComparer);

        public SimilarUsers(int similarUsersCount)
        {
            _similarUsersCount = similarUsersCount;
            _similarUsersForUser = new Dictionary<string, IntervalHeap<SimilarUser>>();
        }

        public static SimilarUser CreateSimilarUser(string userId, double similarity) => new SimilarUser(userId, similarity);

        public void AddUserSimilarUser(string userId, SimilarUser similarUser)
        {
            if (_similarUsersForUser.ContainsKey(userId))
                _currentProcessedSimilarUsers = _similarUsersForUser[userId];
            else
            {
                _currentProcessedSimilarUsers = NewSimilarUsersCollection;
                _similarUsersForUser.Add(userId, _currentProcessedSimilarUsers);
            }

            AddSimilarUserToCollection(similarUser);
        }

        private void AddSimilarUserToCollection(SimilarUser similarUser)
        {
            if (_currentProcessedSimilarUsers.Count == _similarUsersCount)
                TryReplaceUserWithLowerSimilarity(similarUser);
            else
                _currentProcessedSimilarUsers.Add(similarUser);
        }

        private void TryReplaceUserWithLowerSimilarity(SimilarUser similarUser)
        {
            var userWithLowestSimilarity = _currentProcessedSimilarUsers.FindMin();

            if (!(similarUser.Similarity > userWithLowestSimilarity.Similarity))
                return;

            _currentProcessedSimilarUsers.DeleteMin();
            _currentProcessedSimilarUsers.Add(similarUser);
        }
    }
}