using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Reversi
{
    public class BookManager
    {
        /// <summary>
        /// このクラスのインスタンス
        /// </summary>
        private static BookManager _instance = null;

        /// <summary>
        /// インスタンスを外部から呼び出すためのプロパティ
        /// </summary>
        public static BookManager Instance
        {
            // ゲッター
            get
            {
                if( _instance != null )
                {   // インスタンスが存在していれば取得
                    return _instance;
                }
                else if( _instance == null )
                {   // 見つからない場合生成
                    _instance = new BookManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 定石木構造の根
        /// </summary>
        private Node _root = null;

        /// <summary>
        /// ロードされた定石ファイルアセット
        /// </summary>
        private static TextAsset _loadedBook = null;

        /// <summary>
        /// ロードされているかどうか
        /// </summary>
        private static bool _isLoaded = false;

        private static bool _isLoading = false;
 
        /// <summary>
        /// コンストラクタ。
        /// 定石ファイルから読み込みを行う。
        /// シングルトン実装のためprivate指定.
        /// </summary>
        private BookManager()
        {
            _root = new Node
            {
                point = new Point("f5")
            };
        }

        /// <summary>
        /// 定石読み込み
        /// </summary>
        public async void LoadBookFile(AssetReference assetReference)
        {
            _isLoading = true;
            if(!_isLoaded) Addressables.LoadAssetAsync<TextAsset>(assetReference).Completed += load =>
            {
                _loadedBook = load.Result;

                _isLoading = false;
                _isLoaded = true;
                _instance.CreateBookTree(_loadedBook);
                Debug.Log("Loaded");
            };

            while(_isLoading)
            {
                await Task.Delay(10);
            }
        }

        /// <summary>
        /// 定石木を作成する
        /// </summary>
        /// <param name="bookAsset"></param>
        private void CreateBookTree(TextAsset bookAsset)
        {
            StringReader reader = new StringReader(bookAsset.text);
            while (reader.Peek() != -1) // reader.Peekが-1になるまで
            {
                string line = reader.ReadLine(); // 一行ずつ読み込み

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
        /// 打てる手を並べたListを返す。
        /// 定石手があればその手のみを返し、ない場合は通常の配置可能なマスが返される。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public List<Point> Find(in Board board)
        {
            List<Point> history = board.GetHistory();
            Node node = _root;

            if(history.Count <= 0 || !_isLoaded) return board.GetMovablePoints();

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
                    UnityEngine.Debug.Log("Out of book.");
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

            Debug.Log("Found in book.");

            return list;
        }

        /// <summary>
        /// 次の手を候補の中からランダムで取得する
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Point GetNextMove(Node node)
        {
            // 候補
            List<Point> candidates = new List<Point>();
            for(Node n = node.child; n != null; n = n.sibling)
            {
                candidates.Add(n.point);
            }
            
            // 乱数シードを生成
            System.Random random = new System.Random(System.DateTime.Now.Millisecond);

            // 候補の中からランダムで選択
            int index = random.Next(0,int.MaxValue) % candidates.Count;
            Point point = candidates[index];
            
            return new Point(point.x,point.y);
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
                        if(node.point.Equals(point)) break;
                        
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

        /// <summary>
        /// ロードが完了していればルートノードを取得, そうでない場合はnull
        /// </summary>
        /// <returns></returns>
        public Node GetBook()
        {
            if(_isLoaded) return _root;
            return null;
        }
    }
}
