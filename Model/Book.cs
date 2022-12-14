using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Learning_Application
{
    public partial class Engine
    {
        public class Book
        {
            public Dictionary<string, HashSet<Chessboard.Move>> Responses;

            Book()
            {
                Responses = new();
            }

            public static Book LoadBookFromTxtFile(int maxNumberOfResponsesInBook = 6)
            {
                Book book = new();
                string pathToBook = Path.Combine("Assets", "Book.txt");
                //path = Path.Combine(Application.StartupPath, "Reports", "Book.txt")
                Chessboard chessboard = new();
                chessboard.SetChessbordUsingFEN(Chessboard.StartingFEN);
                if (File.Exists(pathToBook) == false)
                {
                    book.Responses["p1"] = new();
                    return book;
                }
                var lines = File.ReadLines(pathToBook);
                foreach (var line in lines)
                {
                    string[] moves = line.Split(" ");
                    if (moves[0] == "")
                        break;

                    string game = "";

                    for (int i = 0; i < maxNumberOfResponsesInBook * 2; i++)
                    {
                        if (book.Responses.ContainsKey(game) == false)
                            book.Responses[game] = new();
                        HashSet<Chessboard.Move> tempList = book.Responses[game];
                        tempList.Add(Chessboard.Move.GetLastMoveFromPNG(game + moves[i]));
                        book.Responses[game] = tempList;
                        //book.Responses[game].Add(Chessboard.Move.GetLastMoveFromPNG(game + moves[i] + " " ));
                        game += moves[i] + " ";
                    }
                }

                    return book;
            }

            public async static Task CreateBookFile()
            {
                string pathToBook = Path.Combine("Assets", "Book.txt");
                DirectoryInfo d = new DirectoryInfo("PGNs");
                foreach (var file in d.GetFiles("*.pgn"))
                {
                    var lines = File.ReadLines(file.FullName);

                    string game = "";
                    bool newGame = false;
                    bool isStandardGame = true;
                    foreach (var line in lines)
                    {
                        if (line == "")
                        {
                            if (newGame == true)
                            {
                                if (isStandardGame)
                                {
                                    using (StreamWriter w = File.AppendText(pathToBook))
                                    {
                                        w.WriteLine(game);
                                    }
                                }
                                isStandardGame = true;
                            }
                            newGame = false;
                            game = "";
                            continue;
                        }
                        if (line[0] == '[')
                        {
                            if (line[1] == 'F' && line[2] =='E')
                            {
                                string startPos = line.Split('"')[1];
                                if (startPos != Chessboard.StartingFEN)
                                    isStandardGame = false;
                            }
                            newGame = false;
                            continue;
                        }

                        newGame = true;
                        string round = "";
                        char lastChar = 'c';
                        foreach (var c in line)
                        {
                            if (c == '.')
                            {
                                int index = round.LastIndexOf(" ");
                                round = round.Remove(index + 1);
                                game += round;
                                round = "";
                                lastChar = c;
                                continue;
                            }
                            if (lastChar =='.')
                            {
                                lastChar = c;
                                continue;
                            }
                            round += c;
                            lastChar = c;
                            
                        }
                        //game += " ";

                    }

                }
                var linesInBook = System.IO.File.ReadAllLines(pathToBook);
                System.IO.File.WriteAllLines(pathToBook, linesInBook.Take(linesInBook.Length - 1).ToArray());
            }
        } // Book
    } // Engine
} // namespace
