// <copyright file="PlayGamesScore.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
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
    using System;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Represents a Google Play Games score that can be sent to a leaderboard.
    /// </summary>
    public class PlayGamesScore : IScore
    {
        private string mLbId = null;
        private long mValue = 0;
        private ulong mRank = 0;
        private string mPlayerId = string.Empty;
        private string mMetadata = string.Empty;

        private DateTime mDate = new DateTime(1970, 1, 1, 0, 0, 0);

        internal PlayGamesScore(DateTime date, string leaderboardId,
            ulong rank, string playerId, ulong value, string metadata)
        {
            this.mDate = date;
            mLbId = leaderboardID;
            this.mRank = rank;
            this.mPlayerId = playerId;
<<<<<<< HEAD
            this.mValue = (long) value;
=======
            this.mValue = (long)value;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            this.mMetadata = metadata;
        }

        /// <summary>
        /// Reports the score. Equivalent to <see cref="PlayGamesPlatform.ReportScore" />.
        /// </summary>
        public void ReportScore(Action<bool> callback)
        {
            PlayGamesPlatform.Instance.ReportScore(mValue, mLbId, mMetadata, callback);
        }

        /// <summary>
        /// Gets or sets the leaderboard id.
        /// </summary>
        /// <returns>
        /// The leaderboard id.
        /// </returns>
        public string leaderboardID
        {
<<<<<<< HEAD
            get { return mLbId; }

            set { mLbId = value; }
=======
            get
            {
                return mLbId;
            }

            set
            {
                mLbId = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        /// <returns>
        /// The value.
        /// </returns>
        public long value
        {
<<<<<<< HEAD
            get { return mValue; }

            set { mValue = value; }
=======
            get
            {
                return mValue;
            }

            set
            {
                mValue = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Not implemented. Returns Jan 01, 1970, 00:00:00
        /// </summary>
        public DateTime date
        {
<<<<<<< HEAD
            get { return mDate; }
=======
            get
            {
                return mDate;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Not implemented. Returns the value converted to a string, unformatted.
        /// </summary>
        public string formattedValue
        {
<<<<<<< HEAD
            get { return mValue.ToString(); }
=======
            get
            {
                return mValue.ToString();
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Not implemented. Returns the empty string.
        /// </summary>
        public string userID
        {
<<<<<<< HEAD
            get { return mPlayerId; }
=======
            get
            {
                return mPlayerId;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Not implemented. Returns 1.
        /// </summary>
        public int rank
        {
<<<<<<< HEAD
            get { return (int) mRank; }
=======
            get
            {
                return (int)mRank;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the metaData (scoreTag).
        /// </summary>
        /// <returns>
        /// The metaData.
        /// </returns>
        public string metaData
        {
<<<<<<< HEAD
            get { return mMetadata; }
        }
    }
}
#endif
=======
            get 
            {
                return mMetadata;
            }
        }
    }
}
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
