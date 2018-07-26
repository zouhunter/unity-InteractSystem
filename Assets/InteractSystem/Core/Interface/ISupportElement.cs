using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{
    public interface IActiveAble {
        bool Actived { get; }//即使步骤过去也可以是激活状态
        void SetActive(UnityEngine.Object target);//使用到该元素的步骤开始
        void SetInActive(UnityEngine.Object target);//使用到该元素的步骤结束
        void UnDoChanges(UnityEngine.Object target);////重置元素信息在目标的改变
    }

    public interface IVisiable
    {
        void SetVisible(bool visible);///隐藏显示状态
        GameObject Body { get; }//必须绑定到一个GameObject对象
    }

    public interface IContinueUse
    {
        bool OperateAble { get; }//能否操作
        void RecordPlayer(UnityEngine.Object target);//记录元素
        void RemovePlayer(UnityEngine.Object target);//清除记录
    }

    public interface ISupportElement:IActiveAble, IVisiable, IContinueUse
    {
        string Name { get; }//唯一的名称
        bool IsRuntimeCreated { get; set; }//判断是静态的元素还是动态创建
    }
}
