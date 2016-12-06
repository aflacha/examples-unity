//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudAsyncMatch
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudAsyncMatch(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Creates an instance of an asynchronous match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Create
        /// </remarks>
        /// <param name="in_jsonOpponentIds">
        /// JSON string identifying the opponent platform and id for this match.
        ///
        /// Platforms are identified as:
        /// BC - a brainCloud profile id
        /// FB - a Facebook id
        ///
        /// An exmaple of this string would be:
        /// [
        ///     {
        ///         "platform": "BC",
        ///         "id": "some-braincloud-profile"
        ///     },
        ///     {
        ///         "platform": "FB",
        ///         "id": "some-facebook-id"
        ///     }
        /// ]
        /// </param>
        /// <param name="in_pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void CreateMatch(
            string in_jsonOpponentIds,
            string in_pushNotificationMessage,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            CreateMatchInternal(in_jsonOpponentIds, null, in_pushNotificationMessage, null, null, null, in_success, in_failure, in_cbObject);
        }

        /// <summary>
        /// Creates an instance of an asynchronous match with an initial turn.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Create
        /// </remarks>
        /// <param name="in_jsonOpponentIds">
        /// JSON string identifying the opponent platform and id for this match.
        ///
        /// Platforms are identified as:
        /// BC - a brainCloud profile id
        /// FB - a Facebook id
        ///
        /// An exmaple of this string would be:
        /// [
        ///     {
        ///         "platform": "BC",
        ///         "id": "some-braincloud-profile"
        ///     },
        ///     {
        ///         "platform": "FB",
        ///         "id": "some-facebook-id"
        ///     }
        /// ]
        /// </param>
        /// <param name="in_jsonMatchState">
        /// JSON string blob provided by the caller
        /// </param>
        /// <param name="in_pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
        /// </param>
        /// <param name="in_nextPlayer">
        /// Optionally, force the next player player to be a specific player
        /// </param>
        /// <param name="in_jsonSummary">
        /// Optional JSON string defining what the other player will see as a summary of the game when listing their games
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void CreateMatchWithInitialTurn(
            string in_jsonOpponentIds,
            string in_jsonMatchState,
            string in_pushNotificationMessage,
            string in_nextPlayer,
            string in_jsonSummary,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            CreateMatchInternal(
                in_jsonOpponentIds,
                in_jsonMatchState == null ? "{}" : in_jsonMatchState,
                in_pushNotificationMessage,
                null,
                in_nextPlayer,
                in_jsonSummary,
                in_success, in_failure, in_cbObject);
        }

        /// <summary>
        /// Submits a turn for the given match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - SubmitTurn
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identfier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="version">
        /// Game state version to ensure turns are submitted once and in order
        /// </param>
        /// <param name="jsonMatchState">
        /// JSON string blob provided by the caller
        /// </param>
        /// <param name="pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
        /// </param>
        /// <param name="nextPlayer">
        /// Optionally, force the next player player to be a specific player
        /// </param>
        /// <param name="jsonSummary">
        /// Optional JSON string that other players will see as a summary of the game when listing their games
        /// </param>
        /// <param name="jsonStatistics">
        /// Optional JSON string blob provided by the caller
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void SubmitTurn(
            string in_ownerId,
            string in_matchId,
            UInt64 in_version,
            string in_jsonMatchState,
            string in_pushNotificationMessage,
            string in_nextPlayer,
            string in_jsonSummary,
            string in_jsonStatistics,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;
            data["version"] = in_version;
            data["matchState"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonMatchState);

            if (Util.IsOptionalParameterValid(in_nextPlayer))
            {
                Dictionary<string, object> status = new Dictionary<string, object>();
                status["currentPlayer"] = in_nextPlayer;
                data["status"] = status;
            }

            if (Util.IsOptionalParameterValid(in_jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonSummary);
            }

            if (Util.IsOptionalParameterValid(in_jsonStatistics))
            {
                data["statistics"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonStatistics);
            }

            if (Util.IsOptionalParameterValid(in_pushNotificationMessage))
            {
                data["pushContent"] = in_pushNotificationMessage;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.SubmitTurn, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Allows the current player (only) to update Summary data without having to submit a whole turn.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - UpdateMatchSummary
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identfier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="version">
        /// Game state version to ensure turns are submitted once and in order
        /// </param>
        /// <param name="jsonSummary">
        /// JSON string provided by the caller that other players will see as a summary of the game when listing their games
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void UpdateMatchSummaryData(
            string in_ownerId,
            string in_matchId,
            UInt64 in_version,
            string in_jsonSummary,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;
            data["version"] = in_version;

            if (Util.IsOptionalParameterValid(in_jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonSummary);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.UpdateMatchSummary, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Marks the given match as complete.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Complete
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void CompleteMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Complete, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns the current state of the given match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - ReadMatch
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void ReadMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.ReadMatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns the match history of the given match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - ReadMatchHistory
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void ReadMatchHistory(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.ReadMatchHistory, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns all matches that are NOT in a COMPLETE state for which the player is involved.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - FindMatches
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void FindMatches(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.FindMatches, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns all matches that are in a COMPLETE state for which the player is involved.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - FindMatchesCompleted
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.e is received.
        /// </param>
        public void FindCompleteMatches(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.FindMatchesCompleted, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Marks the given match as abandoned.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Abandon
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AbandonMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Abandon, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Removes the match and match history from the server. DEBUG ONLY, in production it is recommended
        /// the user leave it as completed.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DeleteMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.DeleteMatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void CreateMatchInternal(
            string in_jsonOpponentIds,
            string in_jsonMatchState,
            string in_pushNotificationMessage,
            string in_matchId,
            string in_nextPlayer,
            string in_jsonSummary,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["players"] = JsonReader.Deserialize<object[]>(in_jsonOpponentIds);

            if (Util.IsOptionalParameterValid(in_jsonMatchState))
            {
                data["matchState"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonMatchState);
            }

            if (Util.IsOptionalParameterValid(in_matchId))
            {
                data["matchId"] = in_matchId;
            }

            if (Util.IsOptionalParameterValid(in_nextPlayer))
            {
                Dictionary<string, object> status = new Dictionary<string, object>();
                status["currentPlayer"] = in_nextPlayer;
                data["status"] = status;
            }

            if (Util.IsOptionalParameterValid(in_jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonSummary);
            }

            if (Util.IsOptionalParameterValid(in_pushNotificationMessage))
            {
                data["pushContent"] = in_pushNotificationMessage;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Create, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
