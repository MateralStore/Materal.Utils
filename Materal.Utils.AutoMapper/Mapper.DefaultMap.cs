using System.Collections.Concurrent;
using System.Reflection.Emit;

namespace Materal.Utils.AutoMapper
{
    public partial class Mapper
    {
        private readonly ConcurrentDictionary<Type, Delegate> _copyPropertiesFunc = [];
        private readonly MethodInfo _mapperGenericMethod;
        private readonly MethodInfo _mapperMethod;
        private void DefaultMap(object source, object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();
            Type actionType = GetActionType(sourceType, targetType);
            if (!_copyPropertiesFunc.TryGetValue(actionType, out Delegate? action))
            {
                action = CreateCopyPropertiesDelegate(sourceType, targetType);
                if (action.GetType() != actionType) throw new InvalidOperationException("创建的委托类型不正确");
                _copyPropertiesFunc.TryAdd(actionType, action);
            }
            if (action is null) return;
            try
            {
                action.DynamicInvoke(source, target, this);
            }
            catch (Exception ex)
            {
                throw new MateralAutoMapperException("执行委托失败", ex);
            }
        }
        /// <summary>
        /// 获取Action类型
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static Type GetActionType(Type sourceType, Type targetType) => typeof(Action<,,>).MakeGenericType(sourceType, targetType, typeof(IMapper));
        /// <summary>
        /// 根据Type创建委托
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private Delegate CreateCopyPropertiesDelegate(Type sourceType, Type targetType)
        {
            DynamicMethod dynamicMethod = CreateCopyPropertiesDynamicMethod(sourceType, targetType);
            Type actionType = GetActionType(sourceType, targetType);
            try
            {
                Delegate result = dynamicMethod.CreateDelegate(actionType);
                return result;
            }
            catch (Exception ex)
            {
                throw new MateralAutoMapperException("创建委托失败", ex);
            }
        }
        /// <summary>
        /// 创建属性复制方法
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        /// <exception cref="MateralException"></exception>
        private DynamicMethod CreateCopyPropertiesDynamicMethod(Type sourceType, Type targetType)
        {
            DynamicMethod dynamicMethod = new("MapObject", null, [sourceType, targetType, typeof(IMapper)], sourceType.Module);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            foreach (PropertyInfo sourcePropertyInfo in sourceType.GetProperties())
            {
                if (!sourcePropertyInfo.CanRead || !sourcePropertyInfo.CanWrite) continue;
                PropertyInfo? targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name);
                if (targetPropertyInfo is null || !targetPropertyInfo.CanWrite) continue;
                MethodInfo? getMethod = sourcePropertyInfo.GetGetMethod();
                if (getMethod is null) continue;
                MethodInfo? setMethod = targetPropertyInfo.GetSetMethod();
                if (setMethod is null) continue;
                if (sourcePropertyInfo.PropertyType == targetPropertyInfo.PropertyType)
                {
                    WriteILTheSameType(ilGenerator, getMethod, setMethod);
                }
                else if (targetPropertyInfo.PropertyType.IsNullableType(sourcePropertyInfo.PropertyType))
                {
                    WriteILTheTargetNullType(ilGenerator, getMethod, setMethod, sourcePropertyInfo, targetPropertyInfo);
                }
                else if (sourcePropertyInfo.PropertyType.IsNullableType(targetPropertyInfo.PropertyType))
                {
                    WriteILTheSourceNullType(ilGenerator, getMethod, setMethod, sourcePropertyInfo);
                }
                else
                {
                    if (sourcePropertyInfo.PropertyType.IsPrimitive || sourcePropertyInfo.PropertyType == typeof(string) || sourcePropertyInfo.PropertyType.IsValueType) continue;
                    if (targetPropertyInfo.PropertyType.IsPrimitive || targetPropertyInfo.PropertyType == typeof(string) || targetPropertyInfo.PropertyType.IsValueType) continue;
                    WriteILTheInvokeMapper(ilGenerator, sourcePropertyInfo, targetPropertyInfo);
                }
            }
            // 返回
            ilGenerator.Emit(OpCodes.Ret);
            return dynamicMethod;
        }
        /// <summary>
        /// 写入IL-使用Mapper对象
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="sourcePropertyInfo"></param>
        /// <param name="targetPropertyInfo"></param>
        private void WriteILTheInvokeMapper(ILGenerator ilGenerator, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo)
        {
            /*            
            if(source.Sub is null)
            {
                return;
            }
            if(target.Sub is null)
            {
                target.Sub = mapper.Map<SubC>(source.Sub);
            }
            else
            {
                mapper.Map(source.Sub, target.Sub);
            }
            */
            // 定义当前块所需的局部变量和标签
            LocalBuilder sourceValueLocalBuilder = ilGenerator.DeclareLocal(sourcePropertyInfo.PropertyType);// 用于存储source.Sub
            LocalBuilder isNullLocalBuilder = ilGenerator.DeclareLocal(typeof(bool));// 用于映射块的null判断
            Label sourceNullLabel = ilGenerator.DefineLabel();// source.Sub为null的标签
            Label elseLabel = ilGenerator.DefineLabel();// 映射块else分支
            Label endLabel = ilGenerator.DefineLabel(); // 映射块结束
            MethodInfo sourceGetMethod = sourcePropertyInfo.GetGetMethod()!;
            MethodInfo targetGetMethod = targetPropertyInfo.GetGetMethod()!;
            // 获取并存储source.Sub
            ilGenerator.Emit(OpCodes.Ldarg_0);// source
            ilGenerator.Emit(OpCodes.Callvirt, sourceGetMethod);// 获取source.Sub
            ilGenerator.Emit(OpCodes.Stloc, sourceValueLocalBuilder);// 存储到局部变量
            // 判断source.Sub是否为null
            ilGenerator.Emit(OpCodes.Ldloc, sourceValueLocalBuilder);
            ilGenerator.Emit(OpCodes.Ldnull);
            ilGenerator.Emit(OpCodes.Ceq);
            ilGenerator.Emit(OpCodes.Brtrue_S, sourceNullLabel);// 如果为null则跳到结束
            // 判断target.Sub是否为null
            ilGenerator.Emit(OpCodes.Ldarg_1);// target
            ilGenerator.Emit(OpCodes.Callvirt, targetGetMethod);// 获取target.Sub
            ilGenerator.Emit(OpCodes.Ldnull);
            ilGenerator.Emit(OpCodes.Ceq);// 比较是否为null
            ilGenerator.Emit(OpCodes.Stloc, isNullLocalBuilder);// 存储结果到当前块局部变量
            ilGenerator.Emit(OpCodes.Ldloc, isNullLocalBuilder);// 加载判断结果
            ilGenerator.Emit(OpCodes.Brfalse_S, elseLabel);// 不为null则跳else
            // if分支 (target.Sub == null && source.Sub != null)
            ilGenerator.Emit(OpCodes.Ldarg_1);// target
            ilGenerator.Emit(OpCodes.Ldarg_2);// mapper
            ilGenerator.Emit(OpCodes.Ldloc, sourceValueLocalBuilder);// 加载source.Sub
            // 调用泛型Map<SubModelC>
            MethodInfo mapperGenericMethod = _mapperGenericMethod.MakeGenericMethod(targetPropertyInfo.PropertyType);
            ilGenerator.Emit(OpCodes.Callvirt, mapperGenericMethod);
            MethodInfo targetSetMethod = targetPropertyInfo.GetSetMethod()!;
            ilGenerator.Emit(OpCodes.Callvirt, targetSetMethod);// 赋值给target.Sub
            ilGenerator.Emit(OpCodes.Br_S, endLabel);// 跳转到块结束
            // else分支 (target.Sub != null && source.Sub != null)
            ilGenerator.MarkLabel(elseLabel);
            ilGenerator.Emit(OpCodes.Ldarg_2);// mapper
            ilGenerator.Emit(OpCodes.Ldloc, sourceValueLocalBuilder);// 加载source.Sub
            ilGenerator.Emit(OpCodes.Ldarg_1);// target
            ilGenerator.Emit(OpCodes.Callvirt, targetGetMethod);// target.Sub
            ilGenerator.Emit(OpCodes.Callvirt, _mapperMethod);// 映射到已有对象
            // source.Sub为null的标签
            ilGenerator.MarkLabel(sourceNullLabel);
            // 映射块结束
            ilGenerator.MarkLabel(endLabel);
        }
        /// <summary>
        /// 写入IL-源是可空类型
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="getMethod"></param>
        /// <param name="setMethod"></param>
        /// <param name="sourcePropertyInfo"></param>
        /// <exception cref="Exception"></exception>
        private static void WriteILTheSourceNullType(ILGenerator ilGenerator, MethodInfo getMethod, MethodInfo setMethod, PropertyInfo sourcePropertyInfo)
        {
            /*
            if ((isCopy == null || isCopy("Age")) && source.Age.HasValue)
            {
                target.Age = source.Age.Value;
            }
             */
            // 定义本地变量
            LocalBuilder localBuilder = ilGenerator.DeclareLocal(sourcePropertyInfo.PropertyType);
            // 加载第一个参数（source）到计算堆栈
            ilGenerator.Emit(OpCodes.Ldarg_0);
            // 调用获取属性方法
            ilGenerator.Emit(OpCodes.Callvirt, getMethod);
            // 将结果存储到本地变量
            ilGenerator.Emit(OpCodes.Stloc, localBuilder);
            // 加载本地变量的地址到计算堆栈
            ilGenerator.Emit(OpCodes.Ldloca_S, localBuilder);
            // 调用Source的get_HasValue方法
            MethodInfo getHasValueMethod = sourcePropertyInfo.PropertyType.GetProperty(nameof(Nullable<>.HasValue))?.GetGetMethod() ?? throw new Exception("获取HasValue失败");
            ilGenerator.Emit(OpCodes.Call, getHasValueMethod);
            // 如果get_HasValue返回false，跳转到指定标签
            Label hasValueFalseLabel = ilGenerator.DefineLabel();
            ilGenerator.Emit(OpCodes.Brfalse_S, hasValueFalseLabel);
            // 加载第二个参数（target）和第一个参数（source）到计算堆栈
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            // 调用BClass的get_Age方法
            ilGenerator.Emit(OpCodes.Callvirt, getMethod);
            // 将结果存储到本地变量
            ilGenerator.Emit(OpCodes.Stloc, localBuilder);
            // 加载本地变量的地址到计算堆栈
            ilGenerator.Emit(OpCodes.Ldloca_S, localBuilder);
            // 调用Source的get_Value方法
            MethodInfo getValueMethod = sourcePropertyInfo.PropertyType.GetProperty(nameof(Nullable<>.Value))?.GetGetMethod() ?? throw new Exception("获取Value失败");
            ilGenerator.Emit(OpCodes.Call, getValueMethod);
            // 调用AClass的set_Age方法
            ilGenerator.Emit(OpCodes.Callvirt, setMethod);
            // 标记hasValueFalseLabel的位置
            ilGenerator.MarkLabel(hasValueFalseLabel);
        }
        /// <summary>
        /// 写入IL-目标是可空类型
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="getMethod"></param>
        /// <param name="setMethod"></param>
        /// <param name="sourcePropertyInfo"></param>
        /// <param name="targetPropertyInfo"></param>
        private static void WriteILTheTargetNullType(ILGenerator ilGenerator, MethodInfo getMethod, MethodInfo setMethod, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo)
        {
            /*
            target.Age = new int?(source.Age);
             */
            // 加载第二个参数（target）和第一个参数（source）到计算堆栈
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            // 调用获取属性方法
            ilGenerator.Emit(OpCodes.Callvirt, getMethod);
            // 创建新的可空类型对象
            ConstructorInfo constructorInfo = targetPropertyInfo.PropertyType.GetConstructor([sourcePropertyInfo.PropertyType]) ?? throw new Exception("获取构造函数失败");
            ilGenerator.Emit(OpCodes.Newobj, constructorInfo);
            // 调用设置属性方法
            ilGenerator.Emit(OpCodes.Callvirt, setMethod);
        }
        /// <summary>
        /// 写入IL-相同类型
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="getMethod"></param>
        /// <param name="setMethod"></param>
        private static void WriteILTheSameType(ILGenerator ilGenerator, MethodInfo getMethod, MethodInfo setMethod)
        {
            /*
            target.Age = source.Age;
             */
            // 加载第二个参数（target）和第一个参数（source）到计算堆栈
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            // 调用获取属性方法
            ilGenerator.Emit(OpCodes.Callvirt, getMethod);
            // 调用设置属性方法
            ilGenerator.Emit(OpCodes.Callvirt, setMethod);
        }
    }
}
