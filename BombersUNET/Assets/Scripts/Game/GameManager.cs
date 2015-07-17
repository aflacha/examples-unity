﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using BrainCloudUNETExample.Connection;
using BrainCloudUNETExample.Game.PlayerInput;

namespace BrainCloudUNETExample.Game
{
    public class GameManager : NetworkBehaviour
    {

        private enum eGameState
        {
            GAME_STATE_INITIALIZE_GAME,
            GAME_STATE_WAITING_FOR_PLAYERS,
            GAME_STATE_STARTING_GAME,
            GAME_STATE_SPAWN_PLAYERS,
            GAME_STATE_PLAYING_GAME,
            GAME_STATE_GAME_OVER,
            GAME_STATE_CLOSING_ROOM,
            GAME_STATE_RESETTING_GAME,
            GAME_STATE_SPECTATING
        }

        class KillMessage
        {
            public Color m_color = Color.white;
            public string m_message = "";

            public KillMessage(string aMessage, Color aColor)
            {
                m_color = aColor;
                m_message = aMessage;
            }
        }

        private eGameState m_gameState = eGameState.GAME_STATE_WAITING_FOR_PLAYERS;

        //private ExitGames.Client.Photon.Hashtable m_playerProperties = null;
        //private ExitGames.Client.Photon.Hashtable m_roomProperties = null;
        //private Room m_room = null;

        private GUISkin m_skin;

        private int m_respawnTime = 3;

        private List<BulletController.BulletInfo> m_spawnedBullets;
        private List<BombController.BombInfo> m_spawnedBombs;

        private float m_gameTime = 10 * 60;

        private int m_mapLayout = 0;
        private int m_mapSize = 1;

        private List<MapPresets.Preset> m_mapPresets;
        private List<MapPresets.MapSize> m_mapSizes;

        private float m_currentRespawnTime = 0;

        private float m_team1Score = 0;
        private float m_team2Score = 0;
        private int m_shotsFired = 0;
        private int m_bombsDropped = 0;
        private int m_bombsHit = 0;
        private int m_planesDestroyed = 0;
        private int m_carriersDestroyed = 0;
        private int m_timesDestroyed = 0;

        private bool m_once = false;

        [SerializeField]
        private Collider m_team1SpawnBounds;

        [SerializeField]
        private Collider m_team2SpawnBounds;

        private List<BombPickup> m_bombPickupsSpawned;
        private int m_bombID;

        private List<ShipController> m_spawnedShips;

        private GameObject m_lobbyWindow;
        private GameObject m_gameStartButton;

        private GameObject m_resultsWindow;
        private GameObject m_greenLogo;
        private GameObject m_redLogo;
        private GameObject m_enemyWinText;
        private GameObject m_allyWinText;
        private GameObject m_resetButton;
        private GameObject m_quitButton;
        private GameObject m_greenChevron;
        private GameObject m_redChevron;

        private GameObject m_HUD;

        private GameObject m_allyShipSunk;
        private GameObject m_enemyShipSunk;
        private GameObject m_greenShipLogo;
        private GameObject m_redShipLogo;

        private GameObject m_quitMenu;
        private bool m_showQuitMenu;
        private GameObject m_blackScreen;

        private bool m_showScores = false;

        void Awake()
        {
            m_allyShipSunk = GameObject.Find("ShipSink").transform.FindChild("AllyShipSunk").gameObject;
            m_enemyShipSunk = GameObject.Find("ShipSink").transform.FindChild("EnemyShipSunk").gameObject;
            m_redShipLogo = GameObject.Find("ShipSink").transform.FindChild("RedLogo").gameObject;
            m_greenShipLogo = GameObject.Find("ShipSink").transform.FindChild("GreenLogo").gameObject;
            m_blackScreen = GameObject.Find("BlackScreen");

            m_allyShipSunk.SetActive(false);
            m_enemyShipSunk.SetActive(false);
            m_redShipLogo.SetActive(false);
            m_greenShipLogo.SetActive(false);
            m_quitMenu = GameObject.Find("QuitMenu");
            m_quitMenu.SetActive(false);

            m_greenChevron = GameObject.Find("Team Green Score").transform.FindChild("Chevron").gameObject;
            m_redChevron = GameObject.Find("Team Red Score").transform.FindChild("Chevron").gameObject;
            m_greenLogo = GameObject.Find("Green Logo");
            m_greenLogo.SetActive(false);
            m_redLogo = GameObject.Find("Red Logo");
            m_redLogo.SetActive(false);
            m_enemyWinText = GameObject.Find("Window Title - Loss");
            m_allyWinText = GameObject.Find("Window Title - Win");
            m_resetButton = GameObject.Find("Continue");
            m_quitButton = GameObject.Find("ResultsQuit");
            m_lobbyWindow = GameObject.Find("Lobby");
            m_gameStartButton = GameObject.Find("StartGame");
            m_gameTime = GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_defaultGameTime;
            m_mapPresets = GameObject.Find("MapPresets").GetComponent<MapPresets>().m_presets;
            m_mapSizes = GameObject.Find("MapPresets").GetComponent<MapPresets>().m_mapSizes;
            m_resultsWindow = GameObject.Find("Results");
            m_resultsWindow.SetActive(false);
            m_HUD = GameObject.Find("HUD");
            GameObject.Find("RespawnText").GetComponent<Text>().text = "";

            m_missionText = m_HUD.transform.FindChild("MissionText").gameObject;

            m_HUD.SetActive(false);

        }

        public GameObject m_missionText;
        public GameInfo m_gameInfo;

        void Start()
        {
            GameObject.Find("Version Text").transform.SetParent(GameObject.Find("Canvas").transform);
            GameObject.Find("FullScreen").transform.SetParent(GameObject.Find("Canvas").transform);

            m_mapLayout = m_gameInfo.GetMapLayout();
            m_mapSize = m_gameInfo.GetMapSize();

            SetLightPosition(m_gameInfo.GetLightPosition());
            m_spawnedShips = new List<ShipController>();
            m_bombPickupsSpawned = new List<BombPickup>();

            m_spawnedBullets = new List<BulletController.BulletInfo>();
            m_spawnedBombs = new List<BombController.BombInfo>();
            m_skin = (GUISkin)Resources.Load("skin");
            //m_room = PhotonNetwork.room;
            //m_playerProperties = new ExitGames.Client.Photon.Hashtable();
            //m_playerProperties = PhotonNetwork.player.customProperties;
            StartCoroutine("UpdatePing");
            StartCoroutine("UpdateRoomDisplayName");
            //m_roomProperties = PhotonNetwork.room.customProperties;
            //if (m_playerProperties["Team"] == null)
            //    m_playerProperties["Team"] = 0;

            m_team1Score = 0;
            m_team2Score = 0;

            /*if ((int)m_roomProperties["IsPlaying"] == 1)
            {
                m_gameState = eGameState.GAME_STATE_SPECTATING;
                m_roomProperties["Spectators"] = (int)PhotonNetwork.room.customProperties["Spectators"] + 1;
                PhotonPlayer[] playerList = PhotonNetwork.playerList;
                List<PhotonPlayer> playerListList = new List<PhotonPlayer>();
                for (int i = 0; i < playerList.Length; i++)
                {
                    playerListList.Add(playerList[i]);
                }

                int count = 0;
                while (count < playerListList.Count)
                {
                    if (playerListList[count].customProperties["Team"] == null || (int)playerListList[count].customProperties["Team"] == 0)
                    {
                        playerListList.RemoveAt(count);
                    }
                    else
                    {
                        count++;
                    }
                }
                playerList = playerListList.ToArray().OrderByDescending(x => (int)x.customProperties["Score"]).ToArray();
                m_spectatingTarget = playerList[0];
            }
            else*/

            {

                if (isServer)
                {
                    m_gameInfo.SetLightPosition(Random.Range(1, 5));
                    SetLightPosition(m_gameInfo.GetLightPosition());
                }

                if (m_gameInfo.GetTeamPlayers(2) < m_gameInfo.GetTeamPlayers(1))
                {
                    BombersNetworkManager.m_localPlayer.m_team = 2;
                    m_gameInfo.SetTeamPlayers(2, m_gameInfo.GetTeamPlayers(2) + 1);

                }
                else
                {
                    BombersNetworkManager.m_localPlayer.m_team = 1;
                    m_gameInfo.SetTeamPlayers(1, m_gameInfo.GetTeamPlayers(1) + 1);
                }
            }
            BombersNetworkManager.m_localPlayer.m_score = 0;

            if (m_gameInfo.GetPlaying() == 1)
            {
                //TODO: Change for UNET
                //GetComponent<PhotonView>().RPC("AnnounceJoin", PhotonTargets.All, m_playerProperties["RoomDisplayName"].ToString(), (int)m_playerProperties["Team"]);
            }
        }

        [RPC]
        void AnnounceJoin(string aPlayerName, int aTeam)
        {
            string message = aPlayerName + " has joined the fight\n on the ";
            message += (aTeam == 1) ? "green team!" : "red team!";
            GameObject.Find("DialogDisplay").GetComponent<DialogDisplay>().DisplayDialog(message, true);
        }

        [RPC]
        void SetLightPosition(int aPosition)
        {
            Vector3 position = Vector3.zero;
            switch (aPosition)
            {
                case 1:
                    position = new Vector3(330, 0, 0);
                    break;
                case 2:
                    position = new Vector3(354, 34, 0);
                    break;
                case 3:
                    position = new Vector3(10, 325, 0);
                    break;
                case 4:
                    position = new Vector3(30, 0, 0);
                    break;
            }
            StopCoroutine("MoveLight");
            StartCoroutine("MoveLight", position);
        }

        IEnumerator MoveLight(Vector3 aPosition)
        {
            bool done = false;
            int count = 0;
            while (!done)
            {
                GameObject.Find("Directional Light").transform.rotation = Quaternion.Slerp(GameObject.Find("Directional Light").transform.rotation, Quaternion.Euler(aPosition), 5 * Time.deltaTime);
                count++;
                if (count > 10000)
                {
                    GameObject.Find("Directional Light").transform.rotation = Quaternion.Euler(aPosition);
                    done = true;
                }
                yield return new WaitForSeconds(0.02f);
            }
        }

