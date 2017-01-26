using System.Collections.Generic;

namespace NearestNeighborhood.Data
{
    public struct SimilarUser
    {
        public double Similarity;
        public string UserId;
        public static IComparer<SimilarUser> SimilarUserComparer => new SimilarUsersComparer();

        public SimilarUser(string userId, double similarity)
        {
            UserId = userId;
            Similarity = similarity;
        }
    }

    public class SimilarUsersComparer : IComparer<SimilarUser>
    {
        public int Compare(SimilarUser x, SimilarUser y)
        {
            var result = 0;

            if (x.Similarity > y.Similarity)
                result = 1;
            else if (x.Similarity < y.Similarity)
                result = -1;

            return result;
        }
    }
}