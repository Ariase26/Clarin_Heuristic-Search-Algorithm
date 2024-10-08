using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;

namespace ACT4
{
    public partial class Form1 : Form
    {
        private const int BoardSize = 6;
        private int side;
        private int moveCounter;
        private SixState startState;
        private SixState currentState;
        private int[,] heuristicTable;
        private ArrayList bestMoves;
        private object chosenMove;

        public Form1()
        {
            InitializeComponent();

            side = pictureBox1.Width / BoardSize;
            startState = GenerateRandomSixState();
            currentState = new SixState(startState);

            UpdateUI();
            label1.Text = $"Attacking pairs: {GetAttackingPairs(startState)}";
        }

        private void UpdateUI()
        {
            pictureBox2.Refresh();
            label3.Text = $"Attacking pairs: {GetAttackingPairs(currentState)}";
            label4.Text = $"Moves: {moveCounter}";

            heuristicTable = GenerateHeuristicTable(currentState);
            bestMoves = GetBestMoves(heuristicTable);

            listBox1.Items.Clear();
            foreach (Point move in bestMoves)
            {
                listBox1.Items.Add(move);
            }

            if (bestMoves.Count > 0)
                chosenMove = ChooseMove(bestMoves);

            label2.Text = $"Chosen move: {chosenMove}";
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard(e, startState);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard(e, currentState);
        }

        private void DrawBoard(PaintEventArgs e, SixState state)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Brush squareColor = (i + j) % 2 == 0 ? Brushes.Blue : Brushes.Black;
                    e.Graphics.FillRectangle(squareColor, i * side, j * side, side, side);

                    if (j == state.Y[i])
                    {
                        e.Graphics.FillEllipse(Brushes.Fuchsia, i * side, j * side, side, side);
                    }
                }
            }
        }

        private double GetTemperature(int hValue)
        {
            return Math.Exp(-hValue / (moveCounter + 1));
        }

        private SixState GenerateRandomSixState()
        {
            Random random = new Random();
            return new SixState(
                random.Next(BoardSize),
                random.Next(BoardSize),
                random.Next(BoardSize),
                random.Next(BoardSize),
                random.Next(BoardSize),
                random.Next(BoardSize));
        }

        private int GetAttackingPairs(SixState state)
        {
            int attackers = 0;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = i + 1; j < BoardSize; j++)
                {
                    if (state.Y[i] == state.Y[j])
                        attackers++;
                    
                    if (state.Y[j] == state.Y[i] + (j - i) || state.Y[i] == state.Y[j] + (j - i))
                        attackers++;
                }
            }

            return attackers;
        }

        private int[,] GenerateHeuristicTable(SixState state)
        {
            int[,] heuristicTable = new int[BoardSize, BoardSize];

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    SixState possibleState = new SixState(state);
                    possibleState.Y[i] = j;
                    heuristicTable[i, j] = GetAttackingPairs(possibleState);
                }
            }

            return heuristicTable;
        }

        private ArrayList GetBestMoves(int[,] heuristicTable)
        {
            ArrayList bestMoves = new ArrayList();
            int bestHeuristic = heuristicTable[0, 0];
            Random random = new Random();

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (bestHeuristic > heuristicTable[i, j])
                    {
                        bestHeuristic = heuristicTable[i, j];
                        bestMoves.Clear();
                    }

                    if (bestHeuristic == heuristicTable[i, j] || random.NextDouble() <= GetTemperature(heuristicTable[i, j]))
                    {
                        if (currentState.Y[i] != j)
                            bestMoves.Add(new Point(i, j));
                    }
                }
            }

            label5.Text = $"Possible Moves (H={bestHeuristic})";
            return bestMoves;
        }

        private object ChooseMove(ArrayList moves)
        {
            Random random = new Random();
            int selectedIndex = random.Next(moves.Count);
            return moves[selectedIndex];
        }

        private void ExecuteMove(Point move)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                startState.Y[i] = currentState.Y[i];
            }

            currentState.Y[move.X] = move.Y;
            moveCounter++;

            chosenMove = null;
            UpdateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (GetAttackingPairs(currentState) > 0)
                ExecuteMove((Point)chosenMove);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            startState = GenerateRandomSixState();
            currentState = new SixState(startState);
            moveCounter = 0;
            UpdateUI();
            pictureBox1.Refresh();
            label1.Text = $"Attacking pairs: {GetAttackingPairs(startState)}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while (GetAttackingPairs(currentState) > 0)
            {
                ExecuteMove((Point)chosenMove);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}