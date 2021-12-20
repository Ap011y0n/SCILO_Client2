using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CustomClasses
{
    #region classes
    [System.Serializable]
    public class WelcomeMessage
    {
        public string GUIDplayer1;
        public string GUIDplayer2;
        public string GUIDgun1;
        public string GUIDgun2;
        public List<Spawn> spawns = new List<Spawn>();
        public List<SceneObject> objects = new List<SceneObject>();

    }
    [System.Serializable]
    public class Message
    {
        public int ACK = -1;
        public List<string> messageTypes = new List<string>();
        public List<SceneObject> objects = new List<SceneObject>();
        public List<Input> inputs = new List<Input>();
        public List<Remove> removals = new List<Remove>();
        public List<Spawn> spawns = new List<Spawn>();

        public void addType(string type)
        {
            if (!messageTypes.Contains(type))
                messageTypes.Add(type);
        }
    }

    [System.Serializable]
    public class SceneObject
    {
        public string name;
        private GameObject gameObject;
        private int lastModified;
        private bool modified;
        public Vector3 position;
        public Quaternion rotation;
        public string guid;
        private Vector3 positionChange;

        public GameObject getGO()
        {
            return gameObject;
        }
        public void setGO(GameObject go)
        {
            gameObject = go;
        }
        public int getModDate()
        {
            return lastModified;
        }
        public void setModDate(int mod)
        {
            lastModified = mod;
        }
        public bool CheckMod()
        {
            return modified;
        }
        public void setMod(bool mod)
        {
            modified = mod;
        }
        public Vector3 returnPosChange()
        {
            return positionChange;
        }
        public void setPosChange(Vector3 pos)
        {
            positionChange = pos;
        }
    }
    [System.Serializable]
    public class Input
    {
        public string key;
        public string type;
    }
    [System.Serializable]
    public class Spawn
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        private GameObject Obj;
        public string guid;

        public void setGO(GameObject go)
        {
            Obj = go;
        }
        public GameObject getGO()
        {
            return Obj;
        }
    }
    [System.Serializable]
    public class Remove
    {
        public string name;
        private GameObject Obj;
        public string guid;


        public void setGO(GameObject go)
        {
            Obj = go;
        }
        public GameObject getGO()
        {
            return Obj;
        }
    }
    #endregion

}

public class clientUDP : MonoBehaviour
{
   

    private Socket newSocket;
    IPEndPoint sender;
    EndPoint Remote;

    private Thread MainThread;
    private Thread sendThread = null;
    private Thread receiveThread = null;

    private int port = 7778;
    private string message = "";
    private IPEndPoint ipep;
    private IPAddress adress;

    public GameObject[] prefabs;
    public GameObject[] initialDynamicObjs;

    public Dictionary<string, GameObject> spawnable;

    private List<CustomClasses.Input> inputList = new List<CustomClasses.Input>();
    [HideInInspector]
    public List<CustomClasses.SceneObject> Objs2Update = new List<CustomClasses.SceneObject>();
    int ACK = -1;
    int lastReceivedACK = -1;
    List<CustomClasses.Message> sentMessages = new List<CustomClasses.Message>();
    List<CustomClasses.Spawn> WaitingToSpawn = new List<CustomClasses.Spawn>();
    List<CustomClasses.Remove> WaitingToRemove = new List<CustomClasses.Remove>();

    int sendFrameCounter = 0;
    int receiveFrameCounter = 0;
    float interpolationValue = 1;
    float interpolationTracker = 0;
    bool firstInterpolationFrame = false;

    public Dictionary<string, GameObject> dynamicObjects = new Dictionary<string, GameObject>();

    public Quaternion gunRotation;
    public GameObject gun;
    // Start is called before the first frame update
    void Start()
    {
        ExecuteScript();
    }

