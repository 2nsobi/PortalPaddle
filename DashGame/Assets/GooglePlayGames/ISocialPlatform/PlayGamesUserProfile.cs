// <copyright file="PlayGamesUserProfile.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.  All Rights Reserved.
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
    using System.Collections;
    using GooglePlayGames.OurUtils;
    using UnityEngine;
<<<<<<< HEAD
#if UNITY_2017_1_OR_NEWER
    using UnityEngine.Networking;
#endif
=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Represents a Google Play Games user profile. In the current implementation,
    /// this is only used as a base class of <see cref="PlayGamesLocalUser" />
    /// and should not be used directly.
    /// </summary>
    public class PlayGamesUserProfile : IUserProfile
    {
        private string mDisplayName;
        private string mPlayerId;
        private string mAvatarUrl;

        private volatile bool mImageLoading = false;
        private Texture2D mImage;

        internal PlayGamesUserProfile(string displayName, string playerId,
            string avatarUrl)
        {
            mDisplayName = displayName;
            mPlayerId = playerId;
            mAvatarUrl = avatarUrl;
            mImageLoading = false;
        }

        protected void ResetIdentity(string displayName, string playerId,
            string avatarUrl)
        {
            mDisplayName = displayName;
            mPlayerId = playerId;
            if (mAvatarUrl != avatarUrl)
            {
                mImage = null;
                mAvatarUrl = avatarUrl;
            }
<<<<<<< HEAD

=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            mImageLoading = false;
        }

        #region IUserProfile implementation

        public string userName
        {
<<<<<<< HEAD
            get { return mDisplayName; }
=======
            get
            {
                return mDisplayName;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string id
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

        public bool isFriend
        {
<<<<<<< HEAD
            get { return true; }
=======
            get
            {
                return true;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public UserState state
        {
<<<<<<< HEAD
            get { return UserState.Online; }
=======
            get
            {
                return UserState.Online;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public Texture2D image
        {
            get
            {
                if (!mImageLoading && mImage == null && !string.IsNullOrEmpty(AvatarURL))
                {
                    Debug.Log("Starting to load image: " + AvatarURL);
                    mImageLoading = true;
                    PlayGamesHelperObject.RunCoroutine(LoadImage());
                }

                return mImage;
            }
        }

        #endregion

        public string AvatarURL
        {
<<<<<<< HEAD
            get { return mAvatarUrl; }
=======
            get
            {
                return mAvatarUrl;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Loads the local user's image from the url.  Loading urls
        /// is asynchronous so the return from this call is fast,
        /// the image is returned once it is loaded.  null is returned
        /// up to that point.
        /// </summary>
        internal IEnumerator LoadImage()
        {
            // the url can be null if the user does not have an
            // avatar configured.
            if (!string.IsNullOrEmpty(AvatarURL))
            {
<<<<<<< HEAD
#if UNITY_2017_1_OR_NEWER
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(AvatarURL);
                www.SendWebRequest();
#else
                WWW www = new WWW(AvatarURL);
#endif
=======
                WWW www = new WWW(AvatarURL);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                while (!www.isDone)
                {
                    yield return null;
                }

                if (www.error == null)
                {
<<<<<<< HEAD
#if UNITY_2017_1_OR_NEWER
                    this.mImage = DownloadHandlerTexture.GetContent(www);
#else
                    this.mImage = www.texture;
#endif
=======
                    this.mImage = www.texture;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                }
                else
                {
                    mImage = Texture2D.blackTexture;
                    Debug.Log("Error downloading image: " + www.error);
                }

                mImageLoading = false;
            }
            else
            {
                Debug.Log("No URL found.");
                mImage = Texture2D.blackTexture;
                mImageLoading = false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            PlayGamesUserProfile other = obj as PlayGamesUserProfile;
            if (other == null)
            {
                return false;
            }

            return StringComparer.Ordinal.Equals(mPlayerId, other.mPlayerId);
        }

        public override int GetHashCode()
        {
            return typeof(PlayGamesUserProfile).GetHashCode() ^ mPlayerId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Player: '{0}' (id {1})]", mDisplayName, mPlayerId);
        }
    }
}
<<<<<<< HEAD
#endif
=======
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
