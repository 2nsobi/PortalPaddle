// <copyright file="ScorePageToken.cs" company="Google Inc.">
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

namespace GooglePlayGames.BasicApi
{
    public enum ScorePageDirection
    {
        Forward = 1,
        Backward = 2,
    }

=======
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))

namespace GooglePlayGames.BasicApi
{
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
    /// <summary>
    /// Score page token. This holds the internal token used
    /// to page through the score pages.  The id, collection, and
    /// timespan are added as a convience, and not actually part of the
    /// page token returned from the SDK.
    /// </summary>
    public class ScorePageToken
    {
<<<<<<< HEAD
=======

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        private string mId;
        private object mInternalObject;
        private LeaderboardCollection mCollection;
        private LeaderboardTimeSpan mTimespan;
<<<<<<< HEAD
        private ScorePageDirection mDirection;

        internal ScorePageToken(object internalObject, string id,
            LeaderboardCollection collection, LeaderboardTimeSpan timespan,
            ScorePageDirection direction)
=======

        internal ScorePageToken(object internalObject, string id,
            LeaderboardCollection collection, LeaderboardTimeSpan timespan)
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        {
            mInternalObject = internalObject;
            mId = id;
            mCollection = collection;
            mTimespan = timespan;
<<<<<<< HEAD
            mDirection = direction;
=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public LeaderboardCollection Collection
        {
<<<<<<< HEAD
            get { return mCollection; }
=======
            get
            {
                return mCollection;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public LeaderboardTimeSpan TimeSpan
        {
<<<<<<< HEAD
            get { return mTimespan; }
        }

        public ScorePageDirection Direction
        {
            get { return mDirection; }
=======
            get
            {
                return mTimespan;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string LeaderboardId
        {
<<<<<<< HEAD
            get { return mId; }
=======
            get
            {
                return mId;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        internal object InternalObject
        {
<<<<<<< HEAD
            get { return mInternalObject; }
        }
    }
}
#endif
=======
            get
            {
                return mInternalObject;
            }
        }
    }
}
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