        void OnApplicationQuit()
        {
            LeaveRoom();
            /*
            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.client.Disconnect();
            }
             * */
            //if (m_playerProperties != null)
            //{
            //	m_playerProperties.Clear();
            //}
            //PhotonNetwork.player.SetCustomProperties(m_playerProperties);
            //PhotonNetwork.Disconnect();
        }

        public void LeaveRoom()
        {
            if (BombersNetworkManager.m_localPlayer.m_team == 1)
            {
                m_gameInfo.SetTeamPlayers(1, m_gameInfo.GetTeamPlayers(1) - 1);
            }
            else if (BombersNetworkManager.m_localPlayer.m_team == 1)
            {
                m_gameInfo.SetTeamPlayers(2, m_gameInfo.GetTeamPlayers(2) - 1);
            }

            GameObject.Find("Version Text").transform.SetParent(null);
            GameObject.Find("FullScreen").transform.SetParent(null);

            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.client.Disconnect();
            }
        }

        public void OnLeftRoom()
        {
            //GameObject.Find("Version Text").transform.SetParent(null);
            //GameObject.Find("FullScreen").transform.SetParent(null);
            //PhotonNetwork.LoadLevel("Matchmaking");
        }

        public void ForceStartGame()
        {
            m_gameState = eGameState.GAME_STATE_STARTING_GAME;
        }

        public void ReturnToWaitingRoom()
        {
            if (m_gameState == eGameState.GAME_STATE_GAME_OVER)
            {
                RpcResetGame();
                //GetComponent<PhotonView>().RPC("ResetGame", PhotonTargets.All);
            }
        }

        public void CloseQuitMenu()
        {
            m_showQuitMenu = false;
        }

        //void OnMasterClientSwitched(PhotonPlayer newMasterClient)
        //{
        //    GameObject.Find("DialogDisplay").GetComponent<DialogDisplay>().HostLeft();
        //}

        void OnGUI()
        {
            GUI.skin = m_skin;
            //if (PhotonNetwork.room == null) return;
            switch (m_gameState)
            {
                case eGameState.GAME_STATE_WAITING_FOR_PLAYERS:
                    m_resultsWindow.GetComponent<CanvasGroup>().alpha = 0;

                    m_lobbyWindow.gameObject.SetActive(true);
                    m_resultsWindow.gameObject.SetActive(false);
                    m_HUD.SetActive(false);
                    OnWaitingForPlayersWindow();
                    break;
                case eGameState.GAME_STATE_STARTING_GAME:
                    m_blackScreen.GetComponent<CanvasGroup>().alpha += Time.fixedDeltaTime * 3;
                    m_resultsWindow.GetComponent<CanvasGroup>().alpha = 0;
                    m_lobbyWindow.gameObject.SetActive(true);
                    m_resultsWindow.gameObject.SetActive(false);
                    m_HUD.SetActive(false);
                    OnWaitingForPlayersWindow();
                    break;
                case eGameState.GAME_STATE_SPECTATING:
                    m_lobbyWindow.gameObject.SetActive(false);
                    if (m_showScores)
                    {
                        m_resultsWindow.GetComponent<CanvasGroup>().alpha = 1;
                        m_resultsWindow.gameObject.SetActive(true);
                        OnMiniScoresWindow();
                    }
                    else
                    {
                        m_resultsWindow.GetComponent<CanvasGroup>().alpha = 0;
                        m_resultsWindow.gameObject.SetActive(false);
                    }
                    m_HUD.SetActive(false);
                    GUI.Label(new Rect(Screen.width / 2 - 100, 20, 200, 20), "Spectating");
                    break;
                case eGameState.GAME_STATE_GAME_OVER:
                    m_lobbyWindow.gameObject.SetActive(false);
                    m_resultsWindow.gameObject.SetActive(true);
                    m_HUD.SetActive(false);

                    OnScoresWindow();
                    break;

                case eGameState.GAME_STATE_PLAYING_GAME:
                    m_blackScreen.GetComponent<CanvasGroup>().alpha -= Time.fixedDeltaTime * 3;
                    m_lobbyWindow.gameObject.SetActive(false);
                    m_resultsWindow.gameObject.SetActive(false);
                    m_HUD.SetActive(true);
                    if (m_showScores)
                    {
                        m_resultsWindow.GetComponent<CanvasGroup>().alpha += Time.fixedDeltaTime * 4;
                        if (m_resultsWindow.GetComponent<CanvasGroup>().alpha > 1) m_resultsWindow.GetComponent<CanvasGroup>().alpha = 1;
                        m_resultsWindow.gameObject.SetActive(true);
                        OnMiniScoresWindow();
                    }
                    else
                    {
                        if (m_resultsWindow.GetComponent<CanvasGroup>().alpha > 0) m_resultsWindow.gameObject.SetActive(true);
                        m_resultsWindow.GetComponent<CanvasGroup>().alpha -= Time.fixedDeltaTime * 4;
                        if (m_resultsWindow.GetComponent<CanvasGroup>().alpha < 0) m_resultsWindow.GetComponent<CanvasGroup>().alpha = 0;
                    }
                    OnHudWindow();
                    break;

                default:
                    m_lobbyWindow.gameObject.SetActive(false);
                    m_resultsWindow.gameObject.SetActive(false);
                    m_HUD.SetActive(false);
                    break;
            }
        }

