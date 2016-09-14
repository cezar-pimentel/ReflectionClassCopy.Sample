using System;

namespace ReflectionClassCopy.Sample.Model
{
	public class TestModelSource
	{
		public short Item1 { get; set; }
		public DateTime Item2 { get; set; }
		public bool Item3 { get; set; }
		public string Item4 { get; set; }
		public int Item5 { get; set; }
		public long Item6 { get; set; }

		public static TestModelSource Fill()
		{
			var src = new TestModelSource
			{
				Item1 = 1,
				Item2 = DateTime.Now,
				Item3 = true,
				Item4 = "S1",
				Item5 = 2,
				Item6 = 3
			};
			return src;
		}
	}
}