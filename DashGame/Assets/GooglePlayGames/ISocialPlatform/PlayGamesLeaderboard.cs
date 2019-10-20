// <copyright file="PlayGamesLeaderboard.cs" company="Google Inc.">
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

namespace GooglePlayGames
{
    using System.Collections.Generic;
    using GooglePlayGames.BasicApi;
    using UnityEngine;
    using UnityEngine.SocialPlatforms;

    public class PlayGamesLeaderboard : ILeaderboard
    {
        private string mId;
        private UserScope mUserScope;
        private Range mRange;
        private TimeScope mTimeScope;
        private string[] mFilteredUserIds;
        private bool mLoading;

        private IScore mLocalUserScore;
        private uint mMaxRange;
        private List<PlayGamesScore> mScoreList = new List<PlayGamesScore>();
        private string mTitle;

        public PlayGamesLeaderboard(string id)
        {
            mId = id;
        }

        #region ILeaderboard implementation

        public void SetUserFilter(string[] userIDs)
        {
            mFilteredUserIds = userIDs;
        }

        public void LoadScores(System.Action<bool> callback)
        {
            PlayGamesPlatform.Instance.LoadScores(this, callback);
        }

        public bool loading
        {
<<<<<<< HEAD
            get { return mLoading; }
            internal set { mLoading = value; }
=======
            get
            {
                return mLoading;
            }
            internal set
            {
                mLoading = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string id
        {
<<<<<<< HEAD
            get { return mId; }
            set { mId = value; }
=======
            get
            {
                return mId;
            }
            set
            {
                mId = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public UserScope userScope
        {
<<<<<<< HEAD
            get { return mUserScope; }
            set { mUserScope = value; }
=======
            get
            {
                return mUserScope;
            }
            set
            {
                mUserScope = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public Range range
        {
<<<<<<< HEAD
            get { return mRange; }
            set { mRange = value; }
=======
            get
            {
                return mRange;
            }
            set
            {
                mRange = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public TimeScope timeScope
        {
<<<<<<< HEAD
            get { return mTimeScope; }
            set { mTimeScope = value; }
=======
            get
            {
                return mTimeScope;
            }
            set
            {
                mTimeScope = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public IScore localUserScore
        {
<<<<<<< HEAD
            get { return mLocalUserScore; }
=======
            get
            {
                return mLocalUserScore;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public uint maxRange
        {
<<<<<<< HEAD
            get { return mMaxRange; }
=======
            get
            {
                return mMaxRange;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public IScore[] scores
        {
            get
            {
                PlayGamesScore[] arr = new PlayGamesScore[mScoreList.Count];
                mScoreList.CopyTo(arr);
                return arr;
            }
        }

        public string title
        {
<<<<<<< HEAD
            get { return mTitle; }
=======
            get
            {
                return mTitle;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        #endregion

        internal bool SetFromData(LeaderboardScoreData data)
        {
            if (data.Valid)
            {
                Debug.Log("Setting leaderboard from: " + data);
                SetMaxRange(data.ApproximateCount);
                SetTitle(data.Title);
<<<<<<< HEAD
                SetLocalUserScore((PlayGamesScore) data.PlayerScore);
                foreach (IScore score in data.Scores)
                {
                    AddScore((PlayGamesScore) score);
                }

=======
                SetLocalUserScore((PlayGamesScore)data.PlayerScore);
                foreach (IScore score in data.Scores)
                {
                    AddScore((PlayGamesScore)score);
                }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                mLoading = data.Scores.Length == 0 || HasAllScores();
            }

            return data.Valid;
        }

        internal void SetMaxRange(ulong val)
        {
<<<<<<< HEAD
            mMaxRange = (uint) val;
=======
            mMaxRange = (uint)val;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        internal void SetTitle(string value)
        {
            mTitle = value;
        }

        internal void SetLocalUserScore(PlayGamesScore score)
        {
            mLocalUserScore = score;
        }

        internal int AddScore(PlayGamesScore score)
        {
            if (mFilteredUserIds == null || mFilteredUserIds.Length == 0)
            {
                mScoreList.Add(score);
            }
            else
            {
                foreach (string fid in mFilteredUserIds)
                {
                    if (fid.Equals(score.userID))
                    {
                        return mScoreList.Count;
                    }
                }
<<<<<<< HEAD

                mScoreList.Add(score);
            }

=======
                mScoreList.Add(score);
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            return mScoreList.Count;
        }

        public int ScoreCount
        {
<<<<<<< HEAD
            get { return mScoreList.Count; }
=======
            get
            {
                return mScoreList.Count;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        internal bool HasAllScores()
        {
            return mScoreList.Count >= mRange.count || mScoreList.Count >= maxRange;
        }
    }
}
<<<<<<< HEAD
#endif
=======
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
