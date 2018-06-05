using UnityEngine;
using System.Collections;
namespace InteractSystem
{
    public enum ControllerType
    {
        Place,//匹配+安装
        Click ,//点击
        Rotate,//旋转
        Connect,//连接
        Rope ,//绳索
        Drag ,//拖拽
        Link,//关联
        Charge,//填充
        Create,//创建
        Detach,//拔除
    }
}