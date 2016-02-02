﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum clientStatuses {paired=0, groupsSynced, uiReady, playerReady, waiting, running, paused, ended}
public enum sessionStatuses {created=0, allPaired, allGroupsSynced, allUiReady, allPlayersReady, running, paused, ended}

public class MongoSchema: Meteor.MongoDocument {
	public string key;
	public System.Guid _GUID;
}

//class used when first opening session
public class OpenSessionResponse : Meteor.MongoDocument {
	public bool success;
	public string sessionID;
	public string clientID;
	public string sessionName;
}

//channel response for any record changes
public class ChannelTemplate : Meteor.MongoDocument {
	public string session_id = "";
	public string sender_id = "";
	public string receiver_id = "";
	public string payload = "";
}

//Channel Repsonse class returned by reportToTabletopClient
public class ChannelResponse : Meteor.MongoDocument {
	public bool success;
	public string message = "";
}

//Parameters that a client record has in the client channel
public class ClientTemplate : Meteor.MongoDocument {
	public string sessionID = "";
	public string deviceType = "";
	public string deviceID = "";
	public string state = "";
	public string currentPlayer = "";
}

//parameters that a session record has in the session channel
public class SessionTemplate : Meteor.MongoDocument {
	public string appID = "";
	public string tabletopDeviceID = "";
	public List<string> groups = new List<string>();
	public List<string> players = new List<string>();
	public string currentPlayer = "";
	public string name = "";
}

//used when creating playerchannel
public class PlayerTemplate : Meteor.MongoDocument {
	public string playerID = "";
	public string playerName = "";
	public string session_id;
}

public class ActorTemplate : MongoSchema{
	public string ActorID="";
	public string ActorName = "";
	public string playerID = "";
	public string playerName = "";
	public int HP = 0;
	public int AC = 0;
	public int MaxMove = 0;
	public int currentMove = 0;
	public bool playerTurn = false;
	public bool onSurface = false;

	public override Dictionary<string, object> toDictionary() {
		Dictionary<string, object> dictionary = new Dictionary<string, object> () {
			{"key",key}, // REQUIRED!!!!
			{"_GUID", _GUID}, // REQUIRED!!!!
			{"actorName",ActorName},
			{"actorId",ActorID},
			{"playerName", playerName},
			{"playerId", playerID},
			{"onSurface", onSurface},
			{"HP", HP},
			{"AC", AC},
			{"currentMove",currentMove},
			{"maxMove",MaxMove}
		};	
		return dictionary;
	}
}
