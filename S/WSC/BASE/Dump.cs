using System;
using System.Text;
using System.Collections;
using System.Reflection;

namespace WSC
{
    public sealed class Dump
    {
        private int level;
        private int depth;
        private int indent;
        private object target;
        private StringBuilder logs;

        public Dump(object target) : this(target, 20, 2)
        {
        }

        public Dump(object target, int depth, int indent)
        {
            this.target = target;
            this.depth = depth;
            this.indent = indent;
        }

        public override string ToString()
        {
            logs = new StringBuilder();
            return ToString(target);
        }

        private string ToString(object element)
        {
            try
            {
                if (element == null || element is ValueType || element is string)
                {
                    ToString(FormatValue(element));
                }
                else if (level < depth)
                {
                    var type = element.GetType();
                    if (typeof(IEnumerable).IsAssignableFrom(type) == false)
                    {
                        ToString("[{0}]", type.FullName);
                        level++;
                    }

                    var enumerable = element as IEnumerable;
                    if (enumerable != null)
                    {
                        ToString("...");

                        foreach (object item in enumerable)
                        {
                            if (item is IEnumerable && !(item is string))
                            {
                                if (level < depth)
                                {
                                    level++;
                                    ToString(item);
                                    level--;
                                }
                            }
                            else
                            {
                                ToString(item);
                            }
                        }
                    }
                    else
                    {
                        MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var member in members)
                        {
                            var f = member as FieldInfo;
                            var p = member as PropertyInfo;

                            if (f == null && p == null)
                                continue;

                            var t = (f != null) ? f.FieldType : p.PropertyType;
                            object v = (f != null) ? f.GetValue(element) : p.GetValue(element, null);

                            if (t.IsValueType || t == typeof(string))
                            {
                                ToString("{0}: {1}", member.Name, FormatValue(v));
                            }
                            else
                            {
                                ToString("{0}: {1}", member.Name, typeof(IEnumerable).IsAssignableFrom(t) ? "..." : "{ }");

                                if (level < depth)
                                {
                                    level++;
                                    ToString(v);
                                    level--;
                                }
                            }
                        }
                    }

                    if (typeof(IEnumerable).IsAssignableFrom(type) == false)
                    {
                        level--;
                    }
                }
            }
            catch (Exception e)
            {
                ToString("<ERROR>: {0}", e.HResult);
            }

            return logs.ToString();
        }

        private void ToString(string value, params object[] args)
        {
            var space = new string(' ', level * indent);

            if (args != null)
                value = string.Format(value, args);

            logs.AppendLine(space + value);
        }

        private string FormatValue(object o)
        {
            if (o == null)
                return ("null");

            if (o is DateTime)
                return (((DateTime)o).ToShortDateString());

            if (o is string)
                return string.Format("\"{0}\"", o);

            if (o is char && (char)o == '\0')
                return string.Empty;

            if (o is ValueType)
                return (o.ToString());

            if (o is IEnumerable)
                return ("...");

            return ("{ }");
        }
    }
}