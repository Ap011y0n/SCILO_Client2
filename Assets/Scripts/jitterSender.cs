using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public class jitterSender : MonoBehaviour
{
    public bool jitter = true;
    public bool packetLoss = true;
    public int minJitt = 0;
    public int maxJitt = 800;
    public int lossThreshold = 90;
    public Socket Server;

    private readonly object myLock = new object();
    public struct Message
    {
        public Byte[] message;
        public DateTime time;
        public UInt32 id;
        public IPEndPoint ip;

    }
    public List<Message> messageBuffer = new List<Message>();
    public Thread start;
    void Start()
    {
        start = new Thread(sendMessages);
        start.Start();
    }
    public void sendMessage(Byte[] text, IPEndPoint ip)
    {
        System.Random r = new System.Random();
        if (((r.Next(0, 100) > lossThreshold) && packetLoss) || !packetLoss) // Don't schedule the message with certain probability
        {
            Message m = new Message();
            m.message = text;
            if (jitter)
            {
                m.time = DateTime.Now.AddMilliseconds(r.Next(minJitt, maxJitt)); // delay the message sending according to parameters
            }
            else
            {
                m.time = DateTime.Now;
            }
            m.id = 0;
            m.ip = ip;

            lock (myLock)
            {
                messageBuffer.Add(m);
            }

            //  Debug.Log(m.time.ToString());
        }

    }
    //Run this always in a separate Thread, to send the delayed messages
    void sendMessages()
    {
        Debug.Log("really sending..");
        bool exit = false;
        while (!exit)
        {
            DateTime d = DateTime.Now;
            int i = 0;
            if (messageBuffer.Count > 0)
            {
               // Debug.Log("This is working");

                List<Message> auxBuffer;
                lock (myLock)
                {
                    auxBuffer = new List<Message>(messageBuffer);
                }
                
                foreach (var m in auxBuffer)
                {
                    if (m.time < d)
                    {
                        Server.SendTo(m.message, m.message.Length, SocketFlags.None, m.ip);
                        lock (myLock)
                        {
                            messageBuffer.RemoveAt(i);
                        }
                        i--;
                        //myLog = Encoding.ASCII.GetString(m.message, 0, m.message.Length);
                        //Debug.Log("message sent!");
                    }
                    i++;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (start != null)
            start.Abort();
    }
}
