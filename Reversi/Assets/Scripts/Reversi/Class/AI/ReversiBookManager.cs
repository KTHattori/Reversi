using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Reversi
{
    public class BookManager
    {
        /// <summary>
        /// 定石木構造の根
        /// </summary>
        private Node _root = null;

        /// <summary>
        /// コンストラクタ。
        /// 定石ファイルから読み込みを行う。
        /// </summary>
        public BookManager()
        {
            _root = new Node
            {
                point = new Point("f5")
            };

            string[] allLine = LoadBookFile();

            foreach(var line in allLine)
            {
                List<Point> book = new List<Point>();
                for(int i = 0;i < line.Length;i +=2)
                {
                    Point point;
                    try
                    {
                        point = new Point(line.Substring(i));
                    }
                    catch(System.ArgumentException)
                    {
                        break;
                    }

                    book.Add(point);
                }

                AddBook(book);
            }
        }

        /// <summary>
        /// 定石ファイルを読み込んで、各行ごとのstring配列を返す。
        /// </summary>
        /// <returns></returns>
        private string[] LoadBookFile()
        {
#if UNITY_EDITOR
            // ファイル読み込み
            string path = "Assets/" + Constant.Book_FileName;
#else
             // ファイル読み込み
            string path = "Application.dataPath" + "/" + "Constant.Book_FileName";           
#endif
            if(File.Exists(path))
            {
                Debug.LogError("Book file not found!");
                return null;
            }

            return File.ReadAllLines(path);
        }

        /// <summary>
        /// 打てる手を並べたListを返す。
        /// 定石手があればその手のみを返し、ない場合は通常の配置可能なマスが返される。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public List<Point> Find(in Board board)
        {
            List<Point> history = board.GetHistory();
            Node node = _root;

            if(history.Count <= 0) return board.GetMovablePoints();

            Point first = history[0];
            CoordTransformer transformer = new CoordTransformer(first);

            // 座標を変換する
            List<Point> normalized = new List<Point>();
            for(int i = 0;i < history.Count; i++)
            {
                Point point = history[i];
                point = transformer.Normalize(point);

                normalized.Add(point);
            }

            // 現在までのリストと定石の対応をとる
            for(int i = 1;i < normalized.Count;i++)
            {
                Point point = normalized[i];

                node = node.child;
                while(node != null)
                {
                    if(node.point.Equals(point)) break;

                    node = node.sibling;
                }
                if(node == null)    // 定石を外れている
                {
                    return board.GetMovablePoints();
                }
            }

            // 履歴と定石の終わりが一致していた場合
            if(node.child == null) return board.GetMovablePoints();

            Point nextMove = GetNextMove(node);
            
            // 元の座標系に戻す
            nextMove = transformer.Denormalize(nextMove);

            List<Point> list = new List<Point>();
            list.Add(nextMove);

            return list;
        }

        public Point GetNextMove(Node node)
        {
            // 候補
            List<Point> candidates = new List<Point>();
            for(Node n = node.child; n != null; n = n.sibling)
            {
                candidates.Add(n.point);
            }
            
            // 乱数シードを初期化
            Random.InitState(System.DateTime.Now.Millisecond);

            // 候補の中からランダムで選択
            int index = Random.Range(0,int.MaxValue) * candidates.Count;
            return candidates[index];
        }

        /// <summary>
        /// bookで指定された定石を定石木に追加する。
        /// </summary>
        /// <param name="book"></param>
        public void AddBook(in List<Point> book)
        {
            Node node = _root;

            for(int i = 1;i < book.Count; i++)
            {
                Point point = book[i];

                if(node.child == null)
                {
                    // 新しい定石手
                    node.child = new Node();
                    node = node.child;
                    node.point.x = point.x;
                    node.point.y = point.y;
                }
                else
                {
                    // 探索に移る
                    node = node.child;

                    while(true)
                    {
                        if(node.sibling == null)
                        {
                            node.sibling = new Node();

                            node = node.sibling;
                            node.point.x = point.x;
                            node.point.y = point.y;
                            break;
                        }

                        node = node.sibling;
                    }
                }
            }
        }
    }
}
