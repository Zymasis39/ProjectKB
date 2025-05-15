using ProjectKB.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Modules
{
    public class ViewManager
    {
        private Game1 _game;

        public MainMenuView mainMenuView;
        public GameplayView gameplayView;

        public BaseView currentView { get; set; }

        public ViewManager(Game1 game)
        {
            _game = game;
            mainMenuView = new MainMenuView();
            gameplayView = new GameplayView();
        }

        public void SwitchView(BaseView view)
        {
            currentView = view;
            currentView.OnSwitch();
        }

        public void Exit()
        {
            _game.Exit();
        }
    }
}
