using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Learning_Application
{
    public class ConsoleUI
    {
        private void Display()
        {
            for (int y = 7; y >=0 ; y--)
            {
                Console.Write(y + "  ||  ");
                for (int x = 0; x < 8; x++)
                {
                    switch (_game.Board.square[x, y].Piece?.Type)
                    {
                        case Chessboard.FigureType.Pawn:
                            {
                                Console.Write("| P" + (int)_game.Board.square[x, y].Piece?.Color! + " |");
                                break;
                            }
                        case Chessboard.FigureType.King:
                            {
                                Console.Write("| K" + (int)_game.Board.square[x, y].Piece?.Color! + " |");
                                break;
                            }
                        case Chessboard.FigureType.Queen:
                            {
                                Console.Write("| Q" + (int)_game.Board.square[x, y].Piece?.Color! + " |");
                                break;
                            }
                        case Chessboard.FigureType.Bishop:
                            {
                                Console.Write("| B" + (int)_game.Board.square[x, y].Piece?.Color! + " |");
                                break;
                            }
                        case Chessboard.FigureType.Knight:
                            {
                                Console.Write("| N" + (int)_game.Board.square[x, y].Piece?.Color! + " |");
                                break;
                            }
                        case Chessboard.FigureType.Rook:
                            {
#pragma warning disable CS8629 // Typ wartości dopuszczający wartość null może być równy null.
                                Console.Write("| R" + (int)_game.Board.square[x, y].Piece?.Color + " |");
#pragma warning restore CS8629 // Typ wartości dopuszczający wartość null może być równy null.
                                break;
                            }
                        default: 
                            Console.Write("| __ |");
                            break;
                    }
                }
                Console.WriteLine( );
            }
            Console.Write("       ");
            for (int x = 0; x < 8; x++)
            {
                Console.Write("|  "+ x +" |");
            }
            Console.WriteLine();
        }
        private Game _game;
        public ConsoleUI()
        {
            //_game = new Game();

            while (true)
            {
                foreach (Chessboard.Figure piece in _game.Board.Figures)
                {
                    if (piece.Color == _game.Turn)
                    {
                        piece.SetLegalMoves();
                        Console.WriteLine("Valid Moves are: ");
                        foreach (Chessboard.Move m in piece.LegalMoves)
                        {
                            Console.WriteLine(m.SX + " " + m.SY + " | " + m.DX + " " + m.DY);
                        }
                    }
                }
                Display();
                Console.WriteLine("Please provide source: ");
                dynamic source = Console.ReadLine()!;
                source = Convert.ToInt32(source);

                Console.WriteLine("Please provide destination: ");
                dynamic destination = Console.ReadLine()!;
                destination = Convert.ToInt32(destination);

                Chessboard.Move move = new( (int)source / 10, source % 10, (int)destination / 10, destination %10);
                Console.WriteLine(move.SX + " "+ move.SY + " " + move.DX + " " + move.DY);
                _game.Board.square[ (int)source / 10, source % 10].Piece?.Move(move);
                //if (_game.Turn == Chessboard.FigureColor.White) { _game.Turn = Chessboard.FigureColor.Black; }
                //else { _game.Turn = Chessboard.FigureColor.White; }
            }



        }
    }
}
