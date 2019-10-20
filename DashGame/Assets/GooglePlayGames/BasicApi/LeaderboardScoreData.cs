// <copyright file="LeaderboardScoreData.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
<<<<<<< HEAD

#if UNITY_ANDROID
=======
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

namespace GooglePlayGames.BasicApi
{
    using System.Collections.Generic;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Leaderboard score data. This is the callback data
    /// when loading leaderboard scores.  There are several SDK
    /// API calls needed to be made to collect all the required data,
    /// so this class is used to simplify the response.
    /// </summary>
    public class LeaderboardScoreData
    {
        private string mId;
        private ResponseStatus mStatus;
        private ulong mApproxCount;
        private string mTitle;
        private IScore mPlayerScore;
        private ScorePageToken mPrevPage;
        private ScorePageToken mNextPage;
        private List<PlayGamesScore> mScores = new List<PlayGamesScore>();

        internal LeaderboardScoreData(string leaderboardId)
        {
            mId = leaderboardId;
        }

        internal LeaderboardScoreData(string leaderboardId, ResponseStatus status)
        {
            mId = leaderboardId;
            mStatus = status;
        }

        public bool Valid
        {
            get
            {
                return mStatus == ResponseStatus.Success ||
<<<<<<< HEAD
                       mStatus == ResponseStatus.SuccessWithStale;
=======
                mStatus == ResponseStatus.SuccessWithStale;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            }
        }

        public ResponseStatus Status
        {
<<<<<<< HEAD
            get { return mStatus; }

            internal set { mStatus = value; }
=======
            get
            {
                return mStatus;
            }

            internal set
            {
                mStatus = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public ulong ApproximateCount
        {
<<<<<<< HEAD
            get { return mApproxCount; }

            internal set { mApproxCount = value; }
=======
            get
            {
                return mApproxCount;
            }

            internal set
            {
                mApproxCount  = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string Title
        {
<<<<<<< HEAD
            get { return mTitle; }

            internal set { mTitle = value; }
=======
            get
            {
                return mTitle;
            }

            internal set
            {
                mTitle = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string Id
        {
<<<<<<< HEAD
            get { return mId; }

            internal set { mId = value; }
=======
            get
            {
                return mId;
            }

            internal set
            {
                mId = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public IScore PlayerScore
        {
<<<<<<< HEAD
            get { return mPlayerScore; }

            internal set { mPlayerScore = value; }
=======
            get
            {
                return mPlayerScore;
            }

            internal set
            {
                mPlayerScore = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public IScore[] Scores
        {
<<<<<<< HEAD
            get { return mScores.ToArray(); }
=======
            get
            {
                return mScores.ToArray();
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        internal int AddScore(PlayGamesScore score)
        {
            mScores.Add(score);
            return mScores.Count;
        }

        public ScorePageToken PrevPageToken
        {
<<<<<<< HEAD
            get { return mPrevPage; }

            internal set { mPrevPage = value; }
=======
            get
            {
                return mPrevPage;
            }

            internal set
            {
                mPrevPage = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public ScorePageToken NextPageToken
        {
<<<<<<< HEAD
            get { return mNextPage; }

            internal set { mNextPage = value; }
=======
            get
            {
                return mNextPage;
            }

            internal set
            {
                mNextPage = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public override string ToString()
        {
            return string.Format("[LeaderboardScoreData: mId={0}, " +
<<<<<<< HEAD
                                 " mStatus={1}, mApproxCount={2}, mTitle={3}]",
=======
                " mStatus={1}, mApproxCount={2}, mTitle={3}]",
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                mId, mStatus, mApproxCount, mTitle);
        }
    }
}
<<<<<<< HEAD
#endif
=======
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
