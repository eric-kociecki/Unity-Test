using System;
using System.Diagnostics;
using UnityEngine;

public static class Benchmark
{
	public static void Measure(Action function, string description)
	{
		Stopwatch watch = new Stopwatch();
		watch.Start();
		function();
		watch.Stop();
		UnityEngine.Debug.Log(String.Format (String.Format ("{0} Time:{1:fffffff}", description, watch.Elapsed)));
	}
}
