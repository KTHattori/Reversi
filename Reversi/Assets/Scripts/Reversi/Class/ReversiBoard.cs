using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

namespace Reversi
{
    /// <summary>
    /// リバーシのボードについて定義したクラス
    /// </summary>
    [System.Serializable]
    public class Board
    {
        // --- private ---
        // enum

        /// <summary>
        /// 方向を表すビットフラグ列挙体
        /// </summary>
        [Flags]
        public enum Direction
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

        /// <summary>
        /// 4方向を表すビットフラグ列挙体
        /// </summary>
        [Flags]
        public enum DirectionQuad
        {
            Top       = 1 << 0,
            Left        = 1 << 2,
            Bottom       = 1 << 4,
            Right       = 1 << 6,
        }

        /// <summary>
        /// 上下左右4方向のうち、maskDirで指定した方向を含んでいるかをチェックする拡張メソッド
        /// 4方向以外が指定された場合はfalseを返す
        /// </summary>
        /// <param name="maskDir">チェックする方向（上下左右）</param>
        /// <returns>含んでいればtrue, いなければfalse</returns>
        private bool GetMask(Direction dir,DirectionQuad maskDir)
        {
            switch(maskDir)
            {
            case DirectionQuad.Top:
                return dir.HasFlag(Direction.Upper | Direction.UpperLeft | Direction.UpperRight);
            case DirectionQuad.Left:
                return dir.HasFlag(Direction.Left | Direction.UpperLeft | Direction.LowerLeft);
            case DirectionQuad.Bottom:
                return dir.HasFlag(Direction.Lower | Direction.LowerLeft | Direction.LowerRight);
            case DirectionQuad.Right:
                return dir.HasFlag(Direction.Right | Direction.UpperRight | Direction.LowerRight);
            default:
                return false;
            }
        }

        // variable

        /// <summary>
        /// ボード上の各マスの石色情報を格納する配列
        /// </summary>
        private DiscColor[,] _rawBoard = new DiscColor[Constant.BoardSize + 2, Constant.BoardSize + 2];

        /// <summary>
        /// マスごとの開放度
        /// </summary>
        private int[,] liberty = new int[Constant.BoardSize + 2, Constant.BoardSize + 2];

        /// <summary>
        /// 現在のターン数
        /// </summary>
        private int _currentTurn;

        /// <summary>
        /// 現在の手番の色
        /// </summary>
        private DiscColor _currentColor;

        /// <summary>
        /// 更新された石の履歴のリスト（全手分）
        /// </summary>
        private List<List<Disc>> _updatedDiscList = new List<List<Disc>>();

        /// <summary>
        /// 一手戻された時に更新された石のリスト（一手分）
        /// </summary>
        private List<Disc> _undoneDiscList = new List<Disc>();

        /// <summary>
        /// 直前のターンで置かれた石を取得
        /// </summary>
        public Disc PlacedUpdated { get{ return GetUpdate()[0];}}

        /// <summary>
        /// 直前の一手戻しで戻された石を取得
        /// </summary>
        public Disc PlacedUndone { get{ return GetUndone()[0];}}

        /// <summary>
        /// 配置可能マスのリストを全手分格納する配列
        /// </summary>
        [SerializeField]
        private List<Point>[] _movablePointList = new List<Point>[Constant.MaxTurn + 1];
        
        /// <summary>
        /// 指定されたマスからひっくり返せる方向を格納した配列
        /// </summary>
        private Direction[,,] _movableDirection = new Direction[Constant.MaxTurn + 1,Constant.BoardSize+2,Constant.BoardSize+2];

        /// <summary>
        /// 黒石, 白石, 空のマスの個数を保存する変数
        /// </summary>
        private ColoredContainer<int> _discAmount = new ColoredContainer<int>();


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

            _discAmount[_currentColor] += discdiff;
            _discAmount[_currentColor.GetInvertedColor()] -= discdiff - 1;
            _discAmount[DiscColor.Empty]--;

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
            if (_rawBoard[disc.x, disc.y] != DiscColor.Empty) return Direction.None;

            int x, y;
            Direction dir = Direction.None;

