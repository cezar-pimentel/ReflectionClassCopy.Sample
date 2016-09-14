using ReflectionClassCopy.Sample.Helpers;
using ReflectionClassCopy.Sample.Model;
using System;

namespace ReflectionClassCopy.Sample
{
	internal class Program
	{
		private static void Main()
		{
			var src = TestModelSource.Fill();
			var sw = new System.Diagnostics.Stopwatch();
			sw.Reset();
			sw.Start();

			var copied = PropertyCopy<TestModelTarget>.CopyFrom(src);

			sw.Stop();

			Console.WriteLine($"{sw.Elapsed.TotalSeconds}");
			Console.WriteLine("FIM");
			Console.ReadLine();
		}
	}
}