﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using ProtoBuf;

namespace Bolt.Samples.ServerMonitor
{
    public class ServerMonitor : MonoBehaviour
    {

        private UdpClient client;
        public static int SERVER_PORT = 6667;
        private float updateInterval = 1;
        private float timer = 0;
        private volatile ServerStats stats = new ServerStats();

        void Start()
        {
            client = new UdpClient(SERVER_PORT);
            client.BeginReceive(new AsyncCallback(receiveCallback), null);
        }

        // Update stats data periodically
        void Update()
        {
            if (timer >= updateInterval)
            {
                timer = 0;
                CollectBoltData();
            }
            timer += Time.deltaTime;
        }

        // process incomming queries for stats
        private void receiveCallback(IAsyncResult res)
        {
            var remote = new IPEndPoint(IPAddress.Any, SERVER_PORT);
            byte[] receivedData = client.EndReceive(res, ref remote);
            string query = System.Text.Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);

            if ("get".Equals(query))
            {
                MemoryStream stream = new MemoryStream();
                Serializer.Serialize(stream, stats);
                byte[] data = stream.ToArray();
                client.Send(data, data.Length, remote);
            }
            client.BeginReceive(new AsyncCallback(receiveCallback), null);
        }

        private void CollectBoltData()
        {
            ServerStats stats = new ServerStats();
            stats.Fps = (short)(1.0f / Time.deltaTime);
            stats.Timestamp = Time.time;
            foreach (BoltConnection conn in BoltNetwork.clients)
            {
                ClientStats c = new ClientStats();
                c.IpAddress = conn.RemoteEndPoint.Address.ToString();
                c.Port = conn.RemoteEndPoint.Port;
                stats.Clients.Add(c);
            }
            this.stats = stats;
        }

        void OnDestroy()
        {
            client.Close();
        }
    }
}