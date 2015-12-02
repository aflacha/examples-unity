//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudIdentity
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudIdentity(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Attach the user's Facebook credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="externalId">
        /// The facebook id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Facebook SDK
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Facebook identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call AuthenticateFacebook().
        /// </returns>
        public void AttachFacebookIdentity(
            string in_externalId,
            string in_authenticationToken,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.AttachIdentity(in_externalId, in_authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided Facebook credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="externalId">
        /// The facebook id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Facebook SDK
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeFacebookIdentity(
            string in_externalId,
            string in_authenticationToken,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.MergeIdentity(in_externalId, in_authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, in_success, in_failure);
        }

        /// <summary>
        /// Detach the Facebook identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="externalId">
        /// The facebook id of the user
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachFacebookIdentity(
            string in_externalId,
            bool in_continueAnon,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.DetachIdentity(in_externalId, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Game Center identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Facebook identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call this method again.
        /// </returns>
        public void AttachGameCenterIdentity(
            string in_gameCenterId,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.AttachIdentity(in_gameCenterId, "", OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, in_success, in_failure);
        }

        /// <summary>Merge the profile associated with the specified Game Center identity with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeGameCenterIdentity(
            string in_gameCenterId,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.MergeIdentity(in_gameCenterId, "", OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, in_success, in_failure);
        }

        /// <summary>Detach the Game Center identity from the current profile.</summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachGameCenterIdentity(string in_gameCenterId,
            bool in_continueAnon,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.DetachIdentity(in_gameCenterId, OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Email and Password identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_email">
        /// The player's e-mail address
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateEmailPassword().
        /// </returns>
        public void AttachEmailIdentity(
            string in_email,
            string in_password,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.AttachIdentity(in_email, in_password, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, in_success, in_failure);
        }

        /// <summary>
        // Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_email">
        /// The player's e-mail address
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeEmailIdentity(
            string in_email,
            string in_password,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.MergeIdentity(in_email, in_password, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, in_success, in_failure);
        }

        /// <summary>Detach the e-mail identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_email">
        /// The player's e-mail address
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachEmailIdentity(
            string in_email,
            bool in_continueAnon,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.DetachIdentity(in_email, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Universal (userid + password) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_userid">
        /// The player's userid
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateEmailPassword().
        /// </returns>
        public void AttachUniversalIdentity(string in_userid,
            string in_password,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.AttachIdentity(in_userid, in_password, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_userid">
        /// The player's userid
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeUniversalIdentity(
            string in_userid,
            string in_password,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.MergeIdentity(in_userid, in_password, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, in_success, in_failure);
        }

        /// <summary>Detach the universal identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_userid">
        /// The player's userid
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachUniversalIdentity(
            string in_userid,
            bool in_continueAnon,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.DetachIdentity(in_userid, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Steam (userid + steamsessionticket) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="in_sessionticket">
        /// The player's session ticket (hex encoded)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateSteam().
        /// </returns>
        public void AttachSteamIdentity(string in_userid,
            string in_sessionticket,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.AttachIdentity(in_userid, in_sessionticket, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided steam userid with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="in_sessionticket">
        /// The player's session ticket (hex encoded)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeSteamIdentity(
            string in_userid,
            string in_sessionticket,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.MergeIdentity(in_userid, in_sessionticket, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, in_success, in_failure);
        }

        /// <summary>Detach the steam identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachSteamIdentity(
            string in_userid,
            bool in_continueAnon,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.DetachIdentity(in_userid, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach the user's Google credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="externalId">
        /// The google id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Google SDK
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Google identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call AuthenticateGoogle().
        /// </returns>
        public void AttachGoogleIdentity(
            string in_externalId,
            string in_authenticationToken,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.AttachIdentity(in_externalId, in_authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthGoogle.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided Google credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="externalId">
        /// The Google id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Google SDK
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeGoogleIdentity(
            string in_externalId,
            string in_authenticationToken,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.MergeIdentity(in_externalId, in_authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthGoogle.Value, in_success, in_failure);
        }

        /// <summary>
        /// Detach the Google identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="externalId">
        /// The Google id of the user
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachGoogleIdentity(
            string in_externalId,
            bool in_continueAnon,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            this.DetachIdentity(in_externalId, OperationParam.AuthenticateServiceAuthenticateAuthGoogle.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach the user's Twitter credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_twitterUserId">
        /// String representation of a Twitter user ID
        /// </param>
        /// <param name="in_authenticationToken">
        /// The authentication token derived via the Twitter apis
        /// </param>
        /// <param name="in_secret">
        /// The secret given when attempting to link with Twitter
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Twitter identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call AuthenticateTwitter().
        /// </returns>
        public void AttachTwitterIdentity(
            string in_twitterUserId,
            string in_authenticationToken,
            string in_secret,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            AttachIdentity(in_twitterUserId, in_authenticationToken + ":" + in_secret, OperationParam.AuthenticateServiceAuthenticateAuthTwitter.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided Twitter credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_twitterUserId">
        /// String representation of a Twitter user ID
        /// </param>
        /// <param name="in_authenticationToken">
        /// The authentication token derived via the Twitter apis
        /// </param>
        /// <param name="in_secret">
        /// The secret given when attempting to link with Twitter
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeTwitterIdentity(
            string in_twitterUserId,
            string in_authenticationToken,
            string in_secret,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            MergeIdentity(in_twitterUserId, in_authenticationToken + ":" + in_secret, OperationParam.AuthenticateServiceAuthenticateAuthTwitter.Value, in_success, in_failure);
        }

        /// <summary>
        /// Detach the Twitter identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_twitterUserId">
        /// The Twitter id of the user
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachTwitterIdentity(
            string in_twitterUserId,
            bool in_continueAnon,
            SuccessCallback in_success,
            FailureCallback in_failure)
        {
            DetachIdentity(in_twitterUserId, OperationParam.AuthenticateServiceAuthenticateAuthTwitter.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Switch to a Child Profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - SWITCH_TO_CHILD_PROFILE
        /// </remarks>
        /// <param name="in_childProfileId">
        /// The profileId of the child profile to switch to
        /// If null and forceCreate is true a new profile will be created
        /// </param>
        /// <param name="in_childGameId">
        /// The appId of the child game to switch to
        /// </param>
        /// <param name="in_forceCreate">
        /// Should a new profile be created if it does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "vcPurchased": 0,
        ///         "experiencePoints": 0,
        ///         "xpCapped": false,
        ///         "playerName": "TestUser",
        ///         "vcClaimed": 0,
        ///         "rewards": {
        ///             "rewardDetails": {},
        ///             "rewards": {},
        ///             "currency": {
        ///                 "credits": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 },
        ///                 "gold": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 }
        ///             }
        ///         },
        ///         "loginCount": 1,
        ///         "server_time": 1441901094386,
        ///         "experienceLevel": 0,
        ///         "currency": {},
        ///         "statistics": {},
        ///         "id": "a17b347b-695b-431f-b1e7-5f783a562310",
        ///         "profileId": "a17t347b-692b-43ef-b1e7-5f783a566310",
        ///         "newUser": false
        ///     }
        /// }
        /// </returns>
        public void SwitchToChildProfile(
            string in_childProfileId,
            string in_childGameId, 
            bool in_forceCreate, 
            SuccessCallback in_success, 
            FailureCallback in_failure)
        {
            SwitchToChildProfile(in_childProfileId, in_childGameId, in_forceCreate, false, in_success, in_failure);
        }

        /// <summary>
        /// Switches to the child profile of an app when only one profile exists
        /// If multiple profiles exist this returns an error
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - SWITCH_TO_CHILD_PROFILE
        /// </remarks>
        /// <param name="in_childGameId">
        /// The App ID of the child game to switch to
        /// </param>
        /// <param name="in_forceCreate">
        /// Should a new profile be created if one does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "vcPurchased": 0,
        ///         "experiencePoints": 0,
        ///         "xpCapped": false,
        ///         "playerName": "TestUser",
        ///         "vcClaimed": 0,
        ///         "rewards": {
        ///             "rewardDetails": {},
        ///             "rewards": {},
        ///             "currency": {
        ///                 "credits": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 },
        ///                 "gold": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 }
        ///             }
        ///         },
        ///         "loginCount": 1,
        ///         "server_time": 1441901094386,
        ///         "experienceLevel": 0,
        ///         "currency": {},
        ///         "statistics": {},
        ///         "id": "a17b347b-695b-431f-b1e7-5f783a562310",
        ///         "profileId": "a17t347b-692b-43ef-b1e7-5f783a566310",
        ///         "newUser": false
        ///     }
        /// }
        /// </returns>
        public void SwitchToSingletonChildProfile(
            string in_childGameId, 
            bool in_forceCreate, 
            SuccessCallback in_success, 
            FailureCallback in_failure)
        {
            SwitchToChildProfile(null, in_childGameId, in_forceCreate, true, in_success, in_failure);
        }

        /// <summary>
        /// Switch to a Parent Profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - SWITCH_TO_PARENT_PROFILE
        /// </remarks>
        /// <param name="in_parentLevelName">
        /// The level of the parent to switch to
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful switch
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error while switching
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "profileId": "1d1h32aa-4c41-404f-bc18-29b3fg5wab8a",
        ///         "gameId": "123456"
        ///     }
        /// }
        /// </returns>
        public void SwitchToParentProfile(
            string in_parentLevelName, 
            SuccessCallback in_success, 
            FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = in_parentLevelName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.SwitchToParentProfile, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns a list of all child profiles in child Apps
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - GET_CHILD_PROFILES
        /// </remarks>
        /// <param name="in_includeSummaryData">
        /// Whether to return the summary friend data along with this call
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "children": [
        ///             {
        ///                 "appId": "123456",
        ///                 "profileId": "b7h4c751-befd-4a89-b6da-cd55hs3b2a86",
        ///                 "profileName": "Child1",
        ///                 "summaryFriendData": null
        ///             },
        ///             {
        ///                 "appId": "123456",
        ///                 "profileId": "a17b347b-195b-45hf-b1e7-5f78g3462310",
        ///                 "profileName": "Child2",
        ///                 "summaryFriendData": null
        ///             }
        ///         ]
        ///     }
        /// }
        /// </returns>
        public void GetChildProfiles(
            bool in_includeSummaryData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStateServiceIncludeSummaryData.Value] = in_includeSummaryData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.GetChildProfiles, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void AttachIdentity(string in_externalId, string in_authenticationToken, string in_authenticationType, SuccessCallback in_success, FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = in_externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = in_authenticationType;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = in_authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Attach, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void MergeIdentity(string in_externalId, string in_authenticationToken, string in_authenticationType, SuccessCallback in_success, FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = in_externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = in_authenticationType;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = in_authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Merge, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void DetachIdentity(string in_externalId, string in_authenticationType, bool in_continueAnon, SuccessCallback in_success, FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = in_externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = in_authenticationType;
            data[OperationParam.IdentityServiceConfirmAnonymous.Value] = in_continueAnon;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Detach, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void SwitchToChildProfile(string in_childProfileId, string in_childGameId, bool in_forceCreate, bool in_forceSingleton, SuccessCallback in_success, FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(in_childProfileId))
            {
                data[OperationParam.ServiceMessageProfileId.Value] = in_childProfileId;
            }

            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = in_childGameId;
            data[OperationParam.AuthenticateServiceAuthenticateForceCreate.Value] = in_forceCreate;
            data[OperationParam.IdentityServiceForceSingleton.Value] = in_forceSingleton;

            data[OperationParam.AuthenticateServiceAuthenticateReleasePlatform.Value] = m_brainCloudClientRef.ReleasePlatform.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateCountryCode.Value] = Util.GetCurrentCountryCode();
            data[OperationParam.AuthenticateServiceAuthenticateLanguageCode.Value] = Util.GetIsoCodeForCurrentLanguage();
            data[OperationParam.AuthenticateServiceAuthenticateTimeZoneOffset.Value] = Util.GetUTCOffsetForCurrentTimeZone();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.SwitchToChildProfile, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
