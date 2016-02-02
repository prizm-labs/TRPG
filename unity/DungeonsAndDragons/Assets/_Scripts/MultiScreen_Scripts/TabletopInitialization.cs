using UnityEngine;
using System.Collections;
using Extensions;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Net;
using Meteor;

public class TabletopInitialization : MonoBehaviour {
	private const string typeName = "MultiScreenDemo";
	public string gameName = "D&DGame";

	private bool readyToBindDDP = false;
	
	//Tables in database
	public Collection<ChannelTemplate> channelRecordGroup;	//how each client 'talks' to each other
	public Collection<ClientTemplate> clientRecordGroup;		//list of clients connected in session
	public Collection<SessionTemplate> sessionRecordGroup;	//keep track of session(s)
	public Collection<PlayerTemplate> playerRecordGroup;		//list of players
	//wrapper of collection
	public PrizmRecordGroup<ActorTemplate> actorRecordGroup; //list of actors

	//subscriptions 
	Subscription channelSubscription;
	Subscription sessionSubscription;
	Subscription clientSubscription;
	Subscription playerSubscription;
	Subscription actorSubscription;

	public Subscription recordGroupSubscription;		//used by PrizmRecordGroup
	
	//URL of Meteor Server
	public string meteorURL = "ws://localhost:6969/websocket";	//thats not michaels' laptop for debugging
	public string appID = "Dungeons&Dragons";							//name of games
	public string sessionID = "defaultSessionID";				//default string initialized for debugging  (gets assigned from server)
	public string clientID = "defaultClientID";					//default string initialized for debugging  (gets assigned from server)
	public string DDPConnectionID = "defaultDDPConnectionID";	//default string initialized for debugging  (gets assigned from server)
	public string deviceID = "defaultDeviceID";					//default string initialized for debugging  (gets assigned from unique identifier)
	public string sessionName = "defaultSessionName";

	//Sync routine variables	
	bool gameSynced = false;									//developer can choose to use (or not)
	clientStatuses clientSyncPosition = (clientStatuses)0;		//keeps track of position in client sync routine
	sessionStatuses sessionSyncPosition = (sessionStatuses)0;	//keeps track of position in client sync routine
	int playersSynced = 0;										//tabletop needs to keep track of how many players are synced
	
	int numberOfPlayers;								//is determined by userIDs that are in list
	
	//meteor connection code needs to be the first to run and connect to database (all further actions depend on finishing this initialization)
	void Awake() {		
		deviceID = SystemInfo.deviceUniqueIdentifier;	//assign device UID for server
	}

	public IEnumerator MeteorInit() {
		LiveData.Instance.DidConnect += (string connectionID) => {	//recieved DDPConnectionID from Meteor
			Debug.Log("METEORCONNECTION TRIGGERED!!!!!!!!");
			DDPConnectionID = connectionID;
			Debug.Log ("received ddpconnectionID: " + DDPConnectionID);		//is this the same ddpconnectionID that is returned by bindClientToDDPConnection?
			readyToBindDDP = true;
		};
		
		Debug.Log ("didconnect handler is added");
		Debug.LogError ("connecting to meteor");
		yield return Connection.Connect (meteorURL);		//establish initial connection to database
		Debug.LogError ("past connect");
		yield return StartCoroutine (OpenSession ());			//creates session document(s) on meteor side
		StartServer ();	

		//creates all collections on Unity side
		yield return StartCoroutine(CreateChannelRecordGroup("channels")); //Create channel documents hand held to hand held subscription
		yield return StartCoroutine (CreateClientRecordGroup ("clients"));		//Create clients document
		yield return StartCoroutine (CreateSessionRecordGroup ("sessions"));	//Create document
		yield return StartCoroutine (CreatePlayerRecordGroup ("players"));		//Create document
		yield return StartCoroutine(CreateActorRecordGroup("actors"));  //create actor document
		//subscribes to record groups
		yield return StartCoroutine(Subscribe ());						//subscribes to tabletopBootstrap, which is all channels

		playerRecordGroup.DidAddRecord += (string arg1, PlayerTemplate arg2) => {
			var doc = arg2;
			if (doc.session_id == sessionID) {			
			}
		};
		channelRecordGroup.DidChangeRecord += HandleDidChangeRecordSync;		//allows HH to gamesync routine with TT
		Debug.Log ("Done with MeteorInit");
	}

