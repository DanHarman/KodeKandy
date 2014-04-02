// <copyright file="ExpressionHelpers.cs" company="million miles per hour ltd">
// Copyright (c) 2013-2014 All Right Reserved
// 
// This source is subject to the MIT License.
// Please see the License.txt file for more information.
// All other rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KodeKandy
{
    public static class ExpressionHelpers
    {
        /// <summary>
        ///     Get a member name from a simple member expression.
        /// </summary>
        /// <param name="memberExpression">A simple member access expression.</param>
        /// <returns></returns>
        public static string GetMemberName<TClass, TMember>(Expression<Func<TClass, TMember>> memberExpression)
        {
            Require.NotNull(memberExpression, "memberExpression");

            var body = memberExpression.Body as MemberExpression;

            if (body == null)
                throw new ArgumentException("The specified expression is not simple.", "memberExpression");

            return body.Member.Name;
        }

        /// <summary>
        ///     Get a MemberInfo from a simple member access expression. The MemberInfo will be either a FieldInfo or PropertyInfo.
        /// </summary>
        /// <returns>Either a PropertyInfo or FieldInfo</returns>
        public static MemberInfo GetMemberInfo<TClass, TMember>(Expression<Func<TClass, TMember>> memberExpression)
        {
            Require.NotNull(memberExpression, "memberExpression");

            var body = memberExpression.Body as MemberExpression;

            if (body == null)
                throw new ArgumentException("The specified expression is not a simple member expression.", "memberExpression");

            return body.Member;
        }

        public static ReadOnlyCollection<string> GetMemberNames(this IEnumerable<MemberInfo> memberInfos)
        {
            Require.NotNull(memberInfos, "memberInfos");

            return new ReadOnlyCollection<string>(memberInfos.Select(memberInfo => memberInfo.Name).ToList());
        }

        public static string[] GetMemberNames<TClass, TValue>(Expression<Func<TClass, TValue>> memberChain)
        {
            Require.NotNull(memberChain, "memberChain");

            var nodes = new List<string>();

            var currentNode = memberChain.Body;

            while (currentNode.NodeType != ExpressionType.Parameter)
            {
                // Ignore boxing operations.
                if (currentNode.NodeType == ExpressionType.Convert || currentNode.NodeType == ExpressionType.ConvertChecked)
                {
                    currentNode = ((UnaryExpression) currentNode).Operand;
                    continue;
                }

                if (currentNode.NodeType == ExpressionType.Call)
                {
                    currentNode = ((MethodCallExpression) currentNode).Object;
                    continue;
                }

                if (currentNode.NodeType != ExpressionType.MemberAccess)
                    throw new ArgumentException(string.Format("memberChain '{0}' has a node that is not a Member '{1}'", memberChain, currentNode));

                var memberExpression = (MemberExpression) currentNode;

                nodes.Insert(0, memberExpression.Member.Name);
                currentNode = memberExpression.Expression;
            }

            return nodes.ToArray();
        }

        public static MemberInfo[] GetExpressionChainMemberInfos<TClass, TValue>(Expression<Func<TClass, TValue>> memberChain)
        {
            Require.NotNull(memberChain, "memberChain");

            var nodes = new List<MemberInfo>();

            var currentNode = memberChain.Body;

            while (currentNode.NodeType != ExpressionType.Parameter)
            {
//                // Ignore boxing operations.
//                if (currentNode.NodeType == ExpressionType.Convert || currentNode.NodeType == ExpressionType.ConvertChecked)
//                {
//                    currentNode = ((UnaryExpression)currentNode).Operand;
//                    continue;
//                }
//
//                if (currentNode.NodeType == ExpressionType.Call)
//                {
//                    currentNode = ((MethodCallExpression)currentNode).Object;
//                    continue;
//                }

                if (currentNode.NodeType != ExpressionType.MemberAccess)
                    throw new ArgumentException(string.Format("memberChain '{0}' has a node that is not a Member '{1}'", memberChain, currentNode));

                var memberExpression = (MemberExpression) currentNode;

                nodes.Insert(0, memberExpression.Member);
                currentNode = memberExpression.Expression;
            }

            return nodes.ToArray();
        }

        /// <summary>
        ///     Takes a flattened member chain string and converts it to a series of member infos if possible.
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="flattenedMemberNames"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<MemberInfo> UnflattenMemberNamesToMemberInfos(Type declaringType, string flattenedMemberNames)
        {
            Require.NotNullOrEmpty(flattenedMemberNames, "flattenedMemberNames");

            var memberInfos = new List<MemberInfo>();

            if (!UnflattenMemberNamesToMemberInfosImpl(memberInfos, declaringType, flattenedMemberNames))
                return new ReadOnlyCollection<MemberInfo>(new MemberInfo[] {});

            return new ReadOnlyCollection<MemberInfo>(memberInfos);
        }

        private static bool UnflattenMemberNamesToMemberInfosImpl(List<MemberInfo> memberInfos, Type declaringType, string flattenedMemberNames)
        {
            if (flattenedMemberNames.Length == 0)
                return true;

            foreach (var partition in ReverseEnumerateBinaryPartitions(flattenedMemberNames))
            {
                var memberInfo = ReflectionHelpers.GetMemberInfo(declaringType, partition.Item1);

                if (memberInfo == null) continue;

                memberInfos.Add(memberInfo);
                return UnflattenMemberNamesToMemberInfosImpl(memberInfos, memberInfo.GetMemberType(), partition.Item2);
            }

            // If we get here we failed to get a match.
            return false;
        }

        /// <summary>
        ///     Takes a string and yields paritioned tuples of the possible sub strings starting from the end.
        ///     e.g. "Hello" =>
        ///     {{"Hello", String.Empty}, {"Hell", "o"}, {"Hel", "lo"}, {"He", "llo"}, {"H", "ello"}, {String.Empty, "Hello"}}
        /// </summary>
        /// <remarks>
        ///     This method is used when unflattening member names.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string>> ReverseEnumerateBinaryPartitions(string input)
        {
            if (string.IsNullOrEmpty(input))
                yield break;

            for (var i = input.Length; i >= 0; --i)
                yield return Tuple.Create(input.Substring(0, i), input.Substring(i, input.Length - i));
        }

        /// <summary>
        ///     Detects if an expression is a member accessor, with no chaining, convertion etc.
        /// </summary>
        /// <returns>Returns true if it is a memberExpression, otherwise false.</returns>
        public static bool IsMemberExpression<TClass, TMember>(Expression<Func<TClass, TMember>> memberExpression)
        {
            Require.NotNull(memberExpression, "memberExpression");

            return memberExpression.Body is MemberExpression;
        }

        public static bool IsComplexChain<TClass, TValue>(Expression<Func<TClass, TValue>> memberChain)
        {
            Require.NotNull(memberChain, "memberChain");

            var memberNodeFound = false;
            var currentNode = memberChain.Body;

            while (currentNode.NodeType != ExpressionType.Parameter)
            {
                // Functions are always complex.
                if (currentNode.NodeType == ExpressionType.Call)
                {
                    return true;
                }

                if (currentNode.NodeType == ExpressionType.MemberAccess)
                {
                    if (memberNodeFound)
                        return true;

                    memberNodeFound = true;
                    currentNode = ((MemberExpression) currentNode).Expression;
                    continue;
                }

                // Ignore boxing operations.
                if (currentNode.NodeType == ExpressionType.Convert || currentNode.NodeType == ExpressionType.ConvertChecked)
                {
                    currentNode = ((UnaryExpression) currentNode).Operand;
                    continue;
                }
            }

            return false;
        }
    }
}