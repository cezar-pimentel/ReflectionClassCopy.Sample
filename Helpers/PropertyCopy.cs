using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ReflectionClassCopy.Sample.Helpers
{
	public static class PropertyCopy<TTarget> where TTarget : class, new()
	{
		public static TTarget CopyFrom<TSource>(TSource source) where TSource : class
		{
			return PropertyCopier<TSource, TTarget>.Copy(source);
		}
	}

	internal static class PropertyCopier<TSource, TTarget>
	{
		private static readonly Func<TSource, TTarget> creator;
		private static readonly List<PropertyInfo> sourceProperties = new List<PropertyInfo>();
		private static readonly List<PropertyInfo> targetProperties = new List<PropertyInfo>();
		private static readonly Exception initializationException;

		internal static TTarget Copy(TSource source)
		{
			if (initializationException != null)
				throw initializationException;

			if (source == null)
				throw new ArgumentNullException("source");

			return creator(source);
		}

		static PropertyCopier()
		{
			try
			{
				creator = BuildCreator();
				initializationException = null;
			}
			catch (Exception e)
			{
				creator = null;
				initializationException = e;
			}
		}

		private static Func<TSource, TTarget> BuildCreator()
		{
			ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "source");
			var bindings = new List<MemberBinding>();

			foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!sourceProperty.CanRead)
					continue;

				PropertyInfo targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);

				if (targetProperty == null)
					throw new ArgumentException("Property " + sourceProperty.Name + " is not present and accessible in " + typeof(TTarget).FullName);

				if (!targetProperty.CanWrite)
					throw new ArgumentException("Property " + sourceProperty.Name + " is not writable in " + typeof(TTarget).FullName);

				if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
					throw new ArgumentException("Property " + sourceProperty.Name + " is static in " + typeof(TTarget).FullName);

				if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
					throw new ArgumentException("Property " + sourceProperty.Name + " has an incompatible type in " + typeof(TTarget).FullName);

				bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
				sourceProperties.Add(sourceProperty);
				targetProperties.Add(targetProperty);
			}

			var initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
			return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
		}
	}
}