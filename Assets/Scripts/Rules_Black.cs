using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Rules_Black : MonoBehaviour
    {
        public GameObject mate;
        public GameObject pat;
        // Start is called before the first frame update
        DragAndDrop_Black dad;
        Chess chess = new Chess("rnbqkbnr/pppppppp/8/8/11111111/11111N11/PPPPPPPP/RNBQKB1R b KQkq - 0 1");
        //Chess Nol = new Chess("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        Brains SkyNet = new Brains();
        Color EnemyColor = Color.white;
        //  string StartPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        //Chess chess = new Chess();
        public Rules_Black()
        {
            dad = new DragAndDrop_Black();
            // chess = new Chess("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public void Start()
        {
            //GameObject temp = GameObject.Find("Panel") as GameObject;
            //temp.SetActive(false);
            Debug.Log(chess.fen);
            ShowFigures();
        }


        public void Restart()
        {
            //GameObject tempur = GameObject.Find("Panel") as GameObject;
            //tempur.SetActive(true);
            mate.SetActive(false);
            chess = new Chess("rnbqkbnr/pppppppp/8/8/11111111/11111N11/PPPPPPPP/RNBQKB1R b KQkq - 0 1");
            // Debug.Log(chess.fen);
            ShowFigures();
        }


        public void Switch()
        {
            //pause.SetActive(true);
            SceneManager.LoadScene("WhiteTurn");
            //Application.LoadLevel("BlackTurn");
        }
        // Update is called once per frame
        void Update()
        {
            if (chess.board.moveColor == EnemyColor)
            {
                EnemyTurn();
            }
            if (dad.Action() == true)
            {
                Debug.Log(chess.fen);
                string move = "";
                string from = getSquare(dad.pickPosition);
                string to = getSquare(dad.dropPosition);
                string figure = chess.GetFigureAt(from).ToString();
                move = figure + from + to;
                if (figure == "K")
                {
                    if (from == "e1")
                    {
                        if (to == "c1")
                            move = "0-0-0";
                        if (to == "g1")
                            move = "0-0";
                    }
                }
                if (figure == "k")
                {
                    if (from == "e8")
                    {
                        if (to == "c8")
                            move = "0-0-0";
                        if (to == "g8")
                            move = "0-0";
                    }
                }
                if (figure == "P")
                {
                    if (to[1] == '8' && from[1] == '7')
                        move += "Q";
                }
                if (figure == "p")
                {
                    if (to[1] == '1' && from[1] == '2')
                        move += "q";
                }

                Debug.Log("+======  " + move);
                chess = chess.Move(move);
                ShowFigures();
                if (chess.IsMate())
                {
                    mate.SetActive(true);
                }
                if (chess.IsPat())
                {
                    pat.SetActive(true);
                }
            }

        }

        void EnemyTurn()
        {
            // Debug.Log("1 - "+chess.board.moveColor);
            string move2 = SkyNet.MiniMaxRoot(3, chess, EnemyColor);
            chess = chess.Move(move2);
            //chess.board.moveColor=Color.white;
            //Debug.Log("2 - " + chess.board.moveColor);
            ShowFigures();
            // Debug.Log(move2);
            // Debug.Log(chess.fen);
            // Debug.Log(chess.board.moveColor);
            if (chess.IsMate())
            {
                mate.SetActive(true);
            }
        }

        string getSquare(Vector2 position)
        {
            int x = Convert.ToInt32(position.x / 2.0);
            int y = Convert.ToInt32(position.y / 2.0);
            //Debug.Log(y);
            return ((char)('a' + x)).ToString() + (y + 1).ToString();
        }

        void ShowFigures()
        {
            int nr = 0;
            //string DeadFigure="";
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)//int y=0;y<8;y++
                {
                    string figure = chess.GetFigureAt(x, y).ToString();
                    if (figure == ".") continue;
                    PlaceFigure("box" + nr, figure, x, y);
                    //DeadFigure = figure;
                    //Debug.Log(figure + " 898899798980908 " + x + y);
                    nr++;
                }
            for (; nr < 32; nr++)
            {
                PlaceFigure("box" + nr, "q", 9, 9);
                //Debug.Log(nr+" ----- after");
            }

        }

        void PlaceFigure(string box, string figure, int x, int y)
        {
            //Debug.Log(box + " " + figure + " " + x + y);
            GameObject goBox = GameObject.Find(box);
            GameObject goFigure = GameObject.Find(figure);
            GameObject goSquare = GameObject.Find("" + y + x);

            var spriteFigure = goFigure.GetComponent<SpriteRenderer>();
            var spriteBox = goBox.GetComponent<SpriteRenderer>();
            spriteBox.sprite = spriteFigure.sprite;

            goBox.transform.position = goSquare.transform.position;
            //goBox.GetComponent<SpriteRenderer>().sortingOrder =Convert.ToInt32( goBox.transform.position.y);
            if (goBox.transform.position.y / 2 == 7)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(7);
            if (goBox.transform.position.y / 2 == 6)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(6);
            if (goBox.transform.position.y / 2 == 5)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(5);
            if (goBox.transform.position.y / 2 == 4)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(4);
            if (goBox.transform.position.y / 2 == 3)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(3);
            if (goBox.transform.position.y / 2 == 2)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(2);
            if (goBox.transform.position.y / 2 == 1)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(1);
            if (goBox.transform.position.y / 2 == 0)
                goBox.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(0);
            var position = goBox.transform.position;
            position.y -= 0.5f;
            goBox.transform.position = position;
        }

        void MarkSquare(int x, int y, bool isMarked)
        {
            GameObject goSquare = GameObject.Find("" + y + x);
            GameObject gocell;
            // string color="";
            string color = (x + y) % 2 == 0 ? "Black" : "White";
            //if ((x + y) % 2 == 0)
            //    color = "Black";
            //if ((x + y) % 2 != 0)
            //    color = "White";
            if (isMarked)
                gocell = GameObject.Find(color + "SquareMarked");
            else
                gocell = GameObject.Find(color + "Square");
            //Debug.Log("" + x+y);
            var spriteSquare = goSquare.GetComponent<SpriteRenderer>();
            var spriteCell = gocell.GetComponent<SpriteRenderer>();
            spriteSquare.sprite = spriteCell.sprite;

        }
    }

    class DragAndDrop_Black
    {
        enum State
        {
            none,
            drag,
        }
        public Vector2 pickPosition { get; private set; }
        public Vector2 dropPosition { get; private set; }

        State state;
        GameObject item;
        Vector2 offset;

        public DragAndDrop_Black()
        {
            state = State.none;
            item = null;
        }

        public bool Action()
        {
            switch (state)
            {
                case State.none:
                    if (IsMouseButtonPressed())
                    {
                        //Debug.Log("Pick");
                        PickUp();
                    }
                    break;
                case State.drag:
                    if (IsMouseButtonPressed())
                    {
                        //Debug.Log("Drag");
                        Drag();
                    }
                    else
                    {
                        //Debug.Log("Drop");
                        Drop();
                        return true;
                    }
                    break;
            }
            return false;
        }

        bool IsMouseButtonPressed()
        {
            return Input.GetMouseButton(0);
        }

        void PickUp()
        {
            Vector2 clickPosition = GetClickPosition();
            Transform clickedItem = GetItemAt(clickPosition);
            if (clickedItem == null) return;
            pickPosition = clickedItem.position;
            item = clickedItem.gameObject;
            //Debug.Log("PickedUp" + item.name);
            state = State.drag;
            offset = pickPosition - clickPosition;
            Debug.Log("PickedUp" + item.name);
        }

        Vector2 GetClickPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        Transform GetItemAt(Vector2 position)
        {
            RaycastHit2D[] figures = Physics2D.RaycastAll(position, position, 0.5f);
            if (figures.Length == 0)
                return null;
            return figures[0].transform;
        }

        void Drag()
        {
            item.transform.position = GetClickPosition();
        }

        void Drop()
        {
            dropPosition = item.transform.position;
            state = State.none;
            item = null;
        }
    }
}