    // Update is called once per frame
    void Update()
    {
        gunRotation = gun.transform.rotation;
        if (WaitingToSpawn.Count > 0)
        {
            for (int i = 0; i < WaitingToSpawn.Count; i++)
            {
                Debug.Log("Spawning: " + WaitingToSpawn[i].name);
                if (!dynamicObjects.ContainsKey(WaitingToSpawn[i].guid))
                    dynamicObjects.Add(WaitingToSpawn[i].guid, Instantiate(prefabs[Convert.ToInt32(WaitingToSpawn[i].name)], WaitingToSpawn[i].position, WaitingToSpawn[i].rotation));
            }
            WaitingToSpawn.Clear();
        }
        if (WaitingToRemove.Count > 0)
        {
            for (int i = 0; i < WaitingToRemove.Count; i++)
            {
                Debug.Log("Removing: " + WaitingToRemove[i].name);
                Destroy(dynamicObjects[WaitingToRemove[i].guid]);
                dynamicObjects.Remove(WaitingToRemove[i].guid);
            }
            WaitingToRemove.Clear();
        }
        if (Objs2Update.Count > 0)
        {
            for(int i = 0; i < Objs2Update.Count; i++)
            {

                if (Objs2Update[i].name != "")
                {
                    GameObject obj2update = dynamicObjects[Objs2Update[i].guid];
                    if (firstInterpolationFrame == true)
                    {
                        Vector3 posChange = Objs2Update[i].position - obj2update.transform.localPosition;
                        Objs2Update[i].setPosChange(posChange);
                    }
                    //Debug.Log(obj.guid);
                    if (obj2update != null)
                    {
                  //      Debug.Log("Interpolation Value: " + interpolationValue);
                 //       Debug.Log(Objs2Update[i].returnPosChange());
                  //      Debug.Log(Objs2Update[i].returnPosChange() / interpolationValue);
                        Vector3 newpos = new Vector3();
                        float x = Objs2Update[i].returnPosChange().x;
                        float y = Objs2Update[i].returnPosChange().y;
                        float z = Objs2Update[i].returnPosChange().z;
                   //     Debug.Log("y = " + y.ToString() + "/ " + interpolationValue.ToString() + "= " + (y * interpolationValue).ToString());
                        newpos.x = x * interpolationValue;
                        newpos.y = y * interpolationValue;
                        newpos.z = z * interpolationValue;

                        obj2update.transform.position = obj2update.transform.position + newpos;

                        obj2update.transform.rotation = Objs2Update[i].rotation;

                        interpolationTracker += interpolationValue;
                    }   
                    else
                        Debug.LogWarning("Can't find object by name" + Objs2Update[i].name);
                }
            }
            if (firstInterpolationFrame == true)
            {
                firstInterpolationFrame = false;
            }
        } 

        if(interpolationTracker >= 1)
        {
            Objs2Update.Clear();
            interpolationTracker = 0;
        }

        receiveFrameCounter++;
        sendFrameCounter++;
    }
    public void AddInput(KeyCode input, string type)
    {
        
        CustomClasses.Input newInput = new CustomClasses.Input();
        newInput.key = input.ToString();
       // Debug.Log(newInput.key);
        newInput.type = type;
        inputList.Add(newInput);
    }
    public void AddInput(int mousebutton, string type)
    {
        CustomClasses.Input newInput = new CustomClasses.Input();
        newInput.key = mousebutton.ToString();
        newInput.type = type;
        inputList.Add(newInput);
    }
    public void ExecuteScript()
    {
        adress = IPAddress.Parse("127.0.0.1");
        ipep = new IPEndPoint(adress, port);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        MainThread = new Thread(UDPConnection);
        MainThread.Start();
        sendThread = new Thread(ThreadSend);
        receiveThread = new Thread(ThreadReceive);
    }

    void UDPConnection()
    {
        try
        {
            byte[] msg = Encoding.ASCII.GetBytes("Ping");
            newSocket.SendTo(msg, msg.Length, SocketFlags.None, ipep);

            sender = new IPEndPoint(IPAddress.Any, 0);
            Remote = (EndPoint)sender;
            Debug.Log("Message sent to server");

            msg = new Byte[10000];
            newSocket.ReceiveFrom(msg, ref Remote);

            interpolationValue = 1;

            MemoryStream stream = new MemoryStream(msg);
            CustomClasses.WelcomeMessage m = deserializeWelcome(stream);

            dynamicObjects.Add(m.GUIDplayer1, initialDynamicObjs[0]);
            dynamicObjects.Add(m.GUIDplayer2, initialDynamicObjs[1]);
            dynamicObjects.Add(m.GUIDgun1, initialDynamicObjs[2]);
            dynamicObjects.Add(m.GUIDgun2, initialDynamicObjs[3]);

            for (int i = 0; i < m.spawns.Count; i++)
            {
                WaitingToSpawn.Add(m.spawns[i]);
            }
            foreach (CustomClasses.SceneObject obj in m.objects)
            {
                Objs2Update.Add(obj);
            }
        }
        catch (SystemException e)
        {
            Debug.Log("Couldn't send or receive message");
            Debug.Log(e.ToString());
            Debug.Log("Disconnecting from server");
            MainThread.Abort();
            try
            {
                newSocket.Shutdown(SocketShutdown.Both);
                newSocket.Close();
            }

            catch (SystemException d)
            {
                Debug.Log("Socket already closed");
                Debug.Log(d.ToString());

            }
        }


        sendThread.Start();
        receiveThread.Start();



    }

