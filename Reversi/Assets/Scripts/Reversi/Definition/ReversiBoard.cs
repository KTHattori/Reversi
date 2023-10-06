using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

namespace Reversi
{
    [System.Serializable]
    public class Board
    {
        // --- private ---
        // enum
        [Flags]
        private enum Direction
        {
            None        = 0,
            Upper       = 1 << 0,
            UpperLeft   = 1 << 1,
            Left        = 1 << 2,
            LowerLeft   = 1 << 3,
            Lower       = 1 << 4,
            LowerRight  = 1 << 5,
            Right       = 1 << 6,
            UpperRight  = 1 << 7,
        }

        // variable
        private DiscType[,] _rawBoard = new DiscType[Constant.BoardSize + 2, Constant.BoardSize + 2];

        private int _currentTurn;
        private DiscType _currentColor;

        private List<List<Disc>> _updatedDiscList = new List<List<Disc>>();
        private List<Disc> _undoneDiscList = new List<Disc>();

        [SerializeField]

        private List<Point>[] _movablePointList = new List<Point>[Constant.MaxTurn + 1];
        private Direction[,,] _movableDirection = new Direction[Constant.MaxTurn + 1,Constant.BoardSize+2,Constant.BoardSize+2];
        private DiscColorStorage<int> _discAmount = new DiscColorStorage<int>();


        // method

        /// <summary>
        /// point で指定された座標に石をうち、挟み込むことができるすべての石を裏返す。<br/>
        /// 「打った石」と「裏返した石」を updatedDiscList に追加する。
        /// </summary>
        /// <param name="point">石を打つ座標</param>
        private void FlipDiscs(in Point point)
        {
            int x,y;

            Direction dir = _movableDirection[_currentTurn,point.x,point.y];
            
            List<Disc> update = new List<Disc>();

            _rawBoard[point.x,point.y] = _currentColor;
            update.Add(new Disc(point.x,point.y,_currentColor));

            Debug.Log(dir);

            // 上に置ける場合
            if(dir.HasFlag(Direction.Upper))
            {
                y = point.y;
                while(_rawBoard[point.x,--y] != _currentColor)
                {
                    _rawBoard[point.x,y] = _currentColor;
                    update.Add(new Disc(point.x,y,_currentColor));
                }
            }

            // 下に置ける場合
            if(dir.HasFlag(Direction.Lower))
            {
                y = point.y;
                while(_rawBoard[point.x,++y] != _currentColor)
                {
                    _rawBoard[point.x,y] = _currentColor;
                    update.Add(new Disc(point.x,y,_currentColor));
                }
            }

            // 左に置ける場合
            if(dir.HasFlag(Direction.Left))
            {
                x = point.x;
                while(_rawBoard[--x,point.y] != _currentColor)
                {
                    _rawBoard[x,point.y] = _currentColor;
                    update.Add(new Disc(x,point.y,_currentColor));
                }
            }

            // 右に置ける場合
            if(dir.HasFlag(Direction.Right))
            {
                x = point.x;
                while(_rawBoard[++x,point.y] != _currentColor)
                {
                    _rawBoard[x,point.y] = _currentColor;
                    update.Add(new Disc(x,point.y,_currentColor));
                }
            }

            // 右上に置ける場合
            if(dir.HasFlag(Direction.UpperRight))
            {
                x = point.x;
                y = point.y;
                while(_rawBoard[++x,--y] != _currentColor)
                {
                    _rawBoard[x,y] = _currentColor;
                    update.Add(new Disc(x,y,_currentColor));
                }
            }

            // 左上に置ける場合
            if(dir.HasFlag(Direction.UpperLeft))
            {
                x = point.x;
                y = point.y;
                while(_rawBoard[--x,--y] != _currentColor)
                {
                    _rawBoard[x,y] = _currentColor;
                    update.Add(new Disc(x,y,_currentColor));
                }
            }

            // 左下に置ける場合
            if(dir.HasFlag(Direction.LowerLeft))
            {
                x = point.x;
                y = point.y;
                while(_rawBoard[--x,++y] != _currentColor)
                {
                    _rawBoard[x,y] = _currentColor;
                    update.Add(new Disc(x,y,_currentColor));
                }
            }

            // 右下に置ける場合
            if(dir.HasFlag(Direction.LowerRight))
            {
                x = point.x;
                y = point.y;
                while(_rawBoard[++x,++y] != _currentColor)
                {
                    _rawBoard[x,y] = _currentColor;
                    update.Add(new Disc(x,y,_currentColor));
                }
            }


            // 石の数を更新、反映
            int discdiff = update.Count;
            Debug.Log(discdiff);

            _discAmount[_currentColor] += discdiff;
            _discAmount[_currentColor.GetInvertedColor()] -= discdiff - 1;
            _discAmount[DiscType.Empty]--;

            _updatedDiscList.Add(update);
        }

