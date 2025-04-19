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
        public MainMenuView mainMenuView;
        public GameplayView gameplayView;

        public BaseView currentView { get; set; }

        public ViewManager()
        {
            mainMenuView = new MainMenuView();
            gameplayView = new GameplayView();
            SwitchView(mainMenuView);
        }

        public void SwitchView(BaseView view)
        {
            currentView = view;
            currentView.OnSwitch();
        }
    }
}
