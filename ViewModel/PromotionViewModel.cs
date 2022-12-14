using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wpf_GUI
{
    public class PromotionViewModel
    {
        public PromotionViewModel()
        {
            PromotionToQueenCommand = new RelayCommand(o => Queen_choosen());
            PromotionToRookCommand = new RelayCommand(o => Rook_choosen());
            PromotionToBishopCommand = new RelayCommand(o => Bishop_choosen());
            PromotionToKnightCommand = new RelayCommand(o => Knight_choosen());
        }

        public ICommand PromotionToQueenCommand { get; private set; }
        public ICommand PromotionToRookCommand { get; private set; }
        public ICommand PromotionToBishopCommand { get; private set; }
        public ICommand PromotionToKnightCommand { get; private set; }

        public event EventHandler<string>? PromotionWasChosenEvent;

        private void Queen_choosen()
        {
            PromotionWasChosenEvent?.Invoke(this, "Queen");

        }
        private void Rook_choosen()
        {
            PromotionWasChosenEvent?.Invoke(this, "Rook");
        }
        private void Bishop_choosen()
        {
            PromotionWasChosenEvent?.Invoke(this, "Bishop");
        }
        private void Knight_choosen()
        {
            PromotionWasChosenEvent?.Invoke(this, "Knight");
        }
    }
}
