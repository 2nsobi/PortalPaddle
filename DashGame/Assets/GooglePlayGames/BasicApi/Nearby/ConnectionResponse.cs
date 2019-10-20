// <copyright file="ConnectionResponse.cs" company="Google Inc.">
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

namespace GooglePlayGames.BasicApi.Nearby
{
    using GooglePlayGames.OurUtils;

    public struct ConnectionResponse
    {
        private static readonly byte[] EmptyPayload = new byte[0];

        public enum Status
        {
            Accepted,
            Rejected,
            ErrorInternal,
            ErrorNetworkNotConnected,
            ErrorEndpointNotConnected,
            ErrorAlreadyConnected
        }

        private readonly long mLocalClientId;
        private readonly string mRemoteEndpointId;
        private readonly Status mResponseStatus;
        private readonly byte[] mPayload;

        private ConnectionResponse(long localClientId, string remoteEndpointId, Status code,
<<<<<<< HEAD
            byte[] payload)
=======
                                   byte[] payload)
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        {
            this.mLocalClientId = localClientId;
            this.mRemoteEndpointId = Misc.CheckNotNull(remoteEndpointId);
            this.mResponseStatus = code;
            this.mPayload = Misc.CheckNotNull(payload);
        }

        public long LocalClientId
        {
<<<<<<< HEAD
            get { return mLocalClientId; }
=======
            get
            {
                return mLocalClientId;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public string RemoteEndpointId
        {
<<<<<<< HEAD
            get { return mRemoteEndpointId; }
=======
            get
            {
                return mRemoteEndpointId;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public Status ResponseStatus
        {
<<<<<<< HEAD
            get { return mResponseStatus; }
=======
            get
            {
                return mResponseStatus;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public byte[] Payload
        {
<<<<<<< HEAD
            get { return mPayload; }
=======
            get
            {
                return mPayload;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public static ConnectionResponse Rejected(long localClientId, string remoteEndpointId)
        {
            return new ConnectionResponse(localClientId, remoteEndpointId, Status.Rejected,
                EmptyPayload);
        }

        public static ConnectionResponse NetworkNotConnected(long localClientId, string remoteEndpointId)
        {
            return new ConnectionResponse(localClientId, remoteEndpointId, Status.ErrorNetworkNotConnected,
                EmptyPayload);
        }

        public static ConnectionResponse InternalError(long localClientId, string remoteEndpointId)
        {
            return new ConnectionResponse(localClientId, remoteEndpointId, Status.ErrorInternal,
                EmptyPayload);
        }

        public static ConnectionResponse EndpointNotConnected(long localClientId, string remoteEndpointId)
        {
            return new ConnectionResponse(localClientId, remoteEndpointId, Status.ErrorEndpointNotConnected,
                EmptyPayload);
        }

        public static ConnectionResponse Accepted(long localClientId, string remoteEndpointId,
<<<<<<< HEAD
            byte[] payload)
=======
                                                  byte[] payload)
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        {
            return new ConnectionResponse(localClientId, remoteEndpointId, Status.Accepted,
                payload);
        }

        public static ConnectionResponse AlreadyConnected(long localClientId,
<<<<<<< HEAD
            string remoteEndpointId)
=======
                                                          string remoteEndpointId)
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        {
            return new ConnectionResponse(localClientId, remoteEndpointId,
                Status.ErrorAlreadyConnected,
                EmptyPayload);
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
