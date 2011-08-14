using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine
{
    public class Error
    {
        private ErrorType errorType;
        private Location location;
        private string[] args;

        internal Error(ErrorType errorType, Location location, params string[] args)
        {
            if (location == null)
                throw new ArgumentNullException("location值不能是null");

            this.errorType = errorType;
            this.location = location;
            this.args = args;
        }

        internal Error(ErrorType errorType, string arg, Location location)
        {
            if (location == null)
                throw new ArgumentNullException("location值不能是null");

            this.errorType = errorType;
            this.location = location;
            this.args = new string[] { arg };
        }

        public string Message
        {
            get
            {
                switch (errorType)
                {
                    case ErrorType.ReadFileError:
                        return "无法读取文件：" + args[0];
                    case ErrorType.DuplicatedMainPlot:
                        return "不能有多个主情节(MainPlot)";
                    case ErrorType.DuplicatedSubPlot:
                        return "子情节名字" + args[0] + "已存在";
                    case ErrorType.DuplicatedActorLayer:
                        return "角色图层名字" + args[0] + "已存在";
                    case ErrorType.DuplicatedFunction:
                        return "函数名" + args[0] + "已存在";
                    case ErrorType.DuplicatedParameter:
                        return "函数参数名" + args[0] + "已在函数" + args[1] + "中存在";
                    case ErrorType.SrcAndAssetMissed:
                        return "未指定资源路径和预定义资源";
                    case ErrorType.BothSrcAndAsset:
                        return "不能同时指定资源路径和预定义资源";
                    case ErrorType.BothActorLayerAndCustom:
                        return "不能同时指定角色图层及已在角色图层中定义的参数，如位置";
                    case ErrorType.DuplicatedAsset:
                        return "预定义资源名\"" + args[0] + "\"已被定义过，不能重复定义";
                    case ErrorType.SourceNotExist:
                        return "资源" + args[0] + "不存在";
                    case ErrorType.SourceNotSupported:
                        return "资源" + args[0] + "的类型未被天火引擎支持";
                    case ErrorType.SourceTypeError:
                        return "指定资源类型为" + args[1] + "，但期望的类型为" + args[0];
                    case ErrorType.AssetNotExist:
                        return "不存在名为\"" + args[0] + "\"的" + args[1] + "类型的预定义资源";
                    case ErrorType.MainPlotNotExist:
                        return "主情节MainPlot不存在";
                    case ErrorType.ContinueNotInLoop:
                        return "<continue/>必须在<loop>...</loop>内使用";
                    case ErrorType.BreakNotInLoop:
                        return "<break/>必须在<loop>...</loop>内使用";
                    case ErrorType.SubPlotNotExist:
                        return "子情节" + args[0] + "不存在";
                    case ErrorType.FunctionNotExist:
                        return "函数" + args[0] + "不存在";
                    case ErrorType.ActionLayerNotExist:
                        return "角色图层" + args[0] + "不存在";
                    case ErrorType.UnknownToken:
                        return "表达式中含有未知符号";
                    case ErrorType.SingleQuote:
                        return "表达式中的引号不配对";
                    case ErrorType.UnknownEscapeChar:
                        return "表达式中含有未知转义符：\\" + args[0];
                    case ErrorType.ExpressionSyntaxError:
                        return "表达式中含有语法错误";
                    case ErrorType.SingleParen:
                        return "表达式中的小括号不配对";
                    case ErrorType.NotLeftValue:
                        return "该表达式必须是左值，通常是一个变量引用";
                    case ErrorType.OprandTypeError:
                        return "操作数类型错误";
                    case ErrorType.ParseTypeError:
                        return "无法将字符串\"" + args[0] + "\"解析为" + args[1] + "类型"; 
                    case ErrorType.VariableNotInitialized:
                        return "变量\"" + args[0] + "\"从未被赋值，因此无法确定其类型和初始值";
                    case ErrorType.DivisorZero:
                        return "除数不能为0";
                    case ErrorType.EqualOnFloat:
                        return "对浮点数（小数）类型的数据做相等或不相等的判断，通常得不到正确结果，建议使用不等号或使用整数类型";
                    case ErrorType.ParaStrDefFormatError:
                        return "参数字串定义的格式有误：" + args[0];
                    case ErrorType.SameNumberInParaStr:
                        return "在同一个函数中，不同的参数字串的参数数量不能相同。FireML只能通过参数数量来匹配参数字串";
                    case ErrorType.ParaStrCallFormatError:
                        return "参数字串调用的格式有误：" + args[0];
                    case ErrorType.DuplicatedParaAndParaStr:
                        return "同一个参数\"" + args[0] + "\"被普通方式和参数字串方式同时定义";
                    case ErrorType.NoMatchedParaStrDef:
                        return "不存在匹配的参数字串定义";
                    case ErrorType.ParameterNotDefined:
                        return "参数\"" + args[0] + "\"未定义";
                    case ErrorType.PlotXMLError:
                        return "剧情文件含有XML语法错误：" + args[0];
                    case ErrorType.AssetXMLError:
                        return "预定义资源文件含有XML语法错误：" + args[0];
                    default:
                        return Enum.GetName(typeof(ErrorType), errorType);
                }
            }
        }

        public Location Location
        {
            get { return location; }
        }
    }

    enum ErrorType
    {
        #region Build AST
        /// <summary>
        /// args: 异常信息
        /// </summary>
        ReadFileError,
        DuplicatedMainPlot,
        /// <summary>
        /// args: 子情节名字
        /// </summary>
        DuplicatedSubPlot,
        /// <summary>
        /// args：ActorLayer名字
        /// </summary>
        DuplicatedActorLayer,
        /// <summary>
        /// args: Function名字
        /// </summary>
        DuplicatedFunction,
        /// <summary>
        /// args: 参数名字
        /// </summary>
        DuplicatedParameter,
        BothSrcAndAsset,
        SrcAndAssetMissed,
        BothActorLayerAndCustom,
        /// <summary>
        /// args: 预定义资源名，资源类型
        /// </summary>
        DuplicatedAsset,
        /// <summary>
        /// args: paraStr
        /// </summary>
        ParaStrDefFormatError,
        SameNumberInParaStr,
        /// <summary>
        /// args: paraStr
        /// </summary>
        ParaStrCallFormatError,
        /// <summary>
        /// args: varName
        /// </summary>
        ParameterNotDefined,
        /// <summary>
        /// args: validation message
        /// </summary>
        PlotXMLError,
        /// <summary>
        /// args: validation message
        /// </summary>
        AssetXMLError,
        #endregion

        #region Content Check
        /// <summary>
        /// args：资源路径
        /// </summary>
        SourceNotExist,
        /// <summary>
        /// args：资源路径
        /// </summary>
        SourceNotSupported,
        /// <summary>
        /// args：expectedType, actualType
        /// </summary>
        SourceTypeError,
        #endregion

        #region AST Check
        /// <summary>
        /// args: assetName, expectedType
        /// </summary>
        AssetNotExist,
        MainPlotNotExist,
        ContinueNotInLoop,
        BreakNotInLoop,
        /// <summary>
        /// args: subPlotName
        /// </summary>
        SubPlotNotExist,
        /// <summary>
        /// args: function
        /// </summary>
        FunctionNotExist,
        /// <summary>
        /// args: actionLayer
        /// </summary>
        ActionLayerNotExist,
        /// <summary>
        /// args: varName
        /// </summary>
        DuplicatedParaAndParaStr,
        NoMatchedParaStrDef,
        #endregion

        #region Build Expression
        UnknownToken,
        SingleQuote,
        /// <summary>
        /// args: escaped char
        /// </summary>
        UnknownEscapeChar,
        ExpressionSyntaxError,
        SingleParen,
        NotLeftValue,
        OprandTypeError,
        /// <summary>
        /// args: str, type
        /// </summary>
        ParseTypeError,
        #endregion

        #region Runtime
        /// <summary>
        /// args: var name
        /// </summary>
        VariableNotInitialized,
        DivisorZero,
        EqualOnFloat,
        #endregion
    }
}
