using FlowOptions.EggOn.Logging;
using System;
using System.Web.Http;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using FlowOptions.EggOn.ModuleCore;
using FlowOptions.EggOn.DataHost;

namespace FlowOptions.EggOn.Service
{
    public static class DataConfig
    {
        public static void Register(HttpConfiguration config, List<Assembly> moduleAssemblies)
        {
            Logger.Debug("Application is preparing the database.");

            List<Type> dataTypes;

            try
            {
                dataTypes = moduleAssemblies.SelectMany(a => a.GetTypes())
                            .Where(p => Attribute.IsDefined(p, typeof(TableNameAttribute)))
                            .ToList();
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.Fatal("Error while loading modules for data configuration: " + e.LoaderExceptions[0]);
                return;
            }

            var L = new List<Type>(); // Empty list that will contain the sorted elements
            var S = GetModelsWithoutDependents(dataTypes); // Set of all nodes with no incoming edges

            var visitedEdges = new HashSet<PropertyInfo>();

            // while S is non-empty do
            while (S.Count != 0)
            {
                // remove a node n from S
                var n = S[S.Count - 1];
                S.RemoveAt(S.Count - 1);

                // add n to tail of L
                L.Add(n);

                // for each node m with an edge e from n to m do
                foreach (var m in GetOutgoingNodes(n))
                {
                    var edges = GetOutgoingEdgesToTarget(n, m);
                    foreach (var edge in edges)
                    {
                        // remove edge e from the graph
                        if (!visitedEdges.Contains(edge))
                            visitedEdges.Add(edge);
                    }

                    // if m has no other incoming edges then
                    if (GetIncomingEdges(m, dataTypes).All(p => visitedEdges.Contains(p)))
                    {
                        // insert m into S
                        S.Add(m);
                    }
                }
            }

            L.Reverse();

            using (var db = new EggOnDatabase())
            {
                foreach (Type t in L)
                {
                    db.CreateOrUpdateTableFromModel(t);
                }
            }
        }

        private static List<Type> GetOutgoingNodes(Type type)
        {
            var constraint = typeof(ConstraintAttribute);

            return type.GetProperties()
                    .Where(p => Attribute.IsDefined(p, constraint) && ((ConstraintAttribute)p.GetCustomAttribute(constraint, false)).ForeignObject != p.DeclaringType)
                    .Select(p => ((ConstraintAttribute)p.GetCustomAttribute(constraint, false)).ForeignObject)
                    .ToList();
        }

        private static List<PropertyInfo> GetOutgoingEdges(Type type)
        {
            var constraint = typeof(ConstraintAttribute);

            return type.GetProperties()
                    .Where(p => Attribute.IsDefined(p, constraint) && ((ConstraintAttribute)p.GetCustomAttribute(constraint, false)).ForeignObject != p.DeclaringType)
                    .ToList();
        }

        private static List<PropertyInfo> GetIncomingEdges(Type type, List<Type> allTypes)
        {
            var constraint = typeof(ConstraintAttribute);

            return allTypes.Where(t => t != type)
                    .SelectMany(t => t.GetProperties())
                    .Where(p => Attribute.IsDefined(p, constraint) && ((ConstraintAttribute)p.GetCustomAttribute(constraint, false)).ForeignObject == type)
                    .ToList();
        }

        private static List<PropertyInfo> GetOutgoingEdgesToTarget(Type type, Type target)
        {
            if (type == target)
                return new List<PropertyInfo>();

            var constraint = typeof(ConstraintAttribute);

            return type.GetProperties()
                       .Where(p => Attribute.IsDefined(p, constraint) && ((ConstraintAttribute)p.GetCustomAttribute(constraint, false)).ForeignObject == target)
                       .ToList();
        }

        private static List<Type> GetModelsWithoutDependents(List<Type> types)
        {
            var constraint = typeof(ConstraintAttribute);

            var hasDependents = types.SelectMany(t => t.GetProperties())
                        .Where(p => Attribute.IsDefined(p, constraint) && ((ConstraintAttribute)p.GetCustomAttribute(constraint, false)).ForeignObject != p.DeclaringType)
                        .Select(p => ((ConstraintAttribute)p.GetCustomAttribute(constraint, false)).ForeignObject)
                        .ToList();

            return types.Where(t => !hasDependents.Contains(t)).ToList();
        }
    }
}