		//associates clientCollection with meteor's client document
	IEnumerator CreateClientRecordGroup(string recordGroupName) {	//
		clientRecordGroup = Collection <ClientTemplate>.Create (recordGroupName);
		yield return clientRecordGroup;	//waits until collection is finished being created
		/* 
		clientCollection.DidAddRecord += (string id, ClientTemplate document) => {				
			Debug.LogError(string.Format("Client document added:\n{0}", document.Serialize()));
		};
		*/
	}

	//associates channelCollection with meteor's channel document
	IEnumerator CreateChannelRecordGroup(string recordGroupName) {
		channelRecordGroup = Collection <ChannelTemplate>.Create (recordGroupName);
		yield return channelRecordGroup;	//waits until collection is finished being created
		/* 
		channelCollection.DidAddRecord += (string id, ChannelTemplate document) => {				
			Debug.LogError(string.Format("Channel document added:\n{0}", document.Serialize()));
		};
		*/
	}

	//associates sessionCollection with meteor's session document
	IEnumerator CreateSessionRecordGroup(string recordGroupName) {	//
		sessionRecordGroup = Collection <SessionTemplate>.Create (recordGroupName);
		yield return sessionRecordGroup;	//waits until collection is finished being created
		/* 
		sessionCollection.DidAddRecord += (string id, SessionTemplate document) => {
			Debug.LogError(string.Format("Session document added:\n{0}", document.Serialize()));
		};
		*/
	}

	//associates playerCollection with meteor's session document
	IEnumerator CreatePlayerRecordGroup(string recordGroupName) {	//
		playerRecordGroup = Collection <PlayerTemplate>.Create (recordGroupName);
		yield return playerRecordGroup;	//waits until collection is finished being created
		/*
		playerCollection.DidAddRecord += (string id, PlayerTemplate document) => {
			Debug.LogError(string.Format("Player document added:\n{0}", document.Serialize()));
		};
		*/
	}	

	//associates actorCollection with prizmRecordGroup document
	IEnumerator CreateActorRecordGroup (string nameOfCollection){
		actorRecordGroup = new PrizmRecordGroup<ActorTemplate> (sessionID, nameOfCollection); 
		yield return StartCoroutine(actorRecordGroup.CreateMeteorCollection ());

		actorRecordGroup.mongoCollection.DidAddRecord += (string arg1, ActorTemplate arg2) => {
			Debug.LogError ("New Actor Added!" + arg1 + " ID is : " + arg2._id);
		};

	}
		

	//Record change handler for initial game sync
	void HandleDidChangeRecordSync (string arg1, ChannelTemplate arg2, IDictionary arg3, string[] arg4)
	{
		if (arg2.receiver_id == clientID) {	//if this message applies to us (the receiverID is us)
			clientSyncPosition = (clientStatuses) System.Enum.Parse (typeof(clientStatuses), arg2.payload);
			Debug.Log ("client Sync position is: " + clientSyncPosition.ToString());
			sessionSyncPosition = (sessionStatuses)(int)(clientSyncPosition + 1);
			Debug.Log ("session Sync position is: " + sessionSyncPosition.ToString());
			if ((int)sessionSyncPosition > (int)sessionStatuses.running) {		//last stage of sync is 'running'
				//don't broadcast anything to the client, they are running
				playersSynced++;
				Debug.Log ("One more client is fully synced!" + sessionSyncPosition + ", total: " + playersSynced);
				if (numberOfPlayers != playersSynced) {
					Debug.Log ("number of players != playersSynced! numplayers: " + numberOfPlayers + ", playersSynced: " + playersSynced);
				}
			} else {
				Debug.LogError ("recieved message for sync: " + arg2.payload + "; on state: " + sessionSyncPosition);
				StartCoroutine (callUpdateSessionStatus (sessionSyncPosition.ToString ()));
			}
		} else {
			Debug.LogError ("this message is not directed at us, senderID: " + arg2.sender_id + ", receiverID: '" + arg2.receiver_id + "', our clientID: '" + clientID + "'");
		}
	}

	//establishes DDP connection
	IEnumerator BindDDPConnection() {
		Debug.LogError ("in BindDDPConnection(), connectionID is: " + DDPConnectionID);
		var methodCall = Method<ChannelResponse>.Call ("bindClientToDDPConnection", clientID, DDPConnectionID);
		yield return (Coroutine)methodCall;
		
		if (methodCall.Response.success) {
			Debug.LogError ("call to bindClientToDDPConnection SUCCEEDED!, response: " + methodCall.Response.message);
			DDPConnectionID = methodCall.Response.message;
		} else {
			Debug.LogError("call to 'bindClientToDDPConnection' did not succeed.");
		}
		Debug.LogError ("out of BindDDPConnection()");

		yield return null;
	}
	
