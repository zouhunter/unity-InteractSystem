using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public interface ISupportElement
    {
        string Name { get; set; }//唯一的名称
        GameObject Body { get; }//必须绑定到一个GameObject对象
        bool IsRuntimeCreated { get; set; }//判断是静态的元素还是动态创建
        bool Active { get; }//即使步骤过去也可以是激活状态
        void StepActive();//使用到该元素的步骤开始
        void StepComplete();//使用到该元素的步骤结束
        void StepUnDo();////使用到该元素的步骤回退
        void SetVisible(bool visible);///隐藏显示状态
    }
}
