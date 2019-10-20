// <copyright file="Participant.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
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

namespace GooglePlayGames.BasicApi.Multiplayer
{
    using System;

    /// <summary>
    /// Represents a participant in a real-time or turn-based match. Note the difference
    /// between a Player and a Participant! A Player is a real-world person with a name
    /// and a Google ID. A Participant is an entity that participates in a real-time
    /// or turn-based match; it may be tied to a Player or not. Particularly, Participant
    /// without Players represent the anonymous participants in an automatch game.
    /// </summary>
    public class Participant : IComparable<Participant>
    {
        public enum ParticipantStatus
        {
            NotInvitedYet,
            Invited,
            Joined,
            Declined,
            Left,
            Finished,
            Unresponsive,
            Unknown
        }

        private string mDisplayName = string.Empty;
        private readonly string mParticipantId = string.Empty;
        private ParticipantStatus mStatus = ParticipantStatus.Unknown;
        private Player mPlayer = null;
        private bool mIsConnectedToRoom = false;

        /// Gets the participant's display name.
        public string DisplayName
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

        /// <summary>
        /// Gets the participant identifier. Important: everyone in a particular match
        /// agrees on what is the participant ID for each participant; however, participant
        /// IDs are not meaningful outside of the particular match where they are used.
        /// If the same user plays two subsequent matches, their Participant Id will likely
        /// be different in the two matches.
        /// </summary>
        /// <value>The participant identifier.</value>
        public string ParticipantId
        {
<<<<<<< HEAD
            get { return mParticipantId; }
=======
            get
            {
                return mParticipantId;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// Gets the participant's status (invited, joined, declined, left, finished, ...)
        public ParticipantStatus Status
        {
<<<<<<< HEAD
            get { return mStatus; }
=======
            get
            {
                return mStatus;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Gets the player that corresponds to this participant. If this is an anonymous
        /// participant, this will be null.
        /// </summary>
        /// <value>The player, or null if this is an anonymous participant.</value>
        public Player Player
        {
<<<<<<< HEAD
            get { return mPlayer; }
=======
            get
            {
                return mPlayer;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// <summary>
        /// Returns whether the participant is connected to the real time room. This has no
        /// meaning in turn-based matches.
        /// </summary>
        public bool IsConnectedToRoom
        {
<<<<<<< HEAD
            get { return mIsConnectedToRoom; }
=======
            get
            {
                return mIsConnectedToRoom;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        /// Returns whether or not this is an automatch participant.
        public bool IsAutomatch
        {
<<<<<<< HEAD
            get { return mPlayer == null; }
        }

        internal Participant(string displayName, string participantId,
            ParticipantStatus status, Player player, bool connectedToRoom)
=======
            get
            {
                return mPlayer == null;
            }
        }

        internal Participant(string displayName, string participantId,
                             ParticipantStatus status, Player player, bool connectedToRoom)
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        {
            mDisplayName = displayName;
            mParticipantId = participantId;
            mStatus = status;
            mPlayer = player;
            mIsConnectedToRoom = connectedToRoom;
        }

        public override string ToString()
        {
            return string.Format("[Participant: '{0}' (id {1}), status={2}, " +
<<<<<<< HEAD
                                 "player={3}, connected={4}]", mDisplayName, mParticipantId, mStatus.ToString(),
=======
                "player={3}, connected={4}]", mDisplayName, mParticipantId, mStatus.ToString(),
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                mPlayer == null ? "NULL" : mPlayer.ToString(), mIsConnectedToRoom);
        }

        public int CompareTo(Participant other)
        {
            return String.Compare(mParticipantId, other.mParticipantId, StringComparison.Ordinal);
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

            if (obj.GetType() != typeof(Participant))
            {
                return false;
            }

<<<<<<< HEAD
            Participant other = (Participant) obj;
=======
            Participant other = (Participant)obj;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            return mParticipantId.Equals(other.mParticipantId);
        }

        public override int GetHashCode()
        {
            return mParticipantId != null ? mParticipantId.GetHashCode() : 0;
        }
    }
}
<<<<<<< HEAD
#endif
=======
#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