        /// <summary>
        /// disc で指定された座標に disc.color の色の石を打てるかどうか、<br/>
        /// また、どの方向に石を裏返せるかを判定する。<br/>
        /// </summary>
        /// <param name="disc">指定する座標</param>
        /// <returns>石を裏返せる方向にフラグが立った整数値</returns>
        /// <seealso cref="Reversi.Board.Direction"/>
        private Direction CheckMobility(in Disc disc)
        {
            // 既に石が置かれていたら置けない
            // not placeable if a disc is already placed
            if (_rawBoard[disc.x, disc.y] != DiscType.Empty) return Direction.None;

            int x, y;
            Direction dir = Direction.None;

            // 上 - Upper
            if (_rawBoard[disc.x,disc.y - 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x;
                y = disc.y - 2;
                while (_rawBoard[x,y] == disc.discColor.GetInvertedColor()) { y--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Upper;
                Debug.Log("Upper");
            }

            // 下 - Lower
            if (_rawBoard[disc.x,disc.y + 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x;
                y = disc.y + 2;
                while (_rawBoard[x,y] == disc.discColor.GetInvertedColor()) { y++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Lower;
                Debug.Log("Lower");
            }

            // 左 - Left
            if (_rawBoard[disc.x - 1, disc.y] == disc.discColor.GetInvertedColor())
            {
                x = disc.x - 2;
                y = disc.y;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Left;
                Debug.Log("Left");
            }

            // 右 - Right
            if (_rawBoard[disc.x + 1, disc.y] == disc.discColor.GetInvertedColor())
            {
                x = disc.x + 2;
                y = disc.y;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Right;
                Debug.Log("Right");
            }

            // 右上 - UpperRight
            if (_rawBoard[disc.x + 1, disc.y - 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x + 2;
                y = disc.y - 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x++; y--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.UpperRight;
                Debug.Log("UpperRight");
            }

            // 左上 - UpperLeft
            if (_rawBoard[disc.x - 1, disc.y - 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x - 2;
                y = disc.y - 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x--; y--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.UpperLeft;
                Debug.Log("UpperLeft");
            }

            // 左下 - LowerLeft
            if (_rawBoard[disc.x - 1, disc.y + 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x - 2;
                y = disc.y + 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x--; y++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.LowerLeft;
                Debug.Log("LowerLeft");
            }


            // 右下 - LowerRight
            if (_rawBoard[disc.x + 1, disc.y + 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x + 2;
                y = disc.y + 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x++; y++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.LowerRight;
                Debug.Log("LowerRight");
            }

            return dir;
        }

        /// <summary>
        /// 現在の手番における着手可能な手を調べなおす。<br/>
        /// movablePoint, movableDirection を更新する。
        /// </summary>
        private void InitMovable()
        {
            Disc disc;
                
            Direction dir;
            _movablePointList[_currentTurn].Clear();

            for(int x = 1; x <= Constant.BoardSize; x++)
            {
                for(int y = 1 ; y <= Constant.BoardSize; y++)
                {
                    disc = new Disc(x,y,_currentColor);
                    dir = CheckMobility(disc);
                    if(dir != Direction.None)
                    {
                        // 配置可能な場所に追加
                        _movablePointList[_currentTurn].Add(disc);
                    }
                    _movableDirection[_currentTurn,x,y] = dir;
                }
            }
        }


        // --- public ---

        // constructor

        /// <summary>
        /// コンストラクタ。初期化を行う。
        /// </summary>
        public Board()
        {
            // それぞれのListを初期化
            for(int i = 0; i <= Constant.MaxTurn; i++)
            {
                _movablePointList[i] = new List<Point>();
            }

            Init();
        }


        // method

        /// <summary>
        /// ボードをゲーム開始直後の状態にする。
        /// </summary>
        public void Init()
        {
            // 全てのマスを空きマスに設定
            for(int x = 1;x <= Constant.BoardSize; x++)
            {
                for(int y = 1; y <= Constant.BoardSize; y++)
                {
                    _rawBoard[x,y] = DiscType.Empty;
                }
            }

            // 壁の設定
            for(int y = 0;y < Constant.BoardSize + 2; y++)
            {
                _rawBoard[0,y] = DiscType.Wall;
                _rawBoard[Constant.BoardSize + 1,y] = DiscType.Wall;
            }
            for(int x = 0;x < Constant.BoardSize + 2; x++)
            {
                _rawBoard[x,0] = DiscType.Wall;
                _rawBoard[x,Constant.BoardSize + 1] = DiscType.Wall;
            }

            // 初期配置
            _rawBoard[4,4] = DiscType.White;
            _rawBoard[5,5] = DiscType.White;
            _rawBoard[4,5] = DiscType.Black;
            _rawBoard[5,4] = DiscType.Black;

            // 石数の初期設定
            _discAmount[DiscType.Black] = 2;
            _discAmount[DiscType.White] = 2;
            _discAmount[DiscType.Empty] = Constant.BoardSize * Constant.BoardSize - 4;

            _currentTurn = 0;    // 手数は0スタート
            _currentColor = DiscType.Black; // 先手黒

            // 更新履歴をすべて削除
            _updatedDiscList.Clear();

            InitMovable();
        }

        /// <summary>
        /// point で指定された位置に石を打つ。
        /// </summary>
        /// <param name="point">石を打つ位置</param>
        /// <returns>
        /// 処理が成功したらtrue, 失敗したらfalse
        /// </returns>
        public bool Move(in Point point)
        {
            // 石が打てる位置かどうかを判定する
            // 打てない位置ならfalseで処理抜け
            // 座標の値が正しい範囲かどうかもここでチェック
            if(point.x < 0 || point.x > Constant.BoardSize) {Debug.Log("out of bounds!"); return false;}
            if(point.y < 0 || point.y > Constant.BoardSize) {Debug.Log("out of bounds!"); return false;}
            if(_movableDirection[_currentTurn,point.x,point.y] == Direction.None) {Debug.Log("nowhere to place!"); return false; }

            // 石を返す
            FlipDiscs(point);

            // 手番の色や現在の手数などを更新
            _currentTurn++;
            _currentColor = _currentColor.GetInvertedColor();

            // movableDir, movablePointを調べなおす
            InitMovable();

            return true;
        }

        /// <summary>
        /// パスを試み、その結果を返す。
        /// </summary>
        /// <returns>成功すればtru, 失敗したらfalse</returns>
        public bool Pass()
        {
            // 打つ手があれば、パスはできない
            if(_movablePointList[_currentTurn].Count != 0) return false;

            // ゲームが終了しているなら、パスはできない
            if(IsGameOver()) return false;
            _currentColor = _currentColor.GetInvertedColor();

            // 空の更新情報を追加
            _updatedDiscList.Add(new List<Disc>());

            InitMovable();

            return true;
        }

        /// <summary>
        /// 直前の一手を元に戻すことを試み、その結果を返す。
        /// </summary>
        /// <returns>成功するとtrue, 元に戻せない場合はfalse</returns>
        public bool Undo()
        {
            if(_currentTurn == 0) return false;

            _currentColor = _currentColor.GetInvertedColor();

            // リストをコピー
            List<Disc> update = new List<Disc>(_updatedDiscList[_updatedDiscList.Count - 1]);

            if(update.Count == 0)   // 前回がパスの場合
            {
                // movablePointListとmovableDirectionを再構築
                _movablePointList[_currentTurn].Clear();
                for(int x = 1;x <= Constant.BoardSize;x++)
                {
                    for(int y = 1;y <= Constant.BoardSize;y++)
                    {
                        _movableDirection[_currentTurn,x,y] = Direction.None;
                    }
                }
            }
            else    // 前回がパスでない
            {
                _currentTurn--;

                // 石を元に戻す
                _rawBoard[update[0].x,update[0].y] = DiscType.Empty;
                for(int i = 1; i < update.Count; i++)
                {
                    _rawBoard[update[i].x,update[i].y] = _currentColor.GetInvertedColor();
                }

                // 石数の更新
                int discdiff = update.Count;
                _discAmount[_currentColor] -= discdiff;
                _discAmount[_currentColor.GetInvertedColor()] += discdiff - 1;
                _discAmount[DiscType.Empty]++;
            }



            // やり直しリストを初期化
            if(_undoneDiscList != null) _undoneDiscList.Clear();
            else _undoneDiscList = new List<Disc>();
            
            // やり直しを行った石をリストに追加
            foreach(Disc disc in update)
            {
                _undoneDiscList.Add(new Disc(disc.x,disc.y,GetColor(disc.x,disc.y)));
            }


            // 不要になったupdateを1つ削除
            _updatedDiscList.RemoveAt(_updatedDiscList.Count - 1);

            return true;
        }

        /// <summary>
        /// ゲームが終了しているかどうかを返す。
        /// </summary>
        /// <returns>終了していればtrue, 終了していなければfalse</returns>
        public bool IsGameOver()
        {
            // 60手に達していたらゲーム終了
            if(_currentTurn == Constant.MaxTurn - 1) return true;

            // 打てる手があればゲーム終了ではない
            if(_movablePointList[_currentTurn].Count != 0) return false;

            // 現在の手番と逆の色が打てるかどうか調べる
            Disc disc = new Disc();
            disc.discColor = _currentColor.GetInvertedColor();
            for(int x = 1; x <= Constant.BoardSize; x++)
            {
                disc.x = x;
                for(int y = 1; y <= Constant.BoardSize; y++)
                {
                    disc.y = y;
                    // 置ける場所が1つでもある場合、ゲーム終了ではない
                    if(CheckMobility(disc) != Direction.None) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// color で指定された色の石数を数える。<br/>
        /// Black, White, Emptyが使用できる。
        /// </summary>
        /// <param name="color">指定された色</param>
        /// <returns></returns>
        public int CountDisc(DiscType color)
        {
            return _discAmount[color];
        }

        /// <summary>
        /// point で指定された座標の色を返す。
        /// </summary>
        /// <param name="point">指定されたボード座標上の色</param>
        /// <returns></returns>
        public DiscType GetColor(in Point point)
        {
            return _rawBoard[point.x,point.y];
        }

        /// <summary>
        /// x,y の整数値で指定された座標の色を返す
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <returns>指定されたボード座標上の色</returns>
        public DiscType GetColor(int x,int y)
        {
            return _rawBoard[x,y];
        }

        /// <summary>
        /// 石が打てる座標が並んだListを返す。
        /// </summary>
        /// <returns>石が打てる座標を格納したListのコピー</returns>
        public List<Point> GetMovablePoint()
        {
            return new List<Point>(_movablePointList[_currentTurn]);
        }

        /// <summary>
        /// やり直しを行った石が格納されたListを返す。
        /// </summary>
        /// <returns>やり直しされた石を格納したListのコピー</returns>
        public List<Disc> GetUndone()
        {
            return _undoneDiscList;
        }

        

        /// <summary>
        /// 直前の手で打った石と裏返した石が並んだListを返す。
        /// </summary>
        /// <returns>更新のあった石を格納したListのコピー</returns>
        public List<Disc> GetUpdate()
        {
            if(_updatedDiscList.Count == 0) return new List<Disc>();
            else return new List<Disc>(_updatedDiscList[_updatedDiscList.Count - 1]);
        }

        /// <summary>
        /// 現在の手番の色を返す。
        /// </summary>
        /// <returns>現在の手番の色を表す列挙体</returns>
        public DiscType GetCurrentColor()
        {
            return _currentColor;
        }

        /// <summary>
        /// 0から数えた現在の手数を返す。
        /// </summary>
        /// <returns>現在の手数の整数値</returns>
        public int GetCurrentTurn()
        {
            return _currentTurn;
        }
    }
}

