﻿using UnityEngine;
using System.Collections.Generic;
using BrainCloud;
using BrainCloud.Common;
using BrainCloud.Entity;

public class ScreenEntityCustomClass : BCScreen
{
    public class Hobby
    {
        public string Name
        {
            get { return ""; }
        }
    }

    public class Player : BCUserEntity
    {
        public static string ENTITY_TYPE = "player";

        public Player(BrainCloudEntity in_bcEntityService) : base(in_bcEntityService)
        {
            // set up some defaults
            m_entityType = "player";
            Name = "";
            Age = 0;
            Hobbies = new List<Hobby>();
        }

        public string Name
        {
            get { return (string) this ["name"]; }
            set { this ["name"] = value; }
        }

        public int Age
        {
            get { return (int) this ["age"]; }
            set { this ["age"] = value; }
        }

        public IList<Hobby> Hobbies
        {
            get { return this.Get<IList<Hobby>>("hobbies"); }
            set { this["hobbies"] = value; }
        }
    }


    private Player m_player;
    
    public ScreenEntityCustomClass(BrainCloudWrapper bc) : base(bc) { }

    public override void Activate()
    {
        _bc.EntityFactory.RegisterEntityClass<Player>(Player.ENTITY_TYPE);

        _bc.PlayerStateService.ReadUserState(ReadPlayerStateSuccess, Failure_Callback);
        m_mainScene.AddLogNoLn("[ReadPlayerState]... ");

    }

    private void ReadPlayerStateSuccess(string json, object cb)
    {
        m_mainScene.AddLog("SUCCESS");
        m_mainScene.AddLogJson(json);
        m_mainScene.AddLog("");

        // look for the player entity
        IList<BCUserEntity> entities = _bc.EntityFactory.NewUserEntitiesFromReadPlayerState(json);
        foreach (BCUserEntity e in entities)
        {
            if (e.EntityType == Player.ENTITY_TYPE)
            {
                m_player = (Player)e;
            }
        }
    }

    public override void OnScreenGUI()
    {
        GUILayout.BeginVertical();

        int minLabelWidth = 60;

        // entity id
        GUILayout.BeginHorizontal();
        GUILayout.Label("Id", GUILayout.Width(minLabelWidth));
        GUILayout.Box(m_player != null ? m_player.EntityId : "---");
        GUILayout.EndHorizontal();

        // entity type
        GUILayout.BeginHorizontal();
        GUILayout.Label("Type", GUILayout.Width(minLabelWidth));
        GUILayout.Box(m_player != null ? m_player.EntityType : "---");
        GUILayout.EndHorizontal();

        // entity property 'name'
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(minLabelWidth));
        if (m_player != null)
        {
            m_player.Name = GUILayout.TextField((string)m_player.Name);
        } else
        {
            GUILayout.Box("---");
        }
        GUILayout.EndHorizontal();

        // entity property 'age'
        GUILayout.BeginHorizontal();
        GUILayout.Label("Age", GUILayout.Width(minLabelWidth));
        if (m_player != null)
        {
            string ageStr = GUILayout.TextField(((int)m_player.Age).ToString());
            int ageInt = 0;
            if (int.TryParse(ageStr, out ageInt))
            {
                m_player.Age = ageInt;
            }
        } else
        {
            GUILayout.Box("---");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (m_player == null)
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Entity"))
            {
                m_player = new Player(ConnectScene._bc.EntityService);
                m_player.Name = "Johnny Philharmonica";
                m_player.Age = 49;
            }
        }
        if (m_player != null)
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save Entity"))
            {
                m_mainScene.AddLogNoLn("[Entity.StoreAsync()]... ");
                m_player.StoreAsync(Success_Callback, Failure_Callback);
            }
            if (GUILayout.Button("Delete Entity"))
            {
                m_player.DeleteAsync(Success_Callback, Failure_Callback);
                m_player = null;
                m_mainScene.AddLogNoLn("[Entity.DeleteEntity]... ");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
