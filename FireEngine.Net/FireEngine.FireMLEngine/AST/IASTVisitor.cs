using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    interface IASTVisitor
    {
        void Visit(FireMLRoot root, object[] args);
        void Visit(ActionLayerDef actionLayerDef, object[] args);
        void Visit(FunctionDef functionDef, object[] args);
        void Visit(PlotDef plotDef, object[] args);
        void Visit(ContinueStmt continueStmt, object[] args);
        void Visit(FunctionCallStmt funcCallStmt, object[] args);
        void Visit(MusicStmt musicStmt, object[] args);
        void Visit(MusicStopStmt musicStopStmt, object[] args);
        void Visit(MusicVolStmt musicVolStmt, object[] args);
        void Visit(SwitchStmt switchStmt, object[] args);
        void Visit(SelectStmt selectStmt, object[] args);
        void Visit(ActorStmt actorStmt, object[] args);
        void Visit(DialogStmt dialogStmt, object[] args);
        void Visit(IfStmt ifStmt, object[] args);
        void Visit(LoopStmt loopStmt, object[] args);
        void Visit(BackgroundStmt backgroundStmt, object[] args);
        void Visit(EchoStmt echoStmt, object[] args);
        void Visit(IncludeStmt includeStmt, object[] args);
        void Visit(BreakStmt breakStmt, object[] args);
        void Visit(ExpressionStmt expressionStmt, object[] args);
        void Visit(ReturnStmt returnStmt, object[] args);

        void Visit(ParameterDef parameterDef, object[] args);
        void Visit(SelectOption selectOption, object[] args);
        void Visit(SwitchCase switchCase, object[] args);
        void Visit(IfBlock ifBlock, object[] args);

        void Visit(DataStmt dataStmt, object[] args);
        void Visit(AssetDef assetDef, object[] args);
    }
}