    void ThreadSend()
    {
        while (true)
        {
          
          //  CustomClasses.SceneObject obj = new CustomClasses.SceneObject();
          //  obj.rotation = gunRotation;
          //  temp.objects.Add(obj);
            //if (inputList.Count > 0)
            //{
            //    MemoryStream stream = new MemoryStream();
            //    CustomClasses.Message temp = new CustomClasses.Message();
            //    CustomClasses.SceneObject obj = new CustomClasses.SceneObject();
            //    foreach (KeyValuePair<string, GameObject> ah in dynamicObjects)
            //    {
            //        if (ah.Value == gun)
            //            obj.guid = ah.Key;
            //    }
                   
            //    obj.rotation = gunRotation;
            //    temp.objects.Add(obj);
            //    temp.inputs = inputList;
            //    temp.ACK = ACK;
            //    temp.addType("movement");
            //    stream = serializeJson(temp);
            //    sentMessages.Add(temp);
            //    ACK++;
            //    newSocket.SendTo(stream.ToArray(), SocketFlags.None, ipep);
            //}
          

            if (sendFrameCounter >= 5)
            {
                MemoryStream stream = new MemoryStream();
                CustomClasses.Message temp = new CustomClasses.Message();
                CustomClasses.SceneObject obj = new CustomClasses.SceneObject();
                obj.rotation = gunRotation;

                foreach (KeyValuePair<string, GameObject> ah in dynamicObjects)
                {
                    if (ah.Value == gun)
                        obj.guid = ah.Key;
                }

                temp.objects.Add(obj);
                temp.inputs = inputList;
                temp.ACK = ACK;
                temp.addType("movement");
                if (inputList.Count > 0)
                {
                   
                    sentMessages.Add(temp);
                    ACK++;
                }
                stream = serializeJson(temp);

                newSocket.SendTo(stream.ToArray(), SocketFlags.None, ipep);

                sendFrameCounter = 0;
            }
        }

    }
    void ThreadReceive()
    {
        while (true)
        {
            byte[] msg = new Byte[10000];

            newSocket.ReceiveFrom(msg, ref Remote);
            if (receiveFrameCounter == 0)
                receiveFrameCounter = 1;
            interpolationValue = 1.0f / receiveFrameCounter;
            firstInterpolationFrame = true;
            receiveFrameCounter = 0;
            // Debug.Log(Encoding.ASCII.GetString(msg));
            MemoryStream stream = new MemoryStream(msg);
            CustomClasses.Message m = deserializeJson(stream);
            if(m.messageTypes.Contains("movement"))
            {
                foreach (CustomClasses.SceneObject obj in m.objects)
                {
                    // GameObject obj2update = GameObject.Find(obj.name);
                    Objs2Update.Add(obj);
                    //   obj2update.transform.position = obj.transform.position;
                }
            }
            
            int max = sentMessages.Count;
         //   Debug.Log(max);
            for (int i = 0; i < max; i++)
            {
                if (sentMessages[i].ACK <= m.ACK)
                {
                    sentMessages.RemoveAt(i);
                    i--;
                    max--;
                }
            }
            if(m.messageTypes.Contains("acknowledgement"))
            if (ACK > m.ACK)
            {
                CustomClasses.Message temp = new CustomClasses.Message();
                for (int i = 0; i < sentMessages.Count; i++)
                {
                    if (sentMessages[i].ACK > m.ACK)
                    {
                        for (int j = 0; j < sentMessages[i].inputs.Count; j++)
                            temp.inputs.Add(sentMessages[i].inputs[j]);
                    }
                }
                
                temp.ACK = ACK;
                stream = serializeJson(temp);
                newSocket.SendTo(stream.ToArray(), SocketFlags.None, ipep);
                sentMessages.Clear();
                sentMessages.Add(temp);
            }
            if (m.messageTypes.Contains("spawn"))
            {
                for(int i = 0; i < m.spawns.Count; i++)
                {

           
                    WaitingToSpawn.Add(m.spawns[i]);
                }
            }
            if (m.messageTypes.Contains("remove"))
            {
                for (int i = 0; i < m.removals.Count; i++)
                {
                    WaitingToRemove.Add(m.removals[i]);
                }
            }

        }

    }

    public void EndConnection()
    {
        Debug.Log("Disconnecting from server");
        MainThread.Abort();
        try
        {
            newSocket.Shutdown(SocketShutdown.Both);
            newSocket.Close();
        }

        catch (SystemException e)
        {
            Debug.Log("Socket already closed");
            Debug.Log(e.ToString());

        }
        if (MainThread != null)
            MainThread.Abort();
        if (receiveThread != null)
            receiveThread.Abort();
        if (sendThread != null)
            sendThread.Abort();
    }

    void OnApplicationQuit()
    {
        try
        {
            newSocket.Close();

        }

        catch (SystemException e)
        {
            Debug.Log("Socket already closed");
            Debug.Log(e.ToString());

        }
        if (MainThread != null)
            MainThread.Abort();
        if (receiveThread != null)
            receiveThread.Abort();
        if (sendThread != null)
            sendThread.Abort();
    }
    MemoryStream serializeJson(CustomClasses.Message m)
    {
       
        string json = JsonUtility.ToJson(m);
        //Debug.Log(json);
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
        inputList.Clear();
        return stream;
    }

    CustomClasses.Message deserializeJson(MemoryStream stream)
    {
        var m = new CustomClasses.Message();
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        string json = reader.ReadString();
        Debug.Log(json);
        m = JsonUtility.FromJson<CustomClasses.Message>(json);
        return m;
    }

    CustomClasses.WelcomeMessage deserializeWelcome(MemoryStream stream)
    {
        var m = new CustomClasses.WelcomeMessage();
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        string json = reader.ReadString();
             Debug.Log(json);
        m = JsonUtility.FromJson<CustomClasses.WelcomeMessage>(json);
        return m;
    }
}
