// <copyright file="TokenClient.cs" company="Google Inc.">
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
//  limitations under the License.
// </copyright>

<<<<<<< HEAD
#if UNITY_ANDROID
=======
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
namespace GooglePlayGames
{
    using System;

    internal interface TokenClient
    {
        /// <summary>
        /// Gets the user's email.
        /// </summary>
        /// <remarks>The email address returned is selected by the user from the accounts present
        /// on the device. There is no guarantee this uniquely identifies the player.
        /// For unique identification use the id property of the local player.
        /// The user can also choose to not select any email address, meaning it is not
        /// available.</remarks>
        /// <returns>The user email or null if not authenticated or the permission is
        /// not available.</returns>
        string GetEmail();
<<<<<<< HEAD

=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        string GetAuthCode();
        string GetIdToken();

        /// <summary>
        /// Gets another server auth code.
        /// </summary>
        /// <remarks>This method should be called after authenticating, and exchanging
        /// the initial server auth code for a token.  This is implemented by signing in
        /// silently, which if successful returns almost immediately and with a new
        /// server auth code.
        /// </remarks>
        /// <param name="reAuthenticateIfNeeded">Calls Authenticate if needed when
        /// retrieving another auth code. </param>
        /// <param name="callback">Callback.</param>
        void GetAnotherServerAuthCode(bool reAuthenticateIfNeeded,
<<<<<<< HEAD
            Action<string> callback);
=======
                                      Action<string> callback);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

        void Signout();

        void SetRequestAuthCode(bool flag, bool forceRefresh);

        void SetRequestEmail(bool flag);

        void SetRequestIdToken(bool flag);

        void SetWebClientId(string webClientId);

        void SetAccountName(string accountName);

<<<<<<< HEAD
        void AddOauthScopes(params string[] scopes);

        void SetHidePopups(bool flag);

        void FetchTokens(bool silent, Action<int> callback);
=======
        void AddOauthScopes(string[] scopes);

        void SetHidePopups(bool flag);

        bool NeedsToRun();

        void FetchTokens(Action<int> callback);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
    }
}
#endif