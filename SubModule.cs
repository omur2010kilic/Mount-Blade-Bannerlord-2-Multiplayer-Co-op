using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using StoryModeCoOpMP;
using UIExtenderEx;

namespace StoryModeCoOp
{
    public class SubModule : MBSubModuleBase
    {
        private MultiplayerSkeleton _mp;
        private RoomCodeUIBehaviour _uiBehaviour;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // UIExtenderEx başlat
            var extender = new UIExtender("StoryModeCoOp");
            extender.Register(typeof(SubModule).Assembly);
            extender.Enable();

            // MultiplayerSkeleton oluştur
            _mp = new MultiplayerSkeleton();

            bool isHost = true; // Test için, oyunu açan kişi host

            if (isHost)
            {
                _mp.StartServer();
            }
            else
            {
                // Client olarak bağlanacaksa GUI input ile bağlanacak
            }

            // UI behaviour oluştur
            _uiBehaviour = new RoomCodeUIBehaviour(_mp, isHost);
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(
                new InformationMessage("Co-Op Multiplayer Mod Başladı!"));
        }

        public override void OnApplicationTick(float dt)
        {
            // Multiplayer polling
            _mp?.PollServer();
            _mp?.PollClient();

            // UI güncellemesi (host için oda kodunu göster)
            _uiBehaviour?.OnTick();
        }
    }
}
