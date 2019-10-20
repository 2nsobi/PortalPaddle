// <copyright file="SavedGameMetadataUpdate.cs" company="Google Inc.">
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

namespace GooglePlayGames.BasicApi.SavedGame
{
    using System;
    using GooglePlayGames.OurUtils;

    /// <summary>
    /// A struct representing the mutation of saved game metadata. Fields can either have a new value
    /// or be untouched (in which case the corresponding field in the saved game metadata will be
    /// untouched). Instances must be built using <see cref="SavedGameMetadataUpdate.Builder"/>
    /// and once created, these instances are immutable and threadsafe.
    /// </summary>
    public struct SavedGameMetadataUpdate
    {
        private readonly bool mDescriptionUpdated;
        private readonly string mNewDescription;
        private readonly bool mCoverImageUpdated;
        private readonly byte[] mNewPngCoverImage;
        private readonly TimeSpan? mNewPlayedTime;

        private SavedGameMetadataUpdate(Builder builder)
        {
            mDescriptionUpdated = builder.mDescriptionUpdated;
            mNewDescription = builder.mNewDescription;
            mCoverImageUpdated = builder.mCoverImageUpdated;
            mNewPngCoverImage = builder.mNewPngCoverImage;
            mNewPlayedTime = builder.mNewPlayedTime;
        }

        public bool IsDescriptionUpdated
        {
<<<<<<< HEAD
            get { return mDescriptionUpdated; }
=======
            get
            {
                return mDescriptionUpdated;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string UpdatedDescription
        {
<<<<<<< HEAD
            get { return mNewDescription; }
=======
            get
            {
                return mNewDescription;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public bool IsCoverImageUpdated
        {
<<<<<<< HEAD
            get { return mCoverImageUpdated; }
=======
            get
            {
                return mCoverImageUpdated;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public byte[] UpdatedPngCoverImage
        {
<<<<<<< HEAD
            get { return mNewPngCoverImage; }
=======
            get
            {
                return mNewPngCoverImage;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public bool IsPlayedTimeUpdated
        {
<<<<<<< HEAD
            get { return mNewPlayedTime.HasValue; }
=======
            get
            {
                return mNewPlayedTime.HasValue;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public TimeSpan? UpdatedPlayedTime
        {
<<<<<<< HEAD
            get { return mNewPlayedTime; }
=======
            get
            {
                return mNewPlayedTime;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public struct Builder
        {
            internal bool mDescriptionUpdated;
            internal string mNewDescription;
            internal bool mCoverImageUpdated;
            internal byte[] mNewPngCoverImage;
            internal TimeSpan? mNewPlayedTime;

            public Builder WithUpdatedDescription(string description)
            {
                mNewDescription = Misc.CheckNotNull(description);
                mDescriptionUpdated = true;
                return this;
            }

            public Builder WithUpdatedPngCoverImage(byte[] newPngCoverImage)
            {
                mCoverImageUpdated = true;
                mNewPngCoverImage = newPngCoverImage;
                return this;
            }

            public Builder WithUpdatedPlayedTime(TimeSpan newPlayedTime)
            {
                if (newPlayedTime.TotalMilliseconds > ulong.MaxValue)
                {
                    throw new InvalidOperationException("Timespans longer than ulong.MaxValue " +
<<<<<<< HEAD
                                                        "milliseconds are not allowed");
=======
                        "milliseconds are not allowed");
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                }

                mNewPlayedTime = newPlayedTime;
                return this;
            }

            public SavedGameMetadataUpdate Build()
            {
                return new SavedGameMetadataUpdate(this);
            }
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
