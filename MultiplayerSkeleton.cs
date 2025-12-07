using System;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace StoryModeCoOpMP
{
    public class MultiplayerSkeleton
    {
        private NetManager _server;
        private NetManager _client;
        private NetPeer _serverPeer;
        private EventBasedNetListener _listener;

        public string RoomCode { get; private set; } = "";

        // GUI widget referansları
        public RoomCodeWidget RoomCodeDisplay { get; private set; }

        // ======================
        // ROOM CODE ÜRET & CHAT'E YAZ
        // ======================
        public void GenerateRoomCode(int length = 4)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            RoomCode = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Chat'e kalıcı şekilde yaz
            InformationManager.DisplayMessage(new InformationMessage($"[Room Code] {RoomCode}"));

            // GUI oluştur / güncelle
            if (RoomCodeDisplay != null)
            {
                RoomCodeDisplay.UpdateCode(RoomCode);
            }
        }

        // ======================
        // SERVER KODU
        // ======================
        public void StartServer(int port = 14242)
        {
            GenerateRoomCode(); // Kod üret

            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
            _server.Start(port);

            _listener.ConnectionRequestEvent += request =>
            {
                request.AcceptIfKey("StoryModeCoOpMP");
            };

            _listener.PeerConnectedEvent += peer =>
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"{peer.EndPoint} bağlandı!"));
            };

            _listener.NetworkReceiveEvent += (fromPeer, reader, method) =>
            {
                string msg = reader.GetString();
                InformationManager.DisplayMessage(
                    new InformationMessage($"Serverdan mesaj alındı: {msg}"));
            };

            InformationManager.DisplayMessage(
                new InformationMessage($"Server port {port} ile başladı!"));
        }

        public void PollServer()
        {
            _server?.PollEvents();
        }

        public void SendToAllClients(string message)
        {
            if (_server == null) return;
            var writer = new NetDataWriter();
            writer.Put(message);
            foreach (var peer in _server.ConnectedPeerList)
            {
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        // ======================
        // CLIENT KODU
        // ======================
        public void ConnectClient(string hostIP, int port = 14242)
        {
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            _client.Start();

            _serverPeer = _client.Connect(hostIP, port, "StoryModeCoOpMP");

            _listener.PeerConnectedEvent += peer =>
            {
                InformationManager.DisplayMessage(
                    new InformationMessage("Sunucuya bağlandı!"));
            };

            _listener.NetworkReceiveEvent += (fromPeer, reader, method) =>
            {
                string msg = reader.GetString();
                InformationManager.DisplayMessage(
                    new InformationMessage($"Serverdan mesaj alındı: {msg}"));
            };
        }

        public void PollClient()
        {
            _client?.PollEvents();
        }

        public void SendToServer(string message)
        {
            if (_serverPeer == null) return;
            var writer = new NetDataWriter();
            writer.Put(message);
            _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        // ======================
        // CLIENT ANA MENÜDEN KOD GİRME
        // ======================
        public bool ClientEnterRoomCode(string enteredCode, string hostIP)
        {
            if (enteredCode == "")
                return false;

            if (enteredCode == RoomCode)
            {
                ConnectClient(hostIP);
                InformationManager.DisplayMessage(
                    new InformationMessage("Başarıyla bağlanıldı!"));
                return true;
            }
            else
            {
                InformationManager.DisplayMessage(
                    new InformationMessage("Yanlış oda kodu!"));
                return false;
            }
        }

        // ======================
        // ROOM CODE WIDGET ATAMA
        // ======================
        public void SetRoomCodeWidget(RoomCodeWidget widget)
        {
            RoomCodeDisplay = widget;
            widget.UpdateCode(RoomCode);
        }
    }
}