        void OnHudWindow()
        {
            //if (PhotonNetwork.room == null) return;
            m_team1Score = m_gameInfo.GetTeamScore(1);
            m_team2Score = m_gameInfo.GetTeamScore(2);
            int score = BombersNetworkManager.m_localPlayer.m_score;
            System.TimeSpan span = System.TimeSpan.FromSeconds(m_gameTime);
            string timeLeft = span.ToString().Substring(3, 5);

            List<ShipController> team1Ships = new List<ShipController>();
            List<ShipController> team2Ships = new List<ShipController>();
            for (int i = 0; i < m_spawnedShips.Count; i++)
            {
                if (m_spawnedShips[i].m_team == 1 && m_spawnedShips[i].IsAlive())
                {
                    team1Ships.Add(m_spawnedShips[i]);
                }
                else if (m_spawnedShips[i].m_team == 2 && m_spawnedShips[i].IsAlive())
                {
                    team2Ships.Add(m_spawnedShips[i]);
                }
            }

            m_HUD.transform.FindChild("PlayerScore").GetChild(0).GetComponent<Text>().text = score.ToString("n0");
            m_HUD.transform.FindChild("RedScore").GetChild(0).GetComponent<Text>().text = m_team2Score.ToString("n0");
            m_HUD.transform.FindChild("RedScore").GetChild(1).GetComponent<Text>().text = "Ships Left: " + team2Ships.Count.ToString();
            if (team2Ships.Count == 1)
                m_HUD.transform.FindChild("RedScore").GetChild(1).GetComponent<Text>().color = new Color(1, 0, 0, 1);
            else
                m_HUD.transform.FindChild("RedScore").GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 1);
            m_HUD.transform.FindChild("GreenScore").GetChild(0).GetComponent<Text>().text = m_team1Score.ToString("n0");
            m_HUD.transform.FindChild("GreenScore").GetChild(1).GetComponent<Text>().text = "Ships Left: " + team1Ships.Count.ToString();
            if (team1Ships.Count == 1)
                m_HUD.transform.FindChild("GreenScore").GetChild(1).GetComponent<Text>().color = new Color(1, 0, 0, 1);
            else
                m_HUD.transform.FindChild("GreenScore").GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 1);
            m_HUD.transform.FindChild("TimeLeft").GetChild(0).GetComponent<Text>().text = timeLeft;
        }

        void OnMiniScoresWindow()
        {
            m_quitButton.SetActive(false);
            m_resetButton.SetActive(false);
            m_allyWinText.SetActive(false);
            m_enemyWinText.SetActive(false);
            m_greenLogo.SetActive(false);
            m_redLogo.SetActive(false);

            m_team1Score = m_gameInfo.GetTeamScore(1);
            m_team2Score = m_gameInfo.GetTeamScore(2);
            GameObject team = GameObject.Find("Team Green Score");
            team.transform.FindChild("Team Score").GetComponent<Text>().text = m_team1Score.ToString("n0");
            team = GameObject.Find("Team Red Score");
            team.transform.FindChild("Team Score").GetComponent<Text>().text = m_team2Score.ToString("n0");

            GameObject[] playerList = GameObject.FindGameObjectsWithTag("PlayerController");
            List<GameObject> playerListList = new List<GameObject>();

            for (int i = 0; i < playerList.Length; i++)
            {
                playerListList.Add(playerList[i]);
            }

            int count = 0;
            while (count < playerListList.Count)
            {
                if (playerListList[count].GetComponent<BombersPlayerController>().m_team == 0)
                {
                    playerListList.RemoveAt(count);
                }
                else
                {
                    count++;
                }
            }
            playerList = playerListList.ToArray().OrderByDescending(x => x.GetComponent<BombersPlayerController>().m_score).ToArray();

            string greenNamesText = "";
            string greenKDText = "";
            string greenScoreText = "";

            string redNamesText = "";
            string redKDText = "";
            string redScoreText = "";

            int greenPlayers = 0;
            int redPlayers = 0;
            for (int i = 0; i < playerList.Length; i++)
            {
                if ((int)playerList[i].GetComponent<BombersPlayerController>().m_team == 1)
                {
                    if (playerList[i] == BombersNetworkManager.m_localPlayer)
                    {
                        m_redChevron.SetActive(false);
                        m_greenChevron.SetActive(true);
                        m_greenChevron.transform.GetChild(0).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_displayName;
                        m_greenChevron.transform.GetChild(1).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths;
                        m_greenChevron.transform.GetChild(2).GetComponent<Text>().text = (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0");
                        m_greenChevron.GetComponent<RectTransform>().localPosition = new Vector3(m_greenChevron.GetComponent<RectTransform>().localPosition.x, 21.8f - (greenPlayers * 17.7f), m_greenChevron.GetComponent<RectTransform>().localPosition.z);
                        greenNamesText += "\n";
                        greenKDText += "\n";
                        greenScoreText += "\n";
                    }
                    else
                    {
                        greenNamesText += playerList[i].GetComponent<BombersPlayerController>().m_displayName + "\n";
                        greenKDText += playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths + "\n";
                        greenScoreText += (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0") + "\n";
                    }
                    greenPlayers++;
                }
                else
                {
                    if (playerList[i] == BombersNetworkManager.m_localPlayer)
                    {
                        m_redChevron.SetActive(true);
                        m_greenChevron.SetActive(false);
                        m_redChevron.transform.GetChild(0).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_displayName;
                        m_redChevron.transform.GetChild(1).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths;
                        m_redChevron.transform.GetChild(2).GetComponent<Text>().text = (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0");
                        m_redChevron.GetComponent<RectTransform>().localPosition = new Vector3(m_redChevron.GetComponent<RectTransform>().localPosition.x, 21.8f - (redPlayers * 17.7f), m_redChevron.GetComponent<RectTransform>().localPosition.z);

                        redNamesText += "\n";
                        redKDText += "\n";
                        redScoreText += "\n";
                    }
                    else
                    {
                        redNamesText += playerList[i].GetComponent<BombersPlayerController>().m_displayName + "\n";
                        redKDText += playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths + "\n";
                        redScoreText += (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0") + "\n";
                    }
                    redPlayers++;
                }
            }

            team = GameObject.Find("Team Green Score");
            team.transform.FindChild("GreenPlayers").GetComponent<Text>().text = greenNamesText;
            team.transform.FindChild("GreenPlayerKD").GetComponent<Text>().text = greenKDText;
            team.transform.FindChild("GreenPlayerScores").GetComponent<Text>().text = greenScoreText;
            team = GameObject.Find("Team Red Score");
            team.transform.FindChild("RedPlayers").GetComponent<Text>().text = redNamesText;
            team.transform.FindChild("RedPlayerKD").GetComponent<Text>().text = redKDText;
            team.transform.FindChild("RedPlayerScores").GetComponent<Text>().text = redScoreText;
        }

        void OnScoresWindow()
        {
            //if (PhotonNetwork.room == null) return;
            m_resultsWindow.GetComponent<CanvasGroup>().alpha += Time.fixedDeltaTime * 2;
            if (m_resultsWindow.GetComponent<CanvasGroup>().alpha > 1) m_resultsWindow.GetComponent<CanvasGroup>().alpha = 1;
            m_team1Score = m_gameInfo.GetTeamPlayers(1);
            m_team2Score = m_gameInfo.GetTeamPlayers(2);
            GameObject team = GameObject.Find("Team Green Score");
            team.transform.FindChild("Team Score").GetComponent<Text>().text = m_team1Score.ToString("n0");
            team = GameObject.Find("Team Red Score");
            team.transform.FindChild("Team Score").GetComponent<Text>().text = m_team2Score.ToString("n0");

            if (m_gameState != eGameState.GAME_STATE_GAME_OVER)
            {
                m_quitButton.SetActive(false);
                m_resetButton.SetActive(false);
            }
            else if (!isServer)
            {
                m_quitButton.SetActive(false);
                m_resetButton.SetActive(true);
            }
            else
            {
                m_quitButton.SetActive(true);
                m_resetButton.SetActive(true);
            }
            m_allyWinText.SetActive(false);
            m_enemyWinText.SetActive(false);
            if (m_gameState == eGameState.GAME_STATE_GAME_OVER)
            {
                if (m_team1Score > m_team2Score)
                {
                    m_greenLogo.SetActive(true);
                    m_redLogo.SetActive(false);
                    if (BombersNetworkManager.m_localPlayer.m_team == 1)
                    {
                        m_allyWinText.SetActive(true);
                        m_enemyWinText.SetActive(false);
                    }
                    else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                    {
                        m_allyWinText.SetActive(false);
                        m_enemyWinText.SetActive(true);
                    }
                }
                else
                {
                    m_greenLogo.SetActive(false);
                    m_redLogo.SetActive(true);

                    if (BombersNetworkManager.m_localPlayer.m_team == 1)
                    {
                        m_allyWinText.SetActive(false);
                        m_enemyWinText.SetActive(true);
                    }
                    else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                    {
                        m_allyWinText.SetActive(true);
                        m_enemyWinText.SetActive(false);
                    }
                }
            }
            else
            {
                m_greenLogo.SetActive(false);
                m_redLogo.SetActive(false);
            }


            GameObject[] playerList = GameObject.FindGameObjectsWithTag("PlayerController");
            List<GameObject> playerListList = new List<GameObject>();

            for (int i = 0; i < playerList.Length; i++)
            {
                playerListList.Add(playerList[i]);
            }

            int count = 0;
            while (count < playerListList.Count)
            {
                if (playerListList[count].GetComponent<BombersPlayerController>().m_team == 0)
                {
                    playerListList.RemoveAt(count);
                }
                else
                {
                    count++;
                }
            }
            playerList = playerListList.ToArray().OrderByDescending(x => x.GetComponent<BombersPlayerController>().m_score).ToArray();

            string greenNamesText = "";
            string greenKDText = "";
            string greenScoreText = "";

            string redNamesText = "";
            string redKDText = "";
            string redScoreText = "";

            int greenPlayers = 0;
            int redPlayers = 0;
            for (int i = 0; i < playerList.Length; i++)
            {
                if ((int)playerList[i].GetComponent<BombersPlayerController>().m_team == 1)
                {
                    if (playerList[i] == BombersNetworkManager.m_localPlayer)
                    {
                        m_redChevron.SetActive(false);
                        m_greenChevron.SetActive(true);
                        m_greenChevron.transform.GetChild(0).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_displayName;
                        m_greenChevron.transform.GetChild(1).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths;
                        m_greenChevron.transform.GetChild(2).GetComponent<Text>().text = (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0");
                        m_greenChevron.GetComponent<RectTransform>().localPosition = new Vector3(m_greenChevron.GetComponent<RectTransform>().localPosition.x, 21.8f - (greenPlayers * 17.7f), m_greenChevron.GetComponent<RectTransform>().localPosition.z);
                        greenNamesText += "\n";
                        greenKDText += "\n";
                        greenScoreText += "\n";
                    }
                    else
                    {
                        greenNamesText += playerList[i].GetComponent<BombersPlayerController>().m_displayName + "\n";
                        greenKDText += playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths + "\n";
                        greenScoreText += (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0") + "\n";
                    }
                    greenPlayers++;
                }
                else
                {
                    if (playerList[i] == BombersNetworkManager.m_localPlayer)
                    {
                        m_redChevron.SetActive(true);
                        m_greenChevron.SetActive(false);
                        m_redChevron.transform.GetChild(0).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_displayName;
                        m_redChevron.transform.GetChild(1).GetComponent<Text>().text = playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths;
                        m_redChevron.transform.GetChild(2).GetComponent<Text>().text = (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0");
                        m_redChevron.GetComponent<RectTransform>().localPosition = new Vector3(m_redChevron.GetComponent<RectTransform>().localPosition.x, 21.8f - (redPlayers * 17.7f), m_redChevron.GetComponent<RectTransform>().localPosition.z);

                        redNamesText += "\n";
                        redKDText += "\n";
                        redScoreText += "\n";
                    }
                    else
                    {
                        redNamesText += playerList[i].GetComponent<BombersPlayerController>().m_displayName + "\n";
                        redKDText += playerList[i].GetComponent<BombersPlayerController>().m_kills + "/" + playerList[i].GetComponent<BombersPlayerController>().m_deaths + "\n";
                        redScoreText += (playerList[i].GetComponent<BombersPlayerController>().m_score).ToString("n0") + "\n";
                    }
                    redPlayers++;
                }
            }

            team = GameObject.Find("Team Green Score");
            team.transform.FindChild("GreenPlayers").GetComponent<Text>().text = greenNamesText;
            team.transform.FindChild("GreenPlayerKD").GetComponent<Text>().text = greenKDText;
            team.transform.FindChild("GreenPlayerScores").GetComponent<Text>().text = greenScoreText;
            team = GameObject.Find("Team Red Score");
            team.transform.FindChild("RedPlayers").GetComponent<Text>().text = redNamesText;
            team.transform.FindChild("RedPlayerKD").GetComponent<Text>().text = redKDText;
            team.transform.FindChild("RedPlayerScores").GetComponent<Text>().text = redScoreText;
        }

        public void ChangeTeam()
        {
            if (BombersNetworkManager.m_localPlayer.m_team != 1)
            {
                if (BombersNetworkManager.m_localPlayer.m_team == 2)
                {
                    m_gameInfo.SetTeamPlayers(2, m_gameInfo.GetTeamPlayers(2) - 1);
                }

                m_gameInfo.SetTeamPlayers(1, m_gameInfo.GetTeamPlayers(1) + 1);
                BombersNetworkManager.m_localPlayer.m_team = 1;
            }
            else if (BombersNetworkManager.m_localPlayer.m_team != 2)
            {
                if (BombersNetworkManager.m_localPlayer.m_team == 1)
                {
                    m_gameInfo.SetTeamPlayers(1, m_gameInfo.GetTeamPlayers(1) - 1);
                }

                m_gameInfo.SetTeamPlayers(2, m_gameInfo.GetTeamPlayers(2) + 1);
                BombersNetworkManager.m_localPlayer.m_team = 2;
            }
        }

        public BombersPlayerController FindPlayerWithID(int aID)
        {
            return BombersNetworkManager.m_localPlayer;
        }

        void OnWaitingForPlayersWindow()
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("PlayerController");
            List<GameObject> playerListList = new List<GameObject>();

            for (int i = 0; i < playerList.Length; i++)
            {
                playerListList.Add(playerList[i]);
            }

            int count = 0;
            while (count < playerListList.Count)
            {
                if (playerListList[count].GetComponent<BombersPlayerController>().m_team == 0)
                {
                    playerListList.RemoveAt(count);
                }
                else
                {
                    count++;
                }
            }
            playerList = playerListList.ToArray().OrderByDescending(x => x.GetComponent<BombersPlayerController>().m_score).ToArray();

            List<GameObject> greenPlayers = new List<GameObject>();
            List<GameObject> redPlayers = new List<GameObject>();

            for (int i = 0; i < playerList.Length; i++)
            {
                if (playerList[i].GetComponent<BombersPlayerController>().m_team != null)
                {
                    if (playerList[i].GetComponent<BombersPlayerController>().m_team == 1)
                    {
                        greenPlayers.Add(playerList[i]);
                    }
                    else if (playerList[i].GetComponent<BombersPlayerController>().m_team == 2)
                    {
                        redPlayers.Add(playerList[i]);
                    }
                }
            }

            Text teamText = GameObject.Find("GreenPlayerNames").GetComponent<Text>();
            Text teamPingText = GameObject.Find("GreenPings").GetComponent<Text>();

            string nameText = "";
            string pingText = "";
            for (int i = 0; i < greenPlayers.Count; i++)
            {
                nameText += greenPlayers[i].GetComponent<BombersPlayerController>().m_displayName + "\n";
                pingText += greenPlayers[i].GetComponent<BombersPlayerController>().m_ping + "\n";
            }


            teamText.text = nameText;
            teamPingText.text = pingText;
            teamText = GameObject.Find("RedPlayerNames").GetComponent<Text>();
            teamPingText = GameObject.Find("RedPings").GetComponent<Text>();
            nameText = "";
            pingText = "";

            for (int i = 0; i < redPlayers.Count; i++)
            {
                nameText += redPlayers[i].GetComponent<BombersPlayerController>().m_displayName + "\n";
                pingText += redPlayers[i].GetComponent<BombersPlayerController>().m_ping + "\n";
            }
            teamText.text = nameText;
            teamPingText.text = pingText;

            GameObject.Find("GreenPlayers").GetComponent<Text>().text = greenPlayers.Count + "/" + Mathf.Floor(m_gameInfo.GetMaxPlayers() / 2.0f);
            GameObject.Find("RedPlayers").GetComponent<Text>().text = redPlayers.Count + "/" + Mathf.Floor(m_gameInfo.GetMaxPlayers() / 2.0f);
            GameObject.Find("GameName").GetComponent<Text>().text = m_gameInfo.GetGameName();

            if (!isServer || m_gameState != eGameState.GAME_STATE_WAITING_FOR_PLAYERS)
            {
                m_gameStartButton.SetActive(false);

                if (m_gameState == eGameState.GAME_STATE_WAITING_FOR_PLAYERS)
                    GameObject.Find("ChangeTeam").transform.position = m_gameStartButton.transform.position;
            }
            else if (m_gameState == eGameState.GAME_STATE_WAITING_FOR_PLAYERS)
            {
                m_gameStartButton.SetActive(true);
            }
        }

        //[RPC]
        //void ChangeMapLayout(int aLayout)
        //{
        //    m_mapLayout = aLayout;
        //}

        //[RPC]
        //void ChangeMapSize(int aSize)
        //{
        //    m_mapSize = aSize;
        //}

        IEnumerator UpdatePing()
        {
            while (m_gameState != eGameState.GAME_STATE_CLOSING_ROOM)
            {
                byte errorByte;
                BombersNetworkManager.m_localPlayer.m_ping = NetworkTransport.GetCurrentRtt(BombersNetworkManager.m_localConnection.hostId, BombersNetworkManager.m_localConnection.connectionId, out errorByte);
                yield return new WaitForSeconds(1);
            }
        }

        IEnumerator UpdateRoomDisplayName()
        {
            List<string> otherNames = new List<string>();
            while (true)
            {
                otherNames.Clear();
                GameObject[] playerList = GameObject.FindGameObjectsWithTag("PlayerController");
                foreach (GameObject player in playerList)
                {
                    if (player != BombersNetworkManager.m_localPlayer && player.GetComponent<BombersPlayerController>().m_displayName != "")
                    {
                        otherNames.Add(player.GetComponent<BombersPlayerController>().m_displayName);
                    }
                }

                int count = 1;
                string displayName = BombersNetworkManager.m_localPlayer.m_displayName;
                while (otherNames.Contains(displayName))
                {
                    displayName = BombersNetworkManager.m_localPlayer.m_displayName + "(" + count + ")";
                    count++;
                }

                if (BombersNetworkManager.m_localPlayer.m_displayName == "")
                {
                    BombersNetworkManager.m_localPlayer.m_displayName = GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_playerName;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator RespawnPlayer()
        {
            m_currentRespawnTime = (float)m_respawnTime;
            while (m_currentRespawnTime > 0)
            {
                GameObject.Find("RespawnText").GetComponent<Text>().text = "Respawning in " + Mathf.CeilToInt(m_currentRespawnTime);
                yield return new WaitForSeconds(0.1f);
                m_currentRespawnTime -= 0.1f;
            }

            if (m_currentRespawnTime < 0)
            {
                m_currentRespawnTime = 0;
                GameObject.Find("RespawnText").GetComponent<Text>().text = "";
            }

            if (m_gameState == eGameState.GAME_STATE_PLAYING_GAME)
            {
                Vector3 spawnPoint = Vector3.zero;
                spawnPoint.z = 22;

                if (BombersNetworkManager.m_localPlayer.m_team == 1)
                {
                    spawnPoint.x = Random.Range(m_team1SpawnBounds.bounds.center.x - m_team1SpawnBounds.bounds.size.x / 2, m_team1SpawnBounds.bounds.center.x + m_team1SpawnBounds.bounds.size.x / 2);
                    spawnPoint.y = Random.Range(m_team1SpawnBounds.bounds.center.y - m_team1SpawnBounds.bounds.size.y / 2, m_team1SpawnBounds.bounds.center.y + m_team1SpawnBounds.bounds.size.y / 2);
                }
                else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                {
                    spawnPoint.x = Random.Range(m_team2SpawnBounds.bounds.center.x - m_team2SpawnBounds.bounds.size.x / 2, m_team2SpawnBounds.bounds.center.x + m_team2SpawnBounds.bounds.size.x / 2);
                    spawnPoint.y = Random.Range(m_team2SpawnBounds.bounds.center.y - m_team2SpawnBounds.bounds.size.y / 2, m_team2SpawnBounds.bounds.center.y + m_team2SpawnBounds.bounds.size.y / 2);
                }

                GameObject playerPlane = (GameObject)Instantiate((GameObject)Resources.Load("plane"), spawnPoint, Quaternion.LookRotation(Vector3.forward, (Vector3.zero - spawnPoint)));
                NetworkServer.Spawn(playerPlane);

                if (BombersNetworkManager.m_localPlayer.m_team == 1)
                {
                    playerPlane.layer = 8;
                }
                else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                {
                    playerPlane.layer = 9;
                }
                GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().SetPlayerPlane(playerPlane.GetComponent<PlaneController>());
                playerPlane.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        [ClientRpc]
        void RpcSetMapSize(Vector3 newScale)
        {
            GameObject mapBound = GameObject.Find("MapBounds");
            mapBound.transform.localScale = newScale;

            GameObject spawn = GameObject.Find("Team1Spawn");
            spawn.transform.position = new Vector3(mapBound.GetComponent<Collider>().bounds.min.x, 0, 22);
            spawn.transform.localScale = new Vector3(5, 1, newScale.z * 0.75f);
            spawn = GameObject.Find("Team2Spawn");
            spawn.transform.position = new Vector3(mapBound.GetComponent<Collider>().bounds.max.x, 0, 22);
            spawn.transform.localScale = new Vector3(5, 1, newScale.z * 0.75f);
            GameObject.Find("BorderClouds").GetComponent<MapCloudBorder>().SetCloudBorder();
        }

        [Command]
        void CmdSpawnGameStart()
        {
            MapPresets.MapSize mapSize = m_mapSizes[m_mapSize];
            GameObject mapBound = GameObject.Find("MapBounds");
            mapBound.transform.localScale = new Vector3(mapSize.m_horizontalSize, 1, mapSize.m_verticalSize);

            RpcSetMapSize(new Vector3(mapSize.m_horizontalSize, 1, mapSize.m_verticalSize));
            //GetComponent<PhotonView>().RPC("SetMapSizeRPC", PhotonTargets.OthersBuffered, new Vector3(mapSize.m_horizontalSize, 1, mapSize.m_verticalSize));
            RpcGetReady();
            //GetComponent<PhotonView>().RPC("GetReady", PhotonTargets.AllBuffered);
            
            m_gameInfo.SetPlaying(1);

            int shipID = 0;
            bool done = false;
            GameObject ship = null;
            int shipIndex = 0;
            int tryCount = 0;

            GameObject spawn = GameObject.Find("Team1Spawn");
            spawn.transform.position = new Vector3(mapBound.GetComponent<Collider>().bounds.min.x, 0, 22);
            spawn.transform.localScale = new Vector3(5, 1, mapSize.m_verticalSize * 0.75f);
            spawn = GameObject.Find("Team2Spawn");
            spawn.transform.position = new Vector3(mapBound.GetComponent<Collider>().bounds.max.x, 0, 22);
            spawn.transform.localScale = new Vector3(5, 1, mapSize.m_verticalSize * 0.75f);
            GameObject.Find("BorderClouds").GetComponent<MapCloudBorder>().SetCloudBorder();
            if (m_mapLayout == 0)
            {
                GameObject testShip = (GameObject)Instantiate((GameObject)Resources.Load("Carrier01"), Vector3.zero, Quaternion.identity);
                while (!done)
                {
                    tryCount = 0;
                    bool positionFound = false;
                    Vector3 position = new Vector3(0, 0, 122);

                    while (!positionFound)
                    {
                        position.x = Random.Range(-340.0f, 340.0f);
                        position.y = Random.Range(-220.0f, 220.0f);

                        float minDistance = 10000;
                        for (int i = 0; i < m_spawnedShips.Count; i++)
                        {
                            if ((m_spawnedShips[i].transform.position - position).magnitude < minDistance)
                            {
                                minDistance = (m_spawnedShips[i].transform.position - position).magnitude;
                            }
                        }

                        if (minDistance > 170)
                        {
                            positionFound = true;
                        }
                        tryCount++;
                        if (tryCount > 100000)
                        {
                            positionFound = true;
                        }
                    }

                    float rotation = 0;
                    tryCount = 0;
                    bool done2 = false;
                    testShip.transform.position = position;
                    while (!done2)
                    {
                        rotation = Random.Range(0.0f, 360.0f);
                        testShip.transform.rotation = Quaternion.Euler(0, 0, rotation);

                        bool collides = false;
                        for (int i = 0; i < m_spawnedShips.Count; i++)
                        {
                            if (testShip.transform.FindChild("Graphic").GetComponent<Collider>().bounds.Intersects(m_spawnedShips[i].transform.FindChild("ShipGraphic").GetChild(0).FindChild("Graphic").GetComponent<Collider>().bounds))
                            {
                                collides = true;
                                break;
                            }
                        }

                        if (!collides)
                        {
                            done2 = true;
                        }
                        tryCount++;
                        if (tryCount > 1000)
                        {
                            done2 = true;
                        }

                    }

                    ship = (GameObject)Instantiate((GameObject)Resources.Load("Ship"), position, Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                    NetworkServer.Spawn(ship);
                    switch (shipIndex)
                    {
                        case 0:
                            ship.GetComponent<ShipController>().SetShipType(ShipController.eShipType.SHIP_TYPE_CARRIER, (shipID % 2) + 1, shipID);
                            break;
                        case 1:
                            ship.GetComponent<ShipController>().SetShipType(ShipController.eShipType.SHIP_TYPE_BATTLESHIP, (shipID % 2) + 1, shipID);
                            break;
                        case 2:
                            ship.GetComponent<ShipController>().SetShipType(ShipController.eShipType.SHIP_TYPE_CRUISER, (shipID % 2) + 1, shipID);
                            break;
                        case 3:
                            ship.GetComponent<ShipController>().SetShipType(ShipController.eShipType.SHIP_TYPE_PATROLBOAT, (shipID % 2) + 1, shipID);
                            break;
                        case 4:
                            ship.GetComponent<ShipController>().SetShipType(ShipController.eShipType.SHIP_TYPE_DESTROYER, (shipID % 2) + 1, shipID);
                            break;
                    }

                    if (shipID % 2 == 1)
                    {
                        shipIndex++;
                    }
                    shipID++;
                    if (m_spawnedShips.Count >= 10) done = true;
                }
                Destroy(testShip);
            }
            else
            {
                MapPresets.Preset preset = m_mapPresets[m_mapLayout];
                Bounds mapBounds = GameObject.Find("MapBounds").GetComponent<Collider>().bounds;
                for (int i = 0; i < preset.m_numShips; i++)
                {
                    Vector3 position = new Vector3(mapBounds.min.x + (mapBounds.max.x - mapBounds.min.x) * preset.m_ships[i].m_xPositionPercent, mapBounds.min.y + (mapBounds.max.y - mapBounds.min.y) * preset.m_ships[i].m_yPositionPercent, 122);
                    ship = (GameObject)Instantiate((GameObject)Resources.Load("Ship"), position, Quaternion.Euler(0, 0, preset.m_ships[i].m_angle));
                    NetworkServer.Spawn(ship);
                    ship.GetComponent<ShipController>().SetShipType(preset.m_ships[i].m_shipType, preset.m_ships[i].m_team, shipID, preset.m_ships[i].m_angle, position, preset.m_ships[i].m_respawnTime, preset.m_ships[i].m_path, preset.m_ships[i].m_pathSpeed);

                    shipID++;
                }

            }

            Bounds bounds = GameObject.Find("MapBounds").GetComponent<Collider>().bounds;

            for (int i = 0; i < (int)m_mapSizes[m_mapSize].m_horizontalSize / 80 + (int)m_mapSizes[m_mapSize].m_verticalSize / 80; i++)
            {
                Vector3 position = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), 122);
                Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360.0f)));

                if (!Physics.CheckSphere(position, 15, (1 << 16 | 1 << 17 | 1 << 20)))
                {
                    GameObject rock = (GameObject)Instantiate((GameObject)Resources.Load("Rock0" + Random.Range(1, 5)), position, rotation);
                    NetworkServer.Spawn(rock);
                }
                else
                {
                    i--;
                }
            }
        }

        void Update()
        {
            switch (m_gameState)
            {
                case eGameState.GAME_STATE_WAITING_FOR_PLAYERS:
                    m_showScores = false;
                    //if (NetworkManager.singleton.matchSize == m_gameInfo)
                    //{
                    //    m_gameState = eGameState.GAME_STATE_STARTING_GAME;
                    //}

                    if (GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().isPlaying)
                    {
                        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Stop();
                    }
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, 0, -90), Time.deltaTime);
                    break;

                case eGameState.GAME_STATE_STARTING_GAME:
                    m_showScores = false;
                    if (isServer && !m_once)
                    {
                        m_once = true;
                        StartCoroutine("SpawnGameStart");
                        StartCoroutine("WaitForReadyPlayers");
                    }

                    break;
                case eGameState.GAME_STATE_SPAWN_PLAYERS:
                    m_showScores = false;
                    if (isServer && m_once)
                    {
                        m_once = false;
                        //TODO: Change for UNET
                        RpcSpawnPlayer();
                        //GetComponent<PhotonView>().RPC("SpawnPlayer", PhotonTargets.AllBuffered);
                    }

                    break;

                case eGameState.GAME_STATE_PLAYING_GAME:

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        m_showQuitMenu = !m_showQuitMenu;
                    }

                    if (Input.GetKey(KeyCode.Tab))
                    {
                        m_showScores = true;
                    }
                    else
                    {
                        m_showScores = false;
                    }

                    if (m_showQuitMenu)
                    {
                        m_quitMenu.SetActive(true);
                    }
                    else
                    {
                        m_quitMenu.SetActive(false);
                    }


                    if (!m_once)
                    {
                        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Play();
                        m_once = true;
                    }
                    if (isServer)
                    {
                        m_gameTime = m_gameInfo.GetGameTime();
                        m_gameTime -= Time.deltaTime;
                        m_gameInfo.SetGameTime(m_gameTime);
                        List<ShipController> team1Ships = new List<ShipController>();
                        List<ShipController> team2Ships = new List<ShipController>();
                        for (int i = 0; i < m_spawnedShips.Count; i++)
                        {
                            if (m_spawnedShips[i].m_team == 1)
                            {
                                team1Ships.Add(m_spawnedShips[i]);
                            }
                            else if (m_spawnedShips[i].m_team == 2)
                            {
                                team2Ships.Add(m_spawnedShips[i]);
                            }
                        }
                        bool team1IsDestroyed = true;
                        bool team2IsDestroyed = true;
                        for (int i = 0; i < team1Ships.Count; i++)
                        {
                            if (team1Ships[i].IsAlive())
                            {
                                team1IsDestroyed = false;
                                break;
                            }
                        }

                        for (int i = 0; i < team2Ships.Count; i++)
                        {
                            if (team2Ships[i].IsAlive())
                            {
                                team2IsDestroyed = false;
                                break;
                            }
                        }

                        if (m_gameTime <= 0 || team1IsDestroyed || team2IsDestroyed)
                        {
                            //TODO: Change for UNET
                            RpcEndGame();
                            //GetComponent<PhotonView>().RPC("EndGame", PhotonTargets.AllBuffered);
                        }
                    }
                    else
                    {
                        m_gameTime = m_gameInfo.GetGameTime();
                    }

                    break;

                case eGameState.GAME_STATE_GAME_OVER:
                    m_showScores = false;
                    if (m_once)
                    {
                        m_once = false;
                        m_team1Score = m_gameInfo.GetTeamScore(1);
                        m_team2Score = m_gameInfo.GetTeamScore(2);
                        GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().EndGame();
                        if (isServer)
                        {
                            if (m_team1Score > m_team2Score)
                            {
                                CmdAwardExperience(1);
                            }
                            else if (m_team2Score > m_team1Score)
                            {
                                CmdAwardExperience(2);
                            }
                            else
                            {
                                CmdAwardExperience(0);
                            }
                        }
                    }

                    if (isServer)
                    {
                        //m_gameTime -= Time.deltaTime;
                        //m_roomProperties = PhotonNetwork.room.customProperties;
                        //m_roomProperties["GameTime"] = m_gameTime;
                        //PhotonNetwork.room.SetCustomProperties(m_roomProperties);
                    }
                    else
                    {
                        m_gameTime = m_gameInfo.GetGameTime();
                    }
                    break;
                case eGameState.GAME_STATE_SPECTATING:
                    //if (Input.GetKey(KeyCode.Tab))
                    //{
                    //    m_showScores = true;
                    //}
                    //else
                    //{
                    //    m_showScores = false;
                    //}
                    //m_gameTime = m_gameInfo.GetGameTime();

                    //PhotonPlayer[] playerList = PhotonNetwork.playerList;
                    //List<PhotonPlayer> playerListList = new List<PhotonPlayer>();
                    //for (int i = 0; i < playerList.Length; i++)
                    //{
                    //    playerListList.Add(playerList[i]);
                    //}

                    //int count = 0;
                    //while (count < playerListList.Count)
                    //{
                    //    if (playerListList[count].customProperties["Team"] == null || (int)playerListList[count].customProperties["Team"] == 0)
                    //    {
                    //        playerListList.RemoveAt(count);
                    //    }
                    //    else
                    //    {
                    //        count++;
                    //    }
                    //}
                    //playerList = playerListList.ToArray().OrderByDescending(x => (int)x.customProperties["Score"]).ToArray();

                    //int playerIndex = -1;

                    //if (m_spectatingTarget != null)
                    //{
                    //    for (int i = 0; i < playerList.Length; i++)
                    //    {
                    //        if (playerList[i] == m_spectatingTarget)
                    //        {
                    //            playerIndex = i;
                    //            break;
                    //        }
                    //    }
                    //}

                    //if (Input.GetMouseButtonDown(0))
                    //{
                    //    if (playerIndex == 0 || playerIndex == -1)
                    //    {
                    //        playerIndex = playerList.Length - 1;
                    //    }
                    //    else
                    //    {
                    //        playerIndex--;
                    //    }
                    //}
                    //else if (Input.GetMouseButtonDown(1))
                    //{
                    //    if (playerIndex == playerList.Length - 1 || playerIndex == -1)
                    //    {
                    //        playerIndex = 0;
                    //    }
                    //    else
                    //    {
                    //        playerIndex++;
                    //    }
                    //}

                    //if (playerIndex != -1)
                    //    m_spectatingTarget = playerList[playerIndex];

                    break;
            }
            if (isClient)
            {
                m_team1Score = m_gameInfo.GetTeamScore(1);
                m_team2Score = m_gameInfo.GetTeamScore(2);
            }
        }

        public void AddSpawnedShip(ShipController aShip)
        {
            m_spawnedShips.Add(aShip);
        }

        private GameObject m_spectatingTarget;

        void LateUpdate()
        {
            //if (m_gameState == eGameState.GAME_STATE_SPECTATING)
            //{
            //    Vector3 camPos = Camera.main.transform.position;

            //    GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");
            //    Vector3 targetPos = Vector3.zero;
            //    if (planes.Length > 0)
            //    {
            //        for (int i = 0; i < planes.Length; i++)
            //        {
            //            if (planes[i].GetComponent<PhotonView>().owner == m_spectatingTarget)
            //            {
            //                targetPos = planes[i].transform.position;
            //            }
            //            planes[i].transform.FindChild("NameTag").GetComponent<TextMesh>().characterSize = 0.14f;
            //        }
            //    }
            //    else
            //    {
            //        targetPos = camPos;
            //    }

            //    camPos = Vector3.Lerp(Camera.main.transform.position, new Vector3(targetPos.x, targetPos.y, -180), 5 * Time.deltaTime);

            //    Camera.main.transform.position = camPos;
            //}
        }

        [ClientRpc]
        void RpcEndGame()
        {
            StopCoroutine("RespawnPlayer");
            m_gameState = eGameState.GAME_STATE_GAME_OVER;
            m_allyShipSunk.SetActive(false);
            m_enemyShipSunk.SetActive(false);
            m_redShipLogo.SetActive(false);
            m_greenShipLogo.SetActive(false);
            GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().DestroyPlayerPlane();
        }

        [ClientRpc]
        void RpcResetGame()
        {
            m_gameInfo.SetPlaying(0);
            GameObject.Find("Version Text").transform.SetParent(null);
            GameObject.Find("FullScreen").transform.SetParent(null);
            //TODO: Change for UNET
            //PhotonNetwork.LoadLevel("Game");
        }

        [Command]
        public void CmdHitShipTargetPoint(ShipController.ShipTarget aShipTarget, BombController.BombInfo aBombInfo)
        {
            RpcHitShipTargetPoint(aShipTarget, aBombInfo);
            //GetComponent<PhotonView>().RPC("HitShipTargetPointRPC", PhotonTargets.AllBuffered, aShipTarget, aBombInfo);
        }

        [Command]
        public void CmdRespawnShip(int aShipID)
        {
            ShipController ship = null;
            for (int i = 0; i < m_spawnedShips.Count; i++)
            {
                if (m_spawnedShips[i].m_shipID == aShipID)
                {
                    ship = m_spawnedShips[i];
                    break;
                }
            }

            if (ship == null)
            {
                return;
            }
            ship.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            m_spawnedShips.Remove(ship);
            if (isServer)
            {
                ship.SetShipType(ship.GetShipType(), ship.m_team, aShipID);
            }
            RpcRespawnShip(aShipID);
            //GetComponent<PhotonView>().RPC("RespawnShipRPC", PhotonTargets.AllBuffered, aShip.m_shipID); 
        }

        [ClientRpc]
        void RpcRespawnShip(int aShipID)
        {
            ShipController ship = null;
            for (int i = 0; i < m_spawnedShips.Count; i++)
            {
                if (m_spawnedShips[i].m_shipID == aShipID)
                {
                    ship = m_spawnedShips[i];
                    break;
                }
            }

            if (ship == null)
            {
                return;
            }
            ship.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            m_spawnedShips.Remove(ship);
        }

        [ClientRpc]
        void RpcHitShipTargetPoint(ShipController.ShipTarget aShipTarget, BombController.BombInfo aBombInfo)
        {
            ShipController.ShipTarget shipTarget = null;
            GameObject ship = null;

            for (int i = 0; i < m_spawnedShips.Count; i++)
            {
                if (m_spawnedShips[i].ContainsShipTarget(aShipTarget))
                {
                    shipTarget = m_spawnedShips[i].GetShipTarget(aShipTarget);
                    ship = m_spawnedShips[i].gameObject;
                    break;
                }
            }
            //if (aBombInfo.m_shooter == BombersNetworkManager)
            {
                m_bombsHit++;
                //m_playerProperties["Score"] = (int)m_playerProperties["Score"] + GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_pointsForWeakpointDestruction;
            }

            //if (PhotonNetwork.isMasterClient)
            //{
            //    if ((int)aBombInfo.m_shooter.customProperties["Team"] == 1)
            //    {
            //        m_team1Score += GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_pointsForWeakpointDestruction;
            //        m_roomProperties["Team1Score"] = m_team1Score;
            //    }
            //    else if ((int)aBombInfo.m_shooter.customProperties["Team"] == 2)
            //    {
            //        m_team2Score += GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_pointsForWeakpointDestruction;
            //        m_roomProperties["Team2Score"] = m_team2Score;
            //    }
            //}

            Plane[] frustrum = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            if (GeometryUtility.TestPlanesAABB(frustrum, ship.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>().bounds))
            {
                GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().ShakeCamera(GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_weakpointIntensity, GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_shakeTime);
            }

            if (shipTarget == null) return;
            GameObject explosion = (GameObject)Instantiate((GameObject)Resources.Load("WeakpointExplosion"), shipTarget.m_position.position, shipTarget.m_position.rotation);
            explosion.transform.parent = ship.transform;
            explosion.GetComponent<AudioSource>().Play();
            Destroy(shipTarget.m_targetGraphic);

        }

        public GameObject GetClosestEnemyShip(Vector3 aPosition, int aTeam)
        {
            GameObject ship = null;
            float minDistance = 100000;
            for (int i = 0; i < m_spawnedShips.Count; i++)
            {
                if (m_spawnedShips[i].IsAlive() && m_spawnedShips[i].m_team != aTeam && (m_spawnedShips[i].gameObject.transform.position - aPosition).magnitude < minDistance)
                {
                    minDistance = (m_spawnedShips[i].gameObject.transform.position - aPosition).magnitude;
                    ship = m_spawnedShips[i].gameObject;
                }
            }
            return ship;
        }

        IEnumerator FadeOutShipMessage(GameObject aText, GameObject aLogo)
        {
            float time = 0.5f;
            m_allyShipSunk.SetActive(true);
            m_enemyShipSunk.SetActive(true);
            m_redShipLogo.SetActive(true);
            m_greenShipLogo.SetActive(true);
            m_allyShipSunk.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            m_enemyShipSunk.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            m_redShipLogo.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            m_greenShipLogo.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            Color fadeColor = new Color(1, 1, 1, 0);
            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                fadeColor = new Color(1, 1, 1, fadeColor.a + Time.fixedDeltaTime * 2.4f);
                aText.GetComponent<Image>().color = fadeColor;
                aLogo.GetComponent<Image>().color = fadeColor;
                yield return new WaitForFixedUpdate();
            }
            time = 2;
            aText.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            aLogo.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            fadeColor = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(2);

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                fadeColor = new Color(1, 1, 1, fadeColor.a - Time.fixedDeltaTime);
                aText.GetComponent<Image>().color = fadeColor;
                aLogo.GetComponent<Image>().color = fadeColor;
                yield return new WaitForFixedUpdate();
            }
        }

        [Command]
        public void CmdDestroyedShip(int aShipID, BombController.BombInfo aBombInfo)
        {
            RpcDestroyedShip(aShipID, aBombInfo);
            //GetComponent<PhotonView>().RPC("DestroyedShipRPC", PhotonTargets.AllBuffered, aShip.m_shipID, aBombInfo);
        }

        [ClientRpc]
        void RpcDestroyedShip(int aShipID, BombController.BombInfo aBombInfo)
        {
            ShipController ship = null;
            for (int i = 0; i < m_spawnedShips.Count; i++)
            {
                if (m_spawnedShips[i].m_shipID == aShipID)
                {
                    ship = m_spawnedShips[i];
                    break;
                }
            }
            //if (isServer)
            //{
            //    if ((int)aBombInfo.m_shooter.customProperties["Team"] == 1)
            //    {
            //        m_team1Score += GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_pointsForShipDestruction;
            //        m_roomProperties["Team1Score"] = m_team1Score;
            //    }
            //    else
            //    {
            //        m_team2Score += GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_pointsForShipDestruction;
            //        m_roomProperties["Team2Score"] = m_team2Score;
            //    }
            //}

            Plane[] frustrum = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            if (GeometryUtility.TestPlanesAABB(frustrum, ship.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>().bounds))
            {
                GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().ShakeCamera(GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_shipIntensity, GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_shakeTime);
            }

            if (ship == null) return;

            ship.m_isAlive = false;
            StopCoroutine("FadeOutShipMessage");
            if (ship.m_team == 1)
            {
                if (BombersNetworkManager.m_localPlayer.m_team == 1)
                {
                    StartCoroutine(FadeOutShipMessage(m_allyShipSunk, m_greenShipLogo));
                }
                else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                {
                    StartCoroutine(FadeOutShipMessage(m_enemyShipSunk, m_greenShipLogo));
                }
            }
            else
            {
                if (BombersNetworkManager.m_localPlayer.m_team == 1)
                {
                    StartCoroutine(FadeOutShipMessage(m_enemyShipSunk, m_redShipLogo));
                }
                else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                {
                    StartCoroutine(FadeOutShipMessage(m_allyShipSunk, m_redShipLogo));
                }
            }


            string shipName = "";
            ship.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            int children = ship.transform.childCount;
            for (int i = 1; i < children; i++)
            {
                ship.transform.GetChild(i).GetChild(0).GetChild(4).GetComponent<ParticleSystem>().enableEmission = false;
            }
            Destroy(ship.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject);
            GameObject explosion;
            string path = "";
            switch (ship.GetShipType())
            {
                case ShipController.eShipType.SHIP_TYPE_CARRIER:


                    if ((int)aBombInfo.m_shooter == 1)
                    {
                        shipName += "Red ";
                        path = "CarrierExplosion02";
                    }
                    else
                    {
                        shipName += "Green ";
                        path = "CarrierExplosion01";
                    }
                    explosion = (GameObject)Instantiate((GameObject)Resources.Load(path), ship.transform.position, ship.transform.rotation);
                    explosion.GetComponent<AudioSource>().Play();
                    shipName += "Carrier";
                    break;
                case ShipController.eShipType.SHIP_TYPE_BATTLESHIP:

                    if ((int)aBombInfo.m_shooter == 1)
                    {
                        shipName += "Red ";
                        path = "BattleshipExplosion02";
                    }
                    else
                    {
                        shipName += "Green ";
                        path = "BattleshipExplosion01";
                    }
                    explosion = (GameObject)Instantiate((GameObject)Resources.Load(path), ship.transform.position, ship.transform.rotation);
                    explosion.GetComponent<AudioSource>().Play();
                    shipName += "Battleship";
                    break;
                case ShipController.eShipType.SHIP_TYPE_CRUISER:
                    if ((int)aBombInfo.m_shooter == 1)
                    {
                        shipName += "Red ";
                        path = "CruiserExplosion02";
                    }
                    else
                    {
                        shipName += "Green ";
                        path = "CruiserExplosion01";
                    }
                    explosion = (GameObject)Instantiate((GameObject)Resources.Load(path), ship.transform.position, ship.transform.rotation);
                    explosion.GetComponent<AudioSource>().Play();
                    shipName += "Cruiser";
                    break;
                case ShipController.eShipType.SHIP_TYPE_PATROLBOAT:
                    if ((int)aBombInfo.m_shooter == 1)
                    {
                        shipName += "Red ";
                        path = "PatrolBoatExplosion02";
                    }
                    else
                    {
                        shipName += "Green ";
                        path = "PatrolBoatExplosion01";
                    }
                    explosion = (GameObject)Instantiate((GameObject)Resources.Load(path), ship.transform.position, ship.transform.rotation);
                    explosion.GetComponent<AudioSource>().Play();
                    shipName += "Patrol Boat";
                    break;
                case ShipController.eShipType.SHIP_TYPE_DESTROYER:
                    if ((int)aBombInfo.m_shooter == 1)
                    {
                        shipName += "Red ";
                        path = "DestroyerExplosion02";
                    }
                    else
                    {
                        shipName += "Green ";
                        path = "DestroyerExplosion01";
                    }
                    explosion = (GameObject)Instantiate((GameObject)Resources.Load(path), ship.transform.position, ship.transform.rotation);
                    explosion.GetComponent<AudioSource>().Play();
                    shipName += "Destroyer";
                    break;
            }

            if (isServer)
                ship.StartRespawn();

            //if (aBombInfo.m_shooter == PhotonNetwork.player)
            {
                m_carriersDestroyed++;
                //BombersNetworkManager.m_localPlayer.m_score += GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_pointsForShipDestruction;
                //m_playerProperties["Score"] = (int)m_playerProperties["Score"] + GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_pointsForShipDestruction;
            }
        }

        [Command]
        void CmdAwardExperience(int aWinningTeam)
        {
            RpcAwardExperience(aWinningTeam);
            //GetComponent<PhotonView>().RPC("AwardExperienceRPC", PhotonTargets.All, aWinningTeam);
        }

        [ClientRpc]
        void RpcAwardExperience(int aWinningTeam)
        {
            if (BombersNetworkManager.m_localPlayer.m_team == 0) return;

            m_timesDestroyed = BombersNetworkManager.m_localPlayer.m_deaths;
            m_planesDestroyed = BombersNetworkManager.m_localPlayer.m_kills;
            int gamesWon = (BombersNetworkManager.m_localPlayer.m_team == aWinningTeam) ? 1 : 0;
            if (m_planesDestroyed >= 5)
            {
                GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().Get5KillsAchievement();
            }
            GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().IncrementStatisticsToBrainCloud(1, gamesWon, m_timesDestroyed, m_shotsFired, m_bombsDropped, m_planesDestroyed, m_carriersDestroyed, m_bombsHit);
            GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().IncrementExperienceToBrainCloud(m_planesDestroyed * GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().m_expForKill);
            GameObject.Find("BrainCloudStats").GetComponent<BrainCloudStats>().SubmitLeaderboardData(m_planesDestroyed, m_bombsHit, m_timesDestroyed);
            m_shotsFired = 0;
            m_bombsDropped = 0;
            m_bombsHit = 0;
            m_planesDestroyed = 0;
            m_carriersDestroyed = 0;
            m_timesDestroyed = 0;
        }

        [Command]
        public void CmdSpawnFlare(Vector3 aPosition, Vector3 aVelocity, int aPlayerID)
        {
            RpcSpawnFlare(aPosition, aVelocity, aPlayerID);
            //GetComponent<PhotonView>().RPC("SpawnFlareRPC", PhotonTargets.All, aPosition, aVelocity, PhotonNetwork.player);
        }

        [ClientRpc]
        void RpcSpawnFlare(Vector3 aPosition, Vector3 aVelocity, int aPlayer)
        {
            GameObject flare = (GameObject)Instantiate((GameObject)Resources.Load("Flare"), aPosition, Quaternion.identity);
            flare.GetComponent<FlareController>().Activate(aPlayer);
            flare.GetComponent<Rigidbody>().velocity = aVelocity;
        }

        [ClientRpc]
        void RpcGetReady()
        {
            m_gameState = eGameState.GAME_STATE_STARTING_GAME;
            BombersNetworkManager.m_localPlayer.m_deaths = 0;
            BombersNetworkManager.m_localPlayer.m_kills = 0;
        }

        [ClientRpc]
        void RpcSpawnPlayer()
        {
            if (BombersNetworkManager.m_localPlayer.m_team == 0)
            {
                m_gameState = eGameState.GAME_STATE_SPECTATING;
            }
            else
            {
                Vector3 spawnPoint = Vector3.zero;
                spawnPoint.z = 22;

                if (BombersNetworkManager.m_localPlayer.m_team == 1)
                {
                    spawnPoint.x = Random.Range(m_team1SpawnBounds.bounds.center.x - m_team1SpawnBounds.bounds.size.x / 2, m_team1SpawnBounds.bounds.center.x + m_team1SpawnBounds.bounds.size.x / 2) - 10;
                    spawnPoint.y = Random.Range(m_team1SpawnBounds.bounds.center.y - m_team1SpawnBounds.bounds.size.y / 2, m_team1SpawnBounds.bounds.center.y + m_team1SpawnBounds.bounds.size.y / 2);
                }
                else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                {
                    spawnPoint.x = Random.Range(m_team2SpawnBounds.bounds.center.x - m_team2SpawnBounds.bounds.size.x / 2, m_team2SpawnBounds.bounds.center.x + m_team2SpawnBounds.bounds.size.x / 2) + 10;
                    spawnPoint.y = Random.Range(m_team2SpawnBounds.bounds.center.y - m_team2SpawnBounds.bounds.size.y / 2, m_team2SpawnBounds.bounds.center.y + m_team2SpawnBounds.bounds.size.y / 2);
                }

                GameObject playerPlane = (GameObject)Instantiate((GameObject)Resources.Load("Plane"), spawnPoint, Quaternion.LookRotation(Vector3.forward, (new Vector3(0, 0, 22) - spawnPoint)));
                NetworkServer.Spawn(playerPlane);
                if (BombersNetworkManager.m_localPlayer.m_team == 1)
                {
                    playerPlane.layer = 8;
                }
                else if (BombersNetworkManager.m_localPlayer.m_team == 2)
                {
                    playerPlane.layer = 9;
                }
                BombersNetworkManager.m_localPlayer.SetPlayerPlane(playerPlane.GetComponent<PlaneController>());
                playerPlane.GetComponent<Rigidbody>().isKinematic = false;
                m_gameState = eGameState.GAME_STATE_PLAYING_GAME;
            }
        }

        [Command]
        public void CmdespawnBombPickup(int aPickupID)
        {
            RpcDespawnBombPickup(aPickupID);
            //GetComponent<PhotonView>().RPC("DespawnBombPickupRPC", PhotonTargets.All, aPickupID);
        }

        [ClientRpc]
        void RpcDespawnBombPickup(int aPickupID)
        {
            for (int i = 0; i < m_bombPickupsSpawned.Count; i++)
            {
                if (m_bombPickupsSpawned[i].m_pickupID == aPickupID)
                {
                    Destroy(m_bombPickupsSpawned[i].gameObject);
                    m_bombPickupsSpawned.RemoveAt(i);
                    break;
                }
            }
        }

        [Command]
        public void CmdSpawnBombPickup(Vector3 aPosition, int playerID)
        {
            int bombID = Random.Range(-20000000, 20000000) * 100 + playerID;
            RpcSpawnBombPickup(aPosition, bombID);
            //GetComponent<PhotonView>().RPC("SpawnBombPickupRPC", PhotonTargets.All, aPosition, bombID);
        }

        [ClientRpc]
        void RpcSpawnBombPickup(Vector3 aPosition, int bombID)
        {
            GameObject bombPickup = (GameObject)Instantiate((GameObject)Resources.Load("BombPickup"), aPosition, Quaternion.identity);
            bombPickup.GetComponent<BombPickup>().Activate(bombID);
            m_bombPickupsSpawned.Add(bombPickup.GetComponent<BombPickup>());
        }

        [Command]
        public void CmdBombPickedUp(int aPlayer, int aPickupID)
        {
            RpcBombPickedUp(aPlayer, aPickupID);
            //GetComponent<PhotonView>().RPC("BombPickedUpRPC", PhotonTargets.All, aPlayer, aPickupID);
        }

        [ClientRpc]
        void RpcBombPickedUp(int aPlayer, int aPickupID)
        {
            for (int i = 0; i < m_bombPickupsSpawned.Count; i++)
            {
                if (m_bombPickupsSpawned[i].m_pickupID == aPickupID)
                {
                    Destroy(m_bombPickupsSpawned[i].gameObject);
                    m_bombPickupsSpawned.RemoveAt(i);
                    break;
                }
            }

            //if (aPlayer == PhotonNetwork.player)
            {
                GameObject.Find("PlayerController").GetComponent<WeaponController>().AddBomb();
            }
        }

        [Command]
        public void CmdSpawnBomb(BombController.BombInfo aBombInfo)
        {
            m_bombsDropped++;
            int id = GetNextBombID();
            aBombInfo.m_bombID = id;
            RpcSpawnBomb(aBombInfo);
            //GetComponent<PhotonView>().RPC("SpawnBombRPC", PhotonTargets.All, aBombInfo);
        }

        [ClientRpc]
        void RpcSpawnBomb(BombController.BombInfo aBombInfo)
        {
            if (isServer)
            {
                aBombInfo.m_isMaster = true;
            }

            GameObject bomb = GameObject.Find("PlayerController").GetComponent<WeaponController>().SpawnBomb(aBombInfo);
            m_spawnedBombs.Add(bomb.GetComponent<BombController>().GetBombInfo());
            int playerTeam = (int)aBombInfo.m_shooter;//.customProperties["Team"];

            switch (playerTeam)
            {
                case 1:
                    bomb.layer = 14;
                    break;
                case 2:
                    bomb.layer = 15;
                    break;
            }
        }

        [Command]
        public void CmdDeleteBomb(BombController.BombInfo aBombInfo, int aHitSurface)
        {
            RpcDeleteBomb(aBombInfo, aHitSurface);
            //GetComponent<PhotonView>().RPC("DeleteBombRPC", PhotonTargets.All, aBombInfo, aHitSurface);
        }

        [ClientRpc]
        void RpcDeleteBomb(BombController.BombInfo aBombInfo, int aHitSurface)
        {
            if (m_spawnedBombs.Contains(aBombInfo))
            {
                int index = m_spawnedBombs.IndexOf(aBombInfo);
                GameObject bomb = m_spawnedBombs[index].gameObject;
                GameObject explosion;
                if (!bomb.GetComponent<BombController>().m_isActive)
                {
                    if (aHitSurface == 0)
                    {
                        explosion = (GameObject)Instantiate((GameObject)Resources.Load("BombWaterExplosion"), bomb.transform.position, Quaternion.identity);
                        explosion.GetComponent<AudioSource>().Play();
                    }
                    else if (aHitSurface == 1)
                    {
                        explosion = (GameObject)Instantiate((GameObject)Resources.Load("BombExplosion"), bomb.transform.position, Quaternion.identity);
                        explosion.GetComponent<AudioSource>().Play();
                    }
                    else
                    {
                        explosion = (GameObject)Instantiate((GameObject)Resources.Load("BombDud"), bomb.transform.position, Quaternion.identity);
                    }
                }
                Destroy(bomb);
                m_spawnedBombs.Remove(aBombInfo);
            }
        }

        [Command]
        public void CmdSpawnBullet(BulletController.BulletInfo aBulletInfo)
        {
            m_shotsFired++;
            int id = GetNextBulletID();
            aBulletInfo.m_bulletID = id;
            RpcSpawnBullet(aBulletInfo);
            //GetComponent<PhotonView>().RPC("SpawnBulletRPC", PhotonTargets.All, aBulletInfo);
        }

        [ClientRpc]
        void RpcSpawnBullet(BulletController.BulletInfo aBulletInfo)
        {
            //if (PhotonNetwork.player == aBulletInfo.m_shooter)
            {
                aBulletInfo.m_isMaster = true;
            }

            GameObject bullet = GameObject.Find("PlayerController").GetComponent<WeaponController>().SpawnBullet(aBulletInfo);
            m_spawnedBullets.Add(bullet.GetComponent<BulletController>().GetBulletInfo());
            int playerTeam = (int)aBulletInfo.m_shooter;//.customProperties["Team"];

            //if (PhotonNetwork.player != aBulletInfo.m_shooter)
            {
                bullet.GetComponent<Collider>().isTrigger = true;
            }

            switch (playerTeam)
            {
                case 1:
                    bullet.layer = 10;
                    break;
                case 2:
                    bullet.layer = 11;
                    break;
            }
        }

        [Command]
        public void CmdDeleteBullet(BulletController.BulletInfo aBulletInfo)
        {
            RpcDeleteBullet(aBulletInfo);
            //GetComponent<PhotonView>().RPC("DeleteBulletRPC", PhotonTargets.All, aBulletInfo);
        }

        [ClientRpc]
        void RpcDeleteBullet(BulletController.BulletInfo aBulletInfo)
        {
            if (m_spawnedBullets.Contains(aBulletInfo))
            {
                int index = m_spawnedBullets.IndexOf(aBulletInfo);
                GameObject bullet = m_spawnedBullets[index].gameObject;
                Destroy(bullet);
                m_spawnedBullets.Remove(aBulletInfo);
            }
        }

        [Command]
        public void CmdBulletHitPlayer(BulletController.BulletInfo aBulletInfo, Collision aCollision)
        {
            aBulletInfo.gameObject.transform.parent = aCollision.gameObject.transform;
            Vector3 relativeHitPoint = aBulletInfo.gameObject.transform.localPosition;
            int hitPlayer = 0;//aCollision.gameObject.GetComponent<PhotonView>().owner;
            int shooter = aBulletInfo.m_shooter;
            aBulletInfo.gameObject.transform.parent = null;
            CmdDeleteBullet(aBulletInfo);
            RpcBulletHitPlayer(relativeHitPoint, aBulletInfo, shooter, hitPlayer);
            //GetComponent<PhotonView>().RPC("BulletHitPlayerRPC", PhotonTargets.All, relativeHitPoint, aBulletInfo, shooter, hitPlayer);
        }

        [ClientRpc]
        void RpcBulletHitPlayer(Vector3 aHitPoint, BulletController.BulletInfo aBulletInfo, int aShooter, int aHitPlayer)
        {
            foreach (GameObject plane in GameObject.FindGameObjectsWithTag("Plane"))
            {
                //if (plane.GetComponent<PhotonView>().owner == aHitPlayer)
                {
                    Instantiate((GameObject)Resources.Load("BulletHit"), plane.transform.position + aHitPoint, Quaternion.LookRotation(aBulletInfo.m_startDirection, -Vector3.forward));
                    break;
                }
            }

            //if (aHitPlayer == PhotonNetwork.player)
            {
               // GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().TakeBulletDamage(aShooter);
            }
        }

        [Command]
        public void CmdDestroyPlayerPlane(int aVictim, int aShooterID)
        {
            RpcDestroyPlayerPlane(aVictim, aShooterID);
            //GetComponent<PhotonView>().RPC("DestroyPlayerPlaneRPC", PhotonTargets.All, aVictim, aShooter);
        }

        [ClientRpc]
        void RpcDestroyPlayerPlane(int aVictim, int aShooter)
        {
            foreach (GameObject plane in GameObject.FindGameObjectsWithTag("Plane"))
            {
                //if (plane.GetComponent<PhotonView>().owner == aVictim)
                {
                    GameObject explosion = (GameObject)Instantiate((GameObject)Resources.Load("PlayerExplosion"), plane.transform.position, plane.transform.rotation);
                    explosion.GetComponent<AudioSource>().Play();
                    break;
                }
            }

            //if (m_gameState == eGameState.GAME_STATE_SPECTATING)
            //{
            //    if (m_spectatingTarget == aVictim)
            //        m_spectatingTarget = aShooter;
            //}

            if (aShooter == -1)
            {
                //if (aVictim == PhotonNetwork.player)
                {
                    GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().DestroyPlayerPlane();
                    BombersNetworkManager.m_localPlayer.m_deaths += 1;
                    StopCoroutine("RespawnPlayer");
                    StartCoroutine("RespawnPlayer");
                }
            }
            else
            {

                //if (aVictim == PhotonNetwork.player)
                {
                    GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().DestroyPlayerPlane();
                    BombersNetworkManager.m_localPlayer.m_deaths += 1;
                    StopCoroutine("RespawnPlayer");
                    StartCoroutine("RespawnPlayer");
                }
                //else if (aShooter == PhotonNetwork.player)
                {
                    BombersNetworkManager.m_localPlayer.m_kills += 1;
                }
            }
        }

        int GetNextBulletID()
        {
            return Random.Range(-20000000, 20000000) * 100;// +PhotonNetwork.player.ID;
        }

        int GetNextBombID()
        {
            return Random.Range(-20000000, 20000000) * 100;// +PhotonNetwork.player.ID;
        }

        IEnumerator WaitForReadyPlayers()
        {
            bool playersReady = false;

            while (!playersReady)
            {
                //GameObject[] playerList = PhotonNetwork.playerList.OrderBy(x => x.ID).ToArray();

                playersReady = true;
                for (int i = 0; i < m_gameInfo.GetMaxPlayers(); i++)
                {
                    //if (i < playerList.Length)
                    {
                        //if (playerList[i].customProperties["IsReady"] == null) playersReady = false;
                        break;
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }

            m_gameState = eGameState.GAME_STATE_SPAWN_PLAYERS;
        }

    }
}