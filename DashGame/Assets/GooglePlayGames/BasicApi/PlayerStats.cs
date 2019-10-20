// <copyright file="PlayerStats.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc.
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

    /// <summary>
    /// Player stats. See https://developers.google.com/games/services/android/stats
    /// </summary>
    public class PlayerStats
    {
        private static float UNSET_VALUE = -1.0f;

<<<<<<< HEAD
        public PlayerStats(
            int numberOfPurchases,
            float avgSessionLength,
            int daysSinceLastPlayed,
            int numberOfSessions,
            float sessPercentile,
            float spendPercentile,
            float spendProbability,
            float churnProbability,
            float highSpenderProbability,
            float totalSpendNext28Days)
        {
            mValid = true;
            mNumberOfPurchases = numberOfPurchases;
            mAvgSessionLength = avgSessionLength;
            mDaysSinceLastPlayed = daysSinceLastPlayed;
            mNumberOfSessions = numberOfSessions;
            mSessPercentile = sessPercentile;
            mSpendPercentile = spendPercentile;
            mSpendProbability = spendProbability;
            mChurnProbability = churnProbability;
            mHighSpenderProbability = highSpenderProbability;
            mTotalSpendNext28Days = totalSpendNext28Days;
        }

        public PlayerStats()
        {
            mValid = false;
        }

        private bool mValid;
        private int mNumberOfPurchases;
        private float mAvgSessionLength;
        private int mDaysSinceLastPlayed;
        private int mNumberOfSessions;
        private float mSessPercentile;
        private float mSpendPercentile;
        private float mSpendProbability;
        private float mChurnProbability;
        private float mHighSpenderProbability;
        private float mTotalSpendNext28Days;

=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        /// <summary>
        /// If this PlayerStats object is valid (i.e. successfully retrieved from games services).
        /// </summary>
        /// <remarks>
        /// Note that a PlayerStats with all stats unset may still be valid.
        /// </remarks>
        public bool Valid
        {
<<<<<<< HEAD
            get { return mValid; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The number of in-app purchases.
        /// </summary>
        public int NumberOfPurchases
        {
<<<<<<< HEAD
            get { return mNumberOfPurchases; }
        }

        /// <summary>
        /// The length of the avg session in minutes.
        /// </summary>
        public float AvgSessionLength
        {
            get { return mAvgSessionLength; }
=======
            get;
            set;
        }

        /// <summary>
        /// The length of the avg sesson in minutes.
        /// </summary>
        public float AvgSessonLength
        {
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The days since last played.
        /// </summary>
        public int DaysSinceLastPlayed
        {
<<<<<<< HEAD
            get { return mDaysSinceLastPlayed; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The number of sessions based on sign-ins.
        /// </summary>
        public int NumberOfSessions
        {
<<<<<<< HEAD
            get { return mNumberOfSessions; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The approximation of sessions percentile for the player.
        /// </summary>
        /// <remarks>
        /// This value is given as a decimal value between 0 and 1 (inclusive).
        /// It indicates how many sessions the current player has
        /// played in comparison to the rest of this game's player base.
        /// Higher numbers indicate that this player has played more sessions.
        /// A return value less than zero indicates this value is not available.
        /// </remarks>
        public float SessPercentile
        {
<<<<<<< HEAD
            get { return mSessPercentile; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The approximate spend percentile of the player.
        /// </summary>
        /// <remarks>
        /// This value is given as a decimal value between 0 and 1 (inclusive).
        /// It indicates how much the current player has spent in
        /// comparison to the rest of this game's player base. Higher
        /// numbers indicate that this player has spent more.
        /// A return value less than zero indicates this value is not available.
        /// </remarks>
        public float SpendPercentile
        {
<<<<<<< HEAD
            get { return mSpendPercentile; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The approximate probability of the player choosing to spend in this game.
        /// </summary>
        /// <remarks>
        /// This value is given as a decimal value between 0 and 1 (inclusive).
        ///  Higher values indicate that a player is more likely to spend.
        /// A return value less than zero indicates this value is not available.
        /// </remarks>
        public float SpendProbability
        {
<<<<<<< HEAD
            get { return mSpendProbability; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The approximate probability of the player not returning to play the game.
        /// </summary>
        /// <remarks>
        /// Higher values indicate that a player is less likely to return.
        /// A return value less than zero indicates this value is not available.
        /// </remarks>
        public float ChurnProbability
        {
<<<<<<< HEAD
            get { return mChurnProbability; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The high spender probability of this player.
        /// </summary>
        public float HighSpenderProbability
        {
<<<<<<< HEAD
            get { return mHighSpenderProbability; }
=======
            get;
            set;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// The predicted total spend of this player over the next 28 days.
        /// </summary>
        public float TotalSpendNext28Days
        {
<<<<<<< HEAD
            get { return mTotalSpendNext28Days; }
=======
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GooglePlayGames.BasicApi.PlayerStats"/> class.
        /// Sets all values to -1.
        /// </summary>
        public PlayerStats() {
            Valid = false;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Determines whether this instance has NumberOfPurchases.
        /// </summary>
        /// <returns><c>true</c> if this instance has NumberOfPurchases; otherwise, <c>false</c>.</returns>
        public bool HasNumberOfPurchases()
        {
<<<<<<< HEAD
            return NumberOfPurchases != (int) UNSET_VALUE;
        }

        /// <summary>
        /// Determines whether this instance has AvgSessionLength.
        /// </summary>
        /// <returns><c>true</c> if this instance has AvgSessionLength; otherwise, <c>false</c>.</returns>
        public bool HasAvgSessionLength()
        {
            return AvgSessionLength != UNSET_VALUE;
=======
            return NumberOfPurchases != (int)UNSET_VALUE;
        }

        /// <summary>
        /// Determines whether this instance has AvgSessonLength.
        /// </summary>
        /// <returns><c>true</c> if this instance has AvgSessonLength; otherwise, <c>false</c>.</returns>
        public bool HasAvgSessonLength()
        {
            return AvgSessonLength != UNSET_VALUE;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Determines whether this instance has DaysSinceLastPlayed.
        /// </summary>
        /// <returns><c>true</c> if this instance has DaysSinceLastPlayed; otherwise, <c>false</c>.</returns>
        public bool HasDaysSinceLastPlayed()
        {
<<<<<<< HEAD
            return DaysSinceLastPlayed != (int) UNSET_VALUE;
=======
            return DaysSinceLastPlayed != (int)UNSET_VALUE;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Determines whether this instance has NumberOfSessions.
        /// </summary>
        /// <returns><c>true</c> if this instance has NumberOfSessions; otherwise, <c>false</c>.</returns>
        public bool HasNumberOfSessions()
        {
<<<<<<< HEAD
            return NumberOfSessions != (int) UNSET_VALUE;
=======
            return NumberOfSessions != (int)UNSET_VALUE;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Determines whether this instance has SessPercentile.
        /// </summary>
        /// <returns><c>true</c> if this instance has SessPercentile; otherwise, <c>false</c>.</returns>
        public bool HasSessPercentile()
        {
            return SessPercentile != UNSET_VALUE;
        }

        /// <summary>
        /// Determines whether this instance has SpendPercentile.
        /// </summary>
        /// <returns><c>true</c> if this instance has SpendPercentile; otherwise, <c>false</c>.</returns>
        public bool HasSpendPercentile()
        {
            return SpendPercentile != UNSET_VALUE;
        }

        /// <summary>
        /// Determines whether this instance has ChurnProbability.
        /// </summary>
        /// <returns><c>true</c> if this instance has ChurnProbability; otherwise, <c>false</c>.</returns>
        public bool HasChurnProbability()
        {
            return ChurnProbability != UNSET_VALUE;
        }

        /// <summary>
        /// Determines whether this instance has HighSpenderProbability.
        /// </summary>
        /// <returns><c>true</c> if this instance has HighSpenderProbability; otherwise, <c>false</c>.</returns>
        public bool HasHighSpenderProbability()
        {
            return HighSpenderProbability != UNSET_VALUE;
        }

        /// <summary>
        /// Determines whether this instance has TotalSpendNext28Days.
        /// </summary>
        /// <returns><c>true</c> if this instance has TotalSpendNext28Days; otherwise, <c>false</c>.</returns>
        public bool HasTotalSpendNext28Days()
        {
            return TotalSpendNext28Days != UNSET_VALUE;
        }
<<<<<<< HEAD
    }
}
#endif
=======

    }
}
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