            // 上 - Upper
            if (_rawBoard[disc.x,disc.y - 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x;
                y = disc.y - 2;
                while (_rawBoard[x,y] == disc.discColor.GetInvertedColor()) { y--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Upper;
            }

            // 下 - Lower
            if (_rawBoard[disc.x,disc.y + 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x;
                y = disc.y + 2;
                while (_rawBoard[x,y] == disc.discColor.GetInvertedColor()) { y++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Lower;
            }

            // 左 - Left
            if (_rawBoard[disc.x - 1, disc.y] == disc.discColor.GetInvertedColor())
            {
                x = disc.x - 2;
                y = disc.y;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Left;
            }

            // 右 - Right
            if (_rawBoard[disc.x + 1, disc.y] == disc.discColor.GetInvertedColor())
            {
                x = disc.x + 2;
                y = disc.y;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.Right;
            }

            // 右上 - UpperRight
            if (_rawBoard[disc.x + 1, disc.y - 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x + 2;
                y = disc.y - 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x++; y--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.UpperRight;
            }

            // 左上 - UpperLeft
            if (_rawBoard[disc.x - 1, disc.y - 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x - 2;
                y = disc.y - 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x--; y--; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.UpperLeft;
            }

            // 左下 - LowerLeft
            if (_rawBoard[disc.x - 1, disc.y + 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x - 2;
                y = disc.y + 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x--; y++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.LowerLeft;
            }


            // 右下 - LowerRight
            if (_rawBoard[disc.x + 1, disc.y + 1] == disc.discColor.GetInvertedColor())
            {
                x = disc.x + 2;
                y = disc.y + 2;
                while (_rawBoard[x, y] == disc.discColor.GetInvertedColor()) { x++; y++; }
                if (_rawBoard[x, y] == disc.discColor) dir |= Direction.LowerRight;
            }

            return dir;
        }


        private void UpdateLiberty(in Point point)
        {
            liberty[point.x,point.y - 1]--;
            liberty[point.x - 1,point.y - 1]--;
            liberty[point.x - 1,point.y]--;
            liberty[point.x - 1,point.y + 1]--;
            liberty[point.x,point.y + 1]--;
            liberty[point.x + 1,point.y + 1]--;
            liberty[point.x + 1,point.y]--;
            liberty[point.x + 1,point.y - 1]--;
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

            for(int y = 1; y <= Constant.BoardSize; y++)
            {
                for(int x = 1 ; x <= Constant.BoardSize; x++)
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

            // Init();
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
                    _rawBoard[x,y] = DiscColor.Empty;
                }
            }

            // 壁の設定
            for(int y = 0;y < Constant.BoardSize + 2; y++)
            {
                _rawBoard[0,y] = DiscColor.Wall;
                _rawBoard[Constant.BoardSize + 1,y] = DiscColor.Wall;
            }
            for(int x = 0;x < Constant.BoardSize + 2; x++)
            {
                _rawBoard[x,0] = DiscColor.Wall;
                _rawBoard[x,Constant.BoardSize + 1] = DiscColor.Wall;
            }

            // 初期配置
            _rawBoard[4,4] = DiscColor.White;
            _rawBoard[5,5] = DiscColor.White;
            _rawBoard[4,5] = DiscColor.Black;
            _rawBoard[5,4] = DiscColor.Black;

            // 石数の初期設定
            _discAmount[DiscColor.Black] = 2;
            _discAmount[DiscColor.White] = 2;
            _discAmount[DiscColor.Empty] = Constant.BoardSize * Constant.BoardSize - 4;

            _currentTurn = 0;    // 手数は0スタート
            _currentColor = DiscColor.Black; // 先手黒

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
            UpdateLiberty(point);

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
                _rawBoard[update[0].x,update[0].y] = DiscColor.Empty;
                for(int i = 1; i < update.Count; i++)
                {
                    _rawBoard[update[i].x,update[i].y] = _currentColor.GetInvertedColor();
                }

                // 石数の更新
                int discdiff = update.Count;
                _discAmount[_currentColor] -= discdiff;
                _discAmount[_currentColor.GetInvertedColor()] += discdiff - 1;
                _discAmount[DiscColor.Empty]++;
            }



            // やり直しリストを初期化
            if(_undoneDiscList != null) _undoneDiscList.Clear();
            else _undoneDiscList = new List<Disc>();
            
            // やり直しを行った石をリストに追加
            foreach(Disc disc in update)
            {
                _undoneDiscList.Add(new Disc(disc.x,disc.y,GetColorAt(disc.x,disc.y)));
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
            if(_currentTurn == Constant.MaxTurn) return true;

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
        public int CountDisc(DiscColor color)
        {
            return _discAmount[color];
        }

        /// <summary>
        /// 現在の手番での石数の差を返す。
        /// </summary>
        /// <returns></returns>
        public int GetDiscDiff()
        {
            return (int)_currentColor * CountDisc(DiscColor.Black) - CountDisc(DiscColor.White);
        }

        /// <summary>
        /// point で指定された座標での開放度を返す。
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int GetLibertyAt(in Point point)
        {
            return liberty[point.x,point.y];
        }

        /// <summary>
        /// point で指定された座標の色を返す。
        /// </summary>
        /// <param name="point">指定されたボード座標上の色</param>
        /// <returns></returns>
        public DiscColor GetColorAt(in Point point)
        {
            return _rawBoard[point.x,point.y];
        }

        /// <summary>
        /// x,y の整数値で指定された座標の色を返す
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <returns>指定されたボード座標上の色</returns>
        public DiscColor GetColorAt(int x,int y)
        {
            return _rawBoard[x,y];
        }

        /// <summary>
        /// 石が打てる座標が並んだListを返す。
        /// </summary>
        /// <returns>石が打てる座標を格納したListのコピー</returns>
        public List<Point> GetMovablePoints()
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
        public DiscColor GetCurrentColor()
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

        /// <summary>
        /// 現在の手でパス可能かどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool IsPassable()
        {
            // 打つ手があれば、パスはできない
            if(_movablePointList[_currentTurn].Count != 0) return false;

            // ゲームが終了しているなら、パスはできない
            if(IsGameOver()) return false;

            return true;
        }

        /// <summary>
        /// これまでに打たれた手のリストを返す
        /// </summary>
        /// <returns></returns>
        public List<Point> GetHistory()
        {
            List<Point> history = new List<Point>();

            for(int i = 0;i < _updatedDiscList.Count; i++)
            {
                List<Disc> update = _updatedDiscList[i];
                if(update.Count <= 0) continue; // パスの場合は飛ばす
                history.Add(update[0]);
            }

            return history;
        }
    }
}

