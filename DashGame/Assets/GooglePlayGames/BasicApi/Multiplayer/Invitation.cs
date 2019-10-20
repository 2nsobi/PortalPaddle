// <copyright file="Invitation.cs" company="Google Inc.">
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

namespace GooglePlayGames.BasicApi.Multiplayer
{
    using System;

=======
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))

namespace GooglePlayGames.BasicApi.Multiplayer
{
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
    /// <summary>
    /// Represents an invitation to a multiplayer game. The invitation may be for
    /// a turn-based or real-time game.
    /// </summary>
    public class Invitation
    {
        public enum InvType
        {
            RealTime,
            TurnBased,
            Unknown
        }

        private InvType mInvitationType;
        private string mInvitationId;
        private Participant mInviter;
        private int mVariant;
<<<<<<< HEAD
        private DateTime mCreationTime;

        internal Invitation(InvType invType, string invId, Participant inviter, int variant, DateTime creationTime)
=======

        internal Invitation(InvType invType, string invId, Participant inviter, int variant)
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        {
            mInvitationType = invType;
            mInvitationId = invId;
            mInviter = inviter;
            mVariant = variant;
<<<<<<< HEAD
            mCreationTime = creationTime;
=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the type of the invitation.
        /// </summary>
        /// <value>The type of the invitation (real-time or turn-based).</value>
        public InvType InvitationType
        {
<<<<<<< HEAD
            get { return mInvitationType; }
=======
            get
            {
                return mInvitationType;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the invitation id.
        /// </summary>
        /// <value>The invitation id.</value>
        public string InvitationId
        {
<<<<<<< HEAD
            get { return mInvitationId; }
=======
            get
            {
                return mInvitationId;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the participant who issued the invitation.
        /// </summary>
        /// <value>The participant who issued the invitation.</value>
        public Participant Inviter
        {
<<<<<<< HEAD
            get { return mInviter; }
=======
            get
            {
                return mInviter;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the match variant. The meaning of this parameter is defined by the game.
        /// It usually indicates a particular game type or mode (for example "capture the flag",
        // "first to 10 points", etc).
        /// </summary>
        /// <value>The match variant. 0 means default (unset).</value>
        public int Variant
        {
<<<<<<< HEAD
            get { return mVariant; }
        }

        /// <summary>
        /// Gets the creation time of the invitation
        /// </summary>
        /// <value>The creation timestamp (UTC)</value>
        public DateTime CreationTime
        {
            get { return mCreationTime; }
=======
            get
            {
                return mVariant;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public override string ToString()
        {
            return string.Format("[Invitation: InvitationType={0}, InvitationId={1}, Inviter={2}, " +
<<<<<<< HEAD
                                 "Variant={3}, CreationTime={4}]", InvitationType, InvitationId, Inviter, Variant,
                CreationTime);
=======
                "Variant={3}]", InvitationType, InvitationId, Inviter, Variant);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }
    }
}
#endif