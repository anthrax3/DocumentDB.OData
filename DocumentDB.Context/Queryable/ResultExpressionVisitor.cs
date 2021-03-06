﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataServiceProvider;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace DocumentDB.Context.Queryable
{
    public class ResultExpressionVisitor : DSPMethodTranslatingVisitor
    {
        public override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method == GetValueMethodInfo)
            {
                if (m.Arguments[0].Type == typeof(JObject))
                {
                    return Expression.Call(
                        m.Method,
                        ExpressionUtils.ReplaceParameterType(m.Arguments[0], typeof(DSPResource), Visit),
                        Visit(m.Arguments[1]));
                }
            }

            return base.VisitMethodCall(m);
        }
    }
}
