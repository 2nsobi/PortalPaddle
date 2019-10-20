// <copyright file="Achievement.cs" company="Google Inc.">
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

namespace GooglePlayGames.BasicApi
{
    using System;

    /// <summary>Data interface for retrieving achievement information.</summary>
    /// <remarks>
    /// There are 3 states an achievement can be in:
    /// <para>
    ///    Hidden - indicating the name and description of the achievement is
    ///     not visible to the player.
    /// </para><para>
    ///    Revealed - indicating the name and description of the achievement is
    ///     visible to the player.
    ///    Unlocked - indicating the player has unlocked, or achieved, the achievment.
    /// </para><para>
    /// Achievements has two types, standard which is unlocked in one step,
    /// and incremental, which require multiple steps to unlock.
    /// </para>
    /// </remarks>
    public class Achievement
    {
        static readonly DateTime UnixEpoch =
<<<<<<< HEAD
            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
=======
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

        private string mId = string.Empty;
        private bool mIsIncremental = false;
        private bool mIsRevealed = false;
        private bool mIsUnlocked = false;
        private int mCurrentSteps = 0;
        private int mTotalSteps = 0;
        private string mDescription = string.Empty;
        private string mName = string.Empty;
        private long mLastModifiedTime = 0;
        private ulong mPoints;
        private string mRevealedImageUrl;
        private string mUnlockedImageUrl;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GooglePlayGames.BasicApi.Achievement"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GooglePlayGames.BasicApi.Achievement"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                "[Achievement] id={0}, name={1}, desc={2}, type={3}, revealed={4}, unlocked={5}, steps={6}/{7}",
                mId, mName, mDescription, mIsIncremental ? "INCREMENTAL" : "STANDARD",
                mIsRevealed, mIsUnlocked, mCurrentSteps, mTotalSteps);
        }

        public Achievement()
        {
        }

        /// <summary>
        /// Indicates whether this achievement is incremental.
        /// </summary>
        public bool IsIncremental
        {
<<<<<<< HEAD
            get { return mIsIncremental; }

            set { mIsIncremental = value; }
=======
            get
            {
                return mIsIncremental;
            }

            set
            {
                mIsIncremental = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The number of steps the user has gone towards unlocking this achievement.
        /// </summary>
        public int CurrentSteps
        {
<<<<<<< HEAD
            get { return mCurrentSteps; }

            set { mCurrentSteps = value; }
=======
            get
            {
                return mCurrentSteps;
            }

            set
            {
                mCurrentSteps = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The total number of steps needed to unlock this achievement.
        /// </summary>
        public int TotalSteps
        {
<<<<<<< HEAD
            get { return mTotalSteps; }

            set { mTotalSteps = value; }
=======
            get
            {
                return mTotalSteps;
            }

            set
            {
                mTotalSteps = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Indicates whether the achievement is unlocked or not.
        /// </summary>
        public bool IsUnlocked
        {
<<<<<<< HEAD
            get { return mIsUnlocked; }

            set { mIsUnlocked = value; }
=======
            get
            {
                return mIsUnlocked;
            }

            set
            {
               mIsUnlocked = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Indicates whether the achievement is revealed or not (hidden).
        /// </summary>
        public bool IsRevealed
        {
<<<<<<< HEAD
            get { return mIsRevealed; }

            set { mIsRevealed = value; }
=======
            get
            {
                return mIsRevealed;
            }

            set
            {
                mIsRevealed = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The ID string of this achievement.
        /// </summary>
        public string Id
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

        /// <summary>
        /// The description of this achievement.
        /// </summary>
        public string Description
        {
<<<<<<< HEAD
            get { return this.mDescription; }

            set { mDescription = value; }
=======
            get
            {
                return this.mDescription;
            }

            set
            {
                mDescription = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The name of this achievement.
        /// </summary>
        public string Name
        {
<<<<<<< HEAD
            get { return this.mName; }

            set { mName = value; }
=======
            get
            {
                return this.mName;
            }

            set
            {
                mName = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The date and time the state of the achievement was modified.
        /// </summary>
        /// <remarks>
        /// The value is invalid (-1 long) if the achievement state has
        /// never been updated.
        /// </remarks>
        public DateTime LastModifiedTime
        {
<<<<<<< HEAD
            get { return UnixEpoch.AddMilliseconds(mLastModifiedTime); }
=======
            get
            {
                return UnixEpoch.AddMilliseconds(mLastModifiedTime);
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

            set
            {
                TimeSpan ts = value - UnixEpoch;
<<<<<<< HEAD
                mLastModifiedTime = (long) ts.TotalMilliseconds;
=======
                mLastModifiedTime = (long)ts.TotalMilliseconds;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            }
        }

        /// <summary>
        /// The number of experience points earned for unlocking this Achievement.
        /// </summary>
        public ulong Points
        {
<<<<<<< HEAD
            get { return mPoints; }

            set { mPoints = value; }
=======
            get
            {
                return mPoints;
            }

            set
            {
                mPoints = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The URL to the image to display when the achievement is revealed.
        /// </summary>
        public string RevealedImageUrl
        {
<<<<<<< HEAD
            get { return mRevealedImageUrl; }

            set { mRevealedImageUrl = value; }
=======
            get
            {
                return mRevealedImageUrl;
            }

            set
            {
                mRevealedImageUrl = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The URL to the image to display when the achievement is unlocked.
        /// </summary>
        public string UnlockedImageUrl
        {
<<<<<<< HEAD
            get { return mUnlockedImageUrl; }

            set { mUnlockedImageUrl = value; }
        }
    }
}
#endif
=======
            get
            {
                return mUnlockedImageUrl;
            }

            set
            {
                mUnlockedImageUrl = value;
            }
        }
    }
}
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
