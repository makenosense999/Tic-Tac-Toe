using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TicTacToe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private List<List<Step>> board;
        private string currentPlayer = "X";

        private void ResetBoard()
        {
            board = Enumerable.Range(0, 3)
                .Select(_ => Enumerable.Repeat(new Step(), 3).ToList())
                .ToList();
            currentPlayer = "X";
        }

        [HttpPost]
        public IActionResult Move([FromBody] Move move)
        {
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
                Step firstStep = board[i][0];

                if (firstStep == null)
                {
                    break;
                }

                for (int j = 1; j < 3; j++)
                {
                    Step step = board[j][i];

                    if (step == null)
                    {

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
                    break;
                }

                if (nextStep == null)
                {
                    break;
                }

                if (firstStep.Player != nextStep.Player)
                {
                    hasWon = false;

                    break;
                }

                hasWon = true;
            }


            for (int i = 2; i > 1; i--)
            {
                Step firstStep = board[i][i];
                Step nextStep = board[i - 1][i - 1];

                if (firstStep == null)
                {
                    break;
                }

                if (nextStep == null)
                {
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

            string json = JsonSerializer.Serialize(board);
            System.IO.File.WriteAllText("data.json", json);

            return Ok("Move successful");

        }
    }

    class Step
    {
        public string Player { get; set; }
    }
}

