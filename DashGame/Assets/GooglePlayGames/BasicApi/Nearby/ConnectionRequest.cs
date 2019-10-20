// <copyright file="ConnectionRequest.cs" company="Google Inc.">
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

    public struct ConnectionRequest
    {
        private readonly EndpointDetails mRemoteEndpoint;
        private readonly byte[] mPayload;

        public ConnectionRequest(string remoteEndpointId,
<<<<<<< HEAD
            string remoteEndpointName, string serviceId, byte[] payload)
=======
                             string remoteEndpointName, string serviceId, byte[] payload)
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        {
            Logger.d("Constructing ConnectionRequest");
            mRemoteEndpoint = new EndpointDetails(remoteEndpointId, remoteEndpointName, serviceId);
            this.mPayload = Misc.CheckNotNull(payload);
        }

        public EndpointDetails RemoteEndpoint
        {
<<<<<<< HEAD
            get { return mRemoteEndpoint; }
=======
            get
            {
                return mRemoteEndpoint;
            }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        public byte[] Payload
        {
<<<<<<< HEAD
            get { return mPayload; }
        }
    }
}
=======
            get
            {
                return mPayload;
            }
        }
    }
}
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
