// <copyright file="PlayGamesAchievement.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc. All Rights Reserved.
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
    using GooglePlayGames.BasicApi;
    using UnityEngine;
<<<<<<< HEAD
#if UNITY_2017_1_OR_NEWER
    using UnityEngine.Networking;
#endif
=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
    using UnityEngine.SocialPlatforms;

    internal delegate void ReportProgress(string id, double progress, Action<bool> callback);

    /// <summary>
    /// Represents a Google Play Games achievement. It can be used to report an achievement
    /// to the API, offering identical functionality as <see cref="PlayGamesPlatform.ReportProgress" />.
    /// </summary>
    internal class PlayGamesAchievement : IAchievement, IAchievementDescription
    {
        private readonly ReportProgress mProgressCallback;
        private string mId = string.Empty;
        private bool mIsIncremental = false;
        private int mCurrentSteps = 0;
        private int mTotalSteps = 0;
        private double mPercentComplete = 0.0;
        private bool mCompleted = false;
        private bool mHidden = false;
<<<<<<< HEAD
        private DateTime mLastModifiedTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private string mTitle = string.Empty;
        private string mRevealedImageUrl = string.Empty;
        private string mUnlockedImageUrl = string.Empty;
#if UNITY_2017_1_OR_NEWER
        private UnityWebRequest mImageFetcher = null;
#else
        private WWW mImageFetcher = null;
#endif
=======
        private DateTime mLastModifiedTime = new DateTime (1970, 1, 1, 0, 0, 0, 0);
        private string mTitle = string.Empty;
        private string mRevealedImageUrl = string.Empty;
        private string mUnlockedImageUrl = string.Empty;
        private WWW mImageFetcher = null;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        private Texture2D mImage = null;
        private string mDescription = string.Empty;
        private ulong mPoints = 0;

        internal PlayGamesAchievement()
            : this(PlayGamesPlatform.Instance.ReportProgress)
        {
        }

        internal PlayGamesAchievement(ReportProgress progressCallback)
        {
            mProgressCallback = progressCallback;
        }

        internal PlayGamesAchievement(Achievement ach) : this()
        {
            this.mId = ach.Id;
            this.mIsIncremental = ach.IsIncremental;
            this.mCurrentSteps = ach.CurrentSteps;
            this.mTotalSteps = ach.TotalSteps;
            if (ach.IsIncremental)
            {
                if (ach.TotalSteps > 0)
                {
                    this.mPercentComplete =
<<<<<<< HEAD
                        ((double) ach.CurrentSteps / (double) ach.TotalSteps) * 100.0;
=======
                        ((double)ach.CurrentSteps / (double)ach.TotalSteps) * 100.0;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                }
                else
                {
                    this.mPercentComplete = 0.0;
                }
            }
            else
            {
                this.mPercentComplete = ach.IsUnlocked ? 100.0 : 0.0;
            }
<<<<<<< HEAD

=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            this.mCompleted = ach.IsUnlocked;
            this.mHidden = !ach.IsRevealed;
            this.mLastModifiedTime = ach.LastModifiedTime;
            this.mTitle = ach.Name;
            this.mDescription = ach.Description;
            this.mPoints = ach.Points;
            this.mRevealedImageUrl = ach.RevealedImageUrl;
            this.mUnlockedImageUrl = ach.UnlockedImageUrl;
<<<<<<< HEAD
=======

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Reveals, unlocks or increment achievement.
        /// </summary>
        /// <remarks>
        /// Call after setting <see cref="id" />, <see cref="completed" />,
        /// as well as <see cref="currentSteps" /> and <see cref="totalSteps" />
        /// for incremental achievements. Equivalent to calling
        /// <see cref="PlayGamesPlatform.ReportProgress" />.
        /// </remarks>
        public void ReportProgress(Action<bool> callback)
        {
            mProgressCallback.Invoke(mId, mPercentComplete, callback);
        }

        /// <summary>
        /// Loads the local user's image from the url.  Loading urls
        /// is asynchronous so the return from this call is fast,
        /// the image is returned once it is loaded.  null is returned
        /// up to that point.
        /// </summary>
        private Texture2D LoadImage()
        {
<<<<<<< HEAD
            if (hidden)
            {
                // return null, we dont have images for hidden achievements.
                return null;
            }

=======
            if (hidden) {
                // return null, we dont have images for hidden achievements.
                return null;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            string url = completed ? mUnlockedImageUrl : mRevealedImageUrl;

            // the url can be null if the image is not configured.
            if (!string.IsNullOrEmpty(url))
            {
                if (mImageFetcher == null || mImageFetcher.url != url)
                {
<<<<<<< HEAD
#if UNITY_2017_1_OR_NEWER
                    mImageFetcher = UnityWebRequestTexture.GetTexture(url);
#else
                    mImageFetcher = new WWW(url);
#endif
=======
                    mImageFetcher = new WWW(url);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                    mImage = null;
                }

                // if we have the texture, just return, this avoids excessive
                // memory usage calling www.texture repeatedly.
                if (mImage != null)
                {
                    return mImage;
                }

                if (mImageFetcher.isDone)
                {
<<<<<<< HEAD
#if UNITY_2017_1_OR_NEWER
                    mImage = DownloadHandlerTexture.GetContent(mImageFetcher);
#else
                    mImage = mImageFetcher.texture;
#endif
=======
                    mImage = mImageFetcher.texture;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                    return mImage;
                }
            }

            // if there is no url, always return null.
            return null;
        }


        /// <summary>
        /// Gets or sets the id of this achievement.
        /// </summary>
        /// <returns>
        /// The identifier.
        /// </returns>
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

        /// <summary>
        /// Gets a value indicating whether this achievement is incremental.
        /// </summary>
        /// <remarks>
        /// This value is only set by PlayGamesPlatform.LoadAchievements
        /// </remarks>
        /// <returns><c>true</c> if incremental; otherwise, <c>false</c>.</returns>
        public bool isIncremental
        {
<<<<<<< HEAD
            get { return mIsIncremental; }
=======
            get
            {
                return mIsIncremental;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the current steps completed of this achievement.
        /// </summary>
        /// <remarks>
        /// Undefined for standard (i.e. non-incremental) achievements.
        /// This value is only set by PlayGamesPlatform.LoadAchievements, changing the
        /// percentComplete will not affect this.
        /// </remarks>
        /// <returns>The current steps.</returns>
        public int currentSteps
        {
<<<<<<< HEAD
            get { return mCurrentSteps; }
=======
            get
            {
                return mCurrentSteps;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the total steps of this achievement.
        /// </summary>
        /// <remarks>
        /// Undefined for standard (i.e. non-incremental) achievements.
        /// This value is only set by PlayGamesPlatform.LoadAchievements, changing the
        /// percentComplete will not affect this.
        /// </remarks>
        /// <returns>The total steps.</returns>
        public int totalSteps
        {
<<<<<<< HEAD
            get { return mTotalSteps; }
=======
            get
            {
                return mTotalSteps;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets or sets the percent completed.
        /// </summary>
        /// <returns>
        /// The percent completed.
        /// </returns>
        public double percentCompleted
        {
<<<<<<< HEAD
            get { return mPercentComplete; }

            set { mPercentComplete = value; }
=======
            get
            {
                return mPercentComplete;
            }

            set
            {
                mPercentComplete = value;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets a value indicating whether this achievement is completed.
        /// </summary>
        /// <remarks>
        /// This value is only set by PlayGamesPlatform.LoadAchievements, changing the
        /// percentComplete will not affect this.
        /// </remarks>
        /// <returns><c>true</c> if completed; otherwise, <c>false</c>.</returns>
        public bool completed
        {
<<<<<<< HEAD
            get { return this.mCompleted; }
=======
            get
            {
                return this.mCompleted;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets a value indicating whether this achievement is hidden.
        /// </summary>
        /// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
        public bool hidden
        {
<<<<<<< HEAD
            get { return this.mHidden; }
=======
            get
            {
                return this.mHidden;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public DateTime lastReportedDate
        {
<<<<<<< HEAD
            get { return mLastModifiedTime; }
=======
            get
            {
                return mLastModifiedTime;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public String title
        {
<<<<<<< HEAD
            get { return mTitle; }
        }

        public Texture2D image
        {
            get { return LoadImage(); }
=======
            get
            {
                return mTitle;
            }
        }
        public Texture2D image
        {
            get
            {
                return LoadImage();
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string achievedDescription
        {
<<<<<<< HEAD
            get { return mDescription; }
=======
            get
            {
                return mDescription;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string unachievedDescription
        {
<<<<<<< HEAD
            get { return mDescription; }
=======
            get
            {
                return mDescription;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public int points
        {
<<<<<<< HEAD
            get { return (int) mPoints; }
        }
    }
}
#endif
=======
            get
            {
                return (int) mPoints;
            }
        }

    }
}
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
