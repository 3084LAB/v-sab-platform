using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPSender : MonoBehaviour
{
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener;
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;
	#endregion

	public string MyIPadress = "133.91.74.201";
	public Int32 TCP_port = 2345;

	// TCPで数値を送りたい変数が宣言されているファイルがアタッチされたオブジェクト
	public GameObject HandObj;
	// スクリプトオブジェクト
    public ExtendForearm ExtendScript;
	
	private void Awake()

	{
		HandObj = GameObject.FindGameObjectWithTag("Hand");
		// <ファイル名>からアタッチされているスクリプトを取得
		ExtendScript = HandObj.GetComponent<ExtendForearm>();
	}


	// Use this for initialization
	void Start()
	{
		// Start TcpServer background thread 		
		tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Start();
		
	}

	// Update is called once per frame
	void Update()
	{
		SendMessage();
		if (Input.GetKeyDown(KeyCode.Space))
		{
			//SendMessage();
			Debug.Log("sending_m");

		}
		if (Input.GetKeyDown("r"))
		{
			Invoke("find_obj", 0.1f);
		}
	}


	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests()
	{
		try
		{
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse(MyIPadress), TCP_port);
			tcpListener.Start();
			Debug.Log("Server is listening");
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary.
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incommingData = new byte[length];
							Array.Copy(bytes, 0, incommingData, 0, length);
							// Convert byte array to string message.
							string clientMessage = Encoding.ASCII.GetString(incommingData);
							Debug.Log("client message received as: " + clientMessage);
						}
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}
	/// <summary>
	/// Send message to client using socket connection.
	/// </summary> 	
	private void SendMessage()
	{
		if (connectedTcpClient == null)
		{
			return;
		}

		try
		{
			// Get a stream object for writing.
			NetworkStream stream = connectedTcpClient.GetStream();
			if (stream.CanWrite)
			{
                string serverMessage = "20";	// 60.0 kg -> x10 -> 600
				serverMessage = "" + ((int)(ExtendScript.elbowTipWeight * 10.0f)).ToString() + ((int)(ExtendScript.forearmLength * ExtendScript.extendRatio * 10.0f)).ToString("D2");
				//Debug.Log(serverMessage);
                byte[] serverMessageAsByteArray = System.Text.Encoding.ASCII.GetBytes(serverMessage);
				//Debug.Log(serverMessageAsByteArray[0]);
				//Debug.Log(serverMessageAsByteArray[1]);
				//Debug.Log("--");
				//byte[] serverMessageAsByteArray = { 0, 1, 2, 3, 4, 5, 6 };

				//byte[] serverMessageAsByteArray = BitConverter.GetBytes(send_Int32);


				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
				//Debug.Log("Server sent his message - should be received by client");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	private void checkmsg(string m)
	{

		if (Convert.ToInt32(m) < 10 && Convert.ToInt32(m) > -1)
		{
			//player.PlayerMode = "0" + Convert.ToInt32(m).ToString();
		}
		else if (Convert.ToInt32(m) >= 10 && Convert.ToInt32(m) <= 99)
		{

		}
		else
		{
			//player.PlayerMode = "00";
		}
	}

	private void find_obj()
	{
	//	player_sphare = GameObject.Find("Weight");
	}
}