	//update session status to 'msg' (i.e. 'allPaired')
	IEnumerator callUpdateSessionStatus(string msg) {
		Debug.Log ("Calling updateSession Status with: " + msg);
		var methodCall = Method<ChannelResponse>.Call ("updateSessionStatus", sessionID, clientID, msg);	//make clientID the handheld ID later, last parameter is dictionary for intiial statuses
		yield return (Coroutine)methodCall;
		if (methodCall.Response.success) {
			Debug.LogError("session status updated successfully, received: " + methodCall.Response.message);
		} else {
			Debug.LogError ("updateSessionStatus returned false: " + methodCall.Response.message);
		}
		
		//broadcast updated status to all handhelds using channels:
		yield return StartCoroutine (callBroadcastToHandheldClients (msg));
		//Debug.LogError ("Done with updatesessionstatus");
	}
	
	//broadcast a message to all handhelds with this
	IEnumerator callBroadcastToHandheldClients(string msg) {
		//Debug.LogError ("Broadcasting to all channels: " + msg);
		var methodCall = Method<ChannelResponse>.Call ("broadcastToHandheldClients", sessionID, clientID, msg);	//make clientID the handheld ID later, last parameter is dictionary for intiial statuses
		yield return (Coroutine)methodCall;
		if (methodCall.Response.success) {
			//Debug.LogError("broadcast to all channels successfully, received: " + methodCall.Response.message);
		} else {
			Debug.LogError ("broadcastToAllChannels returned false");
		}
	}
	
	//calls openTabletopSession on meteor, stores the sessionID and clientID	
	IEnumerator OpenSession() {
		Debug.LogError ("Calling 'openTableTopSession' :" + appID + " " + deviceID);
		var methodCall = Method<OpenSessionResponse>.Call ("openTabletopSession", appID, deviceID);
		yield return (Coroutine)methodCall;
		Debug.LogError ("Called 'openTabletopsession' ");
		
		// Get the value returned by the method.
		if (methodCall.Response.success) {
			Debug.LogError ("Open Session succeeded!" + "clientID: " + methodCall.Response.clientID + ", sessionID: " + methodCall.Response.sessionID + ", name: " + methodCall.Response.sessionName);
			clientID = methodCall.Response.clientID;
			sessionID = methodCall.Response.sessionID;
			sessionName = methodCall.Response.sessionName;
			gameName = methodCall.Response.sessionName;
			Debug.Log ("gameName set: " + gameName);

			if (readyToBindDDP) {
				Debug.Log ("calling bindddp connection with: " + DDPConnectionID);
				yield return StartCoroutine (BindDDPConnection ());		//binds low-level DDP connection
			}

		} else {

			Debug.LogError ("Open Session failed :(\nclientID and sessionID not set!");
		}

		yield return null;
	}
		
	//subscribes to all channels relevant to Tabletop device
	IEnumerator Subscribe() {
		var subscription = Subscription.Subscribe ("tabletopBootstrap", sessionID, clientID);
		yield return (Coroutine)subscription;	//wait until subscription successful
		Debug.Log ("Subscribe() in TabletopInitilization finished");
	}
	
	//calls a generic method on the meteor server of 'methodName' with parameters 'args'
	//method must return a success bool and a string
	IEnumerator MethodCall(string methodName, string[] args) {	
		var methodCall = Method<ChannelResponse>.Call (methodName, args);	
		yield return (Coroutine)methodCall;
		
		if (methodCall.Response.success) {
			Debug.LogError (methodName + " executed successfully! Response: " + methodCall.Response.message);
		} else {
			Debug.LogError (methodName + " did NOT execute successfully.");
		}
	}	

	private string GetIP() {
		string strHostName = "";
		strHostName = System.Net.Dns.GetHostName ();
		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry (strHostName);
		IPAddress[] addr = ipEntry.AddressList;
		Debug.Log ("addr: " + addr.ToString());
		return addr [addr.Length - 1].ToString ();
	}

	private void StartServer() {
		Network.InitializeServer (32, 25000, !Network.HavePublicAddress ());
		Debug.Log ("starting server: " + typeName + ": " + gameName);
		MasterServer.RegisterHost (typeName, gameName);
	}

	void OnServerInitialized() {
		Debug.Log ("Server initialized");
		Debug.Log ("Server master ip address: " + MasterServer.ipAddress + " , our local IP: " + GetIP ());
	}
	
}

