using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TicTacToe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private GameBoard gameBoard;
        private void ResetBoard()
        {
            gameBoard = new GameBoard();
        }

        [HttpPost]
        public IActionResult Move([FromBody] Move move)
        {
            string currentPlayer = "X";
            string jsonData;
            var gameBoard = new GameBoard();
            if (!System.IO.File.Exists("data.json"))
            {
                jsonData = JsonSerializer.Serialize(gameBoard);
                System.IO.File.WriteAllText("data.json", jsonData);
            }
            else
            {
                string json = System.IO.File.ReadAllText("data.json");
                gameBoard = JsonSerializer.Deserialize<GameBoard>(json);
            }

            List<List<Step>> board = gameBoard.board;
            currentPlayer = gameBoard.currentPlayer;

            if (board == null)
            {
                ResetBoard();
            }

            if (move.Row < 0 || move.Row > 2 || move.Column < 0 || move.Column > 2)
            {
                return BadRequest("Invalid move. Row and column values must be between 0 and 2.");
            }

            if (board[move.Row][move.Column] != null)
            {
                return BadRequest("Invalid move. That spot is already taken.");
            }

            board[move.Row][move.Column] = new Step
            {
                Player = currentPlayer,
            };

            bool hasWon = false;

            //ROWS
            for (int i = 0; i < 3; i++)
            {
                Step firstStep = board[i][0];

                if (firstStep == null)
                {
                    break;
                }

                var isRowWon = false;

                for (int j = 1; j < 3; j++)
                {
                    Step step = board[i][j];

                    if (step == null)
                    {
                        isRowWon = false;

                        break;
                    }

                    if (step.Player != firstStep.Player) 
                    {
                        isRowWon = false;

                        break;
                    }

                    isRowWon = true;
                }

                if (isRowWon)
                {
                    ResetBoard();
                    return Ok($"Player {currentPlayer} has won!");
                }
            }

            //COLUMNS
            for (int i = 0; i < 3; i++)
            {
                Step firstStep = board[0][i];

                if (firstStep == null)
                {
                    break;
                }

                for (int j = 1; j < 3; j++)
                {
                    Step step = board[j][i];

                    if (step == null)
                    {
                        hasWon = false;

                        break;
                    }

                    if (step.Player != firstStep.Player)
                    {
                        hasWon = false;

                        break;
                    }

                    hasWon = true;
                }

                if (hasWon)
                {
                    ResetBoard();
                    return Ok($"Player {currentPlayer} has won!");
                }
            }

            //DIAGONALS
            for (int i = 0; i < 2; i++)
            {
                Step firstStep = board[i][i];
                Step nextStep = board[i + 1][i + 1];

                if (firstStep == null)
                {
                    hasWon = false;
                 
                    break;
                }

                if (nextStep == null)
                {
                    hasWon = false;

                    break;
                }

                if (firstStep.Player != nextStep.Player)
                {
                    hasWon = false;

                    break;
                }

                hasWon = true;
            }
            
            if (hasWon)
            {
                ResetBoard();
                return Ok($"Player {currentPlayer} has won!");
            }


            for (int c = 2, r = 0; c > 0; c--, r++)
            {
                Step firstStep = board[r][c];
                Step nextStep = board[r + 1][c - 1];

                if (firstStep == null)
                {
                    hasWon = false;

                    break;
                }

                if (nextStep == null)
                {
                    hasWon = false;

                    break;
                }

                if (firstStep.Player != nextStep.Player)
                {
                    hasWon = false;

                    break;
                }

                hasWon = true;
            }

            if (hasWon)
            {
                ResetBoard();
                return Ok($"Player {currentPlayer} has won!");
            }

            if (currentPlayer == "X")
            {
                currentPlayer = "O";
            }
            else
            {
                currentPlayer = "X";
            }

            bool isTie = true;
            foreach (List<Step> rows in board)
            {
                foreach (Step step in rows)
                {
                    if (step == null)
                    {
                        isTie = false;
                        break;
                    }
                }
                if (!isTie)
                {
                    break;
                }
            }
            if (isTie)
            {
                ResetBoard();
                return Ok("The game ended in a tie.");
            }

            jsonData = JsonSerializer.Serialize(gameBoard);
            System.IO.File.WriteAllText("data.json", jsonData);

            return Ok("Move successful");
        }
    }

    class Step
    {
        public string Player { get; set; }
    }

    class GameBoard
    {
        public List<List<Step>> board = new List<List<Step>> {
                new List<Step> { null, null, null },
                new List<Step> { null, null, null },
                new List<Step> { null, null, null },
        };
        public string currentPlayer = "X";
    }
}

