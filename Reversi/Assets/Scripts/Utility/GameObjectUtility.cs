using UnityEngine;
/// <summary>
/// GameObject用の拡張メソッド、関数を定義したクラス
/// 参考・引用：https://qiita.com/msm2001/items/ed5d56ebbfcad19c488d
/// </summary>
public static class GameObjectUtility
{
    /// <summary>
    /// 拡張メソッド：名前を変更する
    /// </summary>
    /// <param name="gameObject">このゲームオブジェクト</param>
    /// <param name="name">変更先の名前</param>
    public static void Rename(this GameObject gameObject, string name)
    {
        gameObject.name = name;
    }

    // ---------------------------------------------
    //  Instantiate
    // ---------------------------------------------

    /// <summary>
    /// 拡張対象のGameObjectを複製(生成)して返す
    /// </summary>
    public static GameObject Instantiate(this GameObject gameObject)
    {
        return Object.Instantiate(gameObject);
    }

    /// <summary>
    /// 生成後に親となるTransformを指定して、拡張対象のGameObjectを複製(生成)して返す
    /// </summary>
    public static GameObject Instantiate(this GameObject gameObject, Transform parent)
    {
        return Object.Instantiate(gameObject, parent);
    }

    /// <summary>
    /// 生成後の座標及び姿勢を指定して、拡張対象のGameObjectを複製(生成)して返す
    /// </summary>
    public static GameObject Instantiate(this GameObject gameObject, Vector3 pos, Quaternion rot)
    {
        return Object.Instantiate(gameObject, pos, rot);
    }

    /// <summary>
    /// 生成後に親となるTransform、また生成後の座標及び姿勢を指定して、拡張対象のGameObjectを複製(生成)して返す
    /// </summary>
    public static GameObject Instantiate(this GameObject gameObject, Vector3 pos, Quaternion rot, Transform parent)
    {
        return Object.Instantiate(gameObject, pos, rot, parent);
    }

    /// <summary>
    /// 生成後に親となるTransform、また生成後のローカル座標を指定して、拡張対象のGameObjectを複製(生成)して返す
    /// </summary>
    public static GameObject InstantiateWithLocalPosition(this GameObject gameObject, Transform parent,
        Vector3 localPos)
    {
        var instance = Object.Instantiate(gameObject, parent);
        instance.transform.localPosition = localPos;
        return instance;
    }

    /// <summary>
    /// T型のComponentを持つGameObjectを生成する
    /// </summary>
    /// <typeparam name="T">コンポーネント クラス</typeparam>
    /// <param name="component">コンポーネント</param>
    /// <returns>コンポーネント参照</returns>
    public static T Instantiate<T>(this T component) where T : Component
    {
        return Object.Instantiate(component);
    }

    /// <summary>
    /// 生成後に親となるTransformを指定して、T型のComponentを持つGameObjectを生成する
    /// </summary>
    /// <typeparam name="T">コンポーネント クラス</typeparam>
    /// <param name="component">コンポーネント</param>
    /// <param name="parent">親Transform</param>
    /// <returns>コンポーネント参照</returns>
    public static T Instantiate<T>(this T component, Transform parent) where T : Component
    {
        return Object.Instantiate(component, parent);
    }

    /// <summary>
    /// 生成後の座標及び姿勢を指定して、拡張対象のGameObjectにT型のComponentをアタッチして複製(生成)して返す
    /// </summary>
    /// <typeparam name="T">コンポーネント クラス</typeparam>
    /// <param name="component">コンポーネント</param>
    /// <param name="pos">座標</param>
    /// <param name="rot">姿勢</param>
    /// <returns>コンポーネント参照</returns>
    public static T Instantiate<T>(this T component, Vector3 pos, Quaternion rot) where T : Component
    {
        return Object.Instantiate(component, pos, rot);
    }

    /// <summary>
    /// 生成後に親となるTransform、また生成後の座標及び姿勢を指定して、T型のComponentをアタッチしたGameObjectを複製(生成)して返す
    /// </summary>
    /// <typeparam name="T">コンポーネント クラス</typeparam>
    /// <param name="component">コンポーネント</param>
    /// <param name="pos">座標</param>
    /// <param name="rot">姿勢</param>
    /// <param name="parent">親Transform</param>
    /// <returns>コンポーネント参照</returns>
    public static T Instantiate<T>(this T component, Vector3 pos, Quaternion rot, Transform parent)
        where T : Component
    {
        return Object.Instantiate(component, pos, rot, parent);
    }

    /// <summary>
    /// 生成後に親となるTransform、また生成後のローカル座標を指定して、T型のComponentをアタッチしたGameObjectを複製(生成)して返す
    /// </summary>
    /// <typeparam name="T">コンポーネント クラス</typeparam>
    /// <param name="component">コンポーネント</param>
    /// <param name="parent">親Transform</param>
    /// <param name="localPos">ローカル座標</param>
    /// <returns>コンポーネント参照</returns>
    public static T InstantiateWithLocalPosition<T>(this T component, Transform parent,
        Vector3 localPos) where T : Component
    {
        var instance = Object.Instantiate(component, parent);
        instance.transform.localPosition = localPos;
        return instance;
    }


    // ---------------------------------------------
    //  Destroy
    // ---------------------------------------------

    /// <summary>
    /// このGameObjectを破棄する
    /// </summary>
    /// <param name="gameObject">このゲームオブジェクト</param>
    public static void Destroy(this GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }

    /// <summary>
    /// ComponentがアタッチされたGameObjectを破棄する
    /// </summary>
    /// <param name="component">このゲームオブジェクト</param>
    public static void Destroy(this Component component)
    {
        Object.Destroy(component);
    }

    /// <summary>
    /// ComponentがアタッチされているGameObjectを破棄する
    /// </summary>
    public static void DestroyGameObject(this Component component)
    {
        Object.Destroy(component.gameObject);
    }
}
