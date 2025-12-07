using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;

namespace StoryModeCoOpMP
{
    public class CustomRoomUIBehavior : GauntletLayer
    {
        private GauntletMovie _movie;

        public override void OnInitialize()
        {
            base.OnInitialize();

            // "CustomRoomUI" prefab ismini XML ile eşleştiriyoruz
            _movie = MovieManager.Instance.CreateMovie("CustomRoomUI", "CustomRoomUI", false);
            _movie.AddLayer(this);

            CreateBindings();
        }

        private void CreateBindings()
        {
            if (_movie == null) return;

            // Room code text widget
            var roomCodeText = _movie.GetTextWidgetByID("RoomCodeText");
            if (roomCodeText != null)
                roomCodeText.Text = "ROOM CODE: " + MultiplayerSkeletonManager.Instance.RoomCode;

            // Input ve button
            var input = _movie.GetEditBoxWidgetByID("JoinCodeInput");
            var joinButton = _movie.GetButtonWidgetByID("JoinButton");

            if (joinButton != null && input != null)
            {
                joinButton.AddButtonOnClick(delegate
                {
                    string enteredCode = input.Text;
                    if (MultiplayerSkeletonManager.Instance.ClientEnterRoomCode(enteredCode, "127.0.0.1"))
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Başarıyla bağlanıldı!"));
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Yanlış oda kodu!"));
                    }
                });
            }
        }
    }
}
