using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    interface IExprVisitor
    {
        void Visit(AddExpr addExpr, object[] args);
        void Visit(AndExpr andExpr, object[] args);
        void Visit(AssignExpr assignExpr, object[] args);
        void Visit(DivExpr divExpr, object[] args);
        void Visit(EquExpr equExpr, object[] args);
        void Visit(GreatEquExpr greatEquExpr, object[] args);
        void Visit(GreatExpr greatExpr, object[] args);
        void Visit(LeftValueExpr leftValueExpr, object[] args);
        void Visit(LessEquExpr lessEquExpr, object[] args);
        void Visit(LessExpr lessExpr, object[] args);
        void Visit(MulExpr mulExpr, object[] args);
        void Visit(NegativeExpr negativeExpr, object[] args);
        void Visit(NeqExpr neqExpr, object[] args);
        void Visit(NotExpr notExpr, object[] args);
        void Visit(OrExpr orExpr, object[] args);
        void Visit(PowExpr powExpr, object[] args);
        void Visit(RightValueExpr rightValueExpr, object[] args);
        void Visit(SubExpr subExpr, object[] args);
    }
}
