using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public interface IPredicate
{
	bool Evaluate();

	public static IPredicate And(IPredicate left, IPredicate right) => new FuncPredicate(() => left.Evaluate() && right.Evaluate());
	public static IPredicate And(Func<bool> left, IPredicate right) => new FuncPredicate(() => left() && right.Evaluate());
	public static IPredicate And(Func<bool> left, Func<bool> right) => new FuncPredicate(() => left() && right());
	public static IPredicate And(IPredicate left, Func<bool> right) => new FuncPredicate(() => left.Evaluate() && right());

	public static IPredicate Or(IPredicate left, IPredicate right) => new FuncPredicate(() => left.Evaluate() || right.Evaluate());
	public static IPredicate Or(Func<bool> left, IPredicate right) => new FuncPredicate(() => left() || right.Evaluate());
	public static IPredicate Or(Func<bool> left, Func<bool> right) => new FuncPredicate(() => left() || right());
	public static IPredicate Or(IPredicate left, Func<bool> right) => new FuncPredicate(() => left.Evaluate() || right());

	public static IPredicate Not(IPredicate pred) => new FuncPredicate(() => !pred.Evaluate());
	public static IPredicate Not(Func<bool> pred) => new FuncPredicate(() => !pred());
}

public class FuncPredicate : IPredicate
{
	readonly Func<bool> func;
	public FuncPredicate(Func<bool> func) => this.func = func;
	public bool Evaluate() => func();
}
